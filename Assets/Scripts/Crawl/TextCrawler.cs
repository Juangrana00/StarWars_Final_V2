using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; // Necesario para poder acceder y modificar TextMeshPro

public class TextCrawler : MonoBehaviour
{
    [Header("Configuración de Movimiento")]
    [SerializeField] private float _scrollSpeed = 20f;

    [Header("Configuración de Fade Out")]
    [SerializeField] private TextMeshProUGUI _textoCrawl; // Arrastrá tu texto acá desde el inspector
    [SerializeField] private float _puntoInicioFade = 400f; // La altura Y donde empieza a volverse transparente
    [SerializeField] private float _puntoFinFade = 800f;   // La altura Y donde desaparece por completo

    void Update()
    {
        // 1. Movimiento: Usamos Vector3.up en espacio local para respetar la rotación del Canvas
        transform.Translate(Vector3.up * _scrollSpeed * Time.deltaTime, Space.Self);

        // 2. Fade Out: Solo lo calculamos si le asignaste el texto
        if (_textoCrawl != null)
        {
            ManejarFadeOut();
        }
    }

    private void ManejarFadeOut()
    {
        // Tomamos la posición Y local del objeto
        float alturaActual = transform.localPosition.y;

        // Si el texto ya superó la marca de inicio de Fade...
        if (alturaActual > _puntoInicioFade)
        {
            // Calculamos un porcentaje de transparencia (de 1 a 0)
            float alpha = 1f - Mathf.Clamp01((alturaActual - _puntoInicioFade) / (_puntoFinFade - _puntoInicioFade));

            // Le aplicamos ese nuevo alpha al color del texto sin modificar el color dorado
            Color colorTexto = _textoCrawl.color;
            colorTexto.a = alpha;
            _textoCrawl.color = colorTexto;
        }
    }
}