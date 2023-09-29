using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class OptenerArmas : MonoBehaviour
{
    public PlayerMove jugador;
    public BoxCollider[] armasCaja;
    public GameObject[] armas;

    void Start()
    {
        DesactivarCajaArmas();
    }
    public void ActivarArma(int numero)
    {
        for (int i = 0; i < armas.Length; i++)
        {
            armas[i].SetActive(false);
        }
        armas[numero].SetActive(true);
    }
    public void ActivarCajaArmas()
    {
        for (int i = 0; i < armasCaja.Length; i++)
        {
            if (jugador.conArma)
            {
                if (armasCaja[i] != null)
                {
                    armasCaja[i].enabled = true;
                }
            }
        }
    }
    public void DesactivarCajaArmas()
    {
        for (int i = 0; i < armasCaja.Length; i++)
        {
            if (armasCaja[i] != null)
            {
                armasCaja[i].enabled = false;
            }

        }
    }
    // void Update()
    // {
    //     if(Input.GetKeyDown(KeyCode.Z))
    //     {
    //         DesactivarArmas();
    //     }
    // }
    // public void DesactivarArmas()
    // {
    //     for (int i = 0; i < armas.Length; i++)
    //     {
    //         armas[i].SetActive(false);
    //     }
    // }
}
