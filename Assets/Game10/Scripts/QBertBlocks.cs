using UnityEngine;

/// <summary>
/// Clase con las funciones principales de los bloques de la pirámide.
/// </summary>
public class QBertBlocks : MonoBehaviour
{
    /// <summary>
    /// Componente AudioSource del bloque.
    /// </summary>
    [SerializeField] AudioSource sound = null;
    /// <summary>
    /// Componente SpriteRenderer del bloque.
    /// </summary>
    [SerializeField] SpriteRenderer sr = null;
    /// <summary>
    /// Componente Animator del bloque.
    /// </summary>
    [SerializeField] Animator anim = null;
    /// <summary>
    /// Sprite azul del bloque.
    /// </summary>
    [SerializeField] Sprite blueSprite = null;
    /// <summary>
    /// Sprite rosa del bloque.
    /// </summary>
    [SerializeField] Sprite pinkSprite = null;
    /// <summary>
    /// Sprite amarillo del bloque.
    /// </summary>
    [SerializeField] Sprite yellowSprite = null;

    /// <summary>
    /// Cada uno de los posibles estados del bloque (0, 1 o 2).
    /// </summary>
    int state = 0;
    /// <summary>
    /// Positivo si están activos los colliders del bloque. Falso si no lo están.
    /// </summary>
    bool canChange = true;

    /// <summary>
    /// Función que activa y desactiva los colliders del bloque.
    /// </summary>
    /// <param name="enable">Verdadero si se activan, falso si se desactivan.</param>
    public void EnableOrDisable(bool enable)
    {
        canChange = enable;
    }

    /// <summary>
    /// Función que activa o desactiva la animación de los bloques.
    /// </summary>
    /// <param name="enable">Verdadero si la activa, falso si la desactiva.</param>
    public void EnableAnimator(bool enable)
    {
        anim.enabled = enable;
    }

    /// <summary>
    /// Función que resetea las variables del bloque, devolviéndolo a su estado inicial.
    /// </summary>
    public void ResetSprite()
    {
        state = 0;
        sr.sprite = blueSprite;
        canChange = true;
    }

    /// <summary>
    /// Función que cambia el color de los bloques.
    /// </summary>
    void ChangeColour()
    {
        if (state == 1) // Si el bloque era azul, pasa a ser rosa.
        {
            sr.sprite = pinkSprite;
        }

        else if (state == 2) // Si el bloque era rosa, pasa a ser amarillo.
        {
            sr.sprite = yellowSprite;

            GameManager10.manager.ReduceBlocks();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el bloque entra en contacto con el jugador o la bola verde, cambia de color.

        if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Game10/GreenBall"))
        {
            if (!canChange)
            {
                return;
            }

            sound.Play();

            if (state == 2)
            {
                return;
            }

            GameManager10.manager.UpdateScore(5);

            state += 1;

            ChangeColour();
        }
    }
}