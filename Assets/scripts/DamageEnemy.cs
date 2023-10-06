using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageEnemy : MonoBehaviour
{
    public int hp;
    public int damageArma;
    public vida vidaScript; // Referencia al script de la barra de vida
    private void Start()
    {
        // Asegúrate de inicializar la vida actual correctamente
        vidaScript.vidaActual = vidaScript.vidaMax;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "armaImpacto")
        {
            hp -= damageArma;
            vidaScript.vidaActual = hp;
            vidaScript.RevisarVida();

        }

        if (hp <= 0)
        {
            // Si no es el jugador, destruye el objeto
            Destroy(gameObject);
        }

    }

}
