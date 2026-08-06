using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    public int vidaActual = 100;

    [Header("Munición")]
    public int balasMaximasCargador = 30;
    public int balasActuales = 30;

    public int municionReservaMaxima = 60;
    public int municionReservaActual = 0;

    [Header("Reacción al daño")]
    public float fuerzaRetroceso = 4f;
    public float duracionRetroceso = 0.2f;
    public float duracionAnimacionDanno = 0.5f;

    [Header("Muerte y reaparición")]
    public float tiempoParaRespawnear = 1.5f;

    [Header("Sonidos")]
    public AudioSource audioEfectos;
    public AudioSource audioPasos;
    public AudioClip sonidoDanno;

    public UnityEvent OnStatsChanged = new UnityEvent();

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerMoveset movimientoJugador;

    private Vector3 puntoInicial;

    private bool recibiendoDanno = false;
    private bool muerto = false;
    private bool invulnerable = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        movimientoJugador = GetComponent<PlayerMoveset>();

        puntoInicial = transform.position;

        vidaActual = Mathf.Clamp(
            vidaActual,
            0,
            vidaMaxima
        );

        balasActuales = Mathf.Clamp(
            balasActuales,
            0,
            balasMaximasCargador
        );

        municionReservaActual = Mathf.Clamp(
            municionReservaActual,
            0,
            municionReservaMaxima
        );

        OnStatsChanged.Invoke();
    }

    public bool TieneBalasEnCargador()
    {
        return balasActuales > 0;
    }

    public bool UsarBala()
    {
        if (balasActuales <= 0)
        {
            Debug.Log("El cargador está vacío");
            return false;
        }

        balasActuales--;

        OnStatsChanged.Invoke();

        return true;
    }

    public bool PuedeRecargar()
    {
        if (balasActuales >= balasMaximasCargador)
        {
            Debug.Log("El cargador ya está lleno");
            return false;
        }

        if (municionReservaActual <= 0)
        {
            Debug.Log("No hay munición de reserva");
            return false;
        }

        return true;
    }

    public void Recargar()
    {
        if (!PuedeRecargar())
        {
            return;
        }

        int balasNecesarias =
            balasMaximasCargador - balasActuales;

        if (municionReservaActual >= balasNecesarias)
        {
            balasActuales += balasNecesarias;
            municionReservaActual -= balasNecesarias;
        }
        else
        {
            balasActuales += municionReservaActual;
            municionReservaActual = 0;
        }

        Debug.Log(
            "Cargador: " + balasActuales +
            " | Reserva: " + municionReservaActual
        );

        OnStatsChanged.Invoke();
    }

    public void AgregarMunicion(int cantidad)
    {
        municionReservaActual = Mathf.Min(
            municionReservaActual + cantidad,
            municionReservaMaxima
        );

        Debug.Log(
            "Munición de reserva: " +
            municionReservaActual
        );

        OnStatsChanged.Invoke();
    }

    public void Curar(int cantidad)
    {
        if (muerto) return;

        vidaActual = Mathf.Min(
            vidaActual + cantidad,
            vidaMaxima
        );

        OnStatsChanged.Invoke();
    }

    public void CambiarInvulnerabilidad(bool estado)
    {
        invulnerable = estado;
    }

    public void RecibirDaño(
        int cantidad,
        Vector3 posicionEnemigo
    )
    {
        if (invulnerable)
        {
            Debug.Log("El jugador esquivó el ataque");
            return;
        }

        if (muerto) return;
        if (recibiendoDanno) return;

        vidaActual = Mathf.Max(
            vidaActual - cantidad,
            0
        );

        Debug.Log(
            "Jugador recibió daño. Vida actual: " +
            vidaActual
        );

        OnStatsChanged.Invoke();

        if (vidaActual <= 0)
        {
            StartCoroutine(MorirYRespawnear());
        }
        else
        {
            StartCoroutine(
                ReaccionarAlDanno(posicionEnemigo)
            );
        }
    }

    IEnumerator ReaccionarAlDanno(
        Vector3 posicionEnemigo
    )
    {
        recibiendoDanno = true;

        if (movimientoJugador != null)
        {
            movimientoJugador.DetenerPasos();
            movimientoJugador.enabled = false;
        }

        if (
            audioPasos != null &&
            audioPasos.isPlaying
        )
        {
            audioPasos.Stop();
        }

        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Hurt");
            animator.SetBool("isRunning", false);
        }

        if (
            audioEfectos != null &&
            sonidoDanno != null
        )
        {
            audioEfectos.PlayOneShot(sonidoDanno);
        }

        float direccionRetroceso;

        if (transform.position.x < posicionEnemigo.x)
        {
            direccionRetroceso = -1;
        }
        else
        {
            direccionRetroceso = 1;
        }

        if (rb != null)
        {
            rb.velocity = new Vector2(
                direccionRetroceso * fuerzaRetroceso,
                rb.velocity.y
            );
        }

        yield return new WaitForSeconds(
            duracionRetroceso
        );

        if (rb != null)
        {
            rb.velocity = new Vector2(
                0,
                rb.velocity.y
            );
        }

        float tiempoRestante =
            duracionAnimacionDanno -
            duracionRetroceso;

        if (tiempoRestante > 0)
        {
            yield return new WaitForSeconds(
                tiempoRestante
            );
        }

        recibiendoDanno = false;

        if (movimientoJugador != null)
        {
            movimientoJugador.enabled = true;
        }
    }

    IEnumerator MorirYRespawnear()
    {
        muerto = true;
        recibiendoDanno = false;
        invulnerable = false;

        if (movimientoJugador != null)
        {
            movimientoJugador.DetenerPasos();
            movimientoJugador.enabled = false;
        }

        if (
            audioPasos != null &&
            audioPasos.isPlaying
        )
        {
            audioPasos.Stop();
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.SetBool("isRunning", false);
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Dead");
        }

        Debug.Log("Jugador muerto");

        yield return new WaitForSeconds(
            tiempoParaRespawnear
        );

        transform.position = puntoInicial;

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        vidaActual = vidaMaxima;

        OnStatsChanged.Invoke();

        if (animator != null)
        {
            animator.Play("Idle");
        }

        muerto = false;

        if (movimientoJugador != null)
        {
            movimientoJugador.enabled = true;
        }

        Debug.Log("Jugador reapareció");
    }
}