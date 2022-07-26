using UnityEngine;

/// <summary>
/// Clase que controla el movimiento de las tortugas.
/// </summary>
public class EnemyTurtle : MonoBehaviour
{
    /// <summary>
    /// Dirección de la tortuga (-1 izquierda, 1 derecha).
    /// </summary>
    int direction = 1;

    private void OnEnable()
    {
        if (transform.position.x < 0) // El misil se genera a la izquierda de la pantalla, con dirección a la derecha.
        {
            direction = 1;
        }
        else // El misil se genera a la derecha de la pantalla, con dirección a la izquierda.
        {
            direction = -1;
        }

        FlipEnemy();
    }

    private void Update()
    {
        transform.Translate(Vector2.right * 3 * direction * Time.deltaTime);
    }

    /// <summary>
    /// Función que orienta a las tortugas de forma acorde a su dirección de movimiento.
    /// </summary>
    void FlipEnemy()
    {
        if (direction == 1) // La tortuga se mueve hacia la derecha.
        {
            transform.localScale = new Vector2(-0.85f, 0.85f);
        }

        else // La tortuga se mueve hacia la izquierda.
        {
            transform.localScale = new Vector2(0.85f, 0.85f);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if ((other.gameObject.CompareTag("Game1/Walls"))) // Al chocar con una pared, las tortugas se giran.
        {
            direction *= -1;

            FlipEnemy();
        }

        else if (other.gameObject.CompareTag("Game1/Pipe")) // Al chocar con una de las tuberías inferiores, la tortugas desaparecen.
        {
            gameObject.SetActive(false);
        }
    }
}