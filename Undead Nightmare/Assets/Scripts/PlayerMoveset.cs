using UnityEngine;

public class PlayerMoveset : MonoBehaviour
{
    public float velocidad = 5f;

    private Rigidbody2D rb;
    private Animator animator;
    private float movimientoHorizontal;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        movimientoHorizontal = Input.GetAxisRaw("Horizontal");

        bool corriendo = Mathf.Abs(movimientoHorizontal) > 0.01f;
        animator.SetBool("isRunning", corriendo);

        if (movimientoHorizontal > 0)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if (movimientoHorizontal < 0)
        {
            transform.localScale = new Vector3(-1, 1, 1);
        }
    }

    void FixedUpdate()
    {
        rb.velocity = new Vector2(movimientoHorizontal * velocidad, rb.velocity.y);
    }
}
