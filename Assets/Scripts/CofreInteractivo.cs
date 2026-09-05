using System.Collections;
using UnityEngine;

public class CofreInteractivo : MonoBehaviour
{
    private bool jugadorCerca;
    private bool cofreAbierto;
    private bool mapaVisible;
    [SerializeField] private Transform tapaCofreLleno;
    [SerializeField] private GameObject panelMapa;
    [SerializeField] private GameObject cofreLleno;
    [SerializeField] private GameObject cofreVacio;
    [SerializeField] private Transform tapaCofreVacio;
    [SerializeField] private float duracionApertura = 0.8f;
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
        Time.timeScale = 1f;
    }

    private void Update()
    {
        if (
            jugadorCerca &&
            controles.Player.Interact.WasPressedThisFrame()
        )
        {
            if (!cofreAbierto)
            {
                cofreAbierto = true;
                StartCoroutine(AbrirCofre());
            }
            else if (mapaVisible)
            {
                mapaVisible = false;
                panelMapa.SetActive(false);

                tapaCofreVacio.localEulerAngles = new Vector3(-210f, 0f, 0f);
                cofreLleno.SetActive(false);
                cofreVacio.SetActive(true);

                Time.timeScale = 1f;

                Debug.Log("El jugador cerró el mapa.");
            }
        }
    }

    private IEnumerator AbrirCofre()
    {
        Quaternion rotacionInicial = tapaCofreLleno.localRotation;
        Quaternion rotacionFinal = Quaternion.Euler(-210f, 0f, 0f);
        float tiempoTranscurrido = 0f;

        while (tiempoTranscurrido < duracionApertura)
        {
            tiempoTranscurrido += Time.deltaTime;
            float progreso = tiempoTranscurrido / duracionApertura;

            tapaCofreLleno.localRotation =
                Quaternion.Lerp(rotacionInicial, rotacionFinal, progreso);

            yield return null;
        }

        tapaCofreLleno.localRotation = rotacionFinal;

        yield return new WaitForSeconds(0.5f);

        mapaVisible = true;
        panelMapa.SetActive(true);
        Time.timeScale = 0f;

        Debug.Log("El jugador abrió el cofre.");
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