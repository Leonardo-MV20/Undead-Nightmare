using UnityEngine;

public class SpecialInfected2Movement : MonoBehaviour
{
    [Header("Movimiento")]
    public float velocidadPatrulla = 1f;
    public float velocidadPersecucion = 4f;
    public float velocidadCercana = 2.5f;
    public float distanciaPatrulla = 2f;

    [Header("Detección")]
    public float distanciaDeteccion = 10f;
    public float distanciaSalto = 6f;
    public float distanciaReactivarSalto = 6f;
    public float distanciaAtaque = 2.5f;

    [Header("Salto")]
    public float fuerzaSaltoHorizontal = 7f;
    public float fuerzaSaltoVertical = 3f;
    public float duracionSalto = 0.8f;
    public float distanciaMaximaSaltoForzado = 6f;

    [Header("Daño de salto")]
    public int dannoSalto = 30;
    public float distanciaDannoSalto = 2f;

    [Header("Ataques")]
    public int dannoAtaque = 20;

    public float tiempoAtaque1 = 0.3f;
    public float tiempoAtaque2 = 0.7f;
    public float tiempoAtaque3 = 1.1f;

    public float duracionCombo = 1.5f;

    [Header("Avance en ataque")]
    public float avanceAtaque = 1.5f;

    [Header("Fases")]
    public float tiempoAgresivo = 10f;
    public float tiempoCansado = 5f;

    [Header("Jugador")]
    public string tagJugador = "Player";

    [Header("Musica")]
    public AudioSource musicaJefe;
    public AudioSource musicaPrincipal;

    private Rigidbody2D rb;
    private Animator animator;
    private Transform jugador;

    private Vector2 puntoInicial;
    private int direccion = 1;

    private bool saltando = false;
    private bool atacando = false;
    private bool dannoSaltoRealizado = false;
    private bool muerto = false;
    private bool puedeSaltar = true;
    private bool musicaIniciada = false;
    private bool cansado = false;
    private bool faseAgresivaIniciada = false;

    private SpecialInfected2Health vida;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        vida = GetComponent<SpecialInfected2Health>();

        if (rb == null)
        {
            Debug.LogError(
                "Special Infected 2 no tiene Rigidbody2D."
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

        if (cansado)
        {
            MantenerseCansado();
            return;
        }

        if (saltando)
        {
            RevisarDannoSalto();
            return;
        }

        if (atacando) return;

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

        if (
            distanciaJugador <= distanciaDeteccion &&
            !faseAgresivaIniciada
        )
        {
            IniciarMusicaJefe();
            IniciarFaseAgresiva();
        }

        if (
            distanciaJugador >=
            distanciaReactivarSalto
        )
        {
            puedeSaltar = true;
        }

        if (distanciaJugador <= distanciaAtaque)
        {
            IniciarCombo();
        }
        else if (
            distanciaJugador <= distanciaSalto &&
            puedeSaltar
        )
        {
            SaltarHaciaJugador();
        }
        else if (
            distanciaJugador <= distanciaSalto
        )
        {
            AcercarseAlJugador();
        }
        else if (
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
            animator.SetBool("Run", false);
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

        ActualizarDireccionJugador();

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", true);
        }

        Vector2 nuevaPosicion =
            rb.position +
            Vector2.right *
            direccion *
            velocidadPersecucion *
            Time.fixedDeltaTime;

        rb.MovePosition(nuevaPosicion);
    }

    void AcercarseAlJugador()
    {
        if (jugador == null) return;

        ActualizarDireccionJugador();

        if (animator != null)
        {
            animator.SetBool("Run", false);
            animator.SetBool("Walk", true);
        }

        Vector2 nuevaPosicion =
            rb.position +
            Vector2.right *
            direccion *
            velocidadCercana *
            Time.fixedDeltaTime;

        rb.MovePosition(nuevaPosicion);
    }

    void SaltarHaciaJugador()
    {
        if (saltando) return;
        if (!puedeSaltar) return;
        if (jugador == null) return;
        if (cansado) return;

        saltando = true;
        puedeSaltar = false;
        dannoSaltoRealizado = false;

        CancelarAtaquesPendientes();

        ActualizarDireccionJugador();

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");

            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }

        rb.velocity = new Vector2(
            direccion * fuerzaSaltoHorizontal,
            fuerzaSaltoVertical
        );

        Invoke(
            nameof(TerminarSalto),
            duracionSalto
        );

        Debug.Log(
            "Special Infected 2 saltó hacia el jugador"
        );
    }

