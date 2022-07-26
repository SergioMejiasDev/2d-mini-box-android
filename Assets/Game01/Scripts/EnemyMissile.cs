using UnityEngine;

/// <summary>
/// Clase que controla el movimiento de los misiles.
/// </summary>
public class EnemyMissile : MonoBehaviour
{
    /// <summary>
    /// Dirección del misil (-1 izquierda, 1 derecha).
    /// </summary>
    int direction;

    private void OnEnable()
    {
        if (transform.position.x < 0) // El misil se genera a la izquierda de la pantalla, con dirección a la derecha.
        {
            direction = 1;
            transform.localScale = new Vector2(-0.85f, 0.85f);
        }
        else // El misil se genera a la derecha de la pantalla, con dirección a la izquierda.
        {
            direction = -1;
            transform.localScale = new Vector2(0.85f, 0.85f);
        }
    }

    void Update()
    {
        transform.Translate(Vector2.right * 4 * direction * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Game1/Ground"))
        {
            gameObject.SetActive(false);
        }
    }
}