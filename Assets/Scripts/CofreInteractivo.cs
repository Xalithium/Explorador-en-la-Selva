using UnityEngine;

public class CofreInteractivo : MonoBehaviour
{
    private bool jugadorCerca;
    private bool cofreAbierto;
    [SerializeField] private Transform tapaCofreLleno;
    private InputSystem_Actions controles;

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

    private void Update()
    {
        if (
            jugadorCerca &&
            !cofreAbierto &&
            controles.Player.Interact.WasPressedThisFrame()
        )
        {
            tapaCofreLleno.localEulerAngles = new Vector3(-210f, 0f, 0f);
            cofreAbierto = true;

            Debug.Log("El jugador abrió el cofre.");
        }
    }

    private void OnTriggerEnter(Collider otro)
    {
        if (otro.CompareTag("Player"))
        {
            jugadorCerca = true;
            Debug.Log("El jugador se acercó al cofre.");
        }
    }

    private void OnTriggerExit(Collider otro)
    {
        if (otro.CompareTag("Player"))
        {
            jugadorCerca = false;
            Debug.Log("El jugador se alejó del cofre.");
        }
    }
}