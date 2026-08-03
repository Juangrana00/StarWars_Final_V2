using System.Collections;
using TMPro;
using UnityEngine;

public class Gun
{
    //External Vars
    private MonoBehaviour _mb;
    private Camera _camera;
    private Transform _attackPoint;
    private TextMeshProUGUI _text;
    private GameObject _laser;
    private Shake _shake;
    private LayerMask _layerMask;
    private Vector3 _personalizedDirection;
    private float _damage;
    private float _timeBetweenShooting;
    private float _spread;
    private float _range;
    private float _reloadTime;
    private float _timeBetweenShots;
    private int _magazineSize;
    private int _bulletsPerTap;
    private bool _usesCamera;

    //Internal Vars
    private RaycastHit _raycastHit;
    private int _bulletsLeft;
    private int _bulletsShot;
    private bool _readyToShoot;
    private bool _reloading;

    public Gun(MonoBehaviour monobehaviour, Camera camera, Transform attackPoint, TextMeshProUGUI text, LayerMask layerMask, float damage, float timeBetweenShooting, float spread, float range, float reloadTime, float timeBetweenShots, int magazineSize, int bulletsPerTap, bool usesCamera, Vector3 direction, GameObject laser, Shake shake)
    {
        _mb = monobehaviour;
        _camera = camera;
        _attackPoint = attackPoint;
        _text = text;
        _layerMask = layerMask;
        _damage = damage;
        _timeBetweenShooting = timeBetweenShooting;
        _spread = spread;
        _range = range;
        _reloadTime = reloadTime;
        _timeBetweenShots = timeBetweenShots;
        _magazineSize = magazineSize;
        _bulletsPerTap = bulletsPerTap;
        _usesCamera = usesCamera;
        _personalizedDirection = direction;
        _laser = laser;
        _shake = shake;
    }

    public void GunStart()
    {
        _bulletsLeft = _magazineSize;
        _readyToShoot = true;
    }

    public void CanReload()
    {
        if (_bulletsLeft < _magazineSize && !_reloading)
        {
            Reload();
        }
    }

    public void Reload()
    {
        _reloading = true;
        _mb.StartCoroutine(ReloadOrShootFinished(_reloadTime, true));
    }

    public void CanShoot()
    {
        if(_readyToShoot && !_reloading && _bulletsLeft > 0)
        {
            _bulletsShot = _bulletsPerTap;
            Shoot();
        }
    }

    private void Shoot()
    {
        _readyToShoot = false;

        if(_usesCamera)
        {
            ShootLogic(_attackPoint.transform, _camera.transform.forward, true);
        }
        else
        {
            ShootLogic(_attackPoint.transform, _personalizedDirection, false);
        }

        _bulletsLeft--;
        _bulletsShot--;
        _mb.StartCoroutine(ReloadOrShootFinished(_timeBetweenShooting, false));

        if(_bulletsShot > 0 && _bulletsLeft > 0)
        {
            _mb.StartCoroutine(MultiShoot());
        }
    }

   private void ShootLogic(Transform transform, Vector3 dir, bool usesShake)
    {
        float xAxis = Random.Range(-_spread, _spread);
        float yAxis = Random.Range(-_spread, _spread);
        Vector3 direction = dir + new Vector3(xAxis, yAxis, 0);

        var laser = UnityEngine.Object.Instantiate(_laser, transform.position, Quaternion.LookRotation(direction) * Quaternion.Euler(90, 0, 0));
        laser.TryGetComponent(out Rigidbody rb);
        rb.velocity = direction * 50f;

        if (Physics.Raycast(transform.position, direction, out _raycastHit, _range, _layerMask))
        {
            _raycastHit.collider.gameObject.TryGetComponent(out Entity entity);
            _raycastHit.collider.gameObject.TryGetComponent(out IDamageable damageable);
            Debug.Log(_raycastHit.collider.gameObject.name);

            if (entity != null)
            {
                entity.TakeDamage(_damage);
            }

            if(damageable != null)
            {
                damageable.TakeDamage(_damage);
            }
        }

        if(usesShake)
        {
            _shake.StartShake();
        }
    }

    private IEnumerator ReloadOrShootFinished(float cooldown, bool afterReload)
    {
        yield return new WaitForSeconds(cooldown);

        if(afterReload)
        {
            _bulletsLeft = _magazineSize;
            _reloading = false;
        }
        else
        {
            _readyToShoot = true;
        }
    }

    private IEnumerator MultiShoot()
    {
        yield return new WaitForSeconds(_timeBetweenShots);
        Shoot();
    }

    public void SetText()
    {
        if(_usesCamera)
        {
            _text.SetText(_bulletsLeft + " / " + _magazineSize);
        }
    }

    public void OutOfBullets()
    {
        if(_bulletsLeft <= 0 && !_reloading)
        {
            Reload();
        }
    }
}
