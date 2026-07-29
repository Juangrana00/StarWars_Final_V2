using UnityEngine;

public class DañoProgresivoNave : MonoBehaviour
{
    [Header("Configuración de Daño")]
    [Tooltip("Cantidad de impactos necesarios para prender un efecto")]
    public int impactosParaEfecto = 3;
    private int impactosAcumulados = 0;

    [Header("Fase 1: Humos (Daño Leve/Moderado)")]
    public GameObject[] focosDeHumo;
    private int indiceHumo = 0;

    [Header("Fase 2: Fuegos (Daño Crítico)")]
    public GameObject[] focosDeFuego;
    private int indiceFuego = 0;

    public void RecibirImpacto()
    {
        impactosAcumulados++;
        Debug.Log("Impacto recibido en " + gameObject.name + ". Acumulados para el próximo efecto: " + impactosAcumulados);

        // Si llega a la cantidad de impactos, intentamos prender un efecto
        if (impactosAcumulados >= impactosParaEfecto)
        {
            EncenderSiguienteEfecto();
            impactosAcumulados = 0; // Reiniciamos la cuenta para el próximo
        }
    }

    private void EncenderSiguienteEfecto()
    {
        // Primero verificamos si quedan humos por prender
        if (indiceHumo < focosDeHumo.Length)
        {
            if (focosDeHumo[indiceHumo] != null)
            {
                focosDeHumo[indiceHumo].SetActive(true);
                Debug.Log("Humo encendido: Índice " + indiceHumo + " en " + gameObject.name);
            }
            indiceHumo++;
        }
        // Si ya no hay más humos, pasamos a prender los fuegos
        else if (indiceFuego < focosDeFuego.Length)
        {
            if (focosDeFuego[indiceFuego] != null)
            {
                focosDeFuego[indiceFuego].SetActive(true);
                Debug.Log("¡Fuego crítico encendido!: Índice " + indiceFuego + " en " + gameObject.name);
            }
            indiceFuego++;
        }
        else
        {
            Debug.Log("El sistema de " + gameObject.name + " ya no tiene más efectos para mostrar. ¡Está destruido!");
        }
    }
}