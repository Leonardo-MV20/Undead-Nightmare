using System.Collections;
using UnityEngine;

public class PlayerMoveset : MonoBehaviour
{
    public float velocidad = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private float movimientoHorizontal;
    private bool accionBloqueada = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
            StartCoroutine(EjecutarAccion("Attack", 0.5f));
        }

        if (Input.GetKeyDown(KeyCode.K) && !accionBloqueada)
        {
            StartCoroutine(EjecutarAccion("Shot", 0.6f));
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
}