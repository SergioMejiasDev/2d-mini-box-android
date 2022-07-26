using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que controla las funciones de las bolas moradas y de las serpientes.
/// </summary>
public class QBertPurpleBall : MonoBehaviour
{
    /// <summary>
    /// La posición del jugador.
    /// </summary>
    [Header("Movement")]
    Transform player;
    /// <summary>
    /// La dirección a la que se está moviendo la bola verde.
    /// </summary>
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
    float waitTime = 1.0f;
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
    /// <summary>
    /// Componente QBertRedBall asignado a la bola.
    /// </summary>
    [SerializeField] QBertRedBall redClass = null;

    void OnEnable()
    {
        GameManager10.StopMovement += StopMovement;

        player = GameObject.FindGameObjectWithTag("Player").transform;
        destination = transform.position;
        newDestination = transform.position;
        falling = false;
        timer = 0;

        StartCoroutine(FirstWait());
    }

    private void OnDisable()
    {
        GameManager10.StopMovement -= StopMovement;

        anim.SetBool("Snake", false);
        redClass.enabled = true;
        enabled = false;
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

        GameManager10.manager.FallSnake();

        if (transform.position.y > -2.9f)
        {
            sr.sortingOrder = -1;
        }

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

    /// <summary>
    /// Corrutina que hace una espera inicial antes de convertir la bola en serpiente.
    /// </summary>
    /// <returns></returns>
    IEnumerator FirstWait()
    {
        yield return new WaitForSeconds(2);

        isActive = true;
        anim.SetBool("Snake", true);
        anim.SetBool("Moving", false);
    }

    /// <summary>
    /// Función que decide la siguiente posición a la que se moverá la bola.
    /// </summary>
    /// <returns>El vector con una de las dos posibles posiciones.</returns>
    Vector2 NewDestination()
    {
        if (player == null)
        {
            return transform.position;
        }

        if (player.position.x >= transform.position.x && player.position.y >= transform.position.y)
        {
            if (InBorder(new Vector2(0.5f, 0.75f)))
            {
                falling = true;
            }

            newDestination = new Vector2(transform.position.x + 0.5f, transform.position.y + 0.75f);
            return new Vector2(transform.position.x + 0.07f, transform.position.y + 0.5f);
        }

        else if (player.position.x >= transform.position.x && player.position.y <= transform.position.y)
        {
            if (InBorder(new Vector2(0.5f, -0.75f)))
            {
                falling = true;
            }

            newDestination = new Vector2(transform.position.x + 0.5f, transform.position.y - 0.75f);
            return new Vector2(transform.position.x + 0.43f, transform.position.y - 0.25f);
        }

        else if (player.position.x <= transform.position.x && player.position.y <= transform.position.y)
        {
            if (InBorder(new Vector2(-0.5f, -0.75f)))
            {
                falling = true;
            }

            newDestination = new Vector2(transform.position.x - 0.5f, transform.position.y - 0.75f);
            return new Vector2(transform.position.x - 0.43f, transform.position.y - 0.25f);
        }

        else if (player.position.x <= transform.position.x && player.position.y >= transform.position.y)
        {
            if (InBorder(new Vector2(-0.5f, 0.75f)))
            {
                falling = true;
            }

            newDestination = new Vector2(transform.position.x - 0.5f, transform.position.y + 0.75f);
            return new Vector2(transform.position.x - 0.07f, transform.position.y + 0.5f);
        }

        else
        {
            return transform.position;
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