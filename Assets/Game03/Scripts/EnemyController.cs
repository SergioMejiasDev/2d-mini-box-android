using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que controla el movimiento y disparo de la oleada de enemigos.
/// </summary>
public class EnemyController : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento de la oleada.
    /// </summary>
    float speed = 0.25f;
    /// <summary>
    /// Tiempo de espera entre movimientos de la oleada. Hace referencia a un momento concreto de la partida.
    /// </summary>
    float waitTime;
    /// <summary>
    /// Tiempo mínimo de espera entre movimientos de la oleada.
    /// </summary>
    readonly float minWait = 0.05f;
    /// <summary>
    /// Tiempo máximo de espera entre movimientos de la oleada.
    /// </summary>
    readonly float maxWait = 1.00f;
    /// <summary>
    /// Los enemigos que forman parte de la oleada.
    /// </summary>
    GameObject[] enemies;
    /// <summary>
    /// Ratio de disparo de los enemigos.
    /// </summary>
    readonly float fireRate = 0.99f;
    /// <summary>
    /// Si es verdadero, los enemigos no podrán moverse.
    /// </summary>
    public bool stopMoving;
    /// <summary>
    /// Componente AudioSource de la oleada de enemigos.
    /// </summary>
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        enemies = GameObject.FindGameObjectsWithTag("Game3/Enemy");
    }

    /// <summary>
    /// Función que inicia el movimiento de los enemigos.
    /// </summary>
    /// <param name="restart">Será verdadero si queremos que la oleada de enemigos vuelva a la posición inicial.</param>
    public void StartPlay(bool restart)
    {
        if (restart)
        {
            transform.position = new Vector2(0, 0.4f);
            waitTime = maxWait;
        }

        stopMoving = false;
        StartCoroutine(MoveEnemies());
    }

    /// <summary>
    /// Función que hace que los enemigos desciendan un nivel cuando alcanzan el borde de la pantalla.
    /// Estos además cambiarán de dirección.
    /// </summary>
    void MoveDown()
    {
        transform.Translate(Vector2.down * 0.4f);
        speed *= -1;
        StartCoroutine(MoveEnemies());
    }

    /// <summary>
    /// Función que hace que disminuya el tiempo de espera entre movimientos.
    /// Se activa cada vez que eliminamos a un enemigo.
    /// </summary>
    public void DecreaseWaitTime()
    {
        waitTime -= ((maxWait - minWait) / 55f);
    }

    /// <summary>
    /// Corrutina encargada del movimiento y los disparos de los enemigos.
    /// </summary>
    /// <returns></returns>
    IEnumerator MoveEnemies()
    {
        // Habrá un tiempo de espera entre movimientos, que será menor a medida que haya menos enemigos en pantalla.

        yield return new WaitForSeconds(waitTime);

        while (true)
        {
            if (stopMoving)
            {
                yield break;
            }

            transform.Translate(Vector2.right * speed);
            audioSource.Play();

            for (int i = 0; i < enemies.Length; i++)
            {
                if ((enemies[i].activeSelf == true) && (Random.value > fireRate))
                {
                    GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Game3/BulletEnemy");

                    if (bullet != null)
                    {
                        bullet.SetActive(true);
                        bullet.transform.position = enemies[i].transform.position;
                        bullet.transform.rotation = Quaternion.identity;
                    }
                }

                if ((enemies[i].transform.position.y < -2) && (enemies[i].activeSelf == true))
                {
                    GameManager3.manager3.LoseHealth(4);
                    yield break;
                }
            }

            for (int i = 0; i < enemies.Length; i++)
            {
                if (((enemies[i].transform.position.x < -7) && (speed < 0)) || ((enemies[i].transform.position.x > 7) && (speed > 0)))
                {
                    if (enemies[i].activeSelf == true)
                    {
                        yield return new WaitForSeconds(waitTime);
                        MoveDown();
                        yield break;
                    }
                }
            }
            yield return new WaitForSeconds(waitTime);
        }
    }
}