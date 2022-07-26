using System.Collections;
using UnityEngine;

/// <summary>
/// Clase encargada del movimiento del globo del jugador.
/// </summary>
public class BalloonMovement : MonoBehaviour
{
    public delegate void BalloonDelegate(bool enable);
    public static event BalloonDelegate StartMagnetMode;

    /// <summary>
    /// Velocidad de movimiento del globo.
    /// </summary>
    [Header("Movement")]
    readonly float speed = 10.0f;
    /// <summary>
    /// Posición máxima que puede alcanzar el globo en el eje X por la izquierda.
    /// </summary>
    readonly float minBound = -3.6f;
    /// <summary>
    /// Posición máxima que puede alcanzar el globo en el eje X por la derecha.
    /// </summary>
    readonly float maxBound = 3.6f;
    /// <summary>
    /// Dirección de movimiento del globo en el eje X.
    /// </summary>
    float h;
    /// <summary>
    /// Verdadero si está activo el modo invulnerable. Falso si no es el caso.
    /// </summary>
    bool colorMode = false;
    /// <summary>
    /// Verdadero si el globo puede moverse. Falso si no puede moverse.
    /// </summary>
    public bool canMove = false;

    /// <summary>
    /// Componente Animator del globo.
    /// </summary>
    [Header("Components")]
    [SerializeField] Animator anim = null;

    /// <summary>
    /// Componente AudioSource del globo.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource pickUpSound = null;

    void Update()
    {
        if (transform.position.x < minBound && h < 0)
        {
            h = 0;
        }

        else if (transform.position.x > maxBound && h > 0)
        {
            h = 0;
        }

        if (canMove)
        {
            transform.Translate(Vector2.right * speed * h * Time.deltaTime);
        }

        if (Input.GetButtonDown("Cancel") && canMove)
        {
            GameManager12.manager.PauseGame();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el globo coge una burbuja, aumenta la puntuación.

        if (collision.gameObject.CompareTag("Game12/Bubble"))
        {
            collision.gameObject.SetActive(false);
            GameManager12.manager.UpdateScore(10);
        }

        // Si el globo coge una burbuja de colores, se activa el modo invulnerable.

        else if (collision.gameObject.CompareTag("Game12/BubbleColor"))
        {
            collision.gameObject.SetActive(false);
            pickUpSound.Play();

            StartCoroutine(ColorMode());
        }

        // Si el globo coge un imán, las burbujas comienzan a acercarse al globo.

        else if (collision.gameObject.CompareTag("Game12/Magnet"))
        {
            collision.gameObject.SetActive(false);
            pickUpSound.Play();

            StartCoroutine(MagnetMode());
        }

        // Si el globo choca con los pinchos, termina la partida.

        else if (collision.gameObject.CompareTag("Game12/Spike1") ||
            collision.gameObject.CompareTag("Game12/Spike2") ||
            collision.gameObject.CompareTag("Game12/Spike3") ||
            collision.gameObject.CompareTag("Game12/Spike4") ||
            collision.gameObject.CompareTag("Game12/Spike5"))
        {
            if (colorMode)
            {
                return;
            }

            StopAllCoroutines();
            GameManager12.manager.GameOver();
            canMove = false;
        }
    }

    /// <summary>
    /// Función que se activa al pulsar los inputs en la pantalla.
    /// </summary>
    /// <param name="input">2 derecha, 4 izquierda, 5 levantar el dedo.</param>
    public void EnableInputs(int input)
    {
        switch (input)
        {
            case 2:
                h = 1;
                break;
            case 4:
                h = -1;
                break;
            case 5:
                h = 0;
                break;
        }
    }

    /// <summary>
    /// Función que reinicia las variables del globo.
    /// </summary>
    public void ResetValues()
    {
        transform.position = new Vector2(0, -2);
        canMove = true;
        h = 0;
        colorMode = false;
        anim.SetBool("ColorMode", false);
    }

    /// <summary>
    /// Corrutina que activa temporalmente el modo invulnerable.
    /// </summary>
    /// <returns></returns>
    IEnumerator ColorMode()
    {
        colorMode = true;
        GameManager12.manager.colorMode = true;

        anim.SetBool("ColorMode", true);

        yield return new WaitForSeconds(5);

        anim.SetBool("ColorMode", false);

        yield return new WaitForSeconds(0.5f);

        colorMode = false;
        GameManager12.manager.colorMode = false;
    }

    /// <summary>
    /// Corrutina que activa temporalmente el modo magnético.
    /// </summary>
    /// <returns></returns>
    IEnumerator MagnetMode()
    {
        StartMagnetMode(true);
        GameManager12.manager.magnetMode = true;

        yield return new WaitForSeconds(7);

        StartMagnetMode(false);
        GameManager12.manager.magnetMode = false;
    }
}