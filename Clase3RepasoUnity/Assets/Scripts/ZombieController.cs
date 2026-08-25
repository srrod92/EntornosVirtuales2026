using System;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    [Tooltip("Indice de la Layer que destuye cosas.")]
    [SerializeField] private int destuctor;

    [Range(100,1000)]
    [SerializeField] private float valorPuntaje = 100;

    [SerializeField] private float toleranciaCaida = -1f;

    public static Action<float> OnZombieMuerto;

    private bool esta_muerto = false;

    private Rigidbody rb;
    private Animator animator;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (rb == null) rb = GetComponent<Rigidbody>();
        if (animator == null) animator = GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (rb.linearVelocity.y < toleranciaCaida)
        {
            animator.SetBool("Cae", true);
        }
        else
        {
            animator.SetBool("Cae", false);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == destuctor && !esta_muerto)
        {
            esta_muerto=true;
            OnZombieMuerto?.Invoke(valorPuntaje);
            Destroy(gameObject,1);            
        }
    }
}
