using UnityEngine;
using UnityEngine.Events;

public class PlayerStats : MonoBehaviour
{
    [Header("Vida")]
    public int vidaMaxima = 100;
    public int vidaActual = 100;

    [Header("Municion")]
    public int municionMaxima = 60;
    public int municionActual = 12;

    public UnityEvent OnStatsChanged = new UnityEvent();

    void Start()
    {
        vidaActual = Mathf.Clamp(vidaActual, 0, vidaMaxima);
        municionActual = Mathf.Clamp(municionActual, 0, municionMaxima);
        OnStatsChanged.Invoke();
    }

    public bool TieneMunicion()
    {
        return municionActual > 0;
    }

    public bool UsarMunicion(int cantidad)
    {
        if (municionActual < cantidad)
        {
            Debug.Log("Sin munición");
            return false;
        }

        municionActual -= cantidad;
        OnStatsChanged.Invoke();
        return true;
    }

    public void AgregarMunicion(int cantidad)
    {
        municionActual = Mathf.Min(municionActual + cantidad, municionMaxima);
        OnStatsChanged.Invoke();
    }

    public void Curar(int cantidad)
    {
        vidaActual = Mathf.Min(vidaActual + cantidad, vidaMaxima);
        OnStatsChanged.Invoke();
    }

    public void RecibirDaño(int cantidad)
    {
        vidaActual = Mathf.Max(vidaActual - cantidad, 0);
        OnStatsChanged.Invoke();

        if (vidaActual <= 0)
        {
            Debug.Log("Jugador muerto");
        }
    }
}