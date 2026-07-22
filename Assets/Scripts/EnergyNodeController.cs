using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class EnergyNodeController : MonoBehaviour
{
    [Header("Configuración de Interacción")]
    public KeyCode interactKey = KeyCode.G;
    public string playerTag = "Player";

    [Header("Configuración de Energía")]
    public float fillSpeed = 1.5f; 

    private Material nodeMaterial;
    private int fillPropertyID;
    private bool isPlayerNear = false;
    
    // Variables para controlar el estado
    private bool isTurnedOn = false;
    private Coroutine activeAnimation;

    void Start()
    {
        nodeMaterial = GetComponent<Renderer>().material;
        fillPropertyID = Shader.PropertyToID("_Fill"); 
        
        // Inicia apagado (1)
        nodeMaterial.SetFloat(fillPropertyID, 1f);
    }

    void Update()
    {
        // Si el jugador está cerca y presiona la tecla
        if (isPlayerNear && Input.GetKeyDown(interactKey))
        {
            // Invertimos el estado (si estaba apagado, lo pasamos a encendido, y viceversa)
            isTurnedOn = !isTurnedOn;
            
            // Si el jugador hace spam de la tecla G, frenamos la animación anterior 
            // para que no se superpongan y generen parpadeos
            if (activeAnimation != null)
            {
                StopCoroutine(activeAnimation);
            }
            
            // Iniciamos la transición hacia 0 (encendido) o 1 (apagado)
            float targetFill = isTurnedOn ? 0f : 1f;
            activeAnimation = StartCoroutine(ToggleCircuitRoutine(targetFill));
        }
    }

    private IEnumerator ToggleCircuitRoutine(float targetFill)
    {
        // Leemos en qué punto exacto quedó el shader por si lo interrumpimos a la mitad
        float currentFill = nodeMaterial.GetFloat(fillPropertyID);

        // Mientras no hayamos llegado al objetivo (con un margen mínimo de error)
        while (Mathf.Abs(currentFill - targetFill) > 0.001f)
        {
            // Movemos el valor suavemente hacia el objetivo
            currentFill = Mathf.MoveTowards(currentFill, targetFill, Time.deltaTime * fillSpeed);
            nodeMaterial.SetFloat(fillPropertyID, currentFill);
            
            yield return null; 
        }
        
        // Forzamos el valor exacto al final para que no queden decimales sueltos
        nodeMaterial.SetFloat(fillPropertyID, targetFill);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNear = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(playerTag))
        {
            isPlayerNear = false;
        }
    }
}