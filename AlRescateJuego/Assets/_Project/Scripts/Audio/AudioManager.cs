using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [Header("Mixer")]
    public AudioMixerGroup musicGroup;
    public AudioMixerGroup sfxGroup;

    [Header("Library")]
    [SerializeField] private List<AudioEntry> sfxLibrary = new();
    [SerializeField] private AudioClip hubMusic;

    [Header("Pool")]
    [SerializeField] private int sfxPoolSize = 8;

    private Dictionary<string, AudioClip> _sfxDict;
    private List<AudioSource> _sfxPool;
    private AudioSource _musicSource;
    private float _lastVolumeMaster = 1f;
    private float _lastVolumeMusic  = 1f;
    private float _lastVolumeSfx    = 1f;

    [System.Serializable]
    public class AudioEntry
    {
        public string key;
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        [Range(0.5f, 2f)] public float pitch = 1f;
    }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        _sfxDict = new Dictionary<string, AudioClip>();
        foreach (var e in sfxLibrary)
            if (!string.IsNullOrEmpty(e.key) && e.clip != null)
                _sfxDict[e.key] = e.clip;

        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.outputAudioMixerGroup = musicGroup;
        _musicSource.loop = true;
        _musicSource.playOnAwake = false;
        _musicSource.volume = 0.7f;

        _sfxPool = new List<AudioSource>();
        for (int i = 0; i < sfxPoolSize; i++)
        {
            var src = gameObject.AddComponent<AudioSource>();
            src.outputAudioMixerGroup = sfxGroup;
            src.playOnAwake = false;
            src.loop = false;
            _sfxPool.Add(src);
        }
    }

    public void PlayHubMusic()
    {
        if (hubMusic == null) { Debug.LogWarning("[AudioManager] hubMusic no asignado."); return; }
        // Garantizar que el mixer no esté en silencio antes de reproducir
        SetMasterVolume(Mathf.Max(0.01f, _lastVolumeMaster));
        SetMusicVolume(Mathf.Max(0.01f, _lastVolumeMusic));
        _musicSource.clip = hubMusic;
        _musicSource.Play();
        Debug.Log("[AudioManager] PlayHubMusic -> " + hubMusic.name);
    }

    public void StopMusic() => _musicSource.Stop();

    public void Play(string key, float volumeScale = 1f, float pitch = 1f)
    {
        if (!_sfxDict.TryGetValue(key, out var clip))
        {
            Debug.LogWarning($"[AudioManager] Clave SFX no encontrada: {key}");
            return;
        }
        var src = GetFreeSource();
        src.clip = clip;
        src.volume = volumeScale;
        src.pitch = pitch;
        src.Play();
    }

    private AudioSource GetFreeSource()
    {
        foreach (var s in _sfxPool) if (!s.isPlaying) return s;
        return _sfxPool[0];
    }

    public void SetMasterVolume(float linear) { _lastVolumeMaster = linear; SetMixer("Volume_Master", linear); }
    public void SetMusicVolume(float linear)  { _lastVolumeMusic  = linear; SetMixer("Volume_Music",  linear); }
    public void SetSfxVolume(float linear)    { _lastVolumeSfx    = linear; SetMixer("Volume_SFX",    linear); }

    private void SetMixer(string param, float linear)
    {
        var mixer = musicGroup != null ? musicGroup.audioMixer : sfxGroup.audioMixer;
        float dB = Mathf.Log10(Mathf.Clamp(linear, 0.0001f, 1f)) * 20f;
        mixer.SetFloat(param, dB);
    }
}
