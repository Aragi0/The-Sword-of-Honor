// opsoleto
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Transform playerTransform; // La transformación del jugador

    private void Start()
    {
        playerTransform = transform.parent; // Suponiendo que la cámara está como hijo del jugador
    }

    // Método para ajustar la posición de la cámara cuando hay colisión con un muro
    public void AdjustCameraPosition(Vector3 hitPoint)
    {
        // Mueve la cámara a una posición cercana al punto de colisión
        Vector3 offset = (playerTransform.position - hitPoint).normalized * 0.1f;
        transform.position = hitPoint + offset;
    }
}
