using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Clase que controla el movimiento de la serpiente.
/// </summary>
public class SnakeMovement : MonoBehaviour
{
    /// <summary>
    /// Vector de dirección hacia el que se está moviendo la serpiente.
    /// </summary>
    [Header("Movement")]
    Vector2 direction;
    /// <summary>
    /// Lista con todos los bloques que componen la cola de la serpiente.
    /// </summary>
    List<Transform> tail = new List<Transform>();
    /// <summary>
    /// Positivo si la serpiente se ha comido una pieza de comida en el último movimiento. Falso si no lo ha hecho.
    /// </summary>
    bool hasEaten = false;
    /// <summary>
    /// Verdadero si se puede realizar un nuevo movimiento, falso si no se puede.
    /// Será falso desde el momento del input hasta que la serpiente se mueve.
    /// </summary>
    bool canMove = true;
    /// <summary>
    /// Prefab de los bloques de la cola de la serpiente.
    /// </summary>
    [SerializeField] GameObject tailPrefab = null;

    /// <summary>
    /// Sonido que se reproduce cuando la serpiente se come una pieza de comida,
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource foodSound = null;
    /// <summary>
    /// Sonido que se reproduce cuando la serpiente se come una pieza de comida roja.
    /// </summary>
    [SerializeField] AudioSource redFoodSound = null;
    /// <summary>
    /// Sonido que se reproduce cuando la serpiente choca con su cola o con la pared.
    /// </summary>
    [SerializeField] AudioSource snakeHurtSound = null;

    void OnEnable()
    {
        tail.Clear();

        GameObject[] activeTail = GameObject.FindGameObjectsWithTag("Game5/Tail");

        if (activeTail != null)
        {
            for (int i = 0; i < activeTail.Length; i++)
            {
                Destroy(activeTail[i]);
            }
        }

        direction = Vector2.right;

        for (int i = 0; i < 5; i++)
        {
            Vector2 tailPosition = new Vector2(transform.position.x - (i + 1), transform.position.y);
            GameObject newTail = Instantiate(tailPrefab, tailPosition, Quaternion.identity);
            tail.Insert(i, newTail.transform);
        }

        // Con el InvokeRepeating se automatiza el movimiento de la serpiente.

        InvokeRepeating("Move", 0.3f, 0.15f);
    }

    void Update()
    {
        // Solo se podrá introducir un nuevo input cuando la serpiente se haya movido hasta el último input introducido.

        if ((canMove) && (Time.timeScale == 1))
        {
            ChangeDirection();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            GameManager5.manager5.PauseGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si la serpiente se come una pieza de comida, aumentará la puntuación y se reproducirá un sonido.

        if (collision.gameObject.CompareTag("Game5/Food"))
        {
            foodSound.Play();
            hasEaten = true;

            GameManager5.manager5.UpdateScore(10);
            GameManager5.manager5.Spawn();

            Destroy(collision.gameObject);
        }

        // Si la serpiente se come una pieza de comida roja, aumentará la puntuación y se reproducirá un sonido.

        else if (collision.gameObject.CompareTag("Game5/RedFood"))
        {
            redFoodSound.Play();
            hasEaten = true;

            GameManager5.manager5.UpdateScore(50);
            GameManager5.manager5.SpawnRed();

            Destroy(collision.gameObject);
        }

        // Si la serpiente choca con su cola o con la pared, termina la partida.

        else if (collision.gameObject.CompareTag("Game5/Tail") || collision.gameObject.CompareTag("Game5/Border"))
        {
            snakeHurtSound.Play();

            CancelInvoke();
            GameManager5.manager5.GameOver();
        }
    }

    /// <summary>
    /// Fnción que permite al jugador cambiar de dirección arrastrando el dedo por la pantalla.
    /// </summary>
    void ChangeDirection()
    {
        if (Input.touchCount > 0)
        {
            Touch firstDetectedTouch = Input.GetTouch(0);

            if (Input.GetTouch(0).phase == TouchPhase.Moved)
            {
                Vector2 dragDistance = firstDetectedTouch.deltaPosition.normalized;

                if (dragDistance.x > dragDistance.y)
                {
                    if ((dragDistance.x > 0.8f) && (direction != -Vector2.right))
                    {
                        direction = Vector2.right;
                        canMove = false;
                    }

                    else if ((dragDistance.y < -0.8f) && (direction != Vector2.up))
                    {
                        direction = -Vector2.up;
                        canMove = false;
                    }
                }

                else if (dragDistance.y > dragDistance.x)
                {
                    if ((dragDistance.y > 0.8f) && (direction != -Vector2.up))
                    {
                        direction = Vector2.up;
                        canMove = false;
                    }

                    else if ((dragDistance.x < -0.8f) && (direction != Vector2.right))
                    {
                        direction = -Vector2.right;
                        canMove = false;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Función que se activa cada vez que la serpiente se mueve.
    /// </summary>
    void Move()
    {
        canMove = true;

        Vector2 position = transform.position;

        transform.Translate(direction);

        if (hasEaten)
        {
            // Si la serpiente acaba de comerse una pieza de comida, aumentará de tamaño durante el movimiento.

            GameObject newTail = Instantiate(tailPrefab, position, Quaternion.identity);

            tail.Insert(0, newTail.transform);

            hasEaten = false;
        }

        else if (tail.Count > 0)
        {
            // El último bloque de la cola pasará a ser el primero.
            // Este es el fundamento del movimiento de la serpiente.

            tail.Last().position = position;

            tail.Insert(0, tail.Last());
            tail.RemoveAt(tail.Count - 1);
        }
    }
}