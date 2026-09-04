using UnityEngine;

public class CamaraSeguidora : MonoBehaviour
{
    [SerializeField] private Transform objetivo;
    [SerializeField] private Vector3 desfase = new Vector3(0f, 6f, -9f);
    [SerializeField] private float suavizado = 5f;

    private void LateUpdate()
    {
        Vector3 posicionDeseada = objetivo.position + objetivo.rotation * desfase;
        transform.position = Vector3.Lerp(transform.position, posicionDeseada, suavizado * Time.deltaTime);
        transform.LookAt(objetivo.position + Vector3.up * 2f);
    }
}
