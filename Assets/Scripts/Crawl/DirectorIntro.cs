using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; 

public class DirectorIntro : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private CanvasGroup _grupoHaceMucho;
    [SerializeField] private CanvasGroup _grupoLogo;
    [SerializeField] private RectTransform _transformLogo;
    [SerializeField] private TextCrawler _scriptTextCrawler; 
    
    [Tooltip("Arrastrá acá tu AudioSource con la música")]
    [SerializeField] private AudioSource _audioMusica;

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
    [SerializeField] private string _nombreEscenaJuego = "NombreDeTuEscena";
    [SerializeField] private CanvasGroup _grupoTextoSaltar;
    [SerializeField] private float _duracionFadeOutMusica = 1.5f;
    
    [Tooltip("Arrastrá acá tu Text (TMP) con el texto amarillo")]
    [SerializeField] private RectTransform _transformTextoCrawl; 
    
    // 👇 VOLVEMOS AL NÚMERO Y DIRECTO 👇
    [Tooltip("Posición Y exacta donde la última letra ya cruzó el borde de la máscara (Ej: 1600)")]
    [SerializeField] private float _alturaFinalDelTexto = 1600f;
    
    [Tooltip("Segundos de silencio en el espacio vacío antes de cargar el nivel")]
    [SerializeField] private float _pausaAntesDeEmpezar = 3f;
    
    [Header("Estilo del Cartel de Saltar")]
    [SerializeField] private float _velocidadParpadeo = 2.5f;
    [SerializeField] private float _alphaMinimo = 0.1f;
    [SerializeField] private float _alphaMaximo = 0.5f;

    private bool _permitirSalto = false; 

    void Start()
    {
        _grupoHaceMucho.alpha = 0;
        _grupoLogo.alpha = 0;
        _grupoTextoSaltar.alpha = 0; 
        
        _transformLogo.localScale = Vector3.one * _escalaInicialLogo; 
        _scriptTextCrawler.enabled = false; 

        StartCoroutine(SecuenciaCinematografica());
    }

    void Update()
    {
        if (_permitirSalto && Input.GetKeyDown(KeyCode.Space))
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

        // FASE 2: ¡MÚSICA Y LOGO!
        _audioMusica.Play(); 
        _grupoLogo.alpha = 1; 
        _transformLogo.localScale = Vector3.one * _escalaInicialLogo;
        
        Coroutine rutinaAchique = StartCoroutine(AchicarLogo());
        yield return new WaitForSeconds(_tiempoAntesDelFadeLogo);

        // FASE 3
        _scriptTextCrawler.enabled = true; 
        yield return StartCoroutine(HacerFade(_grupoLogo, 1, 0, _duracionFadeLogo));
        StopCoroutine(rutinaAchique);
        
        // FASE 4: EL CRAWL Y EL CARTEL PARPADEANTE
        _permitirSalto = true; 
        StartCoroutine(ParpadearCartel()); 
        
        // Compara la posición Y del texto contra el número que pongas en el Inspector
        yield return new WaitUntil(() => _transformTextoCrawl.anchoredPosition.y >= _alturaFinalDelTexto);
        
        // Cuando termina, apagamos la opción de saltar para limpiar la pantalla.
        _permitirSalto = false;
        _grupoTextoSaltar.alpha = 0; 
        
        // El respiro cinematográfico
        yield return new WaitForSeconds(_pausaAntesDeEmpezar);
        
        // Fin de la intro, a jugar
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

    IEnumerator ParpadearCartel()
    {
        while (_permitirSalto) 
        {
            float oscilacion = (Mathf.Sin(Time.time * _velocidadParpadeo) + 1f) / 2f; 
            _grupoTextoSaltar.alpha = Mathf.Lerp(_alphaMinimo, _alphaMaximo, oscilacion);
            yield return null;
        }
    }

  void CargarEscenaDelJuego()
    {
        // Evitamos que el jugador pueda apretar espacio dos veces seguidas
        _permitirSalto = false; 
        StartCoroutine(CargarNivelConFadeOut());
    }

    IEnumerator CargarNivelConFadeOut()
    {
        float tiempo = 0;
        float volumenInicial = _audioMusica.volume;

        // 1. Le decimos a Unity que cargue la escena en la memoria...
        AsyncOperation operacion = SceneManager.LoadSceneAsync(_nombreEscenaJuego);
        // 2. ...pero le prohibimos que la muestre todavía.
        operacion.allowSceneActivation = false; 

        // 3. Hacemos el Fade Out del AudioSource
        while (tiempo < _duracionFadeOutMusica)
        {
            tiempo += Time.deltaTime;
            _audioMusica.volume = Mathf.Lerp(volumenInicial, 0f, tiempo / _duracionFadeOutMusica);
            yield return null;
        }

        _audioMusica.volume = 0f;

        // 4. Ahora sí, le damos permiso a Unity para pasar de escena
        operacion.allowSceneActivation = true; 
    }
}