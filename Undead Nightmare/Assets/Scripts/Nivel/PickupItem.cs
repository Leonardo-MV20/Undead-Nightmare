using UnityEngine;

public enum TipoPickup
{
    Municion,
    Botiquin
}

[RequireComponent(typeof(Collider2D))]
public class PickupItem : MonoBehaviour
{
    public TipoPickup tipoPickup;
    public int cantidad = 10;
    public AudioClip sonidoRecoger;

    private bool recogido = false;

    void Reset()
    {
        GetComponent<Collider2D>().isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (recogido) return;

        if (!collision.CompareTag("Player")) return;

        PlayerStats stats = collision.GetComponent<PlayerStats>();

        if (stats == null)
        {
            stats = collision.GetComponentInParent<PlayerStats>();
        }

        if (stats == null) return;

        if (tipoPickup == TipoPickup.Municion)
        {
            stats.AgregarMunicion(cantidad);
        }
        else if (tipoPickup == TipoPickup.Botiquin)
        {
            stats.Curar(cantidad);
        }

        recogido = true;

        if (sonidoRecoger != null)
        {
            AudioSource.PlayClipAtPoint(sonidoRecoger, transform.position);
        }

        Destroy(gameObject);
    }
}