using UnityEngine;
using UnityEngine.Audio;

public class IniciadorAudioNivel : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixerPrincipal;

    void Start()
    {
        // Lee el volumen guardado (si no hay ninguno, asume 1 por defecto)
        float volumenGuardado = PlayerPrefs.GetFloat("VolumenMusica", 1f);
        
        // Le aplica la fórmula logarítmica al Mixer
        _mixerPrincipal.SetFloat("VolumenMaster", Mathf.Log10(volumenGuardado) * 20f);
    }
}