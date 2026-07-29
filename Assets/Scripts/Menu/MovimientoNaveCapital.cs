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

    void Start()
    {
        // Generamos un tiempo de inicio aleatorio. 
        // Así, si tenés 3 cruceros juntos, no suben y bajan sincronizados como un coro.
        tiempoDesfase = Random.Range(0f, 100f);
    }

    void Update()
    {
        // 1. Movimiento constante hacia adelante (eje Z local)
        transform.Translate(Vector3.forward * velocidadAvance * Time.deltaTime);

        // 2. Rotación súper lenta sobre su eje
        transform.Rotate(ejeRotacion * velocidadRotacion * Time.deltaTime);

        // 3. Efecto de Deriva / Flotación
        // Calculamos una velocidad en el eje Y que sube y baja suavemente con el tiempo
        float velocidadVertical = Mathf.Sin((Time.time + tiempoDesfase) * velocidadDeriva) * amplitudDeriva;
        
        // Aplicamos ese movimiento extra al eje Y de la nave
        transform.Translate(Vector3.up * velocidadVertical * Time.deltaTime, Space.Self);
    }
}