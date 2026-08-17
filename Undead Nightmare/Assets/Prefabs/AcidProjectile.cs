using UnityEngine;

public class AcidProjectile : MonoBehaviour
{
    public float fuerzaHorizontal = 5f;
    public float fuerzaVertical = 6f;
    public int danno = 15;
    public float tiempoVida = 5f;

    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        Destroy(
            gameObject,
            tiempoVida
        );
    }

    public void PrepararProyectil(
        Vector3 posicionJugador
    )
    {
        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }

        float direccionX;

        if (posicionJugador.x > transform.position.x)
        {
            direccionX = 1;
        }
        else
        {
            direccionX = -1;
        }

        rb.velocity = new Vector2(
            direccionX * fuerzaHorizontal,
            fuerzaVertical
        );
    }

    void OnTriggerEnter2D(
        Collider2D collision
    )
    {
        if (collision.CompareTag("Player"))
        {
            PlayerStats statsJugador =
                collision.GetComponent<PlayerStats>();

            if (statsJugador == null)
            {
                statsJugador =
                    collision.GetComponentInParent<PlayerStats>();
            }

            if (statsJugador != null)
            {
                statsJugador.RecibirDaño(
                    danno,
                    transform.position
                );

                Debug.Log(
                    "El ácido golpeó al jugador"
                );
            }

            Destroy(gameObject);
        }
    }
}