using UnityEngine;

public class GestorTabsOpciones : MonoBehaviour
{
    [Header("Arrastrá acá tus 3 paneles")]
    [Tooltip("0 = Audio, 1 = Gráficos, 2 = Controles")]
    [SerializeField] private GameObject[] _paneles;

    // Esta función la vamos a llamar desde el evento OnClick de los botones
    public void CambiarPestana(int indiceDeseado)
    {
        for (int i = 0; i < _paneles.Length; i++)
        {
            // Activa el panel si 'i' es igual al número que mandó el botón. Apaga el resto.
            _paneles[i].SetActive(i == indiceDeseado);
        }
    }
}