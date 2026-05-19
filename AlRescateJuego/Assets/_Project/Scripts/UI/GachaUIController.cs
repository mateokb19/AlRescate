using UnityEngine;
using UnityEngine.UI;

public class GachaUIController : MonoBehaviour
{
    [Header("Botones HUD")]
    public Button btnPullX1;
    public Button btnPullX10;
    public Button btnShop;
    public Button btnCollection;
    public Button btnOptions;

    [Header("Botones Cerrar Paneles")]
    public Button btnCloseShop;
    public Button btnCloseCollection;
    public Button btnCloseOptions;

    [Header("Canvas")]
    public GameObject canvasResult;
    public GameObject canvasShop;
    public GameObject canvasCollection;
    public GameObject canvasOptions;

    [Header("Animacion")]
    public RevealAnimationController revealController;
    public GachaMachineShake gachaMachineShake;

    [Header("Bono de bienvenida")]
    public GameObject welcomePanel;
    public Button welcomeCloseBtn;
    public int welcomeGems = 1600;
    public PetData starterPet;

    void Start()
    {
        btnPullX1.onClick.AddListener(OnPullX1);
        btnPullX10.onClick.AddListener(OnPullX10);
        btnShop.onClick.AddListener(() => SetCanvas(canvasShop, true));
        btnCollection.onClick.AddListener(() => SetCanvas(canvasCollection, true));
        btnOptions.onClick.AddListener(OnOptionsClicked);
        if (btnCloseShop != null) btnCloseShop.onClick.AddListener(() => SetCanvas(canvasShop, false));
        if (btnCloseCollection != null) btnCloseCollection.onClick.AddListener(() => SetCanvas(canvasCollection, false));
        if (btnCloseOptions != null) btnCloseOptions.onClick.AddListener(() => SetCanvas(canvasOptions, false));
        revealController.SetPullAgainAction(OnPullX1);

        // El boton "Cerrar" del panel de resultado tambien detiene el spin de la maquina
        if (revealController != null && revealController.btnClose != null && gachaMachineShake != null)
            revealController.btnClose.onClick.AddListener(gachaMachineShake.StopSpin);

        canvasResult.SetActive(false);
        canvasShop.SetActive(false);
        canvasCollection.SetActive(false);
        if (canvasOptions != null) canvasOptions.SetActive(false);

        if (welcomeCloseBtn != null)
            welcomeCloseBtn.onClick.AddListener(() => welcomePanel?.SetActive(false));

        // Restaurar volúmenes guardados
        AudioManager.Instance.SetMasterVolume(SaveSystem.Current.volumeMaster);
        AudioManager.Instance.SetMusicVolume (SaveSystem.Current.volumeMusic);
        AudioManager.Instance.SetSfxVolume   (SaveSystem.Current.volumeSfx);

        TryGiveWelcomeBonus();
        AudioManager.Instance.PlayHubMusic();

        if (PlayerPrefs.GetInt("OpenCollectionOnStart", 0) == 1)
        {
            PlayerPrefs.SetInt("OpenCollectionOnStart", 0);
            SetCanvas(canvasCollection, true);
        }
    }

    void TryGiveWelcomeBonus()
    {
        if (!SaveSystem.Current.firstLaunch) return;
        SaveSystem.Current.firstLaunch = false;
        PlayerInventory.Instance.AddGems(welcomeGems);
        if (starterPet != null)
        {
            PlayerInventory.Instance.GrantPet(starterPet, out _);
            PlayerInventory.Instance.EquipPet(starterPet);
        }
        if (welcomePanel != null) welcomePanel.SetActive(true);
        SaveSystem.Save();
    }

    public void SetCanvas(GameObject c, bool active)
    {
        AudioManager.Instance.Play(active ? "sfx_ui_click" : "sfx_ui_close");
        var cg = c.GetComponent<CanvasGroup>();
        if (active)
        {
            c.SetActive(true);
            if (cg != null) StartCoroutine(UIAnimator.FadeIn(cg));
        }
        else
        {
            if (cg != null) StartCoroutine(UIAnimator.FadeOut(cg));
            else c.SetActive(false);
        }
    }

    private void OnPullX1()
    {
        Debug.Log($"[GachaUI] OnPullX1 click. gachaMachineShake={(gachaMachineShake == null ? "NULL" : gachaMachineShake.name)}");
        if (GachaManager.Instance.TryPullX1(out var r))
            StartCoroutine(RunPull(revealController.PlayX1(r)));
    }

    private void OnPullX10()
    {
        Debug.Log($"[GachaUI] OnPullX10 click. gachaMachineShake={(gachaMachineShake == null ? "NULL" : gachaMachineShake.name)}");
        if (GachaManager.Instance.TryPullX10(out var results))
            StartCoroutine(RunPull(revealController.PlayX10(results)));
    }

    private System.Collections.IEnumerator RunPull(System.Collections.IEnumerator reveal)
    {
        if (gachaMachineShake != null) gachaMachineShake.StartSpin();
        else Debug.LogWarning("[GachaUI] gachaMachineShake es NULL. Asignalo en el Inspector del HubController o ejecuta 'AlRescate/Configurar Animator GachaMachine'.");
        yield return StartCoroutine(reveal);
        // El spin sigue mientras el panel de resultado este abierto; se detiene al pulsar Cerrar.
    }

    private void OnOptionsClicked()
    {
        if (canvasOptions != null) SetCanvas(canvasOptions, true);
    }
}
