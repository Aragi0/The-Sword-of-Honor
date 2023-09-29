using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivarArmas : MonoBehaviour
{
    public OptenerArmas optenerArmas;
    public int numeroArmas;

    void Start()
    {
        // Buscar el objeto con la etiqueta "Player" en la escena
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            optenerArmas = player.GetComponent<OptenerArmas>();
        }
        else
        {
            Debug.LogError("No se encontró un objeto con la etiqueta 'Player' para obtener OptenerArmas.");
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                // Llama a la función para activar el arma
                optenerArmas.ActivarArma(numeroArmas);
                Destroy(gameObject); // Destruir el objeto que representa el arma en la escena
            }
        }
    }
}
