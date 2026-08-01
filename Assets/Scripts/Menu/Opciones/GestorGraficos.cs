using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GestorGraficos : MonoBehaviour
{
    [Header("Elementos de la UI")]
    [SerializeField] private TMP_Dropdown _dropdownResolucion;
    [SerializeField] private TMP_Dropdown _dropdownCalidad;
    [SerializeField] private Toggle _togglePantallaCompleta;

    private Resolution[] _resoluciones;

    void Start()
    {
        // 1. Configuramos el estado del Toggle de pantalla completa
        _togglePantallaCompleta.isOn = Screen.fullScreen;

        // 2. Configuramos el Dropdown de calidad gráfica
        _dropdownCalidad.value = QualitySettings.GetQualityLevel();

        // 3. Cargamos las resoluciones compatibles con el monitor
        ConfigurarResoluciones();
    }

    private void ConfigurarResoluciones()
    {
        // Unity detecta automáticamente las resoluciones que soporta la pantalla
        _resoluciones = Screen.resolutions;
        _dropdownResolucion.ClearOptions();

        List<string> opciones = new List<string>();
        int indiceResolucionActual = 0;

        for (int i = 0; i < _resoluciones.Length; i++)
        {
            string opcion = _resoluciones[i].width + " x " + _resoluciones[i].height;
            opciones.Add(opcion);

            // Verificamos cuál es la resolución actual para dejarla seleccionada
            if (_resoluciones[i].width == Screen.currentResolution.width &&
                _resoluciones[i].height == Screen.currentResolution.height)
            {
                indiceResolucionActual = i;
            }
        }

        _dropdownResolucion.AddOptions(opciones);
        _dropdownResolucion.value = indiceResolucionActual;
        _dropdownResolucion.RefreshShownValue();
    }

    // 👇 Estas 3 funciones son las que vamos a conectar a la UI 👇

    public void CambiarResolucion(int indiceResolucion)
    {
        Resolution resolucion = _resoluciones[indiceResolucion];
        Screen.SetResolution(resolucion.width, resolucion.height, Screen.fullScreen);
    }

    public void CambiarPantallaCompleta(bool esPantallaCompleta)
    {
        Screen.fullScreen = esPantallaCompleta;
    }

    public void CambiarCalidad(int indiceCalidad)
    {
        QualitySettings.SetQualityLevel(indiceCalidad);
    }
}