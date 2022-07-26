using UnityEngine;

/// <summary>
/// Clase que controla el movimiento de las palas.
/// </summary>
public class Paddle : MonoBehaviour
{
    /// <summary>
    /// Velocidad a la que se mueve la pala.
    /// </summary>
    [Header("Movement")]
    readonly float speed = 3.5f;
    /// <summary>
    /// Falso si la pala se mueve hacia arriba. Verdadero si se mueve hacia abajo.
    /// La variable dontMove debe ser falsa.
    /// </summary>
    bool moveUp = false;
    /// <summary>
    /// Si es verdadero, bloquea el movimiento del jugador.
    /// </summary>
    bool dontMove = true;

    /// <summary>
    /// Componente Rigidbody2D de la pala.
    /// </summary>
    [Header("Components")]
    [SerializeField] Rigidbody2D rb = null;
    /// <summary>
    /// Componente AudioSource de la pala.
    /// </summary>
    [SerializeField] AudioSource audioSource = null;

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager2.manager.PauseGame();
        }

        HandleMoving();
    }

    /// <summary>
    /// Función que gestiona el movimiento del jugador.
    /// </summary>
    void HandleMoving()
    {
        if (dontMove) // Si dontMove es verdadero, se detiene el movimiento de la pala.
        {
            StopMoving();
        }

        else // Si dontMove es falso, se permite el movimiento de la pala.
        {
            if (moveUp) // Si moveUp es verdadero, la pala se mueve hacia arriba.
            {
                MoveUp();
            }

            else if (!moveUp) // Si moveUp es falso, la pala se mueve hacia abajo.
            {
                MoveDown();
            }
        }
    }

    /// <summary>
    /// Función que permite el movimiento del jugador.
    /// Está vinculada a los inputs en la pantalla del dispositivo.
    /// </summary>
    /// <param name="upMovement">Verdadero si el movimiento es hacia arriba. Falso si el movimiento es hacia abajo.</param>
    public void AllowMovement(bool upMovement)
    {
        dontMove = false;
        moveUp = upMovement;
    }

    /// <summary>
    /// Función que detiene el movimiento del jugador.
    /// Se activa cuando se levanta el dedo de la pantalla y se deja de pulsar un botón de movimiento.
    /// </summary>
    public void DontAllowMovement()
    {
        dontMove = true;
    }

    /// <summary>
    /// Función que mueve al jugador hacia arriba.
    /// </summary>
    void MoveUp()
    {
        rb.velocity = new Vector2(rb.velocity.x, speed);
    }

    /// <summary>
    /// Función que mueve al jugador hacia abajo.
    /// </summary>
    void MoveDown()
    {
        rb.velocity = new Vector2(rb.velocity.x, -speed);
    }

    /// <summary>
    /// Función que detiene el movimiento del jugador, manteniéndolo en su posición.
    /// </summary>
    void StopMoving()
    {
        rb.velocity = new Vector2(rb.velocity.x, 0);
    }

    /// <summary>
    /// Función que resetea la posición de la pala y la devuelve al centro de su campo.
    /// </summary>
    public void ResetPosition()
    {
        rb.velocity = Vector2.zero;
        transform.position = new Vector2(-5.75f, 0);
        DontAllowMovement();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si la pala golpea la pelota, se reproducirá un sonido.

        if (collision.gameObject.CompareTag("Game2/Ball"))
        {
            audioSource.Play();
        }
    }
}