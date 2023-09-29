using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rrebote : MonoBehaviour
{
    private float MinDistance = 1;
    private float MaxDistance = 5;
    private float Suavidad = 10;
    private float Distancia;
    private Vector3 direccion;
    private Transform playerTransform; // Declaración del Transform para el padre

    // Start is called before the first frame update
    void Start()
    {
        // Asigna el transform del padre si existe, o el mismo transform si no hay padre
        playerTransform = transform.parent != null ? transform.parent : transform;
        direccion = transform.localPosition.normalized;
        Distancia = transform.localPosition.magnitude;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 posDeCamara = playerTransform.TransformPoint(direccion * MaxDistance);

        RaycastHit hit;
        if (Physics.Linecast(playerTransform.position, posDeCamara, out hit))
        {
            Distancia = Mathf.Clamp(hit.distance * 0.85f, MinDistance, MaxDistance);
        }
        else
        {
            Distancia = MaxDistance;
        }
        transform.localPosition = Vector3.Lerp(transform.localPosition, direccion * Distancia, Suavidad * Time.deltaTime);
    }
}
