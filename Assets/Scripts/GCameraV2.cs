using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GCameraV2 : MonoBehaviour, IDamageable
{
    [Header("Camera Vars")]
    [SerializeField] GameObject _parent;
    [SerializeField] Animator _animator;
    [SerializeField] FOV _fov;
    [SerializeField] float _secondsBeforeRestart = 3.5f, _maxLife = 4;

    [Header("VFX")]
    // CAMBIO 1: Agregamos corchetes [] para convertirlo en una lista de prefabs
    [SerializeField] GameObject[] _explosionPrefabs;

    private Action onView, outOfView;
    private float _life;
    private bool _isDead = false;

    private void Start()
    {
        onView += OnDetect;
        _fov.ActionsToDo(onView, outOfView);
        _fov.StartSearch();
        _animator.speed = 0.5f;
        _life = _maxLife;
    }

    private void OnDetect()
    {
        _animator.enabled = false;
        StartCoroutine(TimeBeforeRestart(_secondsBeforeRestart));
    }

    private IEnumerator TimeBeforeRestart(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TakeDamage(float damage)
    {
        if (_isDead) return;

        _life -= damage;
        Debug.Log("CAMERA LIFE: " + _life);

        if (_life <= 0)
        {
            _isDead = true;

            // CAMBIO 2: Chequeamos que la lista tenga al menos 1 elemento cargado
            if (_explosionPrefabs != null && _explosionPrefabs.Length > 0)
            {
                // Elegimos un número aleatorio entre 0 y la cantidad de elementos en la lista
                int randomIndex = UnityEngine.Random.Range(0, _explosionPrefabs.Length);

                // Agarramos el prefab ganador
                GameObject selectedVFX = _explosionPrefabs[randomIndex];

                // Lo instanciamos
                if (selectedVFX != null)
                {
                    GameObject vfx = Instantiate(selectedVFX, transform.position, transform.rotation);

                    // Opcional: Le decimos a Unity que lo borre a los 2 segundos para no llenar la escena de basura
                    Destroy(vfx, 2f);
                }
            }

            Destroy(_parent);
        }
    }
}