using UnityEngine;

/// <summary>
/// Clase que automatiza el movimiento del suelo.
/// </summary>
public class FloorMovement : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento del suelo.
    /// </summary>
    float speed = 0;
    /// <summary>
    /// La posición del final del suelo en el eje X.
    /// </summary>
    [SerializeField] float end = 0;
    /// <summary>
    /// La posición del principio del suelo en el eje X.
    /// </summary>
    [SerializeField] float begin = 0;

    private void OnEnable()
    {
        GameManager9.ChangeSpeed += ChangeSpeed;
        GameManager9.StopMovement += ChangeSpeed;
    }

    private void OnDisable()
    {
        GameManager9.ChangeSpeed -= ChangeSpeed;
        GameManager9.StopMovement -= ChangeSpeed;
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        
        if (transform.position.x <= end)
        {
            Vector2 startPosition = new Vector2(begin, transform.position.y);
            transform.position = startPosition;
        }
    }

    /// <summary>
    /// Función que se activa a través del delegado para cambiar la velocidad del suelo.
    /// </summary>
    /// <param name="newSpeed">La velocidad que queremos activar.</param>
    void ChangeSpeed(float newSpeed)
    {
        speed = newSpeed;
    }
}