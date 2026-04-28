using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RevealAnimationController : MonoBehaviour
{
    [Header("Referencias")]
    public GameObject canvasResult;
    public RectTransform resultRoot;
    public Image beamOfLight;
    public Transform cardContainer;
    public GameObject resultCardPrefab;
    public Button btnPullAgain, btnClose, btnSkip;

    [Header("Particulas")]
    public ParticleSystem fxCommon, fxRare, fxEpic, fxLegendary;

    [Header("Colores del haz")]
    public Color colorBlue = new Color(0.30f, 0.45f, 1f);
    public Color colorPurple = new Color(0.55f, 0.30f, 1f);
    public Color colorGold = new Color(1f, 0.85f, 0.10f);

    private bool _skipRequested;

    void Start()
    {
        if (btnSkip != null) btnSkip.onClick.AddListener(() => _skipRequested = true);
        if (btnClose != null) btnClose.onClick.AddListener(Close);
    }

    public void SetPullAgainAction(System.Action action)
    {
        if (btnPullAgain == null) return;
        btnPullAgain.onClick.RemoveAllListeners();
        btnPullAgain.onClick.AddListener(() => action?.Invoke());
    }

    public IEnumerator PlayX1(GachaResult r)
    {
        ClearCards();
        SetGridLayout(single: true);
        canvasResult.SetActive(true);
        SetBeam(r.rarity);
        yield return AnimateBeam(0.6f);
        if (beamOfLight != null) beamOfLight.gameObject.SetActive(false);
        AudioManager.Instance.Play("sfx_capsule_open");
        yield return RevealCard(r);
        ShowButtons();
    }

    public IEnumerator PlayX10(List<GachaResult> results)
    {
        ClearCards();
        SetGridLayout(single: false);
        canvasResult.SetActive(true);
        Rarity max = Rarity.Common;
        foreach (var rr in results) if (rr.rarity > max) max = rr.rarity;
        SetBeam(max);
        yield return AnimateBeam(1.2f);
        if (beamOfLight != null) beamOfLight.gameObject.SetActive(false);
        AudioManager.Instance.Play("sfx_capsule_open");
        if (btnSkip != null) btnSkip.gameObject.SetActive(true);
        _skipRequested = false;
        foreach (var rr in results)
        {
            yield return RevealCard(rr);
            if (_skipRequested) break;
        }
        if (_skipRequested) ShowAllCards(results);
        if (btnSkip != null) btnSkip.gameObject.SetActive(false);
        ShowButtons();
    }

    private void SetGridLayout(bool single)
    {
        if (cardContainer == null) return;
        var grid = cardContainer.GetComponent<UnityEngine.UI.GridLayoutGroup>();
        if (grid == null) grid = cardContainer.gameObject.AddComponent<UnityEngine.UI.GridLayoutGroup>();

        grid.childAlignment = TextAnchor.MiddleCenter;
        grid.spacing = new Vector2(12, 12);
        grid.padding = new RectOffset(10, 10, 10, 10);

        if (single)
        {
            grid.cellSize = new Vector2(280, 390);
            grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 1;
        }
        else
        {
            grid.cellSize = new Vector2(170, 240);
            grid.constraint = UnityEngine.UI.GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
        }
    }

    private IEnumerator RevealCard(GachaResult r)
    {
        var card = Instantiate(resultCardPrefab, cardContainer);
        var view = card.GetComponent<ResultCardView>();
        view.Setup(r);
        PlayParticleByRarity(r.rarity);
        AudioManager.Instance.Play(SfxByRarity(r.rarity));
        yield return new WaitForSeconds(DurationByRarity(r.rarity));
    }

    private void SetBeam(Rarity max)
    {
        if (beamOfLight == null) return;
        beamOfLight.color = max == Rarity.Legendary ? colorGold :
                            max == Rarity.Epic ? colorPurple : colorBlue;
    }

    private IEnumerator AnimateBeam(float duration)
    {
        if (beamOfLight == null) yield break;
        beamOfLight.gameObject.SetActive(true);
        Vector3 from = new Vector3(1f, 0f, 1f);
        Vector3 to = new Vector3(1f, 1f, 1f);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            beamOfLight.rectTransform.localScale = Vector3.Lerp(from, to, t / duration);
            yield return null;
        }
        beamOfLight.rectTransform.localScale = to;
    }

    private void PlayParticleByRarity(Rarity r)
    {
        ParticleSystem fx = r switch
        {
            Rarity.Common => fxCommon,
            Rarity.Rare => fxRare,
            Rarity.Epic => fxEpic,
            Rarity.Legendary => fxLegendary,
            _ => null
        };
        if (fx != null) fx.Play();
    }

    private string SfxByRarity(Rarity r) => r switch
    {
        Rarity.Common => "sfx_reveal_common",
        Rarity.Rare => "sfx_reveal_rare",
        Rarity.Epic => "sfx_reveal_epic",
        Rarity.Legendary => "sfx_reveal_legendary",
        _ => "sfx_reveal_common"
    };

    private float DurationByRarity(Rarity r) => r switch
    {
        Rarity.Common => 0.5f,
        Rarity.Rare => 1.0f,
        Rarity.Epic => 1.6f,
        Rarity.Legendary => 3.5f,
        _ => 0.5f
    };

    private void ClearCards()
    {
        for (int i = cardContainer.childCount - 1; i >= 0; i--)
            Destroy(cardContainer.GetChild(i).gameObject);
        if (beamOfLight != null)
        {
            beamOfLight.rectTransform.localScale = new Vector3(1f, 0f, 1f);
            beamOfLight.gameObject.SetActive(false);
        }
    }

    private void ShowAllCards(List<GachaResult> results)
    {
        ClearCards();
        foreach (var r in results)
        {
            var card = Instantiate(resultCardPrefab, cardContainer);
            card.GetComponent<ResultCardView>().Setup(r);
        }
    }

    private void ShowButtons()
    {
        if (btnPullAgain != null) btnPullAgain.gameObject.SetActive(true);
        if (btnClose != null) btnClose.gameObject.SetActive(true);
    }

    private void Close()
    {
        canvasResult.SetActive(false);
        AudioManager.Instance.Play("sfx_ui_close");
    }
}
