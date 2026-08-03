using System.Collections;
using UnityEngine;

public class AceleradorDeDisparo : MonoBehaviour
{
    [Header("Referencias de Geometría")]
    public Transform cilindroRayo; 
    public MeshRenderer rendererRayo; 
    
    [Header("Configuración de Shaders (HDR)")]
    [ColorUsage(true, true)] public Color colorCargaRojo = new Color(2f, 0f, 0f, 1f);
    [ColorUsage(true, true)] public Color colorDisparoVerde = new Color(0f, 3f, 0f, 1f);

    private Material materialRayo;
    private bool estaDisparando = false;
    
    // Nueva variable para guardar el tamaño que le diste a mano en Unity
    private Vector3 escalaBase;

    void Start()
    {
        if (rendererRayo != null)
        {
            materialRayo = rendererRayo.material;
            
            // Guardamos la escala real que tiene el objeto en la escena antes de tocar nada
            escalaBase = cilindroRayo.localScale;
            
            // Estado inicial: Lo hacemos muy finito en X y Z (10% de SU tamaño), pero conservamos el largo (Y)
            cilindroRayo.localScale = new Vector3(escalaBase.x * 0.1f, escalaBase.y, escalaBase.z * 0.1f);
            materialRayo.SetColor("_Color_Rayo", colorCargaRojo);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.O) && !estaDisparando)
        {
            IniciarSecuenciaTubo();
        }
    }

    public void IniciarSecuenciaTubo()
    {
        estaDisparando = true;
        StopAllCoroutines();
        StartCoroutine(SecuenciaAcelerador());
    }

    private IEnumerator SecuenciaAcelerador()
    {
        // FASE 1: CARGA
        cilindroRayo.localScale = new Vector3(escalaBase.x * 0.1f, escalaBase.y, escalaBase.z * 0.1f);
        materialRayo.SetColor("_Color_Rayo", colorCargaRojo);
        
        yield return new WaitForSeconds(2.5f); 

        // FASE 2: MUTACIÓN (De rojo a verde, expansión radial)
        float tiempoTransicion = 0.15f; 
        float tiempoPasado = 0f;

        while (tiempoPasado < tiempoTransicion)
        {
            tiempoPasado += Time.deltaTime;
            float porcentaje = tiempoPasado / tiempoTransicion;

            // Ensanchamos progresivamente hasta llegar a su escala ORIGINAL máxima
            float grosorX = Mathf.Lerp(escalaBase.x * 0.1f, escalaBase.x, porcentaje);
            float grosorZ = Mathf.Lerp(escalaBase.z * 0.1f, escalaBase.z, porcentaje);
            
            cilindroRayo.localScale = new Vector3(grosorX, escalaBase.y, grosorZ);

            Color colorActual = Color.Lerp(colorCargaRojo, colorDisparoVerde, porcentaje);
            materialRayo.SetColor("_Color_Rayo", colorActual);

            yield return null;
        }

        // FASE 3: DISPARO SOSTENIDO
        yield return new WaitForSeconds(2f);

        // FASE 4: APAGADO
       // cilindroRayo.localScale = Vector3.zero;
       // estaDisparando = false; 
    }
}