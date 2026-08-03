using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDroid : EnemyGun, IParent
{
    [Header("Enemy Vars")]
    [SerializeField] GameObject _bulletPrefab;
    [SerializeField] Animator _animator;
    [SerializeField] string _lookName, _walkName, _attackName, _deathName, _detectName, _fireName;
    [SerializeField] float _patrolDelay, _minDistance, _timeAtPoint, _hitRange, _rotationSpeed;
    [SerializeField] List<Transform> _waypoints = new();
    [SerializeField] List<Transform> _attackPoints = new();
    private Vector3 _destiny;
    private Coroutine _boolCoroutine, _patrolCoroutine, _rotCoroutine, _fovRotCoroutine, _noTargetCoroutine, _longAttackCoroutine;
    private Transform _target;
    private bool _canPatrol = false, _isDead = false, _isFov = false;
    private float _secondsToNormal = 6;

    private void Start()
    {
        onView += TargetDetected;
        outOfView += TargetWasDetected;
        fov.ActionsToDo(onView, outOfView);
        ConstructGun(CalculateDirection(), _bulletPrefab);
        gun.GunStart();
    }

    public override void ActivateEnemy(Transform position)
    {
        ActivateCoroutine(ref updateCoroutine, myUpdate(), true);
        playerPos = position.position;
        Chase(playerPos);
        fov.StartSearch();
    }

    public override void TakeDamage(float damage)
    {
        life -= damage;
        Debug.Log(this.name + ": " + life);

        if(life <= 0)
        {
            _isDead = true;
            StopPatrol();
            StopChase();
            StopAllCoroutines();
            _animator.SetBool(_deathName, true);
        }
    }

    public override void Death()
    {
        Destroy(gameObject);
    }

    private IEnumerator myUpdate()
    {
        while(_isDead == false)
        {
            yield return new WaitForSeconds(updateCooldown);
            DecisionRange();
        }
    }

    private void DecisionRange()
    {
        CalculateDistance();
        SightRadius();
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if (_isFov) return;

        if (!playerInSightRange && !playerInAttackRange)
        {
            PatrolLogic();
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            if (_rotCoroutine != null)
            {
                return;
            }
            else
            {
                ActivateCoroutine(ref _rotCoroutine, LookAtTarget(), true);
            }
        }
        else if (playerInSightRange && playerInAttackRange)
        {
            CloseAttack();
        }
    }

    private void Chase(Vector3 position)
    {
        _destiny = position;
        _animator.SetBool(_walkName, true);
        navAgent.SetDestination(position);
    }

    private void StopChase()
    {
        _animator.SetBool(_walkName, false);
        navAgent.SetDestination(transform.position);
    }

    private IEnumerator boolCountdown()
    {
        yield return new WaitForSeconds(_patrolDelay);
        _canPatrol = true;
        _boolCoroutine = null;
    }

    private IEnumerator Patrol()
    {
        int index = 0;

        while(true)
        {
            if(index < _waypoints.Count)
            {
                Vector3 waypointPos = _waypoints[index].position;
                float distance = Vector3.Distance(transform.position, waypointPos);

                if (distance > _minDistance)
                {
                    Chase(waypointPos);
                    yield return null;
                }
                else
                {
                    StopChase();
                    _animator.SetTrigger(_lookName);
                    yield return new WaitForSeconds(_timeAtPoint);
                    index++;
                }
            }
            else
            {
                index = 0;
                yield return null;
            }
        }
    }

    private void StopPatrol()
    {
        ActivateCoroutine(ref _patrolCoroutine, Patrol(), false);
    }

    private void CalculateDistance()
    {
        float distance = Vector3.Distance(_destiny, transform.position);

        if (distance > _minDistance)
        {
            return;
        }
        else
        {
            StopChase();
        }
    }

    private void SightRadius()
    {
        Collider[] colls = Physics.OverlapSphere(transform.position, sightRange, playerLayer);

        if(colls.Length > 0 )
        {
            playerPos = colls[0].transform.position;
            playerInSightRange = true;
            _target = colls[0].transform;
        }
        else
        {
            playerInSightRange = false;
            ActivateCoroutine(ref _rotCoroutine, LookAtTarget(), false);
        }
    }

    private void PatrolLogic()
    {
        if (_canPatrol)
        {
            if (_patrolCoroutine != null)
            {
                return;
            }
            else
            {
                ActivateCoroutine(ref _patrolCoroutine, Patrol(), true);
            }
        }
        else
        {
            if (_boolCoroutine != null)
            {
                return;
            }
            else
            {
                _boolCoroutine = StartCoroutine(boolCountdown());
            }
        }
    }

    private void CloseAttack()
    {
        StopChase();
        StopPatrol();

        if(!alreadyAttacked)
        {
            _animator.SetBool(_attackName, true);
            alreadyAttacked = true;
            StartCoroutine(CloseAttackCooldown());
        }
    }

    private IEnumerator CloseAttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        _animator.SetBool(_attackName, false);
        alreadyAttacked = false;
    }

    private void CloseAttackLogic()
    {
        foreach(Transform hit in _attackPoints)
        {
            RaycastHit rayHit;

            if(Physics.Raycast(hit.position, hit.forward, out rayHit, _hitRange, playerLayer))
            {
                rayHit.collider.gameObject.TryGetComponent(out Entity entity);

                if(entity != null)
                {
                    entity.TakeDamage(damage);
                }
            }
        }
    }

    public void ParentAction()
    {
        CloseAttackLogic();
    }

    public void ParentDeath()
    {
        Death();
    }

    private IEnumerator LookAtTarget()
    {
        while(!playerInAttackRange)
        {
            yield return null;
            Vector3 dir = (_target.position - transform.position).normalized;
            Quaternion targetRot = Quaternion.LookRotation(dir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * _rotationSpeed);
        }
    }

    private void ActivateCoroutine(ref Coroutine coroutineVar, IEnumerator coroutine, bool wantToActivate)
    {
        if(wantToActivate)
        {
            if (coroutineVar != null)
            {
                StopCoroutine(coroutineVar);
                coroutineVar = null;
                coroutineVar = StartCoroutine(coroutine);
            }
            else
            {
                coroutineVar = StartCoroutine(coroutine);
            }
        }
        else
        {
            if(coroutineVar != null)
            {
                StopCoroutine(coroutineVar);
                coroutineVar = null;
            }
        }
    }

    private void TargetDetected()
    {
        StopChase();
        StopPatrol();

        if(!playerInAttackRange)
        {
            _isFov = true;
            ActivateCoroutine(ref _fovRotCoroutine, fov.Test(), true);
            ActivateCoroutine(ref _longAttackCoroutine, LongAttack(), true);
        }
    }

    private void TargetWasDetected()
    {
        ActivateCoroutine(ref _noTargetCoroutine, GoBackToNormal(_secondsToNormal), true);
    }

    private IEnumerator GoBackToNormal(float seconds)
    {
        ActivateCoroutine(ref _longAttackCoroutine, LongAttack(), false);
        var lastPos = fov.ReturnLastTarget();
        Chase(lastPos.position);
        yield return new WaitForSeconds(seconds);
        _isFov = false;
        ActivateCoroutine(ref _fovRotCoroutine, fov.Test(), false);
    }

    private Vector3 CalculateDirection()
    {
        var target = fov.ReturnTarget();

        if(target != null)
        {
            var dir = (target.position - transform.position).normalized;
            return dir;
        }
        else
        {
            var dir = transform.forward;
            return dir;
        }
    }

    private IEnumerator LongAttack()
    {
        while(true)
        {
            yield return null;
            CalculateDirection();
            gun.CanShoot();
            gun.OutOfBullets();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        foreach (Transform hit in _attackPoints)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawRay(hit.position, hit.forward * _hitRange);
        }
    }
}
