using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Esto es vital para poder cambiar de nivel

public class DirectorIntro : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private CanvasGroup _grupoHaceMucho;
    [SerializeField] private CanvasGroup _grupoLogo;
    [SerializeField] private RectTransform _transformLogo;
    [SerializeField] private TextCrawler _scriptTextCrawler;

    [Header("Fase 1: Hace mucho tiempo...")]
    [SerializeField] private float _tiempoLecturaHaceMucho = 3.5f;
    [SerializeField] private float _pausaSilencio = 1.5f;

    [Header("Fase 2: El Logo")]
    [SerializeField] private float _escalaInicialLogo = 6f;
    [SerializeField] private float _velocidadAchiqueLogo = 1.5f;
    [SerializeField] private float _tiempoAntesDelFadeLogo = 4f;

    [Header("Fase 3: Transición al Crawl")]
    [SerializeField] private float _duracionFadeLogo = 2f;

    [Header("Fase 4: Salto de Escena")]
    [Tooltip("El nombre exacto de la escena de tu juego a la que vamos a saltar")]
    [SerializeField] private string _nombreEscenaJuego = "NombreDeTuEscena";
    [Tooltip("El grupo del cartelito de 'Presiona ESPACIO'")]
    [SerializeField] private CanvasGroup _grupoTextoSaltar;
    [Tooltip("¿Cuántos segundos dura el texto subiendo hasta terminar?")]
    [SerializeField] private float _duracionTotalDelTextoAmarillo = 45f;

    void Start()
    {
        _grupoHaceMucho.alpha = 0;
        _grupoLogo.alpha = 0;
        _grupoTextoSaltar.alpha = 0; // Ocultamos el cartel de saltar al arrancar

        _transformLogo.localScale = Vector3.one * _escalaInicialLogo;
        _scriptTextCrawler.enabled = false;

        StartCoroutine(SecuenciaCinematografica());
    }

    void Update()
    {
        // En cualquier momento que apriete ESPACIO, forzamos la carga del juego
        if (Input.GetKeyDown(KeyCode.Space))
        {
            CargarEscenaDelJuego();
        }
    }

    IEnumerator SecuenciaCinematografica()
    {
        // FASE 1
        yield return StartCoroutine(HacerFade(_grupoHaceMucho, 0, 1, 1.5f));
        yield return new WaitForSeconds(_tiempoLecturaHaceMucho);
        yield return StartCoroutine(HacerFade(_grupoHaceMucho, 1, 0, 1.5f));

        yield return new WaitForSeconds(_pausaSilencio);

        // FASE 2
        _grupoLogo.alpha = 1;
        _transformLogo.localScale = Vector3.one * _escalaInicialLogo;

        Coroutine rutinaAchique = StartCoroutine(AchicarLogo());
        yield return new WaitForSeconds(_tiempoAntesDelFadeLogo);

        // FASE 3
        _scriptTextCrawler.enabled = true;
        yield return StartCoroutine(HacerFade(_grupoLogo, 1, 0, _duracionFadeLogo));
        StopCoroutine(rutinaAchique);

        // FASE 4: EL FINAL NATURAL
        // Hacemos aparecer suavemente el cartelito de saltar para que el jugador sepa que puede hacerlo
        StartCoroutine(HacerFade(_grupoTextoSaltar, 0, 0.7f, 2f));

        // Esperamos a que el texto termine de subir todo su recorrido
        yield return new WaitForSeconds(_duracionTotalDelTextoAmarillo - _duracionFadeLogo);

        // Si el jugador nunca apretó espacio y el tiempo se cumplió, cargamos el nivel solos
        CargarEscenaDelJuego();
    }

    IEnumerator AchicarLogo()
    {
        float tiempoViaje = 0f;
        while (true)
        {
            tiempoViaje += Time.deltaTime;
            float nuevaEscala = _escalaInicialLogo / (1f + (_velocidadAchiqueLogo * tiempoViaje));
            _transformLogo.localScale = Vector3.one * nuevaEscala;
            yield return null;
        }
    }

    IEnumerator HacerFade(CanvasGroup grupo, float inicio, float fin, float duracion)
    {
        float tiempo = 0;
        while (tiempo < duracion)
        {
            tiempo += Time.deltaTime;
            grupo.alpha = Mathf.Lerp(inicio, fin, tiempo / duracion);
            yield return null;
        }
        grupo.alpha = fin;
    }

    void CargarEscenaDelJuego()
    {
        // Carga la escena cuyo nombre pusiste en el Inspector
        SceneManager.LoadScene(_nombreEscenaJuego);
    }
}