using System.Collections;
using UnityEngine;

/// <summary>
/// Clase con las principales funciones del jugador.
/// </summary>
public class PacmanMovement : MonoBehaviour
{
    public delegate void PacmanDelegate();
    public static event PacmanDelegate EatBigDot;
    public static event PacmanDelegate PlayerDie;

    /// <summary>
    /// Velocidad de movimiento del jugador.
    /// </summary>
    [Header("Movement")]
    readonly float speed = 0.1f;
    /// <summary>
    /// Vector de posición hacia el que se dirige el jugador.
    /// </summary>
    Vector2 destination = Vector2.zero;
    /// <summary>
    /// Vector de dirección a la que se está moviendo el jugador.
    /// </summary>
    Vector2 currentDirection = Vector2.right;
    /// <summary>
    /// Vector de dirección a la que se va a dirigir el jugador en cuanto sea posible.
    /// Se elige a través de los inputs.
    /// </summary>
    Vector2 selectedDirection = Vector2.right;
    /// <summary>
    /// Será verdadero si el jugador está muerto. Falso si no lo está.
    /// </summary>
    bool isDie = false;

    /// <summary>
    /// Componente CircleCollider2D del jugador.
    /// </summary>
    [Header("Components")]
    [SerializeField] CircleCollider2D col;
    /// <summary>
    /// Componente RigidBody2D del jugador.
    /// </summary>
    [SerializeField] Rigidbody2D rb;
    /// <summary>
    /// Componente Animator del jugador.
    /// </summary>
    [SerializeField] Animator anim;

    /// <summary>
    /// Componente AudioSource con el sonido que se activa al comerse un punto grande.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource bigDotSound = null;
    /// <summary>
    /// Componente AudioSource con el sonido que se activa al morir el jugador.
    /// </summary>
    [SerializeField] AudioSource dieSound = null;

    void Start()
    {
        destination = transform.position;
    }

    private void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager4.manager4.PauseGame();
        }

        if (!isDie && Time.timeScale != 0)
        {
            ChangeDirection();
        }
    }

    void FixedUpdate()
    {
        if (!isDie) // El jugador solo podrá moverse si no está muerto.
        {
            if ((Vector2)transform.position == destination)
            {
                // Si el jugador puede moverse en la dirección elegida, cambiará de dirección.

                if (!RayMovement(1.1f * selectedDirection))
                {
                    currentDirection = selectedDirection;
                }

                // Mientras la dirección elegida no esté disponible, se seguirá moviendo en la dirección actual.

                if (!RayMovement(1.01f * currentDirection))
                {
                    ChangeDestination();
                }
            }

            Vector2 newPosition = Vector2.MoveTowards(transform.position, destination, speed);
            rb.MovePosition(newPosition);

            Vector2 direction = destination - (Vector2)transform.position;
            anim.SetFloat("DirX", direction.x);
            anim.SetFloat("DirY", direction.y);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador coge un punto, aumentará la puntuación.

        if (collision.gameObject.CompareTag("Game4/Dot"))
        {
            collision.gameObject.SetActive(false);
            GameManager4.manager4.DotEaten();
        }

        // Si el jugador coge un punto grande, los fantasmas se volverán vulnerables por unos segundos.

        else if (collision.gameObject.CompareTag("Game4/BigDot"))
        {
            collision.gameObject.SetActive(false);
            bigDotSound.Play();
            GameManager4.manager4.UpdateScore(5);
            GameManager4.manager4.enemiesInScreen = 4;

            if (EatBigDot != null)
            {
                EatBigDot();
            }
        }
    }

    /// <summary>
    /// Comprueba si el jugador puede moverse en la dirección indicada.
    /// </summary>
    /// <param name="direction">Dirección que se quiere comprobar.</param>
    /// <returns>Verdadero si el jugador puede moverse en la dirección indicada. Falso ni no puede moverse en dicha dirección.</returns>
    bool RayMovement(Vector2 direction)
    {
        Vector2 position = transform.position;
        RaycastHit2D hit = Physics2D.Linecast(position + direction, position);

        return (hit.collider.gameObject.CompareTag("Game4/Maze"));
    }

    /// <summary>
    /// Función que asigna una nueva dirección al jugador cuando ha alcanzado su destino anterior.
    /// </summary>
    void ChangeDestination()
    {
        if (currentDirection == Vector2.up)
        {
            destination = (Vector2)transform.position + Vector2.up;
        }

        else if (currentDirection == Vector2.right)
        {
            destination = (Vector2)transform.position + Vector2.right;
        }

        else if (currentDirection == Vector2.down)
        {
            destination = (Vector2)transform.position - Vector2.up;
        }

        else if (currentDirection == Vector2.left)
        {
            destination = (Vector2)transform.position - Vector2.right;
        }
    }

    /// <summary>
    /// Función a la que se llama a través de los inputs cuando queremos cambiar de dirección.
    /// </summary>
    void ChangeDirection()
    {
        if (Input.touchCount > 0) // Si se toca la pantalla.
        {
            Touch firstDetectedTouch = Input.GetTouch(0);

            if (Input.GetTouch(0).phase == TouchPhase.Moved) // Si el dedo se arrastra.
            {
                Vector2 dragDistance = firstDetectedTouch.deltaPosition.normalized;

                if (dragDistance.x > dragDistance.y) 
                {
                    if (dragDistance.x > 0.8f) // El dedo se arrastra hacia la derecha.
                    {
                        selectedDirection = Vector2.right;
                    }

                    else if (dragDistance.y < -0.8f) // El dedo se arrastra hacia abajo.
                    {
                        selectedDirection = Vector2.down;
                    }
                }

                else if (dragDistance.y > dragDistance.x)
                {
                    if (dragDistance.y > 0.8f) // El dedo se arrastra hacia arriba.
                    {
                        selectedDirection = Vector2.up;
                    }

                    else if (dragDistance.x < -0.8f) // El dedo se arrastra hacia la izquierda.
                    {
                        selectedDirection = Vector2.left;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Función que se activa cuando el jugador muere.
    /// </summary>
    public void PlayerDeath()
    {
        if (PlayerDie != null)
        {
            PlayerDie();
        }

        StartCoroutine(PlayerDying());
    }

    /// <summary>
    /// Función que devuelve al jugador a su posición inicial, restaurando las variables de movimiento.
    /// </summary>
    public void ResetPosition()
    {
        isDie = false;
        col.enabled = true;
        destination = transform.position;
        currentDirection = Vector2.right;
        selectedDirection = Vector2.right;
    }

    /// <summary>
    /// Corrutina que se inicia cuando el jugador muere, activando varias funciones.
    /// </summary>
    /// <returns></returns>
    IEnumerator PlayerDying()
    {
        isDie = true;
        col.enabled = false;

        yield return new WaitForSeconds(0.5f);

        dieSound.Play();

        anim.SetFloat("DirX", 0);
        anim.SetFloat("DirY", 0);
        anim.SetBool("PlayerDie", true);

        yield return new WaitForSeconds(1.5f);

        GameManager4.manager4.PlayerDeath();

        gameObject.SetActive(false);
    }
}