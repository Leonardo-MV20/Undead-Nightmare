using System.Collections;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    public int vidaActual;

    [Header("Resistencia a disparos")]
    public int divisorDannoDisparo = 5;

    [Header("Reacción al golpe")]
    public float fuerzaRetroceso = 3f;
    public float duracionRetroceso = 0.2f;
    public float duracionAnimacionDanno = 0.5f;

    [Header("Golpes consecutivos")]
    public float tiempoReinicioGolpes = 2f;

    [Header("Muerte")]
    public float duracionAnimacionMuerte = 1f;

    private Animator animator;
    private Rigidbody2D rb;
    private C_I_Movement movimiento;

    private int golpesConsecutivos = 0;
    private bool recibiendoDanno = false;
    private bool muerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        movimiento = GetComponent<C_I_Movement>();
    }

    public void RecibirDannoCuerpoACuerpo(
        int cantidad,
        Vector3 posicionJugador
    )
    {
        if (muerto) return;

        vidaActual -= cantidad;

        Debug.Log(
            "Infectado recibió golpe. Vida actual: " + vidaActual
        );

        if (movimiento != null)
        {
            movimiento.AlertarEnemigo();
        }

        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        golpesConsecutivos++;

        CancelInvoke(nameof(ReiniciarGolpesConsecutivos));

        if (golpesConsecutivos <= 2)
        {
            if (!recibiendoDanno)
            {
                StartCoroutine(
                    ReaccionarAlGolpe(posicionJugador)
                );
            }

            Invoke(
                nameof(ReiniciarGolpesConsecutivos),
                tiempoReinicioGolpes
            );
        }
        else
        {
            Debug.Log(
                "El infectado recibió tres golpes seguidos y contraatacó"
            );

            golpesConsecutivos = 0;

            if (movimiento != null)
            {
                movimiento.ForzarAtaque();
            }
        }
    }

    public void RecibirDannoDisparo(
        int cantidad,
        Vector3 posicionJugador
    )
    {
        if (muerto) return;

        int dannoReducido = cantidad / divisorDannoDisparo;

        if (dannoReducido < 1)
        {
            dannoReducido = 1;
        }

        vidaActual -= dannoReducido;

        Debug.Log(
            "Infectado recibió disparo reducido: " + dannoReducido
        );

        Debug.Log(
            "Vida actual del infectado: " + vidaActual
        );

        if (movimiento != null)
        {
            movimiento.AlertarEnemigo();
        }

        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    IEnumerator ReaccionarAlGolpe(Vector3 posicionJugador)
    {
        recibiendoDanno = true;

        if (movimiento != null)
        {
            movimiento.ComenzarReaccionDanno();
        }

        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Hurt");
        }

        float direccionRetroceso;

        if (transform.position.x < posicionJugador.x)
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

        yield return new WaitForSeconds(duracionRetroceso);

        if (rb != null)
        {
            rb.velocity = new Vector2(
                0,
                rb.velocity.y
            );
        }

        float tiempoRestante =
            duracionAnimacionDanno - duracionRetroceso;

        if (tiempoRestante > 0)
        {
            yield return new WaitForSeconds(tiempoRestante);
        }

        recibiendoDanno = false;

        if (movimiento != null)
        {
            movimiento.TerminarReaccionDanno();
        }
    }

    void ReiniciarGolpesConsecutivos()
    {
        golpesConsecutivos = 0;

        Debug.Log(
            "Se reinició el contador de golpes del infectado"
        );
    }

    void Morir()
    {
        if (muerto) return;

        muerto = true;

        CancelInvoke();

        Debug.Log("El infectado murió");

        if (movimiento != null)
        {
            movimiento.MarcarComoMuerto();
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        Collider2D colliderEnemigo =
            GetComponent<Collider2D>();

        if (colliderEnemigo != null)
        {
            colliderEnemigo.enabled = false;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Death");
        }

        Destroy(
            gameObject,
            duracionAnimacionMuerte
        );
    }
}