using UnityEngine;

/// <summary>
/// Clase encargada de que aumente la puntuación cada vez que eliminamos a un enemigo.
/// </summary>
public class EnemyScore : MonoBehaviour
{
    /// <summary>
    /// Los puntos que obtendremos al eliminar al enemigo.
    /// </summary>
    [SerializeField] int score = 0;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si una bala del jugador alcanza al enemigo, se activa una animación donde este explota antes de desaparecer.
        // También aumentará la puntuación en función del tipo de enemigo.

        if (collision.gameObject.CompareTag("Game3/BulletPlayer"))
        {
            collision.gameObject.SetActive(false);
            GameManager3.manager3.EnemyDeath(score, transform.position);
            gameObject.SetActive(false);
        }
    }
}