using UnityEngine;

/// <summary>
/// Clase encargada de mover y destruir los objetos en pantalla.
/// </summary>
public class MovingObjectsFrogger : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento del objeto.
    /// </summary>
    [SerializeField] float speed = 2.0f;
    /// <summary>
    /// Dirección de movimiento del objeto (-1 izquierda, 1 derecha).
    /// </summary>
    [SerializeField] int direction = 1;

    void Update()
    {
        transform.Translate(Vector2.right * direction * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto sale de los bordes de la pantalla, se destruirá.

        if (collision.gameObject.CompareTag("Game8/Destructor"))
        {
            gameObject.SetActive(false);
        }
    }
}