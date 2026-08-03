using UnityEngine;
using TMPro;

public abstract class EnemyGun : Enemy
{
    [Header("Gun Vars")]
    public Transform firePoint;
    public float gunDamage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots;
    public int magazineSize, bulletsPerTap;
    protected Gun gun;
    private Shake _shake;

    public virtual void ConstructGun(Vector3 direction, GameObject bulletPrefab)
    {
        TextMeshProUGUI text = new TextMeshProUGUI();
        gun = new Gun(this, Camera.main, firePoint, text, playerLayer, gunDamage, timeBetweenShooting, spread, range, reloadTime, timeBetweenShots, magazineSize, bulletsPerTap, false, direction, bulletPrefab, _shake);
    }
}
