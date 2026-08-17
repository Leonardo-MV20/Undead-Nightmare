using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform jugador;

    public float offsetX = 0f;
    public float offsetY = 1f;
    public float offsetZ = -10f;

    void LateUpdate()
    {
        if (jugador == null)
        {
            return;
        }

        transform.position = new Vector3(
            jugador.position.x + offsetX,
            jugador.position.y + offsetY,
            offsetZ
        );
    }
}