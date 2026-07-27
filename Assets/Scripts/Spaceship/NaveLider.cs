using UnityEngine;

public class NaveLider : MonoBehaviour
{
    [Header("Rangos de Velocidad")]
    public float velocidadMin = 35f;
    public float velocidadMax = 50f;
    public float giroMin = 2f;
    public float giroMax = 4f;

    public float tiempoDeVida = 12f;

    private float velocidadActual;
    private float giroActual;
    private float tiempoCambioDireccion;
    private Vector3 direccionDestino;

    void OnEnable()
    {
        // Cada vez que revive del pool, elige estadísticas nuevas al azar
        velocidadActual = Random.Range(velocidadMin, velocidadMax);
        giroActual = Random.Range(giroMin, giroMax);

        Invoke(nameof(ApagarNave), tiempoDeVida);
        direccionDestino = transform.forward;
    }

    void Update()
    {
        if (Time.time > tiempoCambioDireccion)
        {
            Vector3 direccionAleatoria = transform.forward + Random.insideUnitSphere * 0.8f;
            direccionDestino = direccionAleatoria.normalized;
            tiempoCambioDireccion = Time.time + Random.Range(1f, 2f); // Tiempo de giro también aleatorio
        }

        if (direccionDestino != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccionDestino);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, giroActual * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * velocidadActual * Time.deltaTime);
    }

    void ApagarNave()
    {
        gameObject.SetActive(false);
    }

    void OnDisable()
    {
        CancelInvoke();
    }
}