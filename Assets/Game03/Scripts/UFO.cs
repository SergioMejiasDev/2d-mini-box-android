using UnityEngine;

/// <summary>
/// Clase con las funciones principales del OVNI rojo.
/// </summary>
public class UFO : MonoBehaviour
{
    /// <summary>
    /// Puntuación que obtendremos al destruir el OVNI.
    /// Su valor será aleatorio.
    /// </summary>
    int score;

    private void Start()
    {
        switch (Random.Range(0, 3))
        {
            case 0:
                score = 50;
                break;
            case 1:
                score = 100;
                break;
            case 2:
                score = 150;
                break;
        }
    }

    private void Update()
    {
        transform.Translate(Vector2.left * 2.5f * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el OVNI es alcanzado por una bala del jugador, explotará y aumentará la puntuación.

        if (collision.gameObject.CompareTag("Game3/BulletPlayer"))
        {
            collision.gameObject.SetActive(false);
            GameManager3.manager3.UFODeath(score, transform.position);
            Destroy(gameObject);
        }

        // Si alcanza el límite de la pantalla, desaparecerá.

        else if (collision.gameObject.CompareTag("Game3/Limits"))
        {
            Destroy(gameObject);
        }
    }
}