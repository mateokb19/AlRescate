using System.Collections;
using UnityEngine;

public class GachaMachineShake : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private static readonly int IsSpinningHash = Animator.StringToHash("IsSpinning");
    private Coroutine _shakeCo;
    private Vector3 _baseScale;
    private bool _baseCaptured;

    void Reset() { animator = GetComponent<Animator>(); }

    void Awake()
    {
        if (animator == null) animator = GetComponent<Animator>();
        CaptureBaseScale();
    }

    private void CaptureBaseScale()
    {
        if (_baseCaptured) return;
        _baseScale = transform.localScale;
        _baseCaptured = true;
    }

    public void StartSpin()
    {
        CaptureBaseScale();
        if (animator == null)
        {
            Debug.LogWarning("[GachaMachineShake] Animator no asignado. Solo se ejecutara el shake.");
        }
        else if (animator.runtimeAnimatorController == null)
        {
            Debug.LogWarning("[GachaMachineShake] El Animator no tiene RuntimeAnimatorController. Asigna GachaMachine_AnimatorController.");
        }
        else
        {
            animator.SetBool(IsSpinningHash, true);
            Debug.Log("[GachaMachineShake] StartSpin -> IsSpinning=true");
        }
        if (_shakeCo != null) StopCoroutine(_shakeCo);
        _shakeCo = StartCoroutine(ShakeLoop());
    }

    public void StopSpin()
    {
        if (animator != null && animator.runtimeAnimatorController != null)
        {
            animator.SetBool(IsSpinningHash, false);
            Debug.Log("[GachaMachineShake] StopSpin -> IsSpinning=false");
        }
        if (_shakeCo != null) { StopCoroutine(_shakeCo); _shakeCo = null; }
        if (_baseCaptured) transform.localScale = _baseScale;
    }

    // Retrocompatibilidad
    public void PlaySpin()
    {
        CaptureBaseScale();
        if (_shakeCo != null) StopCoroutine(_shakeCo);
        _shakeCo = StartCoroutine(ShakeOnce(0.45f));
    }

    private IEnumerator ShakeLoop()
    {
        float t = 0f;
        while (true)
        {
            t += Time.deltaTime;
            float shake = Mathf.Sin(t * Mathf.PI * 8f) * 0.05f;
            transform.localScale = new Vector3(_baseScale.x * (1f + shake), _baseScale.y * (1f - shake), _baseScale.z);
            yield return null;
        }
    }

    private IEnumerator ShakeOnce(float duration)
    {
        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float shake = Mathf.Sin(t * Mathf.PI * 6f) * (1f - t) * 0.12f;
            transform.localScale = new Vector3(_baseScale.x * (1f + shake), _baseScale.y * (1f - shake), _baseScale.z);
            yield return null;
        }
        transform.localScale = _baseScale;
        _shakeCo = null;
    }
}
