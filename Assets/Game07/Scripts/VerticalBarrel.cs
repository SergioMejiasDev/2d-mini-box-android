using UnityEngine;

/// <summary>
/// Clase con las funciones principales de los barriles que caen hacia abajo.
/// </summary>
public class VerticalBarrel : MonoBehaviour
{
    /// <summary>
    /// Velocidad a la que caen los barriles.
    /// </summary>
    float speed = 2.5f;

    void Update()
    {
        transform.Translate(Vector2.down * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el barril cae sobre un bidón, se destruirá y aparecerá una llama.

        if (collision.gameObject.name == "Drum")
        {
            GameManager7.manager7.SpawnFlame(true);
            Destroy(gameObject);
        }
    }
}