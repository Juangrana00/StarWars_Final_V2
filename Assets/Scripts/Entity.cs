using UnityEngine;

public abstract class Entity : MonoBehaviour
{
    [Header("Health")]
    public float maxLife;
    [HideInInspector] public float life;

    private void Awake()
    {
        life = maxLife;
    }

    public abstract void TakeDamage(float damage);

    public abstract void Death();
}
