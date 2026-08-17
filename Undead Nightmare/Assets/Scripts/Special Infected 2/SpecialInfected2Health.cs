using UnityEngine;

public class SpecialInfected2Health : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    public int vidaActual;

    [Header("Disparos")]
    public int dannoPorDisparo = 10;
    public int maximoDisparosCansado = 3;

    [Header("Muerte")]
    public float duracionAnimacionMuerte = 1f;

    [Header("Juego completado")]
    public GameCompleteUI gameCompleteUI;

    private Animator animator;
    private Rigidbody2D rb;
    private SpecialInfected2Movement movimiento;

    private int disparosRecibidos = 0;
    private bool muerto = false;

    void Start()
    {
        vidaActual = vidaMaxima;

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();

        movimiento =
            GetComponent<SpecialInfected2Movement>();
    }

    public void RecibirDannoCuerpoACuerpo(
        int cantidad,
        Vector3 posicionJugador
    )
    {
        if (muerto) return;

        Debug.Log(
            "Special Infected 2 es inmune al ataque cuerpo a cuerpo"
        );
    }

    public void RecibirDannoDisparo(
        int cantidad,
        Vector3 posicionJugador
    )
    {
        if (muerto) return;
        if (movimiento == null) return;

        if (!movimiento.EstaCansado())
        {
            Debug.Log(
                "Special Infected 2 no recibe daño mientras está agresivo"
            );

            return;
        }

        vidaActual -= dannoPorDisparo;
        disparosRecibidos++;

        Debug.Log(
            "Special Infected 2 recibió disparo. Vida actual: "
            + vidaActual
        );

        Debug.Log(
            "Disparos durante cansancio: "
            + disparosRecibidos
        );

        if (vidaActual <= 0)
        {
            Morir();
            return;
        }

        if (
            disparosRecibidos >=
            maximoDisparosCansado
        )
        {
            disparosRecibidos = 0;

            movimiento.ReactivarDespuesDeDisparos();

            return;
        }

        if (animator != null)
        {
            animator.ResetTrigger("Hurt");
            animator.SetTrigger("Hurt");
        }
    }

    public void ReiniciarDisparosCansado()
    {
        disparosRecibidos = 0;
    }

    void Morir()
    {
        if (muerto) return;

        muerto = true;

        Debug.Log(
            "Special Infected 2 murió"
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
            animator.SetBool("Walk", false);
            animator.SetBool("Run", false);

            animator.ResetTrigger("Hurt");
            animator.ResetTrigger("Jump");
            animator.ResetTrigger("Attack1");
            animator.ResetTrigger("Attack2");
            animator.ResetTrigger("Attack3");

            animator.SetTrigger("Dead");
        }

        Invoke(
            nameof(MostrarVictoria),
            duracionAnimacionMuerte
        );

        Destroy(
            gameObject,
            duracionAnimacionMuerte
        );
    }

    void MostrarVictoria()
    {
        if (gameCompleteUI != null)
        {
            gameCompleteUI.MostrarJuegoCompletado();
        }
    }
}