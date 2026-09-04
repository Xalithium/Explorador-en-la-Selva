using UnityEngine;

public class MovimientoJugador : MonoBehaviour
{
    [SerializeField] private float velocidadCaminar = 10f;
    [SerializeField] private float velocidadCorrer = 16f;
    [SerializeField] private float fuerzaSalto = 12f;
    [SerializeField] private float gravedad = -20f;
    [SerializeField] private float velocidadGiro = 120f;
    [SerializeField] private float factorRetroceso = 0.5f;
    private float velocidadVertical;
    private InputSystem_Actions controles;
    private CharacterController controlador;
    private Animator animador;

    private void Awake()
    {
        controles = new InputSystem_Actions();
    }

    private void OnEnable()
    {
        controles.Player.Enable();
    }

    private void OnDisable()
    {
        controles.Player.Disable();
    }
    private void Start()
    {
        controlador = GetComponent<CharacterController>();
        animador = GetComponent<Animator>();
    }

 
    private void Update()
    {
        Vector2 entrada = controles.Player.Move.ReadValue<Vector2>();
       
        bool corriendo = controles.Player.Sprint.IsPressed();
        float velocidad;

        if (entrada.y < 0f)
        {
            velocidad = velocidadCaminar * factorRetroceso;
        }
        else if (corriendo)
        {
            velocidad = velocidadCorrer;
        }
        else
        {
            velocidad = velocidadCaminar;
        }

        
        transform.Rotate(0f, entrada.x * velocidadGiro * Time.deltaTime, 0f);

        if (controlador.isGrounded && velocidadVertical < 0f)
        {
            velocidadVertical = -2f;
        }

        if (controles.Player.Jump.WasPressedThisFrame() && controlador.isGrounded)
        {
            velocidadVertical = fuerzaSalto;
        }

        velocidadVertical += gravedad * Time.deltaTime;
        Vector3 desplazamiento = transform.forward * entrada.y * velocidad;
        desplazamiento.y = velocidadVertical;
        controlador.Move(desplazamiento * Time.deltaTime);

        float mezcla = entrada.y * 0.5f;

        if (corriendo && entrada.y > 0f)
        {
            mezcla = entrada.y;
        }

        animador.SetFloat("Velocidad", mezcla);

        animador.SetBool("EnElSuelo", controlador.isGrounded);
    }
}