    void RevisarDannoSalto()
    {
        if (muerto) return;
        if (cansado) return;
        if (jugador == null) return;
        if (dannoSaltoRealizado) return;

        float distanciaX = Mathf.Abs(
            jugador.position.x -
            transform.position.x
        );

        float distanciaY = Mathf.Abs(
            jugador.position.y -
            transform.position.y
        );

        if (
            distanciaX <= distanciaDannoSalto &&
            distanciaY <= 2f
        )
        {
            PlayerStats statsJugador =
                jugador.GetComponent<PlayerStats>();

            if (statsJugador != null)
            {
                statsJugador.RecibirDaño(
                    dannoSalto,
                    transform.position
                );

                dannoSaltoRealizado = true;

                Debug.Log(
                    "El salto del Special Infected 2 dañó al jugador"
                );
            }
        }
    }

    void TerminarSalto()
    {
        saltando = false;

        if (rb != null)
        {
            rb.velocity = new Vector2(
                0,
                rb.velocity.y
            );
        }

        if (animator != null)
        {
            animator.SetBool("Run", false);
            animator.SetBool("Walk", false);
        }
    }

    void IniciarCombo()
    {
        if (atacando) return;
        if (jugador == null) return;
        if (cansado) return;

        CancelarAtaquesPendientes();

        atacando = true;

        ActualizarDireccionJugador();

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
            animator.SetBool("Run", false);

            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");

            animator.SetTrigger("Attack1");
        }

        Invoke(
            nameof(GolpeAtaque1),
            tiempoAtaque1
        );

        Invoke(
            nameof(IniciarAtaque2),
            tiempoAtaque2
        );

        Invoke(
            nameof(IniciarAtaque3),
            tiempoAtaque3
        );

