using UnityEngine;

public class NaveLider : MonoBehaviour
{
    [Header("Rangos de Velocidad")]
    public float velocidadMin = 35f;
    public float velocidadMax = 50f;
    public float giroMin = 2f;
    public float giroMax = 4f;

    public float tiempoDeVida = 12f;

    [Header("Límites de la Pecera")]
    [Tooltip("El objeto central de la batalla. Puede ser el Director_Batalla vacío")]
    public Transform centroEscenario; 
    [Tooltip("Si se aleja más de estos metros, pega la vuelta")]
    public float limiteDistancia = 150f; 

    private float velocidadActual;
    private float giroActual;
    private float tiempoCambioDireccion;
    private Vector3 direccionDestino;

    void OnEnable()
    {
        velocidadActual = Random.Range(velocidadMin, velocidadMax);
        giroActual = Random.Range(giroMin, giroMax);
        Invoke(nameof(ApagarNave), tiempoDeVida);
        direccionDestino = transform.forward;

        // Si te olvidaste de asignarle el centro en el Inspector, busca al Spawner por defecto
        if (centroEscenario == null)
        {
            GameObject spawner = GameObject.Find("Director_Batalla"); // Cambiá este nombre si tu spawner se llama distinto
            if (spawner != null) centroEscenario = spawner.transform;
        }
    }

    void Update()
    {
        if (Time.time > tiempoCambioDireccion)
        {
            float distanciaAlCentro = 0f;
            if (centroEscenario != null)
            {
                distanciaAlCentro = Vector3.Distance(transform.position, centroEscenario.position);
            }

            // Si se salió del límite, la obligamos a mirar hacia el centro
            if (centroEscenario != null && distanciaAlCentro > limiteDistancia)
            {
                Vector3 direccionAlCentro = (centroEscenario.position - transform.position).normalized;
                
                // Le sumamos un poquito de ruido para que el giro no sea matemáticamente perfecto y aburrido
                direccionDestino = (direccionAlCentro + Random.insideUnitSphere * 0.2f).normalized;
            }
            else
            {
                // Comportamiento normal: vagar por el espacio aleatoriamente
                Vector3 direccionAleatoria = transform.forward + Random.insideUnitSphere * 0.8f;
                direccionDestino = direccionAleatoria.normalized;
            }

            tiempoCambioDireccion = Time.time + Random.Range(1f, 2f);
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