using UnityEngine;

public class MovimientoNaveCapital : MonoBehaviour
{
    [Header("Configuración de Movimiento Base")]
    public float velocidadAvance = 2f;
    public Vector3 ejeRotacion = new Vector3(0, 1, 0);
    public float velocidadRotacion = 0.5f;

    [Header("Deriva Espacial (Sensación de Masa)")]
    [Tooltip("Qué tan rápido hace el ciclo de subir y bajar")]
    public float velocidadDeriva = 0.3f;
    [Tooltip("Fuerza o distancia de ese movimiento vertical")]
    public float amplitudDeriva = 1.2f;

    private float tiempoDesfase;
    private float valorSenoAnterior; // <-- LA CLAVE DEL ARREGLO

    void Start()
    {
        tiempoDesfase = Random.Range(0f, 100f);

        // Calculamos exactamente en qué punto de la onda empieza la nave
        valorSenoAnterior = Mathf.Sin(tiempoDesfase * velocidadDeriva) * amplitudDeriva;
    }

    void Update()
    {
        // 1. Movimiento constante hacia adelante
        transform.Translate(Vector3.forward * velocidadAvance * Time.deltaTime);

        // 2. Rotación súper lenta
        transform.Rotate(ejeRotacion * velocidadRotacion * Time.deltaTime);

        // 3. Efecto de Deriva (VERSIÓN CORREGIDA SIN DESFASE)

        // Buscamos dónde debería estar la nave en ESTE frame exacto
        float valorSenoActual = Mathf.Sin((Time.time + tiempoDesfase) * velocidadDeriva) * amplitudDeriva;

        // Sacamos la diferencia matemática entre el frame anterior y este
        float diferenciaVertical = valorSenoActual - valorSenoAnterior;

        // Movemos la nave ESA diferencia exacta (ACÁ NO va el Time.deltaTime)
        transform.Translate(Vector3.up * diferenciaVertical, Space.Self);

        // Guardamos el valor actual para el próximo frame
        valorSenoAnterior = valorSenoActual;
    }
}