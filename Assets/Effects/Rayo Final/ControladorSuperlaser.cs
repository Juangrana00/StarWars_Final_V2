using System.Collections;
using UnityEngine;

public class ControladorSuperlaser : MonoBehaviour
{
    [Header("Interacción y Cinemática")]
    public GameObject camaraJugador;
    public GameObject camaraInterna;
    public GameObject camaraExterna1; 
    public GameObject camaraExterna2; 
    
    private bool jugadorEnPanel = false;
    private bool secuenciaActiva = false;

    [Header("Tiempos de Pausa (Suspenso)")]
    [Tooltip("Segundos extra viendo el tubo interno antes de salir afuera")]
    public float pausaFinInterna = 1.0f; 
    [Tooltip("Segundos extra viendo los 6 rayos unidos antes del corte de cámara")]
    public float pausaFinConvergencia = 0.5f; 
    [Tooltip("Segundos en la cámara 2 antes de que salga el rayo principal")]
    public float pausaAntesDeDisparar = 0.5f; 

    [Header("Referencias Internas")]
    public AceleradorDeDisparo aceleradorInterno;

    [Header("Fase 1: Rayos Tributarios")]
    public LineRenderer[] rayosTributarios; 
    public Transform puntoDeConvergencia;   
    public float tiempoCrecimiento = 0.8f;  

    [Header("Fase 2: Rayo Principal")]
    public LineRenderer rayoPrincipal;
    public Transform naveObjetivo;
    public float tiempoCargaNucleo = 0.5f; 
    public float velocidadDisparoPrincipal = 0.2f;

    [Header("Efectos Finales")]
    public ParticleSystem explosionNave; 

    void Start()
    {
        if (rayoPrincipal != null) rayoPrincipal.positionCount = 0;
    }

    void Update()
    {
        if (jugadorEnPanel && Input.GetKeyDown(KeyCode.E) && !secuenciaActiva)
        {
            StartCoroutine(SecuenciaCinematicaCompleta());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) jugadorEnPanel = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) jugadorEnPanel = false;
    }

    private IEnumerator SecuenciaCinematicaCompleta()
    {
        secuenciaActiva = true;

        // 1. CORTAMOS A CÁMARA INTERNA
        camaraJugador.SetActive(false);
        camaraInterna.SetActive(true);

        // 2. DISPARAMOS EL ACELERADOR INTERNO
        aceleradorInterno.IniciarSecuenciaTubo();
        
        // Esperamos que termine de cargar (2.65s) + tu tiempo de pausa personalizado
        yield return new WaitForSeconds(2.65f + pausaFinInterna); 

        // 3. CORTAMOS A CÁMARA EXTERNA 1
        camaraInterna.SetActive(false);
        camaraExterna1.SetActive(true);

        // 4. LOS 6 RAYOS CONVERGEN
        float tiempoPasado = 0f;
        foreach (LineRenderer rayo in rayosTributarios)
        {
            rayo.useWorldSpace = true; 
            rayo.positionCount = 2; 
            rayo.SetPosition(0, rayo.transform.position); 
            rayo.SetPosition(1, rayo.transform.position); 
        }

        while (tiempoPasado < tiempoCrecimiento)
        {
            tiempoPasado += Time.deltaTime;
            float porcentaje = tiempoPasado / tiempoCrecimiento;
            foreach (LineRenderer rayo in rayosTributarios)
            {
                Vector3 puntaLaser = Vector3.Lerp(rayo.transform.position, puntoDeConvergencia.position, porcentaje);
                rayo.SetPosition(1, puntaLaser);
            }
            yield return null;
        }

        foreach (LineRenderer rayo in rayosTributarios)
        {
            rayo.SetPosition(1, puntoDeConvergencia.position);
        }

        // 5. CARGA DEL NÚCLEO EXTERIOR Y PAUSA DRAMÁTICA
        yield return new WaitForSeconds(tiempoCargaNucleo);
        
        // Agregamos una pausa extra acá para apreciar los 6 rayos antes del corte
        yield return new WaitForSeconds(pausaFinConvergencia);

        // 6. CORTAMOS A CÁMARA EXTERNA 2
        camaraExterna1.SetActive(false);
        camaraExterna2.SetActive(true);

        // Pausa en el nuevo ángulo justo antes de que el rayo salga disparado
        yield return new WaitForSeconds(pausaAntesDeDisparar);

        // 7. DISPARO FINAL A LA NAVE
        rayoPrincipal.useWorldSpace = true;
        rayoPrincipal.positionCount = 2;
        rayoPrincipal.SetPosition(0, puntoDeConvergencia.position);
        rayoPrincipal.SetPosition(1, puntoDeConvergencia.position);

        tiempoPasado = 0f;
        while (tiempoPasado < velocidadDisparoPrincipal)
        {
            tiempoPasado += Time.deltaTime;
            float porcentaje = tiempoPasado / velocidadDisparoPrincipal;
            Vector3 puntaFinal = Vector3.Lerp(puntoDeConvergencia.position, naveObjetivo.position, porcentaje);
            rayoPrincipal.SetPosition(1, puntaFinal);
            yield return null;
        }

        rayoPrincipal.SetPosition(1, naveObjetivo.position);
        
        // 8. IMPACTO Y EXPLOSIÓN
        if (explosionNave != null)
        {
            explosionNave.Play();
        }

        // 9. TIEMPO PARA VER LA DESTRUCCIÓN
        yield return new WaitForSeconds(3f);

        // 10. LIMPIEZA Y VUELTA AL JUGADOR
        rayoPrincipal.positionCount = 0;
        foreach (LineRenderer rayo in rayosTributarios) rayo.positionCount = 0;
        
        camaraExterna2.SetActive(false);
        camaraJugador.SetActive(true);
        secuenciaActiva = false;
    }
}