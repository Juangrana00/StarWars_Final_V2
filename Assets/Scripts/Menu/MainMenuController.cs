using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [Header("Paneles de UI")]
    public GameObject panelPrincipal; // Asigná el panel que tiene Play, Options, Exit
    public GameObject panelOpciones;  // Asigná el panel que tiene los sliders/toggles

    [Header("Configuración de Escenas")]
    [Tooltip("El número de la escena a cargar (Fijate en Build Settings)")]
    public int numeroEscenaCrawl = 1; // Acá ponés el número de la escena del texto

    void Start()
    {
        // Nos aseguramos de que arranque mostrando el menú correcto
        panelPrincipal.SetActive(true);
        panelOpciones.SetActive(false);
    }

    // --- BOTONES DEL MENÚ PRINCIPAL ---

    public void BotonPlay()
    {
        // Ahora carga la escena usando el número entero que definas en el Inspector
        SceneManager.LoadScene(numeroEscenaCrawl);
    }

    public void BotonOpciones()
    {
        // Apaga el menú principal y prende el de opciones
        panelPrincipal.SetActive(false);
        panelOpciones.SetActive(true);
    }

    public void BotonExit()
    {
        Application.Quit();
        Debug.Log("Cerrando el juego... (Esto solo se ve en el editor)");
    }

    // --- BOTONES DEL MENÚ DE OPCIONES ---

    public void BotonVolver()
    {
        // Vuelve al menú principal
        panelOpciones.SetActive(false);
        panelPrincipal.SetActive(true);
    }

    public void TogglePantallaCompleta(bool esPantallaCompleta)
    {
        Screen.fullScreen = esPantallaCompleta;
    }
}