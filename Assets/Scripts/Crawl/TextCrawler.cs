using UnityEngine;

public class TextCrawler : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float _scrollSpeed = 20f;

    void Update()
    {
        // Movimiento: Usamos Vector3.up en espacio local para respetar la rotación 3D del Canvas padre
        transform.Translate(Vector3.up * _scrollSpeed * Time.deltaTime, Space.Self);
    }
}