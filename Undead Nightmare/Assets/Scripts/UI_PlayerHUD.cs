using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UI_PlayerHUD : MonoBehaviour
{
    public PlayerStats playerStats;
    public TMP_Text textoMunicion;
    public Slider barraVida;

    void Start()
    {
        if (playerStats == null)
        {
            playerStats = FindObjectOfType<PlayerStats>();
        }

        if (playerStats != null)
        {
            playerStats.OnStatsChanged.AddListener(ActualizarHUD);
            ActualizarHUD();
        }
    }

    void OnDestroy()
    {
        if (playerStats != null)
        {
            playerStats.OnStatsChanged.RemoveListener(ActualizarHUD);
        }
    }

    void ActualizarHUD()
    {
        if (playerStats == null) return;

        if (textoMunicion != null)
        {
            textoMunicion.text = "Balas: " + playerStats.municionActual + "/" + playerStats.municionMaxima;
        }

        if (barraVida != null)
        {
            barraVida.maxValue = playerStats.vidaMaxima;
            barraVida.value = playerStats.vidaActual;
        }
    }
}
