using UnityEngine;
using System.Collections;

public class CamaraTemblor : MonoBehaviour
{
    // Esto nos permite llamar a la cámara desde cualquier script sin buscarla
    public static CamaraTemblor Instancia; 

    private Vector3 posicionOriginal;

    void Awake()
    {
        Instancia = this;
    }

    void Start()
    {
        posicionOriginal = transform.localPosition;
    }

    public void VibrarLigero()
    {
        StopAllCoroutines(); // Frenamos si ya estaba temblando
        StartCoroutine(Temblor(0.15f, 0.3f)); // Tiempo corto, magnitud baja
    }

    IEnumerator Temblor(float duracion, float magnitud)
    {
        float tiempoPasado = 0.0f;

        while (tiempoPasado < duracion)
        {
            float x = Random.Range(-1f, 1f) * magnitud;
            float y = Random.Range(-1f, 1f) * magnitud;

            // Movemos la cámara de forma local un poquito
            transform.localPosition = new Vector3(posicionOriginal.x + x, posicionOriginal.y + y, posicionOriginal.z);

            tiempoPasado += Time.deltaTime;
            yield return null;
        }

        // Volvemos a dejar la cámara exactamente donde estaba
        transform.localPosition = posicionOriginal;
    }
}