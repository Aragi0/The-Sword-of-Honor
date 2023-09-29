using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class vida : MonoBehaviour
{
    public int vidaMax;
    public float vidaActual;
    public Image ImagenBarraVida;

    void Start()
    {
        // Inicializa la vida actual correctamente
        vidaActual = vidaMax;
        RevisarVida();
    }

    // Update is called once per frame
    void Update()
    {
        if (vidaActual <= 0)
        {
            gameObject.SetActive(false);
        }
    }

    public void RevisarVida()
    {
        ImagenBarraVida.fillAmount = vidaActual / vidaMax;
    }
}
