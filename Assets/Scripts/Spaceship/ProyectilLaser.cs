using UnityEngine;

public class ProyectilLaser : MonoBehaviour
{
    [Header("Configuración de Vuelo")]
    public float velocidad = 200f;
    public float tiempoDeVida = 3f;

    [Header("Efectos Visuales")]
    [Tooltip("Arrastrá acá tu prefab VFX_Explosion_Laser")]
    public GameObject prefabExplosion;

    private void Start()
    {
        // En vez de apagarlo, lo DESTRUIMOS a los 3 segundos para limpiar la RAM
        Destroy(gameObject, tiempoDeVida);
    }

    private void Update()
    {
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Nave"))
        {
            if (prefabExplosion != null)
            {
                Instantiate(prefabExplosion, transform.position, transform.rotation);
            }

            // Desactivamos la nave enemiga para que vuelva a su Pool
            other.gameObject.SetActive(false);

            // Destruimos este láser inmediatamente al chocar
            Destroy(gameObject);
        }
    }
}