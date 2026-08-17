using UnityEngine;

public class GameCompleteUI : MonoBehaviour
{
    public GameObject panelJuegoCompletado;
    public GameObject textoMunicion;
    public GameObject barraVida;
    public PlayerMoveset jugador;

    void Start()
    {
        if (panelJuegoCompletado != null)
        {
            panelJuegoCompletado.SetActive(false);
        }
    }

    public void MostrarJuegoCompletado()
    {
        if (panelJuegoCompletado != null)
        {
            panelJuegoCompletado.SetActive(true);
        }

        if (textoMunicion != null)
        {
            textoMunicion.SetActive(false);
        }

        if (barraVida != null)
        {
            barraVida.SetActive(false);
        }

        if (jugador != null)
        {
            jugador.enabled = false;
        }

        Debug.Log(
            "Juego completado"
        );
    }
}