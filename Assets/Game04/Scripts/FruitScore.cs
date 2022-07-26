using UnityEngine;

/// <summary>
/// Clase que hace que aumente la puntación al coger una pieza de fruta.
/// </summary>
public class FruitScore : MonoBehaviour
{
    /// <summary>
    /// Puntuación de la pieza de fruta.
    /// Será diferente para cada tipo de fruta.
    /// </summary>
    [SerializeField] int score = 0;
    /// <summary>
    /// Imagen con la puntuación que aparecerá cada vez que cojamos la fruta.
    /// </summary>
    [SerializeField] GameObject scoreImage = null;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Cuando cojamos una pieza de fruta, esta desaparecerá.
        // En su lugar aparecerá la puntuación obtenida durante unos segundos.
        // Se aumentará la puntuación del jugador.

        if (collision.gameObject.CompareTag("Player"))
        {
            GameManager4.manager4.FruitSound();
            GameManager4.manager4.UpdateScore(score);
            GameManager4.manager4.GenerateFruit();
            Destroy(Instantiate(scoreImage, transform.position, Quaternion.identity), 1);
            Destroy(gameObject);
        }
    }
}