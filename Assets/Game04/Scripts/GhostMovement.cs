using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que contiene las funciones principales de los fantasmas.
/// </summary>
public class GhostMovement : MonoBehaviour
{
    /// <summary>
    /// La posición inicial del fantasma.
    /// </summary>
    [Header("Movement")]
    Vector3 startPosition;
    /// <summary>
    /// Los diferentes puntos sobre los que se desplazará el fantasma siguiendo una ruta.
    /// </summary>
    [SerializeField] Transform[] waypoints = null;
    /// <summary>
    /// El punto actual en el que se encuentra el fantasma dentro de la ruta.
    /// </summary>
    int currentWaypoint = 0;
    /// <summary>
    /// Velocidad de movimiento del fantasma.
    /// </summary>
    public float speed = 0.1f;
    /// <summary>
    /// Componente Rigidbody2D del fantasma.
    /// </summary>
    [SerializeField] Rigidbody2D rb;

    /// <summary>
    /// Componente Animator del fantasma.
    /// </summary>
    [Header("Animation")]
    [SerializeField] Animator anim;
    /// <summary>
    /// Será verdadero cuando el fantasma esté en modo azul (modo indefenso). Si el fantasma está en estado normal, será falso.
    /// </summary>
    bool blueMode = false;

    /// <summary>
    /// Será verdadero cuando el fantasma acabe de ser comido. Volverá a ser falso cuando el fantasma resucite.
    /// </summary>
    [Header("Death")]
    bool isDead = false;

    private void OnEnable()
    {
        PacmanMovement.EatBigDot += ChangeState;
        PacmanMovement.PlayerDie += PlayerDeath;
        GameManager4.ResetPositions += ResetPosition;
        GameManager4.PlayerWin += PlayerDeath;
    }

    private void OnDisable()
    {
        PacmanMovement.EatBigDot -= ChangeState;
        PacmanMovement.PlayerDie -= PlayerDeath;
        GameManager4.ResetPositions -= ResetPosition;
        GameManager4.PlayerWin -= PlayerDeath;
    }

    void Start()
    {
        startPosition = transform.position;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void FixedUpdate()
    {
        if (!isDead) // El fantasma solo seguirá su ruta habitual si no está muerto.
        {
            if (transform.position != waypoints[currentWaypoint].position)
            {
                Vector2 newPosition = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].position, speed);
                rb.MovePosition(newPosition);
            }

            else currentWaypoint = (currentWaypoint + 1) % waypoints.Length;
        }

        Animation();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Si el fantasma toca al jugador y no está en modo azul, el jugador perderá una vida.

            if (!blueMode && !isDead)
            {
                collision.gameObject.GetComponent<PacmanMovement>().PlayerDeath();
            }

            // Si el fantasma toca al jugador y está en modo azul, el jugador se lo comerá, volviendo el fantasma a su posición inicial.

            else if (blueMode && !isDead)
            {
                GameManager4.manager4.EnemyEaten(transform.position);
                StopAllCoroutines();
                blueMode = false;
                isDead = true;
                anim.SetBool("BlueMode", false);
                anim.SetBool("LastSeconds", false);
                StartCoroutine(Death());
            }
        }
    }

    /// <summary>
    /// Función encargada de las animaciones del fantasma.
    /// </summary>
    void Animation()
    {
        Vector2 direction;

        if (currentWaypoint >= 0)
        {
            direction = waypoints[currentWaypoint].position - transform.position;
        }

        else
        {
            direction = startPosition;
        }

        if (!blueMode && !isDead)
        {
            anim.SetFloat("DirX", direction.x);
            anim.SetFloat("DirY", direction.y);
        }

        else if (isDead)
        {
            if (currentWaypoint >= 0)
            {
                anim.SetFloat("DeadX", direction.x);
                anim.SetFloat("DeadY", direction.y);
            }
            
            else
            {
                anim.SetFloat("DeadX", 0);
                anim.SetFloat("DeadY", -1);
            }
        }

        else if (blueMode)
        {
            anim.SetFloat("DirX", 0);
            anim.SetFloat("DirY", 0);
        }
    }

    /// <summary>
    /// Función que convierte a los fantasmas en azules cuando el jugador se come una bola.
    /// </summary>
    void ChangeState()
    {
        if (!isDead)
        {
            StopAllCoroutines();
            StartCoroutine(ChangeMode());
        }
    }

    /// <summary>
    /// Función que se activa a través del delegado cuando el jugador muere.
    /// </summary>
    void PlayerDeath()
    {
        speed = 0;
    }

    /// <summary>
    /// Función que resetea la posición y demás variables del fantasma.
    /// </summary>
    void ResetPosition()
    {
        transform.position = startPosition;
        currentWaypoint = 0;
        speed = 0.1f;
        blueMode = false;
        isDead = false;
        anim.SetFloat("DeadX", 0);
        anim.SetFloat("DeadY", 0);
        anim.SetBool("BlueMode", false);
        anim.SetBool("LastSeconds", false);
    }

    /// <summary>
    /// Corrutina que se activa cuando los fantasmas se vuelven azules.
    /// Activa una cuenta atrás para que estos vuelvan a su estado original.
    /// </summary>
    /// <returns></returns>
    IEnumerator ChangeMode()
    {
        blueMode = true;
        anim.SetBool("LastSeconds", false);
        anim.SetBool("BlueMode", true);
        speed = 0.05f;
        yield return new WaitForSeconds(8);
        anim.SetBool("BlueMode", false);
        anim.SetBool("LastSeconds", true);
        yield return new WaitForSeconds(3);
        anim.SetBool("LastSeconds", false);
        blueMode = false;
        speed = 0.1f;
    }

    /// <summary>
    /// Corrutina que se inicia cuando el fantasma es comido por el jugador.
    /// </summary>
    /// <returns></returns>
    IEnumerator Death()
    {
        while (currentWaypoint >= 0)
        {
            if (transform.position != waypoints[currentWaypoint].position)
            {
                Vector2 newPosition = Vector2.MoveTowards(transform.position, waypoints[currentWaypoint].position, 0.5f);
                rb.MovePosition(newPosition);
            }

            else
            {
                currentWaypoint = (currentWaypoint - 1) % waypoints.Length;
            }

            yield return null;
        }

        while (transform.position != startPosition)
        {
            Vector2 newPosition = Vector2.MoveTowards(transform.position, startPosition, speed);
            rb.MovePosition(newPosition);

            yield return null;
        }

        currentWaypoint = 0;
        anim.SetFloat("DeadX", 0);
        anim.SetFloat("DeadY", 0);
        isDead = false;
        speed = 0.1f;
    }
}