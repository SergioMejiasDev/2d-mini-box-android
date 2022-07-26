using UnityEngine;

/// <summary>
/// Clase que controla las funciones de las bases.
/// </summary>
public class BaseHealth : MonoBehaviour
{
    /// <summary>
    /// La salud de la base.
    /// </summary>
    int health = 10;
    /// <summary>
    /// Sprites con las diferentes formas que puede adoptar la base a medida que se va destruyendo.
    /// </summary>
    [SerializeField] Sprite[] pieces = new Sprite[4];
    /// <summary>
    /// Componente SpriteRenderer de la base.
    /// </summary>
    [SerializeField] SpriteRenderer sr;

    /// <summary>
    /// Función que devuelve la base a su estado inicial tras reiniciar la partida.
    /// </summary>
    public void Restart()
    {
        sr.sprite = pieces[0];
        health = 10;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si un disparo, ya sea del jugador o de los enemigos, alcanza la base, la salud de esta bajará.
        // A medida que vaya bajando la salud de la base, irá cambiando el sprite para ir mostrando los daños.
        // Cuando la vida llegue a 0, la base desaparecerá.

        if ((collision.gameObject.CompareTag("Game3/BulletPlayer")) || (collision.gameObject.CompareTag("Game3/BulletEnemy")))
        {
            health--;
            collision.gameObject.SetActive(false);

            if (health == 8)
            {
                sr.sprite = pieces[1];
            }

            else if (health == 5)
            {
                sr.sprite = pieces[2];
            }

            else if (health == 2)
            {
                sr.sprite = pieces[3];
            }

            else if (health == 0)
            {
                gameObject.SetActive(false);
            }
        }
    }
}