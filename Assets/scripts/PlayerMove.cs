using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerMove : MonoBehaviour
{
    // Variables para el movimiento
    [Header("Movimiento")]
    public float velocidadMovimiento = 5.0f;
    public float velocidadSprint = 7.0f;

    // Variables para el ataque
    [Header("Ataque")]
    public float tiempoEntreAtaques = 0.1f;
    public float ataqueStaminaCost = 30.0f;
    private float tiempoUltimoAtaque = 0.5f;
    private bool isAtaqueEspecialEnCurso = false;
    private float duracionAtaqueEspecial = 0.5f;
    private float timeSinceLastAction = 0.0f;

    // Variables para la cámara
    [Header("Cámara")]
    public float rotationSmoothness = 10.0f;

    // Variables para la vida
    [Header("Vida")]
    public Image ImagenBarraVida;
    public int vidaMax;
    public float vidaActual;

    // Variables para la stamina
    [Header("Stamina")]
    public Image staminaBarImage;
    private float currentStamina;
    public float maxStamina = 100.0f;
    public float RegenerarStaminaRate = 20.0f;
    public float rodarStaminaCost = 20.0f;

    // Variables para el poise
    [Header("Poise")]
    public Image PoiseBarImage;
    private float currentPoise;
    public float maxPoise = 100.0f;
    public float RegenerarPoiseRate = 10.0f;
    public float RegenerarPoiseEscudoRate = 5.0f;

    // Variables para el escudo/rodar
    [Header("Escudo/Rodar")]
    public bool isGuarding = false;
    public bool estaRodando = false;

    // Variables para otros
    [Header("Otros")]
    public bool conArma;
    private Rigidbody rb;
    private Animator animacion;
    private Transform cameraTransform;
    public string esena = "MainMenu";

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        animacion = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
        Cursor.lockState = CursorLockMode.Locked;
        currentStamina = maxStamina;
        currentPoise = maxPoise;
        vidaActual = vidaMax;
        RevisarVida();
    }

    private void FixedUpdate()
    {
        MoverPersonaje();
        RevisarVida();
    }

    private void Update()
    {
        HandleInput();
        ActualizarEstado();
        RotarPersonaje();
        if (vidaActual <= 0)
        {
            Cursor.lockState = CursorLockMode.None;
            SceneManager.LoadScene(esena);
        }
    }

    private void HandleInput()
    {
        ControlarRodar();
        HandleAttack();
        AtaqueEspecial();
        RegenerarStamina();
        Poise();
        EstaEnGuardia();
    }

    private void ActualizarEstado()
    {
        ControlarAnimaciones();
        UpdateStaminaBar();
    }

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
    private Vector3 ObtenerInputDireccion()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        Vector3 inputDirection = cameraTransform.forward * y + cameraTransform.right * x;
        inputDirection.y = 0f;
        inputDirection.Normalize();
        return inputDirection;
    }

    private void RotarPersonaje()
    {
        Vector3 inputDirection = ObtenerInputDireccion();

        if (inputDirection != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(inputDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSmoothness * Time.fixedDeltaTime);
        }
    }

    private void ControlarAnimaciones()
    {
        float x = Input.GetAxis("Horizontal");
        float y = Input.GetAxis("Vertical");
        animacion.SetBool("enGuardia", Input.GetMouseButton(1));
        animacion.SetFloat("VelX", x);
        animacion.SetFloat("VelY", y);
        animacion.SetBool("estaRodando", estaRodando);

        if (estaRodando && animacion.GetCurrentAnimatorStateInfo(0).IsName("Rodar") && animacion.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
        {
            estaRodando = false;
        }
    }

    private void HandleAttack()
    {
        if (!isGuarding && !estaRodando && Input.GetMouseButtonDown(0) && TiempoDesdeUltimoAtaque() >= tiempoEntreAtaques && currentStamina >= ataqueStaminaCost)
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
            Invoke("FinalizarAtaqueEspecial", duracionAtaqueEspecial);
        }
    }

    private void FinalizarAtaqueEspecial()
    {
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
            currentStamina += RegenerarStaminaRate * Time.deltaTime;
            currentStamina = Mathf.Clamp(currentStamina, 0f, maxStamina);
            UpdateStaminaBar();
        }
    }

    private void Poise()
    {
        if (Time.time - timeSinceLastAction >= 0.5f && currentPoise < maxPoise)
        {
            currentPoise += RegenerarPoiseRate * Time.deltaTime;
            currentPoise = Mathf.Clamp(currentPoise, 0f, maxPoise);
            UpdatePoiseBar();
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

    private void UpdateStaminaBar()
    {
        staminaBarImage.fillAmount = currentStamina / maxStamina;
    }

    private void UpdatePoiseBar()
    {
        PoiseBarImage.fillAmount = currentPoise / maxPoise;
    }
    public void RevisarVida()
    {
        ImagenBarraVida.fillAmount = vidaActual / vidaMax;
    }
    public void EstaEnGuardia()
    {
        isGuarding = Input.GetMouseButton(1);
        animacion.SetBool("enGuardia", isGuarding);
    }
}
