using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que contiene las funciones principales de la rana.
/// </summary>
public class FrogMovement : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento de la rana.
    /// </summary>
    [Header("Movement")]
    readonly float speed = 0.1f;
    /// <summary>
    /// Vector de dirección del movimiento de la rana.
    /// </summary>
    Vector2 destination = Vector2.zero;
    /// <summary>
    /// Capa asignada a los bordes del escenario para evitar que la rana pueda saltar hasta ellos.
    /// </summary>
    [SerializeField] LayerMask borderMask;
    /// <summary>
    /// Será positivo si la rana está muerta, falso si no lo está.
    /// </summary>
    bool isDie = false;
    /// <summary>
    /// Será positivo si es posible usar los inputs. Falso si no es posible.
    /// </summary>
    bool inputsEnabled = false;

    /// <summary>
    /// El objeto movil sobre el que la rana está subida.
    /// </summary>
    [Header("Water Area")]
    GameObject parent;
    /// <summary>
    /// Verdadero si la rana está sobre un objeto en movimiento. Falso si no lo está.
    /// </summary>
    bool moving = false;
    /// <summary>
    /// Verdadero si la rana está en la zona de agua. Falso si no está.
    /// </summary>
    bool inWaterArea = false;
    /// <summary>
    /// Booleano utilizado para saber si se han realizado comprobaciones sobre la posición de la rana en la zona de agua.
    /// </summary>
    bool hasChecked = true;

    /// <summary>
    /// Componente Rigidbody2D de la rana.
    /// </summary>
    [Header("Components")]
    [SerializeField] Rigidbody2D rb = null;
    /// <summary>
    /// Componente Animator de la rana.
    /// </summary>
    [SerializeField] Animator anim = null;
    /// <summary>
    /// Componente CircleCollider2D de la rana.
    /// </summary>
    [SerializeField] CircleCollider2D circleCollider = null;

    /// <summary>
    /// Sonido que se reproducirá cada vez que la rana salte.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource jumpSound = null;
    /// <summary>
    /// Sonido que se reproducirá cuando un coche atropelle a la rana.
    /// </summary>
    [SerializeField] AudioSource plunkSound = null;
    /// <summary>
    /// Sonido que se reproducirá cuando la rana caiga al agua.
    /// </summary>
    [SerializeField] AudioSource squashSound = null;

    void Start()
    {
        destination = transform.position;
    }

    private void Update()
    {
        if ((Vector2)transform.position == destination)
        {
            if (!hasChecked)
            {
                hasChecked = true;

                if (inWaterArea && !moving)
                {
                    plunkSound.Play();
                    ResetValues();
                    StartCoroutine(Die("Plunk"));
                }
            }

            if (!inputsEnabled)
            {
                inputsEnabled = true;
            }
        }

        else if (moving)
        {
            if (!inputsEnabled)
            {
                inputsEnabled = true;
            }
        }

        if (Input.GetButtonDown("Cancel"))
        {
            GameManager8.manager8.PauseGame();
        }
    }

    void FixedUpdate()
    {
        if (moving)
        {
            return;
        }

        Vector2 newPosition = Vector2.MoveTowards(transform.position, destination, speed);
        rb.MovePosition(newPosition);

        Vector2 direction = destination - (Vector2)transform.position;
        anim.SetFloat("DirX", direction.x);
        anim.SetFloat("DirY", direction.y);
        anim.SetBool("InWater", moving);
    }

    /// <summary>
    /// Función que comprueba si el jugador puede moverse en una dirección concreta.
    /// </summary>
    /// <param name="direction">Dirección que queremos comprobar.</param>
    /// <returns>Verdadero si puede moverse, falso si no puede.</returns>
    bool RayMovement(Vector2 direction)
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, 0.5f, borderMask);

        return hit;
    }

    /// <summary>
    /// Función que se activa al pulsar los inputs en la pantalla.
    /// </summary>
    /// <param name="input">1 arriba, 2 derecha, 3 abajo, 4 izquierda.</param>
    public void EnterInputs(int input)
    {
        if (!inputsEnabled || Time.timeScale != 1 || isDie)
        {
            return;
        }

        if (input == 1 && !RayMovement(Vector2.up))
        {
            jumpSound.Play();
            ResetValues();
            destination = (Vector2)transform.position + 0.5f * Vector2.up;
        }

        else if (input == 2 && !RayMovement(Vector2.right))
        {
            jumpSound.Play();
            ResetValues();
            destination = (Vector2)transform.position + 0.45f * Vector2.right;
        }

        else if (input == 3 && !RayMovement(-Vector2.up))
        {
            jumpSound.Play();
            ResetValues();
            destination = (Vector2)transform.position - 0.5f * Vector2.up;
        }

        else if (input == 4 && !RayMovement(-Vector2.right))
        {
            jumpSound.Play();
            ResetValues();
            destination = (Vector2)transform.position - 0.45f * Vector2.right;
        }
    }

    /// <summary>
    /// Función que reinicia las variables relacionadas con el movimiento en la zona de agua.
    /// </summary>
    void ResetValues()
    {
        inputsEnabled = false;
        moving = false;
        parent = null;
        transform.SetParent(null);

        hasChecked = false;
    }

    /// <summary>
    /// Función que reinicia las variables de la rana después de morir.
    /// </summary>
    public void ResetPosition()
    {
        isDie = false;
        inWaterArea = false;
        circleCollider.enabled = true;

        ResetValues();

        transform.position = new Vector2(0, -3.35f);
        destination = transform.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si la rana se sube a un objeto en movimiento en el agua, se desplazará con él.

        if (collision.gameObject.layer == LayerMask.NameToLayer("Mask2"))
        {
            parent = collision.gameObject;
            moving = true;
            inWaterArea = true;
            transform.position = parent.transform.position;
            transform.parent = parent.transform;
        }

        // Si el jugador entra en la zona de agua, se activará la variable inWaterAra para poder activar otras funciones.

        if (collision.gameObject.CompareTag("Game8/Water"))
        {
            inWaterArea = true;
        }

        // Cada vez que la rana suba una posición, aumentará la puntuación.

        if (collision.gameObject.name == "Line")
        {
            collision.gameObject.SetActive(false);
            GameManager8.manager8.UpdateScore(10);
        }

        // Si la rana llega hasta el extremo de la orilla y rescata a otra rana, volverá a su posición inicial.

        if (collision.gameObject.name == "Frog")
        {
            GameManager8.manager8.FrogRescued(collision.gameObject);

            ResetPosition();
        }

        // Si la rana cae al agua, morirá.

        if (collision.gameObject.CompareTag("Game8/DeathZone"))
        {
            destination = transform.position;
            ResetValues();
            plunkSound.Play();
            StartCoroutine(Die("Plunk"));
        }

        // Si un coche atropella a la rana, morirá.

        else if (collision.gameObject.CompareTag("Game8/Car1") ||
            collision.gameObject.CompareTag("Game8/Car2") ||
            collision.gameObject.CompareTag("Game8/Car3") ||
            collision.gameObject.CompareTag("Game8/Car4") ||
            collision.gameObject.CompareTag("Game8/Car5"))
        {
            ResetValues();
            squashSound.Play();
            StartCoroutine(Die("Squash"));
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el jugador sale de la zona de agua, se desactivará la variable inWaterArea.

        if (collision.gameObject.CompareTag("Game8/Water"))
        {
            inWaterArea = false;
        }
    }

    /// <summary>
    /// Corrutina que inicia la animación en que puere la rana.
    /// </summary>
    /// <param name="animation">El parámetro de la animación que se va a iniciar.</param>
    /// <returns></returns>
    IEnumerator Die(string animation)
    {
        isDie = true;
        circleCollider.enabled = false;

        anim.SetBool(animation, true);

        yield return new WaitForSeconds(2);

        anim.SetBool(animation, false);

        GameManager8.manager8.IsDie();

        gameObject.SetActive(false);
    }
}