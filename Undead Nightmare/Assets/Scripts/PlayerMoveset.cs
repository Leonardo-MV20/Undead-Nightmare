using System.Collections;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class PlayerMoveset : MonoBehaviour
{
    public float velocidad = 5f;

    [Header("Combate")]
    public int dañoGolpe = 20;
    public int dañoDisparo = 30;
    public float rangoAtaque = 1f;
    public float rangoDisparo = 6f;
    public Transform puntoAtaque;
    public LayerMask capaInfectado;

    private Rigidbody2D rb;
    private Animator animator;
    private PlayerStats stats;
    private float movimientoHorizontal;
    private bool accionBloqueada = false;

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
            movimientoHorizontal = Input.GetAxisRaw("Horizontal");

            bool corriendo = Mathf.Abs(movimientoHorizontal) > 0.01f;
            animator.SetBool("isRunning", corriendo);

            if (movimientoHorizontal > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (movimientoHorizontal < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
        if (Input.GetKeyDown(KeyCode.J) && !accionBloqueada)
        {
            StartCoroutine(EjecutarAtaque());
        }

        if (Input.GetKeyDown(KeyCode.K) && !accionBloqueada)
        {
            Debug.Log("Presioné K para disparar");

            PlayerStats stats = GetComponent<PlayerStats>();

            if (stats != null && stats.UsarMunicion(1))
            {
                Debug.Log("Tengo munición, voy a disparar");
                StartCoroutine(EjecutarDisparo());
            }
            else
            {
                Debug.Log("No tengo munición");
            }
        }

        if (Input.GetKeyDown(KeyCode.R) && !accionBloqueada)
        {
            StartCoroutine(EjecutarAccion("Recharge", 2.0f));
        }
    }

    void FixedUpdate()
    {
        if (accionBloqueada)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
        }
        else
        {
            rb.velocity = new Vector2(movimientoHorizontal * velocidad, rb.velocity.y);
        }
    }

    IEnumerator EjecutarAccion(string nombreAnimacion, float duracion)
    {
        accionBloqueada = true;
        movimientoHorizontal = 0;
        animator.SetBool("isRunning", false);
        animator.SetTrigger(nombreAnimacion);

        yield return new WaitForSeconds(duracion);

        accionBloqueada = false;
    }

    IEnumerator EjecutarAtaque()
    {
        accionBloqueada = true;
        movimientoHorizontal = 0;
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(0.25f);

        Collider2D[] enemigos = Physics2D.OverlapCircleAll(puntoAtaque.position, rangoAtaque, capaInfectado);

        foreach (Collider2D enemigo in enemigos)
        {
            EnemyHealth vidaEnemigo = enemigo.GetComponent<EnemyHealth>();

            if (vidaEnemigo != null)
            {
                vidaEnemigo.RecibirDaño(dañoGolpe);
            }
        }

        yield return new WaitForSeconds(0.25f);

        accionBloqueada = false;
    }

    IEnumerator EjecutarDisparo()
    {
        Debug.Log("Entré a EjecutarDisparo");

        accionBloqueada = true;
        movimientoHorizontal = 0;
        animator.SetBool("isRunning", false);
        animator.SetTrigger("Shot");

        yield return new WaitForSeconds(0.2f);

        Vector2 direccion = transform.localScale.x > 0 ? Vector2.right : Vector2.left;

        Vector2 origen = (Vector2)transform.position + new Vector2(direccion.x * 1f, -0.5f);

        RaycastHit2D hit = Physics2D.Raycast(origen, direccion, 20f);

        Debug.DrawRay(origen, direccion * 20f, Color.red, 1f);

        Debug.DrawRay(transform.position, direccion * 20f, Color.red, 1f);

        if (hit.collider != null)
        {
            Debug.Log("El disparo golpeó a: " + hit.collider.name);

            EnemyHealth enemigo = hit.collider.GetComponentInParent<EnemyHealth>();

            if (enemigo != null)
            {
                enemigo.RecibirDaño(20);
                Debug.Log("Vida enemigo: " + enemigo.vidaActual);
            }
        }
        else
        {
            Debug.Log("El disparo no golpeó nada");
        }

        yield return new WaitForSeconds(0.4f);

        accionBloqueada = false;
    }

    void OnDrawGizmosSelected()
    {
        if (puntoAtaque != null)
        {
            Gizmos.DrawWireSphere(puntoAtaque.position, rangoAtaque);
        }
    }
}