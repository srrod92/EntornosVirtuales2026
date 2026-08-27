using UnityEngine;
using UnityEngine.InputSystem;

 //este script es el mas complejo que tiene el proyecto, les dejo algunos comentarios explicando que hace cada línea
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

    [Tooltip("Distancia minima de arrastre en píxeles para limitar la potencia.")]
    [SerializeField] private float minArrastrePixeles = 30f;

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

        // detectamos el momento en el que se inicia el arrastre
        if (Pointer.current.press.wasPressedThisFrame)
        {
            inicioToque = Pointer.current.position.ReadValue();
            estaArrastrando = true;
        }

        // detectamos el momento en el que finaliza el arrastre
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

        
        Vector2 deltaArrastre = inicioToque - finToque;

        // si el disparo es muy debil terminamos el metodo.
        if (deltaArrastre.y <= minArrastrePixeles) return;

        // nos aseguramos que el arrastre este dentro del rango permitido
        float distanciaArrastre = Mathf.Clamp(deltaArrastre.y, minArrastrePixeles, maxArrastrePixeles);
        float porcentajePotencia = distanciaArrastre / maxArrastrePixeles;

        // calculamos la desviacion en eje x
        float desvioHorizontal = ( inicioToque.x - finToque.x) / Screen.width;

        // instanciamos la piedra
        Vector3 posicionOrigen = puntoDisparo.position + (camaraAR.transform.forward * offsetDistanciaCamara);
        GameObject piedraInstancia = Instantiate(piedraPrefab, posicionOrigen, Quaternion.identity);

        //es necesario acceder al RigidBody para poder aplicar fuerza al objeto.
        Rigidbody rb = piedraInstancia.GetComponent<Rigidbody>();
        if (rb != null)
        {
            // calculamos el vector de dirección según la orientación de la cámara.
            Vector3 direccionDisparo = (camaraAR.transform.forward + (camaraAR.transform.up * 0.2f) + (camaraAR.transform.right * desvioHorizontal)).normalized;
            float fuerzaFinal = porcentajePotencia * fuerzaMultiplicador;

            rb.AddForce(direccionDisparo * fuerzaFinal, ForceMode.Impulse);
        }
    }


}
