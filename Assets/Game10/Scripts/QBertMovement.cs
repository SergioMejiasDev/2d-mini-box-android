using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que controla los movimientos del jugador.
/// </summary>
public class QBertMovement : MonoBehaviour
{
    /// <summary>
    /// El destino al que se está moviendo el jugador.
    /// </summary>
    [Header("Movement")]
    Vector2 destination = Vector2.zero;
    /// <summary>
    /// El siguiente destino al que se va a mover el jugador.
    /// </summary>
    Vector2 newDestination = Vector2.zero;
    /// <summary>
    /// La capa asignada a los bordes de la pirámide.
    /// </summary>
    [SerializeField] LayerMask borderMask = 8;
    /// <summary>
    /// Verdadero si están activos los inputs, falso si no lo están.
    /// </summary>
    bool inputsEnabled = false;
    /// <summary>
    /// Verdadero si el jugador está en movimiento, falso si está quieto.
    /// </summary>
    bool inMovement = false;
    /// <summary>
    /// Variable que indica si se han comprobado ciertas variables antes de moverse.
    /// </summary>
    bool hasChecked = true;
    /// <summary>
    /// Verdadero si el jugador está cayendo, falso si no lo está haciendo.
    /// </summary>
    bool falling = false;
    /// <summary>
    /// Verdadero si el jugador está muerto. Falso si no lo está.
    /// </summary>
    bool isDead = true;

    /// <summary>
    /// El bocadillo de diálogo que aparece cuando el jugador muere.
    /// </summary>
    [Header("Bubble")]
    [SerializeField] GameObject bubble = null;

    /// <summary>
    /// El componente Rigidbody2D del jugador.
    /// </summary>
    [Header("Components")]
    [SerializeField] Rigidbody2D rb = null;
    /// <summary>
    /// El componente Animator del jugador.
    /// </summary>
    [SerializeField] Animator anim = null;
    /// <summary>
    /// El componente SpriteRenderer del jugador.
    /// </summary>
    [SerializeField] SpriteRenderer sr = null;

    void OnEnable()
    {
        sr.sortingOrder = 1;
        rb.gravityScale = 0;
        ResetValues();

        GameManager10.manager.EnableBlocks(true);

        StartCoroutine(Falling());
    }

    private void OnDisable()
    {
        bubble.SetActive(false);
    }