        Invoke(
            nameof(TerminarCombo),
            duracionCombo
        );
    }

    void GolpeAtaque1()
    {
        if (muerto) return;
        if (cansado) return;

        ActualizarDireccionJugador();

        AvanzarDuranteAtaque();
        HacerDanno();
    }

    void IniciarAtaque2()
    {
        if (muerto) return;
        if (!atacando) return;
        if (cansado) return;

        ActualizarDireccionJugador();

        if (animator != null)
        {
            animator.ResetTrigger("Attack2");
            animator.SetTrigger("Attack2");
        }

        AvanzarDuranteAtaque();
        HacerDanno();
    }

    void IniciarAtaque3()
    {
        if (muerto) return;
        if (!atacando) return;
        if (cansado) return;

        ActualizarDireccionJugador();

        if (animator != null)
        {
            animator.ResetTrigger("Attack3");
            animator.SetTrigger("Attack3");
        }

        AvanzarDuranteAtaque();
        HacerDanno();
    }

    void AvanzarDuranteAtaque()
    {
        if (rb == null) return;

        Vector2 nuevaPosicion =
            rb.position +
            Vector2.right *
            direccion *
            avanceAtaque;

        rb.MovePosition(
            nuevaPosicion
        );
    }

    void HacerDanno()
    {
        if (jugador == null) return;
        if (cansado) return;

        float distanciaJugador = Mathf.Abs(
            jugador.position.x -
            transform.position.x
        );

        if (distanciaJugador <= distanciaAtaque)
        {
            PlayerStats statsJugador =
                jugador.GetComponent<PlayerStats>();

            if (statsJugador != null)
            {
                statsJugador.RecibirDaño(
                    dannoAtaque,
                    transform.position
                );

                Debug.Log(
                    "Special Infected 2 golpeó al jugador"
                );
            }
        }
    }

    void TerminarCombo()
    {
        atacando = false;
    }

    void IniciarFaseAgresiva()
    {
        if (muerto) return;

        faseAgresivaIniciada = true;
        cansado = false;

        CancelInvoke(
            nameof(Cansarse)
        );

        Invoke(
            nameof(Cansarse),
            tiempoAgresivo
        );

        Debug.Log(
            "Special Infected 2 inició su fase agresiva"
        );
    }

    void Cansarse()
    {
        if (muerto) return;

        cansado = true;
        faseAgresivaIniciada = false;

        saltando = false;
        atacando = false;
        puedeSaltar = false;

        CancelarAtaquesPendientes();

        CancelInvoke(
            nameof(TerminarSalto)
        );

        CancelInvoke(
            nameof(RevisarDannoSalto)
        );

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            animator.ResetTrigger("Jump");
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");

            animator.Play("Idle");
        }

        if (vida != null)
        {
            vida.ReiniciarDisparosCansado();
        }

        Invoke(
            nameof(TerminarCansancioPorTiempo),
            tiempoCansado
        );

        Debug.Log(
            "Special Infected 2 está cansado"
        );
    }

    void MantenerseCansado()
    {
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);
        }
    }

    void TerminarCansancioPorTiempo()
    {
        if (muerto) return;
        if (!cansado) return;

        cansado = false;
        faseAgresivaIniciada = false;
        puedeSaltar = true;

        if (vida != null)
        {
            vida.ReiniciarDisparosCansado();
        }

        Debug.Log(
            "Special Infected 2 terminó su descanso"
        );
    }

    public bool EstaCansado()
    {
        return cansado;
    }

    public void ReactivarDespuesDeDisparos()
    {
        if (muerto) return;

        cansado = false;
        faseAgresivaIniciada = true;

        saltando = false;
        atacando = false;
        puedeSaltar = false;

        CancelInvoke();

        BuscarJugador();

        if (jugador == null)
        {
            faseAgresivaIniciada = false;
            puedeSaltar = true;
            return;
        }

        Invoke(
            nameof(Cansarse),
            tiempoAgresivo
        );

        Debug.Log(
            "Special Infected 2 recibió tres disparos y vuelve a atacar"
        );

        SaltarForzadoHaciaJugador();
    }

    void SaltarForzadoHaciaJugador()
    {
        if (muerto) return;
        if (jugador == null) return;

        float distanciaJugador = Mathf.Abs(
            jugador.position.x -
            transform.position.x
        );

        if (
            distanciaJugador >
            distanciaMaximaSaltoForzado
        )
        {
            puedeSaltar = true;
            return;
        }

        saltando = true;
        puedeSaltar = false;

        CancelarAtaquesPendientes();

        ActualizarDireccionJugador();

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            animator.ResetTrigger("Hurt");
            animator.ResetTrigger("Jump");
            animator.SetTrigger("Jump");
        }

        rb.velocity = new Vector2(
            direccion * fuerzaSaltoHorizontal,
            fuerzaSaltoVertical
        );

        Invoke(
            nameof(TerminarSalto),
            duracionSalto
        );

        Debug.Log(
            "Special Infected 2 contraatacó con un salto"
        );
    }

    void CancelarAtaquesPendientes()
    {
        CancelInvoke(
            nameof(GolpeAtaque1)
        );

        CancelInvoke(
            nameof(IniciarAtaque2)
        );

        CancelInvoke(
            nameof(IniciarAtaque3)
        );

        CancelInvoke(
            nameof(TerminarCombo)
        );

        atacando = false;
    }

    void ActualizarDireccionJugador()
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
    }

    public void MarcarComoMuerto()
    {
        muerto = true;

        cansado = false;
        saltando = false;
        atacando = false;

        puedeSaltar = false;
        faseAgresivaIniciada = false;

        CancelInvoke();

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            animator.ResetTrigger("Jump");
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");
        }

        if (musicaJefe != null)
        {
            musicaJefe.Stop();
        }
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

    void IniciarMusicaJefe()
    {
        if (musicaIniciada) return;

        musicaIniciada = true;

        if (musicaPrincipal != null)
        {
            musicaPrincipal.Stop();
        }

        if (musicaJefe != null)
        {
            musicaJefe.Play();
        }

        Debug.Log(
            "Musica de jefe iniciada"
        );
    }
}