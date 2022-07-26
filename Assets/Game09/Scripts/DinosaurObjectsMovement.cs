using UnityEngine;

/// <summary>
/// Clase que automatiza el movimiento de los objetos en el escenario.
/// </summary>
public class DinosaurObjectsMovement : MonoBehaviour
{
    /// <summary>
    /// Será positivo si la velocidad del objeto es variable. Falso si no es el caso.
    /// </summary>
    [SerializeField] bool variableSpeed = false;
    /// <summary>
    /// La velocidad inicial del objeto.
    /// </summary>
    [SerializeField] float speed = 1.5f;

    private void OnEnable()
    {
        if (variableSpeed)
        {
            speed = GameManager9.manager.speed;

            GameManager9.ChangeSpeed += ChangeSpeed;
        }

        GameManager9.StopMovement += ChangeSpeed;
    }

    private void OnDisable()
    {
        if (variableSpeed)
        {
            GameManager9.ChangeSpeed -= ChangeSpeed;
        }

        GameManager9.StopMovement -= ChangeSpeed;
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el objeto sale del escenario, se destruye.

        if (collision.gameObject.CompareTag("Game9/Destructor"))
        {
            gameObject.SetActive(false);
        }

        // Si el dinosaurio tropieza con el objeto, termina la partida.

        else if (collision.gameObject.CompareTag("Player"))
        {
            if (variableSpeed) // Solo los objetos de velocidad variable (cactus y pájaros).
            {
                GameManager9.manager.GameOver();
            }
        }
    }

    /// <summary>
    /// Función que se activa a través del delegado para cambiar la velocidad.
    /// </summary>
    /// <param name="newSpeed">La velocidad que queremos activar.</param>
    void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}