using System.Collections;
using UnityEngine;

public class ActivarPantalla : MonoBehaviour
{
    [Header("Configuración")]
    [Tooltip("Arrastrá acá el objeto Emissives_low")]
    public MeshRenderer rendererPantallas;

    [Tooltip("Tiempo que tarda la luz en subir hasta arriba")]
    public float tiempoEncendido = 1.5f;

    // Estos valores dependen de tu ObjectHeight en el Shader. 
    // Jugá con el material a mano para ver en qué número se apaga y en cuál se llena.
    public float fillApagado = 0f;
    public float fillEncendido = 1f;

    private bool activado = false;
    private bool jugadorCerca = false;

    void Update()
    {
        // Se activa con la letra E
        if (jugadorCerca && Input.GetKeyDown(KeyCode.E) && !activado)
        {
            StartCoroutine(EncenderProgresivamente());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorCerca = false;
        }
    }

    private IEnumerator EncenderProgresivamente()
    {
        activado = true;
        float tiempo = 0f;

        // Almacenamos el material instanciado para modificar solo esta mesa
        Material materialMesa = rendererPantallas.material;

        while (tiempo < tiempoEncendido)
        {
            tiempo += Time.deltaTime;

            // Interpolamos el valor de Fill
            float progreso = Mathf.Lerp(fillApagado, fillEncendido, tiempo / tiempoEncendido);

            // Cambiamos la propiedad _Fill de tu Shader Graph
            materialMesa.SetFloat("_Fill", progreso);

            yield return null;
        }
    }
}