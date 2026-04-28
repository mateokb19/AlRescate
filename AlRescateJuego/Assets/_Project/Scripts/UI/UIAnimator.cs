using System.Collections;
using UnityEngine;

public static class UIAnimator
{
    public static IEnumerator FadeIn(CanvasGroup cg, float duration = 0.3f)
    {
        cg.alpha = 0f;
        cg.gameObject.SetActive(true);
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Clamp01(t / duration);
            yield return null;
        }
        cg.alpha = 1f;
    }

    public static IEnumerator FadeOut(CanvasGroup cg, float duration = 0.2f, bool deactivateAtEnd = true)
    {
        float t = 0f, from = cg.alpha;
        while (t < duration)
        {
            t += Time.deltaTime;
            cg.alpha = Mathf.Lerp(from, 0f, t / duration);
            yield return null;
        }
        cg.alpha = 0f;
        if (deactivateAtEnd) cg.gameObject.SetActive(false);
    }

    public static IEnumerator ScalePop(RectTransform rt, float duration = 0.35f)
    {
        Vector3 from = Vector3.one * 0.6f;
        Vector3 to = Vector3.one;
        rt.localScale = from;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            float p = t / duration;
            float c1 = 1.70158f, c3 = c1 + 1f;
            float e = 1f + c3 * Mathf.Pow(p - 1, 3) + c1 * Mathf.Pow(p - 1, 2);
            rt.localScale = Vector3.LerpUnclamped(from, to, e);
            yield return null;
        }
        rt.localScale = to;
    }

    public static IEnumerator PulseBeat(RectTransform rt, float scale = 1.05f, float duration = 0.1f)
    {
        Vector3 original = rt.localScale;
        Vector3 big = original * scale;
        float t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(original, big, t / duration);
            yield return null;
        }
        t = 0f;
        while (t < duration)
        {
            t += Time.deltaTime;
            rt.localScale = Vector3.Lerp(big, original, t / duration);
            yield return null;
        }
        rt.localScale = original;
    }
}
