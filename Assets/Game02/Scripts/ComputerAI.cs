using UnityEngine;

/// <summary>
/// Clase que controla la IA enemiga.
/// </summary>
public class ComputerAI : MonoBehaviour
{
    /// <summary>
    /// Velocidad a la que se mueve la IA.
    /// </summary>
    [Header("Movement")]
    readonly float speed = 3.5f;
    /// <summary>
    /// Posición de la pelota (1 parte superior de la pantalla, -1 parte inferior).
    /// </summary>
    int ballPosition;

    /// <summary>
    /// Componente Rigidbody2D de la IA.
    /// </summary>
    [Header("Components")]
    [SerializeField] Rigidbody2D rb = null;
    /// <summary>
    /// Componente AudioSource de la IA.
    /// </summary>
    [SerializeField] AudioSource audioSource = null;
    /// <summary>
    /// La pelota que se usará en la partida.
    /// </summary>
    [SerializeField] GameObject ball = null;

    void Update()
    {
        // Se comprueba de forma constante la posición de la pelota para saber en que dirección moverse.

        if (ball.transform.position.y >= transform.position.y)
        {
            ballPosition = 1;
        }

        else
        {
            ballPosition = -1;
        }

        Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si se golpea la pelota, se reproduce un sonido.

        if (collision.gameObject.CompareTag("Game2/Ball"))
        {
            audioSource.Play();
        }
    }

    /// <summary>
    /// Función que resetea la posición de la pala.
    /// </summary>
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        transform.position = new Vector2(5.75f, 0);
    }

    /// <summary>
    /// Función que controla el movimiento de la pala.
    /// </summary>
    void Move()
    {
        rb.velocity = new Vector2(rb.velocity.x, ballPosition * speed);
    }
}