using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FOV : MonoBehaviour
{
    [Header("FOV Vars")]
    [SerializeField] private LayerMask _targetMask;
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private bool _takesBreaks, _stopsAfterTarget;
    [SerializeField] private float _searchCooldown, _waitCooldown;
    [HideInInspector] public List<Transform> visibleTargets = new();
    public float viewRadius, viewAngle;
    public int segments = 25;
    private Action _onView, _outOfView;
    private Coroutine _searchCoroutine, _waitCoroutine;
    private bool _isVisible = false, _wasVisible = false;
    private Transform _targetTransform;

    public void StartSearch()
    {
        if(_searchCoroutine != null)
        {
            StopCoroutine(_searchCoroutine);
            _searchCoroutine = null;
            _searchCoroutine = StartCoroutine(FindTargetsFrequency());
        }
        else
        {
            _searchCoroutine = StartCoroutine(FindTargetsFrequency());
        }
    }

    public void StopSearch()
    {
        if (_searchCoroutine != null)
        {
            StopCoroutine(_searchCoroutine);
            _searchCoroutine = null;
        }
    }

    private void FindVisibleTargets()
    {
        visibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, _targetMask);
        _isVisible = false;

        for(int i = 0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float distToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, distToTarget, _obstacleMask))
                {
                    visibleTargets.Add(target);
                    _isVisible = true;
                    _targetTransform = target;
                    Debug.Log("TARGET DETECTED"); //TRES LÍNEAS ORIGINALES
                }
            }
        }

        if (!_takesBreaks && !_stopsAfterTarget)
        {
            TargetFound();
        }
        else if(_isVisible)
        {
            if(_takesBreaks && !_stopsAfterTarget)
            {
                _waitCoroutine = StartCoroutine(TargetFoundCooldown());
            }
            else if(!_takesBreaks && _stopsAfterTarget)
            {
                _onView?.Invoke();
                StopSearch();
            }
        }
    }

    private IEnumerator FindTargetsFrequency()
    {
        while (true)
        {
            yield return new WaitForSeconds(_searchCooldown);
            FindVisibleTargets();
        }
    }

    private IEnumerator TargetFoundCooldown()
    {
        if(_isVisible)
        {
            _onView?.Invoke();
            StopSearch();
            yield return new WaitForSeconds(_waitCooldown);
            _outOfView.Invoke();
            StartSearch();
        }
    }

    private void TargetFound()
    {
        if(_isVisible)
        {
            _onView?.Invoke();
            _wasVisible = true;
        }
        else if(!_isVisible && _wasVisible)
        {
            _outOfView?.Invoke();
            _wasVisible = false;
        }
    }

    public Transform ReturnTarget()
    {
        if(visibleTargets.Count > 0)
        {
            return visibleTargets[0];
        }
        else return null;
    }

    public void ActionsToDo(Action onView, Action outOfView)
    {
        _onView = onView;
        _outOfView = outOfView;
    }

    public IEnumerator Test()
    {
        while(true)
        {
            yield return null;
            Vector3 dir = (_targetTransform.position - transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * 5);
        }
    }
}
