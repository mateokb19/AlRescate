using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class CreditsScroller : MonoBehaviour
{
    public RectTransform creditsContent;
    public Button btnClose;
    public float scrollSpeed = 80f;

    Coroutine _coroutine;

    void Awake()
    {
        if (btnClose != null) btnClose.onClick.AddListener(Close);
    }

    void OnEnable()
    {
        if (_coroutine != null) StopCoroutine(_coroutine);
        _coroutine = StartCoroutine(ScrollUp());
    }

    void OnDisable()
    {
        if (_coroutine != null) { StopCoroutine(_coroutine); _coroutine = null; }
    }

    IEnumerator ScrollUp()
    {
        yield return null; // esperar 1 frame para que ContentSizeFitter calcule altura

        var panelRT = transform as RectTransform;
        float panelH = panelRT.rect.height;
        float contentH = creditsContent.rect.height;

        creditsContent.anchoredPosition = new Vector2(0, -contentH);

        while (creditsContent.anchoredPosition.y < panelH)
        {
            creditsContent.anchoredPosition += Vector2.up * scrollSpeed * Time.deltaTime;
            yield return null;
        }
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }
}
