using UnityEngine;
using TMPro;

public class CurrencyDisplay : MonoBehaviour
{
    public TextMeshProUGUI gemText;
    private int _displayValue;

    void OnEnable()
    {
        if (PlayerInventory.Instance != null)
        {
            PlayerInventory.Instance.OnGemsChanged += OnGemsChanged;
            _displayValue = PlayerInventory.Instance.Gems;
            gemText.text = _displayValue.ToString("N0");
        }
    }

    void OnDisable()
    {
        if (PlayerInventory.Instance != null)
            PlayerInventory.Instance.OnGemsChanged -= OnGemsChanged;
    }

    private void OnGemsChanged(int newValue)
    {
        StopAllCoroutines();
        StartCoroutine(AnimateTo(newValue));
    }

    private System.Collections.IEnumerator AnimateTo(int target)
    {
        float t = 0f, duration = 0.4f;
        int from = _displayValue;
        while (t < duration)
        {
            t += Time.deltaTime;
            int v = Mathf.RoundToInt(Mathf.Lerp(from, target, t / duration));
            gemText.text = v.ToString("N0");
            yield return null;
        }
        _displayValue = target;
        gemText.text = target.ToString("N0");
    }
}
