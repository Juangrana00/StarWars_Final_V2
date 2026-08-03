using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    [SerializeField] float _duration = 0.1f, _magnitude = 0.2f;
    [SerializeField] AnimationCurve _animCurve;
    Coroutine _shakeCoroutine;

    public void StartShake()
    {
        if (_shakeCoroutine != null) return;
        _shakeCoroutine = StartCoroutine(ShakeCoroutine());
    }

    IEnumerator ShakeCoroutine()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < _duration)
        {
            elapsedTime += Time.deltaTime;
            startPosition = transform.position;
            float strength = _animCurve.Evaluate(elapsedTime / _duration) * _magnitude;
            transform.position = startPosition + Random.insideUnitSphere * strength;
            yield return null;
        }

        transform.position = startPosition;
        _shakeCoroutine = null;
    }
}