    void Update()
    {
        if (Input.GetButtonDown("Cancel"))
        {
            GameManager10.manager.PauseGame();
        }

        if (isDead)
        {
            return;
        }

        if ((Vector2)transform.position == destination)
        {
            if (destination != newDestination)
            {
                destination = newDestination;
                return;
            }

            else
            {
                if (falling)
                {
                    Die();
                }
            }

            if (!hasChecked)
            {
                hasChecked = true;

                ResetValues();
            }

            else if (!inMovement)
            {
                if (!inputsEnabled)
                {
                    inputsEnabled = true;
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (isDead)
        {
            return;
        }

        Vector2 newPosition = Vector2.MoveTowards(transform.position, destination, 0.05f);
        rb.MovePosition(newPosition);

        Vector2 direction = destination - (Vector2)transform.position;
        anim.SetFloat("DirX", direction.x);
        anim.SetFloat("DirY", direction.y);
    }

    /// <summary>
    /// Función que se activa cuando se pulsan los inputs en la pantalla.
    /// </summary>
    /// <param name="input">1 arriba, 2 derecha, 3 abajo, 4 izquierda.</param>
    public void EnterInputs(int input)
    {
        if (!inputsEnabled || Time.timeScale != 1)
        {
            return;
        }

        if (input == 1)
        {
            destination = new Vector2(transform.position.x + 0.05f, transform.position.y + 0.5f);
            newDestination = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.75f);

            inMovement = true;
            hasChecked = false;
            inputsEnabled = false;

            if (InBorder(new Vector2(0.5f, 0.75f)))
            {
                falling = true;
            }
        }

        if (input == 2)
        {
            destination = new Vector2(transform.position.x + 0.45f, transform.position.y - 0.25f);
            newDestination = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.75f);

            inMovement = true;
            hasChecked = false;
            inputsEnabled = false;

            if (InBorder(new Vector2(0.5f, -0.75f)))
            {
                falling = true;
            }
        }

        if (input == 3)
        {
            destination = new Vector2(transform.position.x - 0.45f, transform.position.y - 0.25f);
            newDestination = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.75f);

            inMovement = true;
            hasChecked = false;
            inputsEnabled = false;

            if (InBorder(new Vector2(-0.5f, -0.75f)))
            {
                falling = true;
            }
        }

        if (input == 4)
        {
            destination = new Vector2(transform.position.x - 0.05f, transform.position.y + 0.5f);
            newDestination = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.75f);

            inMovement = true;
            hasChecked = false;
            inputsEnabled = false;

            if (InBorder(new Vector2(-0.5f, 0.75f)))
            {
                falling = true;
            }
        }
    }


    /// <summary>
    /// Función que vuelve al jugador temporalmente invulnerable.
    /// </summary>
    public void Invulnerable()
    {
        isDead = true;
    }

    /// <summary>
    /// Función que resetea la posición del jugador.
    /// </summary>
    public void ResetPosition()
    {
        sr.sortingOrder = 1;
        rb.gravityScale = 0;
        ResetValues();

        StartCoroutine(Falling());
    }

    /// <summary>
    /// Función que se activa cuando el jugador golpea a un enemigo.
    /// </summary>
    void Hit()
    {
        GameManager10.manager.EnableBlocks(false);

        isDead = true;

        bubble.SetActive(true);

        GameManager10.manager.DeathHit();
    }

    /// <summary>
    /// Función que se activa cuando el jugador cae de la pirámide.
    /// </summary>
    void Die()
    {
        GameManager10.manager.EnableBlocks(false);

        isDead = true;

        rb.gravityScale = 0.75f;

        if (transform.position.y > -2.9f)
        {
            sr.sortingOrder = -1;
        }

        GameManager10.manager.DeathFall();
    }

    /// <summary>
    /// Función que resetea las variables de movimiento del jugador.
    /// </summary>
    void ResetValues()
    {
        inMovement = false;
        falling = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador golpea a una bola roja o morada, muere.

        if (collision.gameObject.CompareTag("Game10/RedBall") || collision.gameObject.CompareTag("Game10/PurpleBall"))
        {
            if (!isDead)
            {
                Hit();
            }
        }

        // Si el jugador golpea a una bola verde, desaparece y aumenta la puntuación.

        else if (collision.gameObject.CompareTag("Game10/GreenBall"))
        {
            GameManager10.manager.UpdateScore(75);
            collision.gameObject.SetActive(false);
        }

        // Si el jugador se sube a un disco, se vuelve temporalmente invulnerable.

        else if (collision.gameObject.CompareTag("Game10/Disc"))
        {
            Invulnerable();
        }

        // Si el jugador se sale de los bordes de la pantalla, desaparece.

        else if (collision.gameObject.CompareTag("Game10/Destructor"))
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Corrutina que activa la caída inicial del jugador.
    /// </summary>
    /// <returns></returns>
    IEnumerator Falling()
    {
        while (transform.position.y > 1.5f)
        {
            transform.Translate(Vector2.down * 5f * Time.deltaTime);

            yield return null;
        }

        transform.position = new Vector2(transform.position.x, 1.5f);
        destination = transform.position;
        newDestination = transform.position;

        isDead = false;
    }

    /// <summary>
    /// Función que comprueba si el siguiente movimiento del jugador lo hace caer de la pirámide.
    /// </summary>
    /// <param name="direction">Dirección que se va a comprobar.</param>
    /// <returns>Verdadero si cae de la pirámide, falso si no lo hace.</returns>
    bool InBorder(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, borderMask);

        return hit;
    }
}