using System;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : Entity
{
    [Header("References")]
    public NavMeshAgent navAgent;
    public LayerMask playerLayer, obstacleMask;
    protected Vector3 playerPos;

    [Header("Attack Vars")]
    public float timeBetweenAttacks; 
    public float sightRange, attackRange, updateCooldown, damage;
    protected bool alreadyAttacked = false, playerInAttackRange, playerInSightRange;
    protected Coroutine updateCoroutine;

    [Header("Field Of View")]
    public FOV fov;
    protected Action onView, outOfView;

    public abstract void ActivateEnemy(Transform position);
}
