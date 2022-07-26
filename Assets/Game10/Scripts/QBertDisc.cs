using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que controla las funciones de los discos.
/// </summary>
public class QBertDisc : MonoBehaviour
{
    /// <summary>
    /// Verdadero si el disco está en movimiento (cambiando de posición). Falso si no lo está.
    /// </summary>
    [Header("Movement")]
    bool isActive = false;
    /// <summary>
    /// La dirección a la que se está moviendo el disco.
    /// </summary>
    Vector2 destination;
    /// <summary>
    /// La primera posición en la trayectoria del disco.
    /// </summary>
    Vector2 destination1;
    /// <summary>
    /// La segunda posición en la trayectoria del disco.
    /// </summary>
    readonly Vector2 destination2 = new Vector2(0, 2.45f);
    /// <summary>
    /// El componente Rigidbody2D del jugador (una vez que se ha subido al disco).
    /// </summary>
    Rigidbody2D player;

    /// <summary>
    /// El componente Rigidbody2D del disco.
    /// </summary>
    [Header("Components")]
    [SerializeField] Rigidbody2D rb = null;
    /// <summary>
    /// El collider que sustituirá al disco cuando este desaparezca.
    /// </summary>
    [SerializeField] GameObject borderCollider = null;
    /// <summary>
    /// El componente AudioSource del disco.
    /// </summary>
    [SerializeField] AudioSource sound = null;

    void OnEnable()
    {
        player = rb;

        destination1 = new Vector2(transform.position.x, -0.28f);
        destination = destination1;

        isActive = false;
    }

    void FixedUpdate()
    {
        if (!isActive)
        {
            return;
        }

        if ((Vector2)transform.position == destination1)
        {
            destination = destination2;
        }

        Vector2 newPosition = Vector2.MoveTowards(transform.position, destination, 0.05f);
        rb.MovePosition(newPosition);
        player.MovePosition(newPosition);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador se sube al disco, este lo transportará hasta la parte superior de la pirámide.

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.position = new Vector2(transform.position.x, -1.10f);

            player = collision.gameObject.GetComponent<Rigidbody2D>();

            borderCollider.SetActive(true);

            StartCoroutine(EnableDisc(collision.gameObject));
        }
    }

    /// <summary>
    /// Corrutina que desactiva el disco una vez que ha llegado a la parte superior de la pirámide.
    /// </summary>
    /// <param name="playerObject">El jugador.</param>
    /// <returns></returns>
    IEnumerator EnableDisc(GameObject playerObject)
    {
        yield return new WaitForSeconds(1);

        isActive = true;

        sound.Play();

        yield return new WaitForSeconds(3);

        playerObject.GetComponent<QBertMovement>().ResetPosition();

        gameObject.SetActive(false);
    }
}