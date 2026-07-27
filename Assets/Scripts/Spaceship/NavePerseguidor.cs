using UnityEngine;

public class NavePerseguidor : MonoBehaviour
{
    [Header("Rangos de Movimiento")]
    public float velocidadMin = 45f;
    public float velocidadMax = 60f;
    public float giroMin = 20f;
    public float giroMax = 35f;
    public float tiempoDeVida = 12f;

    [Header("Armamento")]
    public GameObject prefabLaser; // Acá arrastrás el Laser_Rebelde o Laser_Imperio
    public Transform[] puntosDeDisparo; // Acá arrastrás los Empty que hiciste en las alas
    public float cadenciaMin = 0.3f;
    public float cadenciaMax = 0.8f;

    private float velocidadActual;
    private float giroActual;
    private float proximoDisparo;

    [HideInInspector] public Transform objetivoLider;

    void OnEnable()
    {
        velocidadActual = Random.Range(velocidadMin, velocidadMax);
        giroActual = Random.Range(giroMin, giroMax);
        Invoke(nameof(ApagarNave), tiempoDeVida);
    }

    void Update()
    {
        if (objetivoLider == null || !objetivoLider.gameObject.activeInHierarchy)
        {
            transform.Translate(Vector3.forward * velocidadActual * Time.deltaTime);
            return;
        }

        Vector3 posicionFutura = objetivoLider.position + (objetivoLider.forward * 15f);
        Vector3 direccionHaciaObjetivo = posicionFutura - transform.position;

        if (direccionHaciaObjetivo != Vector3.zero)
        {
            Quaternion rotacionDeseada = Quaternion.LookRotation(direccionHaciaObjetivo);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionDeseada, giroActual * Time.deltaTime);
        }

        transform.Translate(Vector3.forward * velocidadActual * Time.deltaTime);

        // --- SISTEMA DE DISPARO ---
        // Verificamos si es momento de disparar
        if (Time.time >= proximoDisparo)
        {
            // Verificamos la distancia (solo dispara si está a menos de 120 metros)
            if (direccionHaciaObjetivo.magnitude < 120f)
            {
                // Calculamos el ángulo para saber si lo tiene en la mira (margen de 15 grados)
                float angulo = Vector3.Angle(transform.forward, direccionHaciaObjetivo);
                if (angulo < 15f)
                {
                    Disparar();
                    proximoDisparo = Time.time + Random.Range(cadenciaMin, cadenciaMax);
                }
            }
        }
    }

    void Disparar()
    {
        // Instancia un láser en cada punto de disparo que le hayas asignado
        foreach (Transform punto in puntosDeDisparo)
        {
            Instantiate(prefabLaser, punto.position, punto.rotation);
        }
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