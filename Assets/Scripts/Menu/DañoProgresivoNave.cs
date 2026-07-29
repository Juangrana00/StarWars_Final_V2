using UnityEngine;

public class DañoProgresivoNave : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [Tooltip("Cada cuántos impactos se prende un incendio nuevo")]
    public int impactosParaPrenderFuego = 3; 
    
    private int impactosAcumulados = 0;
    private int indiceFuegoActual = 0;

    [Header("Fuegos Acomodados a Mano")]
    public GameObject[] focosDeFuego; // Acá arrastrás los objetos que ya tenés en la escena

    void Start()
    {
        // Al darle Play, el código se asegura de apagarlos todos por vos
        foreach (GameObject fuego in focosDeFuego)
        {
            if (fuego != null) fuego.SetActive(false);
        }
    }

    // Le sacamos el (Vector3) porque ya no nos importa dónde pegó exactamente
    public void RecibirImpacto()
    {
        impactosAcumulados++;

        // Si llegó a la cantidad de golpes necesarios y todavía quedan fuegos apagados
        if (impactosAcumulados >= impactosParaPrenderFuego && indiceFuegoActual < focosDeFuego.Length)
        {
            // Prende el fuego que toca
            if (focosDeFuego[indiceFuegoActual] != null)
            {
                focosDeFuego[indiceFuegoActual].SetActive(true);
            }
            
            // Avanza al siguiente fuego de la lista y reinicia la cuenta
            indiceFuegoActual++;
            impactosAcumulados = 0; 
        }
    }
}