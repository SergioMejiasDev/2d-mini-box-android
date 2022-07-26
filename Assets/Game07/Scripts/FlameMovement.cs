using UnityEngine;

/// <summary>
/// Clase que controla la IA de las llamas.
/// </summary>
public class FlameMovement : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento de la llama.
    /// </summary>
    [Header("Movement")]
    readonly float speed = 1.5f;
    /// <summary>
    /// Vector de dirección del movimiento de la llama.
    /// </summary>
    Vector2 direction = Vector2.right;
    /// <summary>
    /// Posición en el eje X hasta la cual se va a desplazar la llama.
    /// </summary>
    float destination;

    /// <summary>
    /// Componente CapsuleCollider2D de la llama.
    /// </summary>
    [Header("Components")]
    [SerializeField] CapsuleCollider2D capsuleCollider;
    /// <summary>
    /// Componente Rigidbody2D de la llama.
    /// </summary>
    [SerializeField] Rigidbody2D rb;

    void Start()
    {
        ChooseDestination();
    }

    void Update()
    {
        if (direction == Vector2.right && transform.position.x < destination)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        else if (direction == Vector2.left && transform.position.x > destination)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        else if (direction == Vector2.up)
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        else
        {
            // Cuando la llama alcanza su destino, se elige un nuevo destino aleatorio.

            ChooseDestination();
        }
    }

    /// <summary>
    /// Función que asigna un nuevo destino a la llama.
    /// </summary>
    void ChooseDestination()
    {
        if (transform.position.y < -4.5f)
        {
            destination = Random.Range(-5.5f, 5.5f);
        }

        else
        {
            destination = Random.Range(-4.15f, 5.5f);
        }

        if (destination > transform.position.x)
        {
            direction = Vector2.right;
            transform.localScale = new Vector2(1, 1);
        }

        else
        {
            direction = Vector2.left;
            transform.localScale = new Vector2(-1, 1);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si la llama toca una escalera, hay una probabilidad del 25% de que la suba.

        if (collision.gameObject.name == "Ladder")
        {
            if (Random.value > 0.25f)
            {
                return;
            }

            transform.position = new Vector2(collision.gameObject.transform.position.x, transform.position.y);
            direction = Vector2.up;

            capsuleCollider.isTrigger = true;
            rb.gravityScale = 0;
        }

        // Estos colliders está en la parte superior de las escaleras.
        // Cuando la llama llega al borde de las escaleras, dejará de subir y elegirá un nuevo destino.

        else if (collision.gameObject.name == "FlameCollider")
        {
            if (direction == Vector2.up)
            {
                capsuleCollider.isTrigger = false;
                rb.gravityScale = 1;

                ChooseDestination();
            }
        }

        // Si la llama es golpeada por el mazo del jugador, se destruirá.

        else if (collision.gameObject.CompareTag("Game7/MalletHit"))
        {
            GameManager7.manager7.DestroyFlame(transform.position);

            Destroy(gameObject);
        }
    }
}