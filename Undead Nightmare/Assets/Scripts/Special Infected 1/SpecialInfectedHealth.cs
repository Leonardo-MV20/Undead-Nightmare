using System.Collections;
using UnityEngine;

public class SpecialInfectedHealth : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 60;
    public int vidaActual;

    [Header("Reacción al golpe")]
    public float fuerzaRetroceso = 3f;
    public float duracionRetroceso = 0.2f;
    public float duracionAnimacionDanno = 0.5f;

    [Header("Muerte")]
    public float duracionAnimacionMuerte = 1f;

    [Header("Puerta")]
    public DoorController puerta;
    private Animator animator;
    private Rigidbody2D rb;
    private SpecialInfectedMovement movimiento;

    private bool recibiendoDanno = false;
    private bool muerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        movimiento =
            GetComponent<SpecialInfectedMovement>();
    }

    public void RecibirDannoCuerpoACuerpo(
        int cantidad,
        Vector3 posicionJugador
    )
    {
        if (muerto) return;

        vidaActual -= cantidad;

        Debug.Log(
            "Infectado especial recibió golpe. Vida actual: "
            + vidaActual
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

        if (!recibiendoDanno)
        {
            StartCoroutine(
                ReaccionarAlGolpe(
                    posicionJugador
                )
            );
        }
    }

    public void RecibirDannoDisparo(
        int cantidad,
        Vector3 posicionJugador
    )
    {
        if (muerto) return;

        Debug.Log(
            "El infectado especial es inmune a las balas"
        );

        if (movimiento != null)
        {
            movimiento.AlertarEnemigo();
        }
    }

    IEnumerator ReaccionarAlGolpe(
        Vector3 posicionJugador
    )
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

        if (
            transform.position.x <
            posicionJugador.x
        )
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
                direccionRetroceso *
                fuerzaRetroceso,
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

        if (movimiento != null)
        {
            movimiento.TerminarReaccionDanno();
            movimiento.PrepararContraataque();
        }
    }

    void Morir()
    {
        if (muerto) return;

        muerto = true;

        CancelInvoke();

        Debug.Log(
            "El infectado especial murió"
        );

        if (movimiento != null)
        {
            movimiento.MarcarComoMuerto();
        }

        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.ResetTrigger("Attack");
            animator.SetTrigger("Dead");
        }

        if (puerta != null)
        {
            puerta.AbrirPuerta();
        }

        Destroy(
            gameObject,
            duracionAnimacionMuerte
        );
    }
}