using System.Collections;
using UnityEngine;

public class GachaMachineShake : MonoBehaviour
{
    public void PlaySpin()
    {
        StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        Vector3 origin = transform.localScale;
        float duration = 0.45f;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float shake = Mathf.Sin(t * Mathf.PI * 6f) * (1f - t) * 0.12f;
            transform.localScale = new Vector3(origin.x * (1f + shake), origin.y * (1f - shake), origin.z);
            yield return null;
        }

        transform.localScale = origin;
    }
}
