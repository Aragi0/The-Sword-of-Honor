using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamaraControl : MonoBehaviour
{
    public Transform player;  // El jugador que la cámara debe seguir
    public float sensibilidad = 2.0f;  // Sensibilidad de la cámara
    private Vector2 currentRotation = Vector2.zero;

    private void Update()
    {
        // Apunta la cámara hacia el jugador
        transform.position = player.position;
    }
}
