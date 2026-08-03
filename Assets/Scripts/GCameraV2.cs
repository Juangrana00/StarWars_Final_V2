using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GCameraV2 : MonoBehaviour, IDamageable
{
    [Header("Camera Vars")]
    [SerializeField] GameObject _parent;
    [SerializeField] Animator _animator;
    [SerializeField] FOV _fov;
    [SerializeField] float _secondsBeforeRestart = 3.5f, _maxLife = 4;
    private Action onView, outOfView;
    private float _life;

    private void Start()
    {
        onView += OnDetect;
        _fov.ActionsToDo(onView, outOfView);
        _fov.StartSearch();
        _animator.speed = 0.5f;
        _life = _maxLife;
    }

    private void OnDetect()
    {
        _animator.enabled = false;
        StartCoroutine(TimeBeforeRestart(_secondsBeforeRestart));
    }

    private IEnumerator TimeBeforeRestart(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TakeDamage(float damage)
    {
        _life -= damage;
        Debug.Log("CAMERA LIFE: " + _life);

        if (_life <= 0)
        {
            Destroy(_parent);
        }
    }
}
