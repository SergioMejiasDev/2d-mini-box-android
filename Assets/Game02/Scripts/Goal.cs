using UnityEngine;

/// <summary>
/// Clase encargada de actualizar la puntuación cuando la pelota alcanza alguna portería.
/// </summary>
public class Goal : MonoBehaviour
{
    /// <summary>
    /// Verdadero si es la portería del jugador 1. Falso si es la portería del jugador 2.
    /// </summary>
    [SerializeField] bool isPlayer1Goal = false;
    /// <summary>
    /// Componente AudioSource de la portería.
    /// </summary>
    [SerializeField] AudioSource audioSource = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si la pelota golpea la portería, se devuelve al centro del campo y se aumenta la puntuación correspondiente.
        // Además, se reproducirá un sonido que indica que se ha puntuado.

        if (collision.gameObject.CompareTag("Game2/Ball"))
        {
            if (isPlayer1Goal) // Marca el jugador 2.
            {
                GameManager2.manager.UpdateScore(2);
            }

            else // Marca el jugador 1.
            {
                GameManager2.manager.UpdateScore(1);
            }

            audioSource.Play();
        }
    }
}