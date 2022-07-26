using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que contiene las funciones principales del jugador.
/// </summary>
public class MarioMovement : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento del jugador.
    /// </summary>
    [Header("Walking")]
    readonly float speed = 1.5f;
    /// <summary>
    /// Fuerza del salto del jugador.
    /// </summary>
    readonly float jump = 4.0f;
    /// <summary>
    /// Capa asignada al suelo donde el jugador puede saltar.
    /// </summary>
    [SerializeField] LayerMask groundMask = 0;
    /// <summary>
    /// Verdadero si el jugador está muerto. Falso si no lo está.
    /// </summary>
    bool isDead = false;
    /// <summary>
    /// Verdadero si el jugador acaba de ganar la partida. Falso si no es el caso.
    /// </summary>
    bool isWinning = false;

    /// <summary>
    /// Velocidad a la que el jugador sube las escaleras.
    /// </summary>
    [Header("Ladders")]
    readonly float climbSpeed = 2.0f;
    /// <summary>
    /// Verdadero si el jugador está en contacto con unas escaleras (puede subirlas pero no lo está haciendo). Falso si no lo está.
    /// </summary>
    bool inLadders = false;
    /// <summary>
    /// Verdadero si el jugador está subiendo unas escaleras. Falso si no lo está haciendo.
    /// </summary>
    bool climbingLadders = false;
    /// <summary>
    /// Posición de las escaleras con las que el jugador está en contacto.
    /// </summary>
    Transform ladders;
    /// <summary>
    /// Capa asignada a las escaleras.
    /// </summary>
    [SerializeField] LayerMask ladderMask = 0;
    /// <summary>
    /// Capa asignada al collider con el que el jugador detectará que está sobre unas escaleras que puede bajar.
    /// </summary>
    [SerializeField] LayerMask descendMask = 0;

    /// <summary>
    /// Verdadero si el jugador tiene activo el mazo. Falso si no lo tiene.
    /// </summary>
    [Header("Mallet")]
    bool malletMode = false;
    /// <summary>
    /// Collider del mazo cuando esté levantado.
    /// </summary>
    [SerializeField] GameObject topCollider;
    /// <summary>
    /// Collider del mazo cuando esté bajado.
    /// </summary>
    [SerializeField] GameObject rightCollider;

    /// <summary>
    /// Componente Animator del jugador.
    /// </summary>
    [Header("Components")]
    [SerializeField] Animator anim;
    /// <summary>
    /// Componente Rigidbody2D del jugador.
    /// </summary>
    [SerializeField] Rigidbody2D rb;
    /// <summary>
    /// Componente BoxCollider2D del jugador.
    /// </summary>
    [SerializeField] BoxCollider2D boxCollider;

    /// <summary>
    /// Sonido que se reproducirá cuando el jugador salte.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource jumpSound = null;
    /// <summary>
    /// Sonido que se reproducirá cuando el jugador muera.
    /// </summary>
    [SerializeField] AudioSource deathSound = null;
    /// <summary>
    /// Sonido que se reproducirá cuando el jugador coja el mazo.
    /// </summary>
    [SerializeField] AudioSource malletSound = null;

    /// <summary>
    /// Dirección de movimiento horizontal (1 derecha, -1 izquierda).
    /// </summary>
    [Header("Inputs")]
    int h = 0;
    /// <summary>
    /// Dirección de movimiento vertical (1 arriba, -1 abajo).
    /// </summary>
    int v = 0;

    /// <summary>
    /// Función que utiliza un Raycast para detectar si el jugador está tocando el suelo.
    /// </summary>
    /// <returns>Verdadero si el jugador está tocando el suelo, falso si no lo está.</returns>
    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, boxCollider.bounds.extents.y + 0.2f, 0), Vector2.down, 0.1f, groundMask);

        return hit;
    }

    /// <summary>
    /// Función que indica si el jugador ha llegado al borde superior de unas escaleras utilizando un Raycast.
    /// </summary>
    /// <returns>Verdadero si se ha llegado al borde de las escaleras. Falso si no se ha llegado.</returns>
    bool InBorderOfLadders()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, boxCollider.bounds.extents.y + 0.2f, 0), Vector2.down, 0.1f, ladderMask);

        return hit;
    }

    /// <summary>
    /// Función que utiliza un Raycast para saber si el jugador está sobre unas escaleras que puede bajar.
    /// </summary>
    /// <returns>Verdadero si el jugador puede bajar las escaleras. Falso si no puede.</returns>
    bool CanDescend()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, boxCollider.bounds.extents.y + 0.2f, 0), Vector2.down, 0.1f, descendMask);
        Debug.DrawRay(transform.position - new Vector3(0, boxCollider.bounds.extents.y + 0.2f, 0), Vector2.down * 0.01f, Color.green);

        if (hit)
        {
            ladders = hit.transform;
        }

        return hit;
    }

    private void OnEnable()
    {
        GameManager7.Stop += StopAnimation;
        GameManager7.Reset += ResetScene;
    }

    private void OnDisable()
    {
        GameManager7.Stop -= StopAnimation;
        GameManager7.Reset -= ResetScene;
    }

    void Update()
    {        
        if (Time.timeScale == 1 && !isDead)
        {
            if (!climbingLadders)
            {
                Movement();

                if (!malletMode)
                {
                    UseLadders();
                }
            }

            else if (climbingLadders)
            {
                MovementInLadders();

                if (InBorderOfLadders())
                {
                    LeaveLadders();
                }
            }

            Animation();
        }

        if (Input.GetButtonDown("Cancel"))
        {
            GameManager7.manager7.PauseGame();
        }
    }

    /// <summary>
    /// Función que se activa al pulsar los inputs en la pantalla.
    /// </summary>
    /// <param name="input">1 arriba, 2 derecha, 3 abajo, 4 izquierda.</param>
    public void EnableInput(int input)
    {
        switch (input)
        {
            case 1:
                v = 1;
                break;

            case 2:
                h = 1;
                break;

            case 3:
                v = -1;
                break;

            case 4:
                h = -1;
                break;
        }
    }

    /// <summary>
    /// Función que se desactiva al dejar de pulsar los inputs en la pantalla.
    /// </summary>
    /// <param name="input">1 arriba, 2 derecha, 3 abajo, 4 izquierda.</param>
    public void DisableInput(int input)
    {
        switch (input)
        {
            case 1:
                v = 0;
                break;

            case 2:
                h = 0;
                break;

            case 3:
                v = 0;
                break;

            case 4:
                h = 0;
                break;
        }
    }

    /// <summary>
    /// Función que se encarga del movimiento del jugador.
    /// </summary>
    void Movement()
    {
        if (h > 0)
        {
            transform.localScale = new Vector2(1, 1);
        }

        else if (h < 0)
        {
            transform.localScale = new Vector2(-1, 1);
        }

        transform.Translate(Vector2.right * speed * Time.deltaTime * h);
    }

    /// <summary>
    /// Función que permite al jugador moverse de forma vertical en las escaleras.
    /// </summary>
    void MovementInLadders()
    {
        transform.Translate(Vector2.up * climbSpeed * Time.deltaTime * v);
    }

    /// <summary>
    /// Función que activa las animaciones del jugador.
    /// </summary>
    void Animation()
    {
        anim.SetBool("IsWalking", (h != 0) && IsGrounded() && !climbingLadders);
        anim.SetBool("IsJumping", !IsGrounded() && !climbingLadders);
        anim.SetBool("IsClimbingIddle", (v == 0) && climbingLadders);
        anim.SetBool("IsClimbingMovement", (v != 0) && climbingLadders);
    }

    /// <summary>
    /// Función que hace el jugador salte.
    /// </summary>
    public void Jump()
    {
        if (!isWinning && !climbingLadders && IsGrounded() && (!inLadders || malletMode))
        {
            jumpSound.Play();

            rb.AddForce(Vector2.up * jump, ForceMode2D.Impulse);
        }
    }

    /// <summary>
    /// Función que hace que el jugador se suba a las escaleras si está sobre o bajo ellas.
    /// </summary>
    void UseLadders()
    {
        if (v == 1 && inLadders)
        {
            transform.position = new Vector3(ladders.position.x, transform.position.y, 0);
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            climbingLadders = true;
        }

        else if (v == -1 && !climbingLadders && CanDescend())
        {
            transform.position = new Vector3(ladders.position.x, transform.position.y - 0.2f, 0);
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            climbingLadders = true;
        }
    }

    /// <summary>
    /// Función que permite al jugador bajarse de las escaleras una vez alcanzados los bordes.
    /// </summary>
    void LeaveLadders()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        climbingLadders = false;
    }

    /// <summary>
    /// Función que activa los colliders del mazo.
    /// </summary>
    /// <param name="isTopCollider">1 si se activa el collider superior, 0 si se activa el collider lateral.</param>
    public void MalletCollider(int isTopCollider)
    {
        if (isTopCollider == 1)
        {
            topCollider.SetActive(true);
            rightCollider.SetActive(false);
        }

        else
        {
            topCollider.SetActive(false);
            rightCollider.SetActive(true);
        }
    }

    /// <summary>
    /// Función que se llama a través del delegado para resetear la posición del jugador en la escena.
    /// </summary>
    void ResetScene()
    {
        boxCollider.enabled = true;
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.velocity = Vector2.zero;

        inLadders = false;
        climbingLadders = false;

        anim.SetBool("IsDying", false);
        anim.SetBool("IsDie", false);

        transform.position = new Vector2(-4.1f, -4.2f);
        transform.localScale = new Vector2(1f, 1f);

        isDead = false;
        isWinning = false;
    }

    /// <summary>
    /// Función que detiene todas las animaciones del jugador.
    /// </summary>
    void StopAnimation()
    {
        anim.SetBool("IsWalking", false);
        anim.SetBool("IsJumping", false);
        anim.SetBool("IsClimbingIddle", false);
        anim.SetBool("IsClimbingMovement", false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el jugador es golpeado por un barril o una llama, morirá, a no ser que tenga el mazo.
        // Si tiene el mazo en la mano destruirá el barril.

        if (collision.gameObject.CompareTag("Game7/Barrel") || collision.gameObject.CompareTag("Game7/Flame"))
        {
            if (malletMode)
            {
                return;
            }

            StartCoroutine(Dying());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el jugador está junto a unas escaleras, se guardará la referencia a las mismas para poder subirse a ellas.

        if (collision.gameObject.CompareTag("Game7/Ladders"))
        {
            ladders = collision.gameObject.transform;
        }

        // Si el jugador coge un mazo, se activará temporalmente el modo invulnerable.

        else if (collision.gameObject.CompareTag("Game7/Mallet"))
        {
            collision.gameObject.SetActive(false);

            malletSound.Play();
            
            StartCoroutine(ModeMallet());
        }

        // Si el jugador salta sobre un barril, aumentará la puntuación.

        else if (collision.gameObject.CompareTag("Game7/JumpArea"))
        {
            if (!climbingLadders)
            {
                GameManager7.manager7.JumpBarrel(collision.gameObject.transform.position);
            }
        }

        // Si el jugador llega a la parte superior del escenario (donde la princesa), ganará la partida.

        else if (collision.gameObject.name == "WinCollider")
        {
            if (!climbingLadders)
            {
                isWinning = true;

                GameManager7.manager7.WinGame();
            }
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Game7/Ladders"))
        {
            if (!inLadders)
            {
                inLadders = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // Si el jugador deja de estár junto a unas escaleras, se perderá la referencia a las mismas y no podrá subirse.

        if (collision.gameObject.CompareTag("Game7/Ladders"))
        {
            inLadders = false;
        }
    }

    /// <summary>
    /// Corrutina que se inicia cuando el jugador coge un mazo.
    /// </summary>
    /// <returns></returns>
    IEnumerator ModeMallet()
    {
        malletMode = true;
        anim.SetBool("ActivateMallet", true);

        yield return new WaitForSeconds(7);

        malletMode = false;
        anim.SetBool("ActivateMallet", false);

        topCollider.SetActive(false);
        rightCollider.SetActive(false);
    }

    /// <summary>
    /// Corrutina que se inicia cuando el jugador muere.
    /// </summary>
    /// <returns></returns>
    IEnumerator Dying()
    {
        deathSound.Play();

        isDead = true;
        boxCollider.enabled = false;
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;

        StopAnimation();
        GameManager7.manager7.CleanScene();
        GameManager7.manager7.StopGame();

        yield return new WaitForSeconds(1f);

        anim.SetBool("IsDying", true);

        yield return new WaitForSeconds(1.5f);

        anim.SetBool("IsDying", false);
        anim.SetBool("IsDie", true);

        yield return new WaitForSeconds(1.5f);

        GameManager7.manager7.GameOver();
    }
}