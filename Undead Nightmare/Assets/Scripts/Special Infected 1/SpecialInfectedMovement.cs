using UnityEngine;

public class SpecialInfectedMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadPatrulla = 1f;
    public float velocidadPersecucion = 3f;
    public float distanciaPatrulla = 2f;

    [Header("Detección")]
    public float distanciaDeteccion = 15f;
    public float distanciaAtaque = 8f;

    [Header("Jugador")]
    public string tagJugador = "Player";

    [Header("Ataque")]
    public float tiempoEntreAtaques = 0.5f;

    [Header("Contraataque")]
    public float tiempoAntesContraataque = 0.3f;

    [Header("Proyectil")]
    public GameObject proyectilAcido;
    public Transform puntoDisparo;
    public float tiempoAntesProyectil = 0.4f;

    private Rigidbody2D rb;
    private Transform jugador;
    private Vector2 puntoInicial;
    private int direccion = 1;

    private bool atacando = false;
    private bool recibiendoDanno = false;
    private bool muerto = false;
    private bool alertado = false;
    private bool puedeAtacar = true;

    private Animator animator;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();

        if (rb == null)
        {
            Debug.LogError(
                "Special Infected no tiene Rigidbody2D."
            );

            enabled = false;
            return;
        }

        puntoInicial = rb.position;

        BuscarJugador();
        MirarHaciaDondeCamina();
    }

    void FixedUpdate()
    {
        if (muerto) return;
        if (atacando) return;
        if (recibiendoDanno) return;

        if (jugador == null)
        {
            BuscarJugador();

            if (jugador == null)
            {
                Patrullar();
                return;
            }
        }

        float distanciaJugador = Mathf.Abs(
            jugador.position.x -
            transform.position.x
        );

        if (distanciaJugador <= distanciaAtaque)
        {
            if (puedeAtacar)
            {
                Atacar();
            }
            else
            {
                EsperarAtaque();
            }
        }
        else if (
            alertado ||
            distanciaJugador <= distanciaDeteccion
        )
        {
            PerseguirJugador();
        }
        else
        {
            Patrullar();
        }
    }

    void BuscarJugador()
    {
        GameObject objetoJugador =
            GameObject.FindGameObjectWithTag(
                tagJugador
            );

        if (objetoJugador != null)
        {
            jugador = objetoJugador.transform;
        }
        else
        {
            jugador = null;
        }
    }

    void Patrullar()
    {
        if (animator != null)
        {
            animator.SetBool("Walk", true);
        }

        Vector2 nuevaPosicion =
            rb.position +
            Vector2.right *
            direccion *
            velocidadPatrulla *
            Time.fixedDeltaTime;

        rb.MovePosition(nuevaPosicion);

        if (
            rb.position.x >=
            puntoInicial.x + distanciaPatrulla
        )
        {
            direccion = -1;
            MirarHaciaDondeCamina();
        }
        else if (
            rb.position.x <=
            puntoInicial.x - distanciaPatrulla
        )
        {
            direccion = 1;
            MirarHaciaDondeCamina();
        }
    }

    void PerseguirJugador()
    {
        if (jugador == null) return;

        float direccionJugador =
            jugador.position.x -
            transform.position.x;

        if (direccionJugador > 0)
        {
            direccion = 1;
        }
        else
        {
            direccion = -1;
        }

        MirarHaciaDondeCamina();

        if (animator != null)
        {
            animator.SetBool("Walk", true);
        }

        float distanciaX = Mathf.Abs(
            jugador.position.x -
            transform.position.x
        );

        if (distanciaX <= distanciaAtaque)
        {
            return;
        }

        Vector2 nuevaPosicion =
            rb.position +
            Vector2.right *
            direccion *
            velocidadPersecucion *
            Time.fixedDeltaTime;

        rb.MovePosition(nuevaPosicion);
    }

    void EsperarAtaque()
    {
        if (rb != null)
        {
            rb.velocity = new Vector2(
                0,
                rb.velocity.y
            );
        }

        if (animator != null)
        {
            animator.SetBool("Walk", false);
        }

        if (jugador != null)
        {
            float direccionJugador =
                jugador.position.x -
                transform.position.x;

            if (direccionJugador > 0)
            {
                direccion = 1;
            }
            else
            {
                direccion = -1;
            }

            MirarHaciaDondeCamina();
        }
    }

    void Atacar()
    {
        if (muerto) return;
        if (atacando) return;
        if (recibiendoDanno) return;
        if (!puedeAtacar) return;

        BuscarJugador();

        if (jugador == null) return;

        float direccionJugador =
            jugador.position.x -
            transform.position.x;

        if (direccionJugador > 0)
        {
            direccion = 1;
        }
        else
        {
            direccion = -1;
        }

        MirarHaciaDondeCamina();

        atacando = true;
        puedeAtacar = false;

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Attack");
        }

        Debug.Log(
            "El infectado especial comenzó su ataque"
        );

        Invoke(
            nameof(DispararProyectil),
            tiempoAntesProyectil
        );

        Invoke(
            nameof(TerminarAtaque),
            1.5f
        );
    }

    void DispararProyectil()
    {
        if (muerto) return;
        if (jugador == null) return;
        if (proyectilAcido == null) return;
        if (puntoDisparo == null) return;

        GameObject nuevoProyectil =
            Instantiate(
                proyectilAcido,
                puntoDisparo.position,
                Quaternion.identity
            );

        AcidProjectile proyectil =
            nuevoProyectil
                .GetComponent<AcidProjectile>();

        if (proyectil != null)
        {
            proyectil.PrepararProyectil(
                jugador.position
            );

            Debug.Log(
                "El infectado especial lanzó ácido"
            );
        }
    }

    public void AlertarEnemigo()
    {
        alertado = true;

        BuscarJugador();

        if (jugador != null)
        {
            float direccionJugador =
                jugador.position.x -
                transform.position.x;

            if (direccionJugador > 0)
            {
                direccion = 1;
            }
            else
            {
                direccion = -1;
            }

            MirarHaciaDondeCamina();
        }
    }

    public void ComenzarReaccionDanno()
    {
        recibiendoDanno = true;
        atacando = false;

        CancelInvoke(
            nameof(TerminarAtaque)
        );

        CancelInvoke(
            nameof(PermitirAtaque)
        );

        CancelInvoke(
            nameof(DispararProyectil)
        );

        CancelInvoke(
            nameof(Contraatacar)
        );

        puedeAtacar = false;

        if (animator != null)
        {
            animator.SetBool("Walk", false);
        }
    }

    public void TerminarReaccionDanno()
    {
        recibiendoDanno = false;
    }

    public void PrepararContraataque()
    {
        if (muerto) return;

        CancelInvoke(
            nameof(Contraatacar)
        );

        Invoke(
            nameof(Contraatacar),
            tiempoAntesContraataque
        );
    }

    void Contraatacar()
    {
        if (muerto) return;

        recibiendoDanno = false;
        atacando = false;
        puedeAtacar = true;

        BuscarJugador();

        if (jugador == null) return;

        float direccionJugador =
            jugador.position.x -
            transform.position.x;

        if (direccionJugador > 0)
        {
            direccion = 1;
        }
        else
        {
            direccion = -1;
        }

        MirarHaciaDondeCamina();

        Atacar();
    }

    public void MarcarComoMuerto()
    {
        muerto = true;
        atacando = false;
        recibiendoDanno = false;
        puedeAtacar = false;

        CancelInvoke();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("Walk", false);
        }
    }

    void TerminarAtaque()
    {
        atacando = false;

        if (animator != null)
        {
            animator.SetBool("Walk", false);
        }

        Invoke(
            nameof(PermitirAtaque),
            tiempoEntreAtaques
        );
    }

    void PermitirAtaque()
    {
        puedeAtacar = true;
    }

    void MirarHaciaDondeCamina()
    {
        Vector3 escala =
            transform.localScale;

        if (direccion == 1)
        {
            escala.x =
                Mathf.Abs(escala.x);
        }
        else
        {
            escala.x =
                -Mathf.Abs(escala.x);
        }

        transform.localScale = escala;
    }
}