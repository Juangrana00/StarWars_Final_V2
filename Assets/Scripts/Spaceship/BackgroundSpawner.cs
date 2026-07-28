using UnityEngine;

public class BackgroundSpawner : MonoBehaviour
{
    [Header("Flota y Velocidad")]
    [Tooltip("Arrastrá acá tus prefabs (ej: la TIE Fighter)")]
    public GameObject[] shipPrefabs; 
    public float minSpeed = 20f;
    public float maxSpeed = 50f;
    
    [Header("Zona de Spawneo")]
    [Tooltip("Cada cuántos segundos sale una nave nueva")]
    public float spawnInterval = 2f; 
    [Tooltip("El tamaño de la caja imaginaria de donde salen")]
    public Vector3 spawnAreaSize = new Vector3(200f, 100f, 0f); 
    [Tooltip("Segundos antes de que la nave se destruya para no consumir RAM")]
    public float lifetime = 15f; 
    
    private float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= spawnInterval)
        {
            SpawnShip();
            timer = 0f;
        }
    }

    void SpawnShip()
    {
        // Si no asignaste ninguna nave en el inspector, no hace nada
        if (shipPrefabs.Length == 0) return;

        // 1. Elige una nave al azar de tu lista
        GameObject shipToSpawn = shipPrefabs[Random.Range(0, shipPrefabs.Length)];

        // 2. Calcula un punto al azar dentro de la "caja"
        Vector3 randomPos = transform.position + new Vector3(
            Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2),
            Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2),
            Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2)
        );

        // 3. Crea la nave en esa posición, con la misma rotación que el Spawner
        GameObject newShip = Instantiate(shipToSpawn, randomPos, transform.rotation);

        // 4. Le agrega un motorcito temporal para que avance
        ShipMover mover = newShip.AddComponent<ShipMover>();
        mover.speed = Random.Range(minSpeed, maxSpeed);

        // 5. Destruye la nave a los X segundos cuando ya salió de cámara
        Destroy(newShip, lifetime);
    }

    // Esto dibuja una caja verde en la escena de Unity para que veas el área
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, spawnAreaSize);
    }
}

// Este es el "motorcito" que hace que la nave avance recta
public class ShipMover : MonoBehaviour
{
    public float speed;
    void Update()
    {
        // Avanza siempre hacia el eje Z local (la flecha azul)
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }
}