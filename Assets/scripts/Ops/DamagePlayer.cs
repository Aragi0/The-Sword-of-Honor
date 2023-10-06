// opsoleto
using UnityEngine;

public class DamagePlayer : MonoBehaviour
{
    private void Start()
    {
        // Suscribe el método al evento del enemigo
        Enemigo enemigo = GetComponentInParent<Enemigo>();
        if (enemigo != null)
        {
            enemigo.OnPlayerInRange.AddListener(ApplicarDanio);
        }
    }

    private void OnDestroy()
    {
        // Asegúrate de que te desuscribes del evento al destruir el objeto
        Enemigo enemigo = GetComponentInParent<Enemigo>();
        if (enemigo != null)
        {
            enemigo.OnPlayerInRange.RemoveListener(ApplicarDanio);
        }
    }

    public void ApplicarDanio()
    {
        // Obtén el script PlayerMove
        PlayerMove playerMove = GetComponentInParent<PlayerMove>();
        if (playerMove != null)
        {
            // Si el jugador está rodando, no aplicar daño
            if (playerMove.estaRodando)
            {
                Debug.Log("NO damage");
                return;
            }

            // Si no está rodando, aplicar daño
            vida playerHealth = GetComponentInParent<vida>();
            if (playerHealth != null)
            {
                playerHealth.vidaActual -= 10;  // Suponiendo que el daño es 10
                playerHealth.RevisarVida();  // Actualiza la barra de vida
                Debug.Log("Si damage");
            }
        }
    }
}
