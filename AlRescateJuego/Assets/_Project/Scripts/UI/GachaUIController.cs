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

    [Header("Canvas")]
    public GameObject canvasResult;
    public GameObject canvasShop;
    public GameObject canvasCollection;

    [Header("Animacion")]
    public RevealAnimationController revealController;
    public Animator gachaMachineAnimator;

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
        revealController.SetPullAgainAction(OnPullX1);

        canvasResult.SetActive(false);
        canvasShop.SetActive(false);
        canvasCollection.SetActive(false);

        if (welcomeCloseBtn != null)
            welcomeCloseBtn.onClick.AddListener(() => welcomePanel?.SetActive(false));

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
        if (GachaManager.Instance.TryPullX1(out var r))
        {
            if (gachaMachineAnimator != null)
                gachaMachineAnimator.SetTrigger("TrigSpin");
            StartCoroutine(revealController.PlayX1(r));
        }
    }

    private void OnPullX10()
    {
        if (GachaManager.Instance.TryPullX10(out var results))
        {
            if (gachaMachineAnimator != null)
                gachaMachineAnimator.SetTrigger("TrigSpin");
            StartCoroutine(revealController.PlayX10(results));
        }
    }

    private void OnOptionsClicked()
    {
        AudioManager.Instance.Play("sfx_ui_click");
        Debug.Log("Opciones (placeholder)");
    }
}
