using UnityEngine;

/// <summary>
/// Clase con las funciones principales de los barriles laterales.
/// </summary>
public class RollingBarrel : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento del barril.
    /// </summary>
    [Header("Movement")]
    float speed = 2.5f;
    /// <summary>
    /// Vector de dirección del movimiento del barril.
    /// </summary>
    Vector2 direction = Vector2.right;

    /// <summary>
    /// Collider situado encima del barril para detectar cuando salta el jugador sobre este.
    /// </summary>
    [Header("Points")]
    [SerializeField] GameObject pointsCollider = null;

    /// <summary>
    /// Componente SpriteRenderer del barril.
    /// </summary>
    [Header("Components")]
    [SerializeField] SpriteRenderer sr;
    /// <summary>
    /// Componente Animator del barril.
    /// </summary>
    [SerializeField] Animator anim;
    /// <summary>
    /// Componente CapsuleCollider2D del barril.
    /// </summary>
    [SerializeField] CapsuleCollider2D capsuleCollider;

    private void OnEnable()
    {
        speed = 2.5f;
        direction = Vector2.right;
        
        capsuleCollider.isTrigger = false;
    }

    void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);
    }

    /// <summary>
    /// Función que gira el barril en función de la dirección de movimiento.
    /// </summary>
    void FlipBarrel()
    {
        speed *= -1;

        sr.flipX = !sr.flipX;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Si el barril choca con las paredes, hay una probabilidad del 40% de que se caiga.
        // En el 60% restante, el barril se gira y continua rodando.

        if (collision.gameObject.CompareTag("Game7/Walls"))
        {
            if (Random.value < 0.6f)
            {
                capsuleCollider.isTrigger = true;
            }

            else
            {
                FlipBarrel();
            }
        }

        // Si el barril choca con el bidón, hay un 50% de probabilidad de que salga una llama del bidón.

        else if (collision.gameObject.name == "Drum")
        {
            GameManager7.manager7.SpawnFlame(false);

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Si el barril es golpeado por el mazo, se destruye y aumenta la puntuación.

        if (collision.gameObject.CompareTag("Game7/MalletHit"))
        {
            GameManager7.manager7.DestroyBarrel(transform.position);

            gameObject.SetActive(false);
        }

        // Si el barril pasa sobre unas escaleras, hay una probabilidad del 50% de que baje a través de ellas.

        else if (collision.gameObject.name == "TopCollider")
        {
            float randomNumber = Random.value;

            if (randomNumber < 0.5f)
            {
                transform.position = new Vector2(collision.gameObject.transform.position.x, transform.position.y);

                anim.SetBool("InLadders", true);
                
                direction = Vector2.down;
                speed = 1.5f;

                pointsCollider.SetActive(false);
                capsuleCollider.isTrigger = true;
            }
        }

        // Cuando el barril llega a la parte inferior de las escaleras, continua rodando.

        else if (collision.gameObject.name == "BottomCollider")
        {
            if (direction == Vector2.down)
            {
                transform.position = new Vector2(transform.position.x, collision.gameObject.transform.position.y);

                anim.SetBool("InLadders", false);

                direction = Vector2.right;
                speed = 2.5f;

                pointsCollider.SetActive(true);
                capsuleCollider.isTrigger = false;

                if (transform.position.x > -0.1f)
                {
                    FlipBarrel();
                }
            }
        }

        // Si el barril cae por debajo del escenario, se destruye.

        else if (collision.gameObject.name == "DownLimit")
        {
            gameObject.SetActive(false);
        }
    }
}