using UnityEngine;
using TMPro;

public class GunSystem : MonoBehaviour
{
    //Gun Stats
    public float damage;
    public float timeBetweenShooting;
    public float spread;
    public float range;
    public float reloadTime;
    public float timeBetweenShots;
    public int magazineSize;
    public int bulletsPerTap;
    public bool allowButtonHold;
    private int _bulletsLeft;
    private int _bulletsShot;

    //Bools
    private bool _shooting;
    private bool _readyToShoot;
    private bool _reloading;

    //References
    public Camera fpsCam;
    public Transform attackPoint;
    public RaycastHit rayHit;
    public LayerMask whatIsEnemy;

    //Text
    public TextMeshProUGUI text;

    private void Start()
    {
        _bulletsLeft = magazineSize;
        _readyToShoot = true;
    }

    private void Update()
    {
        MyInput();
        text.SetText(_bulletsLeft + " / " + magazineSize);
    }

    private void MyInput()
    {
        if(allowButtonHold)
        {
            _shooting = Input.GetKey(KeyCode.Mouse0);
        }
        else
        {
            _shooting = Input.GetKeyDown(KeyCode.Mouse0);
        }

        if(Input.GetKeyDown(KeyCode.R) && _bulletsLeft < magazineSize && !_reloading)
        {
            Reload();
        }

        //Shoot
        if(_readyToShoot &&  _shooting && !_reloading && _bulletsLeft > 0)
        {
            _bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    private void Reload()
    {
        _reloading = true;
        Invoke("ReloadFinished", reloadTime);
    }

    private void Shoot()
    {
        _readyToShoot = false;

        //Spread
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);

        //Calculate direction with spread
        Vector3 direction = fpsCam.transform.forward + new Vector3(x, y, 0);

        //RayCast
        if(Physics.Raycast(fpsCam.transform.position, direction, out rayHit, range, whatIsEnemy))
        {
            Debug.Log(rayHit.collider.name);

            if (rayHit.collider.CompareTag("Enemy"))
            {
                //rayHit.collider.GetComponent<ShootingAI>().TakeDamage(damage);
            }
        }

        _bulletsLeft--;
        _bulletsShot--;

        Invoke("ResetShot", timeBetweenShooting);

        if(_bulletsShot > 0 && _bulletsLeft > 0)
        {
            Invoke("Shoot", timeBetweenShots);
        }
    }

    private void ResetShot()
    {
        _readyToShoot = true;
    }

    private void ReloadFinished()
    {
        _bulletsLeft = magazineSize;
        _reloading = false;
    }
}
