using UnityEngine;

public class DañoProgresivoNave : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [Tooltip("Cada cuántos impactos se prende el siguiente efecto")]
    public int impactosParaEfecto = 3;

    private int impactosAcumulados = 0;
    private int indiceDañoActual = 0;

    [Header("Efectos Secuenciales (Ordenados)")]
    [Tooltip("Poné los Prefabs de HUMO al principio, y los de FUEGO al final")]
    public GameObject[] focosDeDaño;

    void Start()
    {
        // Al darle Play, nos aseguramos de apagarlos todos
        foreach (GameObject foco in focosDeDaño)
        {
            if (foco != null) foco.SetActive(false);
        }
    }

    public void RecibirImpacto()
    {
        impactosAcumulados++;

        // Si llegó a la cantidad de golpes y quedan efectos por prender...
        if (impactosAcumulados >= impactosParaEfecto && indiceDañoActual < focosDeDaño.Length)
        {
            if (focosDeDaño[indiceDañoActual] != null)
            {
                focosDeDaño[indiceDañoActual].SetActive(true);
            }

            // Avanza al siguiente y reinicia la cuenta de impactos
            indiceDañoActual++;
            impactosAcumulados = 0;
        }
    }
}