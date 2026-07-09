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

    public void RecibirDaño(int cantidad)
    {
        vidaActual -= cantidad;
        Debug.Log("Infectado recibió daño. Vida actual: " + vidaActual);

        if (vidaActual <= 0)
        {
            Destroy(gameObject);
        }
    }
}