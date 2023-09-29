
using UnityEngine;
using UnityEngine.Events;

public class Enemigo : MonoBehaviour
{
    private Animator animator;
    public LayerMask capaDelJugador;
    public Transform jugador;
    public UnityEvent OnPlayerInRange;
    public PlayerMove playerMoveScript;  // Debes asignar esta referencia desde el Editor de Unity
    public float rangoDeDeteccion;
    public float rangoDeAtaque;
    public float velocidad;
    public int danioAlJugador = 10;  // Ajusta el daño aquí
    private bool jugadorDetectado = false;
    public float tiempoEntreAtaques = 2f;
    private float tiempoSiguienteAtaque = 0f;
    // Evento para notificar al jugador en rango
    void Start()
    {
        animator = GetComponent<Animator>();
        PlayerMove playerMoveScript = GetComponent<PlayerMove>();
        DamageEnemy DamageEnemyScript = GetComponent<DamageEnemy>();
    }

    void Update()
    {
        float distanciaAlJugador = Vector3.Distance(transform.position, jugador.position);

        if (!jugadorDetectado && distanciaAlJugador <= rangoDeDeteccion)
        {
            jugadorDetectado = true;
        }

        if (jugadorDetectado)
        {
            if (distanciaAlJugador > rangoDeAtaque)
            {
                // Moverse hacia el jugador
                transform.LookAt(jugador);
                transform.Translate(Vector3.forward * velocidad * Time.deltaTime);
            }
            else
            {
                // Si ha pasado suficiente tiempo, realizar el ataque
                if (Time.time >= tiempoSiguienteAtaque)
                {
                    animator.SetTrigger("Attack");
                    RealizarAtaque();
                    tiempoSiguienteAtaque = Time.time + tiempoEntreAtaques;
                }
            }
        }

    }
    private void RealizarAtaque()
    {
        vida playerHealth = jugador.GetComponent<vida>();

        if (playerHealth != null)
        {
            if (playerMoveScript != null && playerMoveScript.estaRodando || playerMoveScript.isGuarding)
            {
                Debug.Log("N0 Damage");
            }
            else
            {
                playerHealth.vidaActual -= danioAlJugador;
                playerHealth.RevisarVida();
                Debug.Log("El jugador ha recibido " + danioAlJugador + " de daño.");
            }
        }
    }

}
