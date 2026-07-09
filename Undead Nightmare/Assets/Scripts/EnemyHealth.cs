using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int vidaMaxima = 100;
    public int vidaActual;

    void Start()
    {
        vidaActual = vidaMaxima;
    }

    public void RecibirDanno(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("Infectado recibio danno. Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
    }
}