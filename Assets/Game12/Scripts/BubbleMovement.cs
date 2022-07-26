using UnityEngine;

/// <summary>
/// Clase encargada del movimiento de los objetos que caen en el juego.
/// </summary>
public class BubbleMovement : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento de los objetos.
    /// </summary>
    float speed = 6.0f;
    /// <summary>
    /// Velocidad de movimiento de los objetos cuando son atraidos por el imán.
    /// </summary>
    float magnetSpeed = 3.0f;

    /// <summary>
    /// Verdadero si es una burbuja normal. Falso si no lo es.
    /// </summary>
    [SerializeField] bool normalBubble = false;
    /// <summary>
    /// Verdadero si está activo el modo magnético. Falso si no lo está.
    /// </summary>
    bool magnetMode = false;
    /// <summary>
    /// La posición del jugador.
    /// </summary>
    Transform player;

    private void OnEnable()
    {
        speed = 6.0f;
        magnetSpeed = 3.0f;

        GameManager12.StopMovement += StopMovement;

        if (normalBubble)
        {
            magnetMode = GameManager12.manager.magnetMode;
            BalloonMovement.StartMagnetMode += StartMagnetMode;
        }
    }

    private void OnDisable()
    {
        GameManager12.StopMovement -= StopMovement;

        if (normalBubble)
        {
            BalloonMovement.StartMagnetMode -= StartMagnetMode;
        }
    }

    private void Start()
    {
        if (normalBubble)
        {
            player = GameObject.FindGameObjectWithTag("Player").transform;
        }
    }

    void Update()
    {
        if (magnetMode && Vector2.Distance(transform.position, player.position) < 3f)
        {
            transform.Translate((player.position - transform.position) * magnetSpeed * Time.deltaTime);

            return;
        }

        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto sale de la pantalla, se destruye.

        if (collision.gameObject.CompareTag("Game12/Destructor"))
        {
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Función que se activa a través del delegado para detener el movimiento del objeto.
    /// </summary>
    void StopMovement()
    {
        speed = 0f;
        magnetSpeed = 0f;
    }

    /// <summary>
    /// Función que se activa a través del delegado para que las burbujas sean atraidas por el globo.
    /// </summary>
    /// <param name="enable">Verdadero para activarlo, falso para desactivarlo.</param>
    void StartMagnetMode(bool enable)
    {
        magnetMode = enable;
    }
}