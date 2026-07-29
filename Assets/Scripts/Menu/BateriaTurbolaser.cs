using UnityEngine;

public class BateriaTurbolaser : MonoBehaviour
{
    [Header("Armamento")]
    public GameObject prefabLaser;
    public Transform objetivo; // Acá arrastrás el punto al que querés que dispare

    [Header("Ritmo de Fuego")]
    public float cadenciaDisparo = 1.5f;
    private float temporizador = 0f;

    [Header("Precisión")]
    public float dispersion = 2f; // Para que no pegue siempre en el mismo milímetro exacto

    void Update()
    {
        temporizador += Time.deltaTime;

        if (temporizador >= cadenciaDisparo)
        {
            Disparar();
            temporizador = 0f;
        }
    }

    void Disparar()
    {
        if (prefabLaser != null && objetivo != null)
        {
            // La torreta "mira" hacia el objetivo
            transform.LookAt(objetivo);

            // Le agregamos una mínima desviación táctica
            Vector3 rotacionAleatoria = new Vector3(
                Random.Range(-dispersion, dispersion),
                Random.Range(-dispersion, dispersion),
                0
            );
            
            Quaternion rotacionFinal = transform.rotation * Quaternion.Euler(rotacionAleatoria);

            // Dispara
            Instantiate(prefabLaser, transform.position, rotacionFinal);
        }
    }
}