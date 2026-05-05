using UnityEngine;
using UnityEngine.UI;

public class OptionsPanelUI : MonoBehaviour
{
    [Header("Sliders")]
    public Slider sliderMaster;
    public Slider sliderMusic;
    public Slider sliderSfx;

    void OnEnable()
    {
        // Cargar valores guardados al abrir el panel
        var data = SaveSystem.Current;
        sliderMaster.SetValueWithoutNotify(data.volumeMaster);
        sliderMusic .SetValueWithoutNotify(data.volumeMusic);
        sliderSfx   .SetValueWithoutNotify(data.volumeSfx);

        // Aplicar al mixer por si acaso no se aplicaron al arrancar
        AudioManager.Instance.SetMasterVolume(data.volumeMaster);
        AudioManager.Instance.SetMusicVolume (data.volumeMusic);
        AudioManager.Instance.SetSfxVolume   (data.volumeSfx);

        sliderMaster.onValueChanged.AddListener(OnMasterChanged);
        sliderMusic .onValueChanged.AddListener(OnMusicChanged);
        sliderSfx   .onValueChanged.AddListener(OnSfxChanged);
    }

    void OnDisable()
    {
        sliderMaster.onValueChanged.RemoveListener(OnMasterChanged);
        sliderMusic .onValueChanged.RemoveListener(OnMusicChanged);
        sliderSfx   .onValueChanged.RemoveListener(OnSfxChanged);

        SaveSystem.Save();
    }

    private void OnMasterChanged(float v)
    {
        AudioManager.Instance.SetMasterVolume(v);
        SaveSystem.Current.volumeMaster = v;
    }

    private void OnMusicChanged(float v)
    {
        AudioManager.Instance.SetMusicVolume(v);
        SaveSystem.Current.volumeMusic = v;
    }

    private void OnSfxChanged(float v)
    {
        AudioManager.Instance.SetSfxVolume(v);
        SaveSystem.Current.volumeSfx = v;
    }
}
