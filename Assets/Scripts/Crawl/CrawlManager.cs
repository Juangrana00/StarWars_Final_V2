using UnityEngine;
using UnityEngine.SceneManagement;

public class CrawlManager : MonoBehaviour
{
    [Header("Configuración del Texto")]
    public RectTransform textoCrawl; // Asigná el texto (TextMeshPro)
    public float velocidadTexto = 30f;
    public float limiteYParaEmpezar = 1500f; // Ajustá este número según qué tan largo sea tu texto

    [Header("Escena a Cargar")]
    public string nombreEscenaNivel = "EscenaNivel"; // Poné el nombre exacto de tu escena de juego

    void Update()
    {
        // Hace que el texto suba constantemente
        textoCrawl.Translate(Vector3.up * velocidadTexto * Time.deltaTime);

        // Termina si el texto llega al límite Y, o si el jugador aprieta Espacio/Enter para saltar la intro
        if (textoCrawl.anchoredPosition.y > limiteYParaEmpezar || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return))
        {
            CargarNivel();
        }
    }

    private void CargarNivel()
    {
        SceneManager.LoadScene(nombreEscenaNivel);
    }
}