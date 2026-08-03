using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameCamera : MonoBehaviour, IDamageable
{
    [Header("Camera Vars")]
    [SerializeField] GameObject _parent;
    [SerializeField] Animator _animator;
    [SerializeField] FOV _fov;
    [SerializeField] string _parameterName;
    [SerializeField] float _doorCooldown, _rotationSpeed, _maxLife = 4;
    [SerializeField] List<Animator> _doorAnimators = new List<Animator>();
    [SerializeField] List<Enemy> _enemiesHidden = new List<Enemy>();

    //Private vars
    Action onView, outOfView;
    Coroutine _delayCoroutine = null, _trackCoroutine = null;
    AnimatorStateInfo _savedState;
    Quaternion _initialRotation;
    bool _parameterValue = false, _firstTime = true;
    int _savedStateHash;
    float _savedNormalizedTime, _life;

    private void Start()
    {
        _initialRotation = transform.localRotation;
        onView += OnDetect;
        outOfView += WasDetected;
        _fov.ActionsToDo(onView, outOfView);
        _fov.StartSearch();
        _life = _maxLife;
    }

    private void OnDetect()
    {
        _savedState = _animator.GetCurrentAnimatorStateInfo(0);
        _savedStateHash = _savedState.fullPathHash;
        _savedNormalizedTime = _savedState.normalizedTime % 1;
        _animator.enabled = false;
        _parameterValue = !_parameterValue;

        if(_trackCoroutine == null)
        {
            _trackCoroutine = StartCoroutine(TrackingTarget());
        }
        else
        {
            return;
        }

        if (_doorAnimators.Count > 0)
        {
            if (_firstTime)
            {
                foreach (var animator in _doorAnimators)
                {
                    animator.SetBool(_parameterName, _parameterValue);
                    _firstTime = false;
                }

                if (_delayCoroutine == null)
                {
                    _delayCoroutine = StartCoroutine(CloseCooldown());
                }
            }
        }

        if (_enemiesHidden.Count > 0)
        {
            foreach (var enemy in _enemiesHidden)
            {
                if (enemy != null)
                {
                    enemy.ActivateEnemy(_fov.ReturnTarget());
                }
            }
        }
    }

    private void WasDetected()
    {
        if(_trackCoroutine != null)
        {
            StopCoroutine(_trackCoroutine);
            _trackCoroutine = null;
        }
        else
        {
            return;
        }

        transform.localRotation = _initialRotation;
        _animator.enabled = true;
        _animator.Play(_savedStateHash, 0, _savedNormalizedTime);
        _animator.speed = 1;
    }

    private IEnumerator CloseCooldown()
    {
        yield return new WaitForSeconds(_doorCooldown);
        _parameterValue = !_parameterValue;

        foreach (var animator in _doorAnimators)
        {
            animator.SetBool(_parameterName, _parameterValue);
        }
    }

    private IEnumerator TrackingTarget()
    {
        while(true)
        {
            Transform target = _fov.ReturnTarget();

            if (target != null)
            {
                yield return null;
                Vector3 dir = (target.position - transform.position).normalized;
                Quaternion targetRot = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
            }
        }
    }

    public void TakeDamage(float damage)
    {
        _life -= damage;
        Debug.Log("CAMERA LIFE: " + _life);

        if(_life <= 0)
        {
            Destroy(_parent);
        }
    }
}
