using UnityEngine;

/// <summary>
/// Clase que se encarga del movimiento del jugador.
/// </summary>
public class Player : MonoBehaviour
{
    #region Variables
    /// <summary>
    /// Velocidad del jugador.
    /// </summary>
    [Header("Movement")]
    readonly float speed = 4;
    /// <summary>
    /// Fuerza del salto del jugador.
    /// </summary>
    readonly float jump = 9.5f;
    /// <summary>
    /// Verdadero si el jugador no está en movimiento.
    /// </summary>
    bool dontMove = true;
    /// <summary>
    /// Verdadero si el jugador se está moviendo hacia la izquierda, Falso si se mueve hacia la izquierda.
    /// La variable dontMove debe ser Verdadero.
    /// </summary>
    bool moveLeft = false;
    /// <summary>
    /// Capa asignada al suelo donde se puede saltar.
    /// </summary>
    [SerializeField] LayerMask groundMask = 0;

    /// <summary>
    /// Componente Rigidbody2D del jugador.
    /// </summary>
    [Header("Components")]
    [SerializeField] Rigidbody2D rb = null;
    /// <summary>
    /// Componente Animator del jugador.
    /// </summary>
    [SerializeField] Animator anim = null;
    /// <summary>
    /// Componente SpriteRenderer del jugador.
    /// </summary>
    [SerializeField] SpriteRenderer sr = null;
    /// <summary>
    /// Componente AudioSource del jugador.
    /// </summary>
    [SerializeField] AudioSource audioSource = null;

    /// <summary>
    /// AudioSource cuyo sonido indicará que nos ha alcanzado un enemigo.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource hurtSound = null;
    #endregion

    private void OnEnable()
    {
        transform.position = new Vector2(-6.3f, -5.4f); // Posición donde se genera el jugador al iniciar la partida.
        DontAllowMovement(); // El jugador debe estar parado al iniciar la partida.
    }

    void Update()
    {
        HandleMoving();

        Animation();
    }

    /// <summary>
    /// Función controla el movimiento del jugador.
    /// </summary>
    void HandleMoving()
    {
        if (!dontMove) // Se está pulsando algún botón en la pantalla.
        {
            if (moveLeft) // Se está pulsando el botón izquierdo.
            {
                MoveLeft();
            }
            else if (!moveLeft) // Se está pulsando el botón derecho.
            {
                MoveRight();
            }
        }

        else // No se está pulsando ningún botón en la pantalla.
        {
            StopMoving();
        }
    }

    /// <summary>
    /// Función que permite o impide el movimiento del jugador según los botones que se están pulsando.
    /// </summary>
    /// <param name="leftMovement">Verdadero si el jugador se está pulsando el botón izquierdo.
    /// Falso si se está pulsando el botón derecho.</param>
    public void AllowMovement(bool leftMovement)
    {
        dontMove = false;
        moveLeft = leftMovement;
    }

    /// <summary>
    /// Función que impide el movimiento del jugador cuando se levanta el dedo de la pantalla.
    /// </summary>
    public void DontAllowMovement()
    {
        dontMove = true;
    }

    /// <summary>
    /// Función que mueve al jugador hacia la izquierda.
    /// </summary>
    void MoveLeft()
    {
        transform.Translate(Vector2.right * -speed * Time.deltaTime);
        transform.localScale = new Vector2(-0.85f, 0.85f);
    }

    /// <summary>
    /// Función que mueve al jugador hacia la derecha.
    /// </summary>
    void MoveRight()
    {
        transform.Translate(Vector2.right * speed * Time.deltaTime);
        transform.localScale = new Vector2(0.85f, 0.85f);
    }

    /// <summary>
    /// Función que mantiene al jugador sin movimiento.
    /// </summary>
    void StopMoving()
    {
        transform.Translate(Vector2.right * 0 * Time.deltaTime);
    }

    /// <summary>
    /// Función que activa las animaciones del jugador.
    /// </summary>
    void Animation()
    {
        anim.SetBool("IsWalking", (!dontMove && IsGrounded())); // Si se está moviendo mientras toca el suelo, está caminando.
        anim.SetBool("IsJumping", !IsGrounded()); // Si no está tocando el suelo, se mueva o no, está saltando.
    }

    /// <summary>
    /// Función que hace saltar al jugador.
    /// </summary>
    public void Jump()
    {
        if (IsGrounded()) // Es necesario estar tocando el suelo para saltar.
        {
            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
            audioSource.Play();
        }
    }

    /// <summary>
    /// Función que indica si el jugador está tocando el suelo o no.
    /// </summary>
    /// <returns>Verdadero si el jugador está en el suelo, falso si no lo está.</returns>
    bool IsGrounded()
    {
        // Mediante un Raycast vertical, miramos si justo debajo del jugador está el suelo.

        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, sr.bounds.extents.y + 0.01f, 0), Vector2.down, 0.1f, groundMask);

        return hit;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        // Si tocamos a un enemigo, perdemos la partida.

        if ((other.gameObject.CompareTag("Game1/Enemy")) || (other.gameObject.CompareTag("Game1/Missile")))
        {
            gameObject.SetActive(false);
            GameManager1.manager.GameOver();

            hurtSound.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si tocamos una moneda, esta desaparece y aumenta nuestra puntuación.

        if (other.gameObject.CompareTag("Game1/Coin"))
        {
            other.gameObject.SetActive(false);
            GameManager1.manager.UpdateScore();
        }
    }
}