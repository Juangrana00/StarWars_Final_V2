using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    public float pausaFinInterna = 1.0f;
    public float pausaFinConvergencia = 0.5f;
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
    [Tooltip("Arrastrá acá los Particle Systems de las explosiones que ya acomodaste en el crucero")]
    public ParticleSystem[] explosionesNave;
    [Tooltip("Creá GameObjects vacíos distribuidos en tu crucero y arrastralos acá para definir dónde explota")]
    public Transform[] puntosDeExplosion;
    [Tooltip("Tiempo de retraso entre cada explosión para generar un efecto en cadena")]
    public float retrasoEntreExplosiones = 0.15f;

    [Header("Fin del Juego")]
    public string nombreEscenaMenu = "MainMenu";

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

        camaraJugador.SetActive(false);
        camaraInterna.SetActive(true);

        aceleradorInterno.IniciarSecuenciaTubo();

        yield return new WaitForSeconds(2.65f + pausaFinInterna);

        camaraInterna.SetActive(false);
        camaraExterna1.SetActive(true);

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

        yield return new WaitForSeconds(tiempoCargaNucleo);
        yield return new WaitForSeconds(pausaFinConvergencia);

        camaraExterna1.SetActive(false);
        camaraExterna2.SetActive(true);

        yield return new WaitForSeconds(pausaAntesDeDisparar);

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

        // 8. IMPACTO Y EXPLOSIONES SIMULTÁNEAS
        if (explosionesNave.Length > 0)
        {
            foreach (ParticleSystem explosion in explosionesNave)
            {
                if (explosion != null)
                {
                    explosion.Play();
                }
            }
        }

        // 9. TIEMPO PARA VER LA DESTRUCCIÓN
        yield return new WaitForSeconds(3f);

        // 10. LIMPIEZA Y VUELTA AL MENÚ PRINCIPAL
        SceneManager.LoadScene(nombreEscenaMenu);
    }
}