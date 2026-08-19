using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public GameObject panelOpciones;

    public Slider sliderVolumen;
    public Slider sliderBrillo;

    public Image imagenBrillo;

    void Start()
    {
        panelOpciones.SetActive(false);

        float volumenGuardado = PlayerPrefs.GetFloat("Volumen", 1f);
        float brilloGuardado = PlayerPrefs.GetFloat("Brillo", 1f);

        sliderVolumen.value = volumenGuardado;
        sliderBrillo.value = brilloGuardado;

        CambiarVolumen(volumenGuardado);
        CambiarBrillo(brilloGuardado);
    }

    public void AbrirOpciones()
    {
        panelOpciones.SetActive(true);
    }

    public void CerrarOpciones()
    {
        panelOpciones.SetActive(false);
    }

    public void CambiarVolumen(float volumen)
    {
        AudioListener.volume = volumen;

        PlayerPrefs.SetFloat("Volumen", volumen);
        PlayerPrefs.Save();
    }

    public void CambiarBrillo(float brillo)
    {
        Color color = imagenBrillo.color;

        color.a = 1f - brillo;

        imagenBrillo.color = color;

        PlayerPrefs.SetFloat("Brillo", brillo);
        PlayerPrefs.Save();
    }
}