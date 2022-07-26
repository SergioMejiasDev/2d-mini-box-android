using UnityEngine;

/// <summary>
/// Clase que controla el movimiento de la pelota.
/// </summary>
public class Ball : MonoBehaviour
{
    /// <summary>
    /// Velocidad a la que se mueve la pelota.
    /// </summary>
    readonly float speed = 4;
    /// <summary>
    /// Componente Rigidbody2D de la pelota.
    /// </summary>
    [SerializeField] Rigidbody2D rb = null;

    void Start()
    {
        Launch();
    }

    /// <summary>
    /// Función que resetea la posición de la pelota y la devuelve al centro de la pantalla.
    /// </summary>
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        transform.position = Vector2.zero;

        Launch();
    }

    /// <summary>
    /// Función que hace que la pelota comienze a moverse.
    /// </summary>
    void Launch()
    {
        float x = Random.Range(0, 2) == 0 ? -1 : 1;
        float y = Random.Range(0, 2) == 0 ? -1 : 1;
        rb.velocity = new Vector2(speed * x, speed * y);
    }
}