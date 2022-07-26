using UnityEngine;

/// <summary>
/// Clase que controla las funciones del dinosario.
/// </summary>
public class Dinosaur : MonoBehaviour
{
    /// <summary>
    /// La fuerza del salto del dinosaurio.
    /// </summary>
    [Header("Movement")]
    readonly float jumpForce = 18;
    /// <summary>
    /// La capa asignada al suelo sobre el que el dinosaurio puede saltar.
    /// </summary>
    [SerializeField] LayerMask groundMask = 0;
    /// <summary>
    /// Verdadero cuando el dinosaurio acaba de chocar con algún objeto.
    /// </summary>
    bool isDie = false;

    /// <summary>
    /// Componente BoxCollider2D del dinosaurio cuando está de pie.
    /// </summary>
    [Header("Colliders")]
    [SerializeField] BoxCollider2D standingCollider = null;
    /// <summary>
    /// Componente BoxCollider2D del dinosaurio cuando está agachado.
    /// </summary>
    [SerializeField] BoxCollider2D crouchCollider = null;

    /// <summary>
    /// Verdadero si el dinosaurio está agachado. Falso si está de pie.
    /// </summary>
    [Header("Components")]
    bool isCrouch;
    /// <summary>
    /// Componente Rigidbody2D del dinosaurio.
    /// </summary>
    [SerializeField] Rigidbody2D rb = null;
    /// <summary>
    /// Componente Animator del dinosaurio.
    /// </summary>
    [SerializeField] Animator anim = null;
    /// <summary>
    /// Componente Audiosource con el sonido del salto.
    /// </summary>
    [SerializeField] AudioSource jumpSound = null;

    private void OnEnable()
    {
        GameManager9.StopMovement += Hit;
    }

    private void OnDisable()
    {
        GameManager9.StopMovement -= Hit;
    }

    void Update()
    {
        if (isDie)
        {
            return;
        }

        if (Input.GetButtonDown("Cancel"))
        {
            GameManager9.manager.PauseGame();
        }
        
        Animation();
    }

    /// <summary>
    /// Función que hace que el dinosaurio salte.
    /// </summary>
    public void Jump()
    {
        if (IsGrounded() && !isCrouch)
        {
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpSound.Play();
        }
    }

    /// <summary>
    /// Función que hace que el dinosaurio se agache o se levante.
    /// </summary>
    /// <param name="enable">Verdadero para que se agache, falso para que se levante.</param>
    public void Crouch(bool enable)
    {
        if (enable)
        {
            isCrouch = true;

            standingCollider.enabled = false;
            crouchCollider.enabled = true;
        }

        else
        {
            isCrouch = false;

            standingCollider.enabled = true;
            crouchCollider.enabled = false;
        }
    }

    /// <summary>
    /// Función que activa las animaciones del dinosaurio.
    /// </summary>
    void Animation()
    {
        anim.SetBool("Jump", !IsGrounded());
        anim.SetBool("Crouch", isCrouch);
    }

    /// <summary>
    /// Función que reinicia los valores del dinosaurio al reiniciar la partida.
    /// </summary>
    public void ResetValues()
    {
        transform.position = new Vector2(-5.66f, -1.53f);

        anim.SetBool("Hit", false);

        rb.gravityScale = 5;

        Crouch(false);

        isDie = false;
    }

    /// <summary>
    /// Función que se activa a través del delegado cuando el dinosaurio choca con algo.
    /// </summary>
    void Hit(float speed)
    {
        anim.SetBool("Hit", true);

        isDie = true;

        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
    }

    /// <summary>
    /// Función que utiliza un Raycast para saber si el dinosaurio está en el suelo.
    /// </summary>
    /// <returns>Verdadero si está en el suelo, falso si no lo está.</returns>
    bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position - new Vector3(0, 0.7f, 0), Vector2.down, 0.1f, groundMask);

        return hit;
    }
}