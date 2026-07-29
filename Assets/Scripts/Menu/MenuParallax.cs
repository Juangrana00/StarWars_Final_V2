using UnityEngine;

public class MenuParallax : MonoBehaviour
{
    [Header("Configuración del Parallax")]
    public float limiteMovimiento = 0.5f; // Qué tanto se mueve la cámara
    public float velocidadSuavizado = 2f; // Qué tan rápido sigue al mouse

    private Vector3 posicionInicial;

    void Start()
    {
        // Guardamos la posición original de la cámara
        posicionInicial = transform.position;
    }

    void Update()
    {
        // Calculamos la posición del mouse en pantalla (valores entre -1 y 1)
        float mouseX = (Input.mousePosition.x / Screen.width) * 2 - 1;
        float mouseY = (Input.mousePosition.y / Screen.height) * 2 - 1;

        // Calculamos a dónde debería ir la cámara
        Vector3 posicionObjetivo = new Vector3(mouseX * limiteMovimiento, mouseY * limiteMovimiento, 0) + posicionInicial;

        // Movemos la cámara suavemente usando Lerp
        transform.position = Vector3.Lerp(transform.position, posicionObjetivo, Time.deltaTime * velocidadSuavizado);
    }
}