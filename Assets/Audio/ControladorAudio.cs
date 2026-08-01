using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI; // Agregamos esto para poder hablarle al Slider

public class ControladorAudio : MonoBehaviour
{
    [Header("Configuración")]
    [SerializeField] private AudioMixer _mixerPrincipal;
    [SerializeField] private Slider _sliderVolumen; // Arrastrá tu slider acá en el Inspector

    private void Start()
    {
        // Busca si hay un volumen guardado. Si es la primera vez que juega, ponele 1.
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        
        // Actualiza el slider visualmente, lo que automáticamente llama a CambiarVolumenMaster
        if(_sliderVolumen != null)
        {
            _sliderVolumen.value = volumenGuardado;
        }
    }

    public void CambiarVolumenMaster(float valorSlider)
    {
        _mixerPrincipal.SetFloat("VolumenMaster", Mathf.Log10(valorSlider) * 20f);
        
        // Guarda el valor para la próxima escena o cuando vuelva a abrir el juego
        PlayerPrefs.SetFloat("VolumenMusica", valorSlider);
        PlayerPrefs.Save();
    }
}