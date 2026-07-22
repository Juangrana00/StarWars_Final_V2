using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyC110P : Enemy, IParent
{
    [Header("Enemy Vars")]
    [SerializeField] Animator _animator;
    [SerializeField] float _hitRange, _patrolCountdown = 5;
    [SerializeField] string _attackName, _deathName, _patrolName, _walkName;
    [SerializeField] Transform _pointOfView;
    [SerializeField] List<Transform> _attackPoints = new List<Transform>();

    private Coroutine _boolCoroutine = null;
    private bool _isDead = false, _isPatroling = false, _canPatrol = false;

    private void Start()
    {
        onView += Chase;
        fov.ActionsToDo(onView, outOfView);
    }

    public override void TakeDamage(float damage)
    {
        life -= damage;
        Debug.Log( this.name + ": " + life);

        if (life <= 0)
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
        while (true)
        {
            yield return new WaitForSeconds(updateCooldown);
            DecisionRange();
        }
    }

    private void DecisionRange()
    {
        if (_isDead) return;
        Collider[] colls = Physics.OverlapSphere(transform.position, sightRange, playerLayer);

        if(colls.Length > 0)
        {
            playerPos = colls[0].transform.position;
            playerInSightRange = true;
        }
        else
        {
            playerInSightRange = false;
        }

        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, playerLayer);

        if(!playerInSightRange && !playerInAttackRange)
        {
            if(_canPatrol)
            {
                Patrol();
            }
            else
            {
                if(_boolCoroutine != null)
                {
                    return;
                }
                else
                {
                    _boolCoroutine = StartCoroutine(BoolCountdown());
                }
            }
        }
        else if (playerInSightRange && !playerInAttackRange)
        {
            Chase();
        }
        else if (playerInSightRange && playerInAttackRange)
        {
            Attack();
        }
    }

    private void Attack()
    {
        StopPatrol();
        StopChase();

        if(!alreadyAttacked)
        {
            _animator.SetBool(_attackName, true);
            alreadyAttacked = true;
            StartCoroutine(AttackCooldown());
        }
    }

    private void Chase()
    {
        StopPatrol();
        Transform target = fov.ReturnTarget();

        if (target != null)
        {
            playerPos = target.position;
        }

        _animator.SetBool(_walkName, true);
        navAgent.SetDestination(playerPos);
    }

    public override void ActivateEnemy(Transform position)
    {
        if(updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
            updateCoroutine = StartCoroutine(myUpdate());
        }
        else
        {
            updateCoroutine = StartCoroutine(myUpdate());
        }

        playerPos = position.position;
        Chase();
    }

    private IEnumerator AttackCooldown()
    {
        yield return new WaitForSeconds(timeBetweenAttacks);
        _animator.SetBool(_attackName, false);
        alreadyAttacked = false;
    }

    public void AttackPoints()
    {
        foreach (Transform hit in _attackPoints)
        {
            RaycastHit raycastHit;

            if (Physics.Raycast(hit.position, hit.forward, out raycastHit, _hitRange, playerLayer))
            {
                var entity = raycastHit.collider.gameObject.GetComponent<Entity>();

                if (entity != null)
                {
                    entity.TakeDamage(damage);
                }
            }
        }
    }

    public void ParentAction()
    {
        AttackPoints();
    }

    public void ParentDeath()
    {
        Death();
    }

    private void Patrol()
    {
        if (_isPatroling) return;

        _isPatroling = true;
        StopChase();
        _animator.SetBool(_patrolName, true);
        fov.StartSearch();
    }

    private void StopPatrol()
    {
        _canPatrol = false;
        _isPatroling = false;
        _animator.SetBool(_patrolName, false);
        fov.StopSearch();
    }

    private void StopChase()
    {
        _animator.SetBool(_walkName, false);
        navAgent.SetDestination(transform.position);
    }

    private IEnumerator BoolCountdown()
    {
        yield return new WaitForSeconds(_patrolCountdown);
        _canPatrol = true;
        _boolCoroutine = null;
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
