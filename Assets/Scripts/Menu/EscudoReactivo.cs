using UnityEngine;

public class EscudoReactivo : MonoBehaviour
{
    private Material[] materialesEscudo; // Ahora guardamos una lista de materiales
    private float radioActual = 0f;
    
    [Header("Configuración del Impacto")]
    public float radioMaximo = 40f; 
    public float velocidadDesvanecimiento = 15f; 

    void Start()
    {
        // Buscamos todos los MeshRenderer que haya en este objeto y en sus hijos
        Renderer[] renderers = GetComponentsInChildren<Renderer>();
        materialesEscudo = new Material[renderers.Length];

        // Guardamos las instancias de los materiales y los apagamos
        for (int i = 0; i < renderers.Length; i++)
        {
            materialesEscudo[i] = renderers[i].material;
            materialesEscudo[i].SetFloat("_HitRadius", 0f); 
        }
    }

    void Update()
    {
        if (radioActual > 0)
        {
            radioActual -= Time.deltaTime * velocidadDesvanecimiento;
            float radioCalculado = Mathf.Max(0, radioActual);

            // Actualizamos todos los pedazos de la nave al mismo tiempo
            foreach (Material mat in materialesEscudo)
            {
                mat.SetFloat("_HitRadius", radioCalculado);
            }
        }
    }

    public void RecibirImpacto(Vector3 posicionImpacto)
    {
        // Le pasamos la coordenada a todos los pedazos
        foreach (Material mat in materialesEscudo)
        {
            mat.SetVector("_HitPosition", posicionImpacto);
        }
        
        radioActual = radioMaximo; 
    }
}