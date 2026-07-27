using System.Collections.Generic;
using UnityEngine;

public class SpawnerBatalla : MonoBehaviour
{
    [Header("Faccion: Rebeldes (Ej: X-Wing)")]
    public GameObject[] prefabsLideresRebeldes;
    public GameObject[] prefabsPerseguidoresRebeldes;

    [Header("Faccion: Imperio (Ej: TIE)")]
    public GameObject[] prefabsLideresImperio;
    public GameObject[] prefabsPerseguidoresImperio;

    [Header("Configuración de Batalla")]
    public int cantidadPorPool = 10;
    public float radioDeSpawn = 300f;
    public float tiempoEntreSpawns = 1.5f;

    // 4 Pools separados para mantener el orden
    private List<GameObject> poolLideresRebeldes = new List<GameObject>();
    private List<GameObject> poolLideresImperio = new List<GameObject>();
    private List<GameObject> poolPerseguidoresRebeldes = new List<GameObject>();
    private List<GameObject> poolPerseguidoresImperio = new List<GameObject>();

    void Start()
    {
        // Inicializamos los 4 pools
        LlenarPool(prefabsLideresRebeldes, poolLideresRebeldes);
        LlenarPool(prefabsLideresImperio, poolLideresImperio);
        LlenarPool(prefabsPerseguidoresRebeldes, poolPerseguidoresRebeldes);
        LlenarPool(prefabsPerseguidoresImperio, poolPerseguidoresImperio);

        InvokeRepeating(nameof(SpawnearPareja), 1f, tiempoEntreSpawns);
    }

    void LlenarPool(GameObject[] prefabs, List<GameObject> pool)
    {
        if (prefabs.Length == 0) return;

        for (int i = 0; i < cantidadPorPool; i++)
        {
            GameObject nave = Instantiate(prefabs[Random.Range(0, prefabs.Length)], transform);
            nave.SetActive(false);
            pool.Add(nave);
        }
    }

    void SpawnearPareja()
    {
        // Tiramos una moneda virtual (50% de probabilidad)
        bool rebeldeHuye = Random.value > 0.5f;

        // Asignamos los roles cruzados según la moneda
        GameObject lider = rebeldeHuye ? ObtenerNaveInactiva(poolLideresRebeldes) : ObtenerNaveInactiva(poolLideresImperio);
        GameObject perseguidor = rebeldeHuye ? ObtenerNaveInactiva(poolPerseguidoresImperio) : ObtenerNaveInactiva(poolPerseguidoresRebeldes);

        if (lider != null && perseguidor != null)
        {
            Vector3 posicionLider = transform.position + (Random.onUnitSphere * radioDeSpawn);
            Vector3 direccionVuelo = (transform.position - posicionLider).normalized;

            lider.transform.position = posicionLider;
            lider.transform.rotation = Quaternion.LookRotation(direccionVuelo);

            // El perseguidor aparece un poco atrás, pero con una leve desviación aleatoria (entre 20 y 35 metros)
            perseguidor.transform.position = posicionLider - (direccionVuelo * Random.Range(20f, 35f));
            perseguidor.transform.rotation = Quaternion.LookRotation(direccionVuelo);

            perseguidor.GetComponent<NavePerseguidor>().objetivoLider = lider.transform;

            lider.SetActive(true);
            perseguidor.SetActive(true);
        }
    }

    GameObject ObtenerNaveInactiva(List<GameObject> pool)
    {
        foreach (GameObject nave in pool)
        {
            if (!nave.activeInHierarchy) return nave;
        }
        return null;
    }
}