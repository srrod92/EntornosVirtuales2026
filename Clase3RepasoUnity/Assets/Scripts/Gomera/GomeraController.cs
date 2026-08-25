using UnityEngine;
using UnityEngine.InputSystem;

public class GomeraController : MonoBehaviour
{
    [Header("Configuración del Proyectil")]
    [Tooltip("Prefab de la Piedra (debe tener Rigidbody y Collider).")]
    [SerializeField] private GameObject piedraPrefab;

    [Tooltip("Punto de origen del disparo. Si se deja nulo, usa la posición de la Cámara AR.")]
    [SerializeField] private Transform puntoDisparo;

    [Header("Física del Disparo")]
    [Tooltip("Multiplicador de fuerza aplicada a la piedra.")]
    [SerializeField] private float fuerzaMultiplicador = 150f;

    [Tooltip("Distancia máxima de arrastre en píxeles para limitar la potencia.")]
    [SerializeField] private float maxArrastrePixeles = 400f;

    [Tooltip("Distancia frente a la cámara donde aparece la piedra al disparar.")]
    [SerializeField] private float offsetDistanciaCamara = 0.4f;

    private Camera camaraAR;
    private Vector2 inicioToque;
    private Vector2 finToque;
    private bool estaArrastrando = false;

    private void Start()
    {
        camaraAR = Camera.main;
        if (puntoDisparo == null && camaraAR != null)
        {
            puntoDisparo = camaraAR.transform;
        }
    }

    private void Update()
    {
        ProcesarEntradaInputSystem();
    }

    private void ProcesarEntradaInputSystem()
    {
        // Pointer.current detecta tanto toques en pantalla como clics de mouse
        if (Pointer.current == null) return;

        // 1. Detecta el instante inicial al presionar la pantalla / hacer clic
        if (Pointer.current.press.wasPressedThisFrame)
        {
            inicioToque = Pointer.current.position.ReadValue();
            estaArrastrando = true;
        }

        // 2. Detecta el instante en que se suelta el dedo / clic
        if (Pointer.current.press.wasReleasedThisFrame && estaArrastrando)
        {
            finToque = Pointer.current.position.ReadValue();
            DispararPiedra();
            estaArrastrando = false;
        }
    }

    private void DispararPiedra()
    {
        if (piedraPrefab == null)
        {
            Debug.LogError("No se ha asignado el Prefab de la Piedra en el Inspector.");
            return;
        }

        // Calcular la distancia de arrastre (deslizar hacia abajo da deltaY positivo)
        Vector2 deltaArrastre = inicioToque - finToque;

        // Ignorar si desliza hacia arriba o no hay movimiento
        if (deltaArrastre.y <= 0) return;

        // Limitar y calcular porcentaje de potencia
        float distanciaArrastre = Mathf.Clamp(deltaArrastre.y, 0f, maxArrastrePixeles);
        float porcentajePotencia = distanciaArrastre / maxArrastrePixeles;

        // Desviación horizontal relativa
        float desvioHorizontal = ( inicioToque.x - finToque.x) / Screen.width;

        // Instanciación del proyectil
        Vector3 posicionOrigen = puntoDisparo.position + (camaraAR.transform.forward * offsetDistanciaCamara);
        GameObject piedraInstancia = Instantiate(piedraPrefab, posicionOrigen, Quaternion.identity);

        Rigidbody rb = piedraInstancia.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // Vector de dirección 3D según la orientación de la cámara AR
            Vector3 direccionDisparo = (camaraAR.transform.forward + (camaraAR.transform.up * 0.2f) + (camaraAR.transform.right * desvioHorizontal)).normalized;
            float fuerzaFinal = porcentajePotencia * fuerzaMultiplicador;

            rb.AddForce(direccionDisparo * fuerzaFinal, ForceMode.Impulse);
        }
    }


}
