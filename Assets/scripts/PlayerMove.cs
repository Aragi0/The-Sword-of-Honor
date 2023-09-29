using UnityEngine;
using UnityEngine.UI;

public class PlayerMove : MonoBehaviour
{
    public Rigidbody rb;
    public Animator animacion;
    public LayerMask collisionLayer;
    private Transform cameraTransform;

    [Header("Movimiento")]
    public float velocidadMovimiento = 5.0f;
    public float velocidadSprint = 7.0f;

    [Header("Ataque")]
    public float tiempoEntreAtaques = 0.1f;
    public float ataqueStaminaCost = 30.0f;
    private float tiempoUltimoAtaque = 0.5f;
    private bool isAtaqueEspecialEnCurso = false;
    private float duracionAtaqueEspecial = 0.5f;
    private float timeSinceLastAction = 0.0f;

    [Header("Cámara")]
    public float sensibilidadRaton = 100.0f;
    public float maxVerticalRotation = 90.0f;
    public float rotationSmoothness = 10.0f;
    private float rotacionX = 0.0f;

    [Header("Stamina")]
    public Image staminaBarImage;
    private float currentStamina;
    public float maxStamina = 100.0f;
    public float RegenerarStamna = 20.0f;
    public float rodarStaminaCost = 20.0f;

    [Header("Escudo/Rodar")]
    public bool isGuarding = false;
    public bool estaRodando = false;

    [Header("Otros")]
    public bool conArma;

    private void Start()
    {
        animacion = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        currentStamina = maxStamina;
    }

    private void FixedUpdate()
    {
        MoverPersonaje();
    }

    private void Update()
    {
        HandleInput();
        ActualizarEstado();
        RotarPersonaje();
        RotarCamara();
    }

    private void HandleInput()
    {
        ControlarRodar();
        HandleAttack();
        AtaqueEspecial();
        RegenerarStamina();
        EstaEnGuardia();
    }

    private void ActualizarEstado()
    {
        ControlarAnimaciones();
        UpdateStaminaBar();
    }

    // Resto de tus métodos existentes...

    private void MoverPersonaje()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");

        Vector3 inputDirection = new Vector3(x, 0f, y).normalized;

        // Calcular la velocidad basada en la dirección de movimiento y si está haciendo sprint
        float velocidadActual = inputDirection.magnitude * (Input.GetKey(KeyCode.LeftShift) ? velocidadSprint : velocidadMovimiento);

        // Obtener la rotación del personaje actual sin la componente Y
        Quaternion characterRotation = Quaternion.Euler(0f, cameraTransform.rotation.eulerAngles.y, 0f);

        // Transformar la dirección de entrada a la orientación del personaje
        Vector3 movement = characterRotation * inputDirection * velocidadActual * Time.deltaTime;

        RaycastHit hit;
        bool canMove = !Physics.Raycast(transform.position, movement, out hit, movement.magnitude);

        if (canMove)
        {
            rb.MovePosition(transform.position + movement);
        }
        else
        {
            rb.MovePosition(hit.point - movement.normalized * 0.1f);
        }
    }

    private void RotarPersonaje()
    {
        Vector3 inputDirection = ObtenerInputDireccion();

        if (inputDirection != Vector3.zero)
        {
            // Proyecta la dirección al plano horizontal (ignora la componente Y)
            Vector3 horizontalDirection = inputDirection;
            horizontalDirection.y = 0;

            Quaternion targetRotation = Quaternion.LookRotation(horizontalDirection);

            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.fixedDeltaTime);
        }
    }

    private Vector3 ObtenerInputDireccion()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        return cameraTransform.forward * y + cameraTransform.right * x;
    }

    private void ControlarAnimaciones()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        animacion.SetBool("enGuardia", Input.GetMouseButton(1));
        animacion.SetFloat("VelX", x);
        animacion.SetFloat("VelY", y);
        animacion.SetBool("estaRodando", estaRodando);  // Informar si está rodando
        if (estaRodando && animacion.GetCurrentAnimatorStateInfo(0).IsName("Rodar") && animacion.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            estaRodando = false;  // Desmarcar cuando ha terminado de rodar
        }
    }

    private void HandleAttack()
    {
        if (!isGuarding && !estaRodando && Input.GetMouseButtonDown(0) &&
            TiempoDesdeUltimoAtaque() >= tiempoEntreAtaques && currentStamina >= ataqueStaminaCost)
        {
            IniciarAtaque();
        }
    }

    private void AtaqueEspecial()
    {
        if (!isGuarding && !estaRodando && Input.GetKeyDown(KeyCode.F) && !isAtaqueEspecialEnCurso)
        {
            animacion.SetTrigger("especial");
            isAtaqueEspecialEnCurso = true;
            // Llamar a una función para manejar el final del ataque especial
            Invoke("FinalizarAtaqueEspecial", duracionAtaqueEspecial);
        }
    }

    private void FinalizarAtaqueEspecial()
    {
        // Llamar a esta función cuando el ataque especial haya concluido
        isAtaqueEspecialEnCurso = false;
    }

    private void IniciarAtaque()
    {
        animacion.SetTrigger("ataque");
        tiempoUltimoAtaque = Time.time;
        currentStamina -= ataqueStaminaCost;
        timeSinceLastAction = Time.time;
        UpdateStaminaBar();
    }



    private void RegenerarStamina()
    {
        if (Time.time - timeSinceLastAction >= 0.5f && currentStamina < maxStamina)
        {
            currentStamina += RegenerarStamna * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            UpdateStaminaBar();
        }
    }

    private float TiempoDesdeUltimoAtaque()
    {
        return Time.time - tiempoUltimoAtaque;
    }
    private void ControlarRodar()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !animacion.GetCurrentAnimatorStateInfo(0).IsName("Rodar") && currentStamina >= rodarStaminaCost)
        {
            animacion.Play("Rodar", 0, 0f);
            estaRodando = true;
            currentStamina -= rodarStaminaCost;
            timeSinceLastAction = Time.time;
            UpdateStaminaBar();
        }
    }
    private void RotarCamara()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensibilidadRaton * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensibilidadRaton * Time.deltaTime;

        mouseX *= sensibilidadRaton;  // Multiplica por la sensibilidad del ratón
        mouseY *= sensibilidadRaton;  // Multiplica por la sensibilidad del ratón

        transform.Rotate(Vector3.up, mouseX);

        rotacionX -= mouseY;
        rotacionX = Mathf.Clamp(rotacionX, -maxVerticalRotation, maxVerticalRotation);

        Quaternion targetRotation = Quaternion.Euler(rotacionX, 0.0f, 0.0f);
        cameraTransform.localRotation = targetRotation;
    }

    private void UpdateStaminaBar()
    {
        staminaBarImage.fillAmount = currentStamina / maxStamina;
    }
    public void EstaEnGuardia()
    {
        if (Input.GetMouseButton(1))
        {
            isGuarding = true;
            animacion.SetBool("enGuardia", true);

        }
        else
        {
            isGuarding = false;
            animacion.SetBool("enGuardia", false);  // Desactiva la animación de ponerse en guardia
        }
    }
}