using UnityEngine;

public class DoorController : MonoBehaviour
{
    public Animator animator;
    public BoxCollider2D colliderPuerta;

    private bool abierta = false;

    void Start()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        if (colliderPuerta == null)
        {
            colliderPuerta = GetComponent<BoxCollider2D>();
        }
    }

    public void AbrirPuerta()
    {
        if (abierta) return;

        abierta = true;

        if (animator != null)
        {
            animator.SetTrigger("Open");
        }

        if (colliderPuerta != null)
        {
            colliderPuerta.enabled = false;

            Debug.Log(
                "Collider de " +
                gameObject.name +
                " desactivado"
            );
        }
    }
}