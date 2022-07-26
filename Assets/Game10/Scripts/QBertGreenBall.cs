using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que controla las funciones de las bolas verdes.
/// </summary>
public class QBertGreenBall : MonoBehaviour
{
    /// <summary>
    /// La dirección a la que se está moviendo la bola verde.
    /// </summary>
    [Header("Movement")]
    Vector2 destination = Vector2.zero;
    /// <summary>
    /// La siguiente dirección a la que se va a mover la bola verde.
    /// </summary>
    Vector2 newDestination = Vector2.zero;
    /// <summary>
    /// La capa asignada al borde de la pirámide.
    /// </summary>
    [SerializeField] LayerMask borderMask = 8;
    /// <summary>
    /// Verdadero si la bola está cayendo. Falso si no lo está haciendo.
    /// </summary>
    bool falling = false;
    /// <summary>
    /// Verdadero si la bola está activa. Falso si no lo está.
    /// </summary>
    bool isActive = false;
    /// <summary>
    /// El tiempo de espera entre saltos de la bola.
    /// </summary>
    float waitTime = 0.5f;
    /// <summary>
    /// Contador que indica cuánto ha pasado desde el último salto.
    /// </summary>
    float timer;

    /// <summary>
    /// Componente Rigidbody2D de la bola.
    /// </summary>
    [Header("Components")]
    [SerializeField] Rigidbody2D rb = null;
    /// <summary>
    /// Componente Animator de la bola.
    /// </summary>
    [SerializeField] Animator anim = null;
    /// <summary>
    /// Componente SpriteRenderer de la bola.
    /// </summary>
    [SerializeField] SpriteRenderer sr = null;

    void OnEnable()
    {
        GameManager10.StopMovement += StopMovement;

        timer = 0;
        isActive = false;
        falling = false;
        rb.gravityScale = 0;
        sr.sortingOrder = 1;

        StartCoroutine(Falling());
    }

    private void OnDisable()
    {
        GameManager10.StopMovement -= StopMovement;
    }

    void Update()
    {
        if (!isActive)
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
                    Fall();
                    return;
                }
            }

            if (timer > waitTime)
            {
                timer = 0;

                destination = NewDestination();
            }
        }

        timer = timer + Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (!isActive)
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
    /// Función que hace que la bola se vea afectada por la gravedad y caiga.
    /// </summary>
    void Fall()
    {
        rb.gravityScale = 0.75f;

        isActive = false;
    }

    /// <summary>
    /// Función que se activa a través del delegado y detiene el movimiento de la bola.
    /// </summary>
    void StopMovement()
    {
        isActive = false;
        StopAllCoroutines();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si la bola se sale del escenario, se destruye.

        if (collision.gameObject.CompareTag("Game10/Destructor"))
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Corrutina que activa la caida inicial de la bola.
    /// </summary>
    /// <returns></returns>
    IEnumerator Falling()
    {
        while (transform.position.y > 0.55f)
        {
            transform.Translate(Vector2.down * 5f * Time.deltaTime);

            yield return null;
        }

        transform.position = new Vector2(transform.position.x, 0.55f);
        destination = transform.position;
        newDestination = transform.position;

        isActive = true;
    }

    /// <summary>
    /// Función que decide la siguiente posición a la que se moverá la bola.
    /// </summary>
    /// <returns>El vector con una de las dos posibles posiciones.</returns>
    Vector2 NewDestination()
    {
        if (Random.value < 0.5f)
        {
            if (InBorder(new Vector2(0.5f, -0.75f)))
            {
                falling = true;
            }

            newDestination = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.75f);
            return new Vector2(transform.position.x + 0.43f, transform.position.y - 0.25f);
        }

        else
        {
            if (InBorder(new Vector2(-0.5f, -0.75f)))
            {
                falling = true;
            }

            newDestination = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.75f);
            return new Vector2(transform.position.x - 0.43f, transform.position.y - 0.25f);
        }
    }

    /// <summary>
    /// Función que comprueba si el siguiente movimiento hará que la bola se caiga de la pirámide.
    /// </summary>
    /// <param name="direction">La dirección que se va a comprobar.</param>
    /// <returns>Verdadero si la bola cae, falso si no cae.</returns>
    bool InBorder(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 1f, borderMask);

        return hit;
    }
}