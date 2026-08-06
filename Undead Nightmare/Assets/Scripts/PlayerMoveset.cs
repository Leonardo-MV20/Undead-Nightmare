using System.Collections;
using UnityEngine;

public class PlayerMoveset : MonoBehaviour
{
    public float velocidad = 5f;

    [Header("Combate")]
    public int dannoGolpe = 20;
    public int dannoDisparo = 30;
    public float rangoAtaque = 1f;
    public float rangoDisparo = 6f;
    public Transform puntoAtaque;
    public LayerMask capaInfectado;

    [Header("Recarga")]
    public float duracionRecarga = 2f;

    [Header("Esquive")]
    public float velocidadEsquive = 10f;
    public float duracionEsquive = 0.25f;
    public float tiempoEsperaEsquive = 1f;

    [Header("Sonidos")]
    public AudioSource audioPasos;
    public AudioSource audioEfectos;

    public AudioClip sonidoAtaque;
    public AudioClip sonidoDisparo;
    public AudioClip sonidoRecarga;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerStats stats;

    private float movimientoHorizontal;
    private bool accionBloqueada = false;

    private bool esquivando = false;
    private bool puedeEsquivar = true;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        stats = GetComponent<PlayerStats>();
    }

    void Update()
    {
        if (!accionBloqueada)
        {
            movimientoHorizontal =
                Input.GetAxisRaw("Horizontal");

            bool corriendo =
                Mathf.Abs(movimientoHorizontal) > 0.01f;

            animator.SetBool("isRunning", corriendo);

            if (movimientoHorizontal > 0)
            {
                transform.localScale =
                    new Vector3(1, 1, 1);
            }
            else if (movimientoHorizontal < 0)
            {
                transform.localScale =
                    new Vector3(-1, 1, 1);
            }

            if (audioPasos != null)
            {
                if (corriendo && !audioPasos.isPlaying)
                {
                    audioPasos.Play();
                }
                else if (!corriendo && audioPasos.isPlaying)
                {
                    audioPasos.Stop();
                }
            }
        }
        else
        {
            DetenerPasos();
        }

        if (
            Input.GetKeyDown(KeyCode.L) &&
            !accionBloqueada &&
            puedeEsquivar
        )
        {
            StartCoroutine(EjecutarEsquive());
        }

        if (
            Input.GetKeyDown(KeyCode.J) &&
            !accionBloqueada
        )
        {
            StartCoroutine(EjecutarAtaque());
        }

        if (
            Input.GetKeyDown(KeyCode.K) &&
            !accionBloqueada
        )
        {
            if (
                stats != null &&
                stats.UsarBala()
            )
            {
                StartCoroutine(EjecutarDisparo());
            }
            else if (
                stats != null &&
                stats.PuedeRecargar()
            )
            {
                StartCoroutine(EjecutarRecarga());
            }
            else
            {
                Debug.Log(
                    "No hay balas ni munición de reserva"
                );
            }
        }

        if (
            Input.GetKeyDown(KeyCode.R) &&
            !accionBloqueada
        )
        {
            if (
                stats != null &&
                stats.PuedeRecargar()
            )
            {
                StartCoroutine(EjecutarRecarga());
            }
        }
    }

    void FixedUpdate()
    {
        if (esquivando)
        {
            return;
        }

        if (accionBloqueada)
        {
            rb.velocity = new Vector2(
                0,
                rb.velocity.y
            );
        }
        else
        {
            rb.velocity = new Vector2(
                movimientoHorizontal * velocidad,
                rb.velocity.y
            );
        }
    }

    IEnumerator EjecutarEsquive()
    {
        accionBloqueada = true;
        esquivando = true;
        puedeEsquivar = false;
        movimientoHorizontal = 0;

        DetenerPasos();

        animator.SetBool("isRunning", true);

        float direccionEsquive;

        if (transform.localScale.x > 0)
        {
            direccionEsquive = 1;
        }
        else
        {
            direccionEsquive = -1;
        }

        if (stats != null)
        {
            stats.CambiarInvulnerabilidad(true);
        }

        rb.velocity = new Vector2(
            direccionEsquive * velocidadEsquive,
            rb.velocity.y
        );

        yield return new WaitForSeconds(
            duracionEsquive
        );

        rb.velocity = new Vector2(
            0,
            rb.velocity.y
        );

        if (stats != null)
        {
            stats.CambiarInvulnerabilidad(false);
        }

        animator.SetBool("isRunning", false);

        esquivando = false;
        accionBloqueada = false;

        yield return new WaitForSeconds(
            tiempoEsperaEsquive
        );

        puedeEsquivar = true;
    }

    IEnumerator EjecutarAtaque()
    {
        accionBloqueada = true;
        movimientoHorizontal = 0;

        DetenerPasos();

        animator.SetBool("isRunning", false);
        animator.SetTrigger("Attack");

        if (
            audioEfectos != null &&
            sonidoAtaque != null
        )
        {
            audioEfectos.PlayOneShot(sonidoAtaque);
        }

        yield return new WaitForSeconds(0.25f);

        Collider2D[] enemigos =
            Physics2D.OverlapCircleAll(
                puntoAtaque.position,
                rangoAtaque,
                capaInfectado
            );

        foreach (Collider2D enemigo in enemigos)
        {
            EnemyHealth vidaEnemigo =
                enemigo.GetComponentInParent<EnemyHealth>();

            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDannoCuerpoACuerpo(
                    dannoGolpe,
                    transform.position
                );
            }
        }

        yield return new WaitForSeconds(0.25f);

        accionBloqueada = false;
    }

    IEnumerator EjecutarDisparo()
    {
        accionBloqueada = true;
        movimientoHorizontal = 0;

        DetenerPasos();

        animator.SetBool("isRunning", false);
        animator.SetTrigger("Shot");

        yield return new WaitForSeconds(0.2f);

        if (
            audioEfectos != null &&
            sonidoDisparo != null
        )
        {
            audioEfectos.PlayOneShot(sonidoDisparo);
        }

        Vector2 direccion;

        if (transform.localScale.x > 0)
        {
            direccion = Vector2.right;
        }
        else
        {
            direccion = Vector2.left;
        }

        Vector2 origen =
            (Vector2)transform.position +
            new Vector2(
                direccion.x * 1f,
                -0.5f
            );

        RaycastHit2D hit =
            Physics2D.Raycast(
                origen,
                direccion,
                rangoDisparo
            );

        Debug.DrawRay(
            origen,
            direccion * rangoDisparo,
            Color.red,
            1f
        );

        if (hit.collider != null)
        {
            Debug.Log(
                "El disparo golpeó a: " +
                hit.collider.name
            );

            EnemyHealth enemigo =
                hit.collider
                    .GetComponentInParent<EnemyHealth>();

            if (enemigo != null)
            {
                enemigo.RecibirDannoDisparo(
                    dannoDisparo,
                    transform.position
                );

                Debug.Log(
                    "Vida enemigo: " +
                    enemigo.vidaActual
                );
            }
        }
        else
        {
            Debug.Log(
                "El disparo no golpeó nada"
            );
        }

        yield return new WaitForSeconds(0.4f);

        accionBloqueada = false;

        if (
            stats != null &&
            stats.balasActuales <= 0 &&
            stats.PuedeRecargar()
        )
        {
            StartCoroutine(EjecutarRecarga());
        }
    }

    IEnumerator EjecutarRecarga()
    {
        accionBloqueada = true;
        movimientoHorizontal = 0;

        DetenerPasos();

        animator.SetBool("isRunning", false);
        animator.SetTrigger("Recharge");

        if (
            audioEfectos != null &&
            sonidoRecarga != null
        )
        {
            audioEfectos.PlayOneShot(sonidoRecarga);
        }

        yield return new WaitForSeconds(
            duracionRecarga
        );

        if (stats != null)
        {
            stats.Recargar();
        }

        accionBloqueada = false;
    }

    public void DetenerPasos()
    {
        if (
            audioPasos != null &&
            audioPasos.isPlaying
        )
        {
            audioPasos.Stop();
        }
    }

    void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.DrawWireSphere(
                puntoAtaque.position,
                rangoAtaque
            );
        }
    }
}