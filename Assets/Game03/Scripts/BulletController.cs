using UnityEngine;

/// <summary>
/// Clase encargada del movimiento y desaparición de las balas.
/// </summary>
public class BulletController : MonoBehaviour
{
    /// <summary>
    /// Verdadero si es una bala del enemigo. Falso si es una bala del jugador.
    /// </summary>
    [SerializeField] bool isEnemy = false;
    /// <summary>
    /// Velocidad a la que se mueven las balas del jugador.
    /// </summary>
    readonly float speedPlayer = 9;
    /// <summary>
    /// Velocidad a la que se mueven las balas del enemigo.
    /// </summary>
    readonly float speedEnemy = 5;

    void Update()
    {
        // La velocidad de movimiento será diferente según quien dispare la bala.

        if (!isEnemy)
        {
            transform.Translate(Vector2.up * speedPlayer * Time.deltaTime);
        }

        else
        {
            transform.Translate(Vector2.down * speedEnemy * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si la bala sale de los límites de la pantalla, se destruye.

        if (collision.gameObject.CompareTag("Game3/Limits"))
        {
            gameObject.SetActive(false);
        }
    }
}