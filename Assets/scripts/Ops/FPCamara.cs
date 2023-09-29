// opsoleto

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPCamara : MonoBehaviour
{
    public Vector2 sensibility;
    public float Sprit = 2.0f; // Velocidad de caminar
    public float runSpeed = 5.0f;  // Velocidad de correr
    public Animator animacion;
    private Transform camara;
    public CharacterController characterController;
    private float tiempoEntreAtaques = 1.0f; // El tiempo en segundos entre cada ataque
    private float tiempoUltimoAtaque = 0.0f; // El tiempo en el que se realizó el último ataque
    private bool isRunning = false;

    void Start()
    {
        camara = transform.Find("camara");
        Cursor.lockState = CursorLockMode.Locked;
        characterController = GetComponent<CharacterController>();
        animacion = GetComponent<Animator>();
    }

    void Update()
    {
        HandleMovement();
        UpdateAnimatorParameters();
        HandleAttack();
    }
    void HandleAttack()
    {
        if (Input.GetMouseButtonDown(0)) // Botón izquierdo del mouse
        {
            // Calcula el tiempo actual
            float tiempoActual = Time.time;

            // Verifica si ha pasado suficiente tiempo desde el último ataque
            if (tiempoActual - tiempoUltimoAtaque >= tiempoEntreAtaques)
            {
                // Activa la animación de ataque en el Animator
                animacion.SetTrigger("Ataque");

                // Aquí puedes agregar más lógica de ataque, como aplicar daño a enemigos, lanzar proyectiles, etc.

                // Actualiza el tiempo del último ataque
                tiempoUltimoAtaque = tiempoActual;
            }
        }
    }

    void HandleMovement()
    {
        // Manejar la rotación de la cámara con el ratón
        float hor = Input.GetAxis("Mouse X");
        float ver = Input.GetAxis("Mouse Y");

        if (hor != 0)
        {
            transform.Rotate(Vector3.up * hor * sensibility.x);
        }
        if (ver != 0)
        {
            float angle = (camara.localEulerAngles.x - ver * sensibility.y + 360) % 360;
            if (angle > 180) { angle -= 360; }
            angle = Mathf.Clamp(angle, -80, 80);
            camara.localEulerAngles = Vector3.right * angle;
        }

        // Manejar el movimiento del personaje
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Calcular la dirección de movimiento en función de la rotación de la cámara
        Vector3 moveDirection = transform.forward * moveVertical + transform.right * moveHorizontal;

        // Determinar si el personaje está corriendo o caminando
        isRunning = Input.GetKey(KeyCode.LeftShift);

        // Aplicar velocidad al CharacterController
        float currentSpeed = isRunning ? runSpeed : Sprit;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);
    }

    void UpdateAnimatorParameters()
    {
        // Actualizar los parámetros del Animator
        animacion.SetFloat("VelX", Input.GetAxis("Horizontal"));
        animacion.SetFloat("VelY", Input.GetAxis("Vertical"));
    }
}
