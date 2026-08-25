using TMPro;
using UnityEngine;

public class GameController : MonoBehaviour
{
    private float puntajeActual = 0;

    [SerializeField] private TextMeshProUGUI txtPuntaje;

    [SerializeField] GameObject objetosDeJuego;
    [SerializeField] GameObject Boton;

    int contadorDeZombies = 4;

    bool jugando = false;

    private void OnEnable()
    {
        ZombieController.OnZombieMuerto += ActualizarPuntaje;
    }

    private void OnDisable()
    {
        ZombieController.OnZombieMuerto -= ActualizarPuntaje;
    }
    void Start()
    {
        ActualizarPuntaje(0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

     void ActualizarPuntaje(float modificador)
    {
        puntajeActual += modificador;

        if (txtPuntaje != null)
        {
            txtPuntaje.text = $"Puntaje: {puntajeActual}";
        }
        contadorDeZombies--;
    }

    public void IniciarJuego()
    {
        jugando=true;
        objetosDeJuego.SetActive(true);
        Boton.SetActive(false);
    }


}
