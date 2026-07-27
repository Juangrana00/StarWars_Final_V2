using UnityEngine;

public class ProyectilLaser : MonoBehaviour
{
    public float velocidad = 150f;
    public float tiempoDeVida = 2f;

    void Start()
    {
        // El láser se destruye si no choca con nada en 2 segundos
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        // Movimiento del láser
        transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
    }

    // Esta función se ejecuta automáticamente cuando el Trigger del láser toca otro Collider
    void OnTriggerEnter(Collider other)
    {
        // Verificamos si el objeto con el que chocamos tiene la etiqueta "Nave"
        if (other.CompareTag("Nave"))
        {
            // Desactivamos la nave objetivo (aprovechando tu sistema de Object Pooling)
            other.gameObject.SetActive(false);

            // Opcional: Acá a futuro podés instanciar un prefab de partículas de explosión
            // Instantiate(prefabExplosion, transform.position, transform.rotation);

            // Destruimos el láser que acaba de impactar
            Destroy(gameObject);
        }
    }
}