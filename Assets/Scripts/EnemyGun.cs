using UnityEngine;
using TMPro;

public abstract class EnemyGun : Enemy
{
    [Header("Gun Vars")]
    public Transform firePoint;
    public float gunDamage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    protected Gun gun;

    public virtual void ConstructGun()
    {
        TextMeshProUGUI text = new TextMeshProUGUI();
        gun = new Gun(this, Camera.main, firePoint, text, playerLayer, gunDamage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots, magazineSize, bulletsPerTap, false);
    }
}
