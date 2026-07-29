using UnityEngine;

public class TurbolaserPesado : MonoBehaviour
{
    [Header("Físicas del Disparo")]
    public float velocidadAvance = 150f;
    public float tiempoDeVida = 4f;

    [Header("Variantes de Explosión")]
    public GameObject[] particulasImpacto; // ¡Ahora es una lista (Array)!

    void Start()
    {
        Destroy(gameObject, tiempoDeVida);
    }

    void Update()
    {
        transform.Translate(Vector3.forward * velocidadAvance * Time.deltaTime);
    }

    void OnTriggerEnter(Collider otro)
    {
        // Buscamos si el objeto tiene escudo o si tiene daño directo
        EscudoReactivo escudo = otro.GetComponentInParent<EscudoReactivo>();
        DañoProgresivoNave casco = otro.GetComponentInParent<DañoProgresivoNave>();
        
        if (escudo != null || casco != null)
        {
            Vector3 puntoChoque = otro.ClosestPoint(transform.position);
            
            if (escudo != null)
            {
                escudo.RecibirImpacto(puntoChoque);
            }
            else if (casco != null)
            {
                casco.RecibirImpacto(); // Le suma un impacto a la cuenta para el fuego
            }
            
            GenerarExplosion(puntoChoque);

            // Llamamos al temblor de cámara (lo armamos en el paso 3)
            if (CamaraTemblor.Instancia != null)
            {
                CamaraTemblor.Instancia.VibrarLigero();
            }
            
            Destroy(gameObject);
        }
    }

    void GenerarExplosion(Vector3 posicion)
    {
        // Elige una explosión al azar de la lista y la instancia
        if (particulasImpacto != null && particulasImpacto.Length > 0)
        {
            int indiceAleatorio = Random.Range(0, particulasImpacto.Length);
            Instantiate(particulasImpacto[indiceAleatorio], posicion, Quaternion.identity);
        }
    }
}