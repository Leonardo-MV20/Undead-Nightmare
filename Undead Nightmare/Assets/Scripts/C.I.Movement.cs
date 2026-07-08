using UnityEngine;

public class C_I_Movement : MonoBehaviour
{
    public float velocidadPatrulla = 1f;
    public float velocidadPersecucion = 2.5f;
    public float distanciaPatrulla = 2f;
    public float distanciaDeteccion = 5f;
    public float distanciaAtaque = 1f;

    private Rigidbody2D rb;
    private Transform jugador;
    private Vector2 puntoInicial;
    private int direccion = 1;
    private bool atacando = false;
    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        jugador = GameObject.FindGameObjectWithTag("Player").transform;
        puntoInicial = rb.position;

        MirarHaciaDondeCamina();
    }

    void FixedUpdate()
    {
        if (atacando) return;

        float distanciaJugador = Mathf.Abs(jugador.position.x - transform.position.x);

        if (distanciaJugador <= distanciaAtaque)
        {
            Atacar();
        }
        else if (distanciaJugador <= distanciaDeteccion)
        {
            PerseguirJugador();
        }
        else
        {
            Patrullar();
        }
    }

    void Patrullar()
    {
        Vector2 nuevaPosicion = rb.position + Vector2.right * direccion * velocidadPatrulla * Time.fixedDeltaTime;
        rb.MovePosition(nuevaPosicion);

        if (rb.position.x >= puntoInicial.x + distanciaPatrulla)
        {
            direccion = -1;
            MirarHaciaDondeCamina();
        }
        else if (rb.position.x <= puntoInicial.x - distanciaPatrulla)
        {
            direccion = 1;
            MirarHaciaDondeCamina();
        }
    }

    void PerseguirJugador()
    {
        float direccionJugador = jugador.position.x - transform.position.x;
        direccion = direccionJugador > 0 ? 1 : -1;

        MirarHaciaDondeCamina();

        float distanciaX = Mathf.Abs(jugador.position.x - transform.position.x);

        if (distanciaX <= distanciaAtaque)
        {
            return;
        }

        Vector2 nuevaPosicion = rb.position + Vector2.right * direccion * velocidadPersecucion * Time.fixedDeltaTime;
        rb.MovePosition(nuevaPosicion);
    }

   void Atacar()
    {
        atacando = true;
        
        animator.ResetTrigger("Attack");
        animator.SetTrigger("Attack");

        Invoke("TerminarAtaque", 1f);
    }

    void TerminarAtaque()
    {
        atacando = false;
    }

    void MirarHaciaDondeCamina()
    {
        Vector3 escala = transform.localScale;

        if (direccion == 1)
        escala.x = -Mathf.Abs(escala.x);
        else
        escala.x = Mathf.Abs(escala.x);

        transform.localScale = escala;
    }
}
