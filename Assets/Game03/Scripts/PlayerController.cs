using UnityEngine;

/// <summary>
/// Clase con las funciones principales del jugador.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// Velocidad de movimiento de la nave del jugador.
    /// </summary>
    readonly float speed = 5;
    /// <summary>
    /// Si es verdadero, la nave no podrá moverse. Si es falso, podrá moverse.
    /// </summary>
    bool dontMove = true;
    /// <summary>
    /// Si es falso, la nave se moverá a la izquierda. Si es verdadero, se moverá a la derecha.
    /// La variable dontMove debe ser falsa.
    /// </summary>
    bool moveLeft = false;
    /// <summary>
    /// Límite de la pantalla por la derecha en el eje X.
    /// </summary>
    readonly float maxBound = 7;
    /// <summary>
    /// Límite de la pantalla por la izquierda en el eje X.
    /// </summary>
    readonly float minBound = -7;
    /// <summary>
    /// Componente AudioSource de la nave.
    /// </summary>
    [SerializeField] AudioSource shootAudio = null;
    /// <summary>
    /// Posición desde la que se generarán las balas.
    /// </summary>
    [SerializeField] Transform shootPoint = null;
    /// <summary>
    /// Cadencia de disparo de la nave.
    /// </summary>
    readonly float cadency = 1;
    /// <summary>
    /// Si es verdadero, la nave podrá disparar. Si es falso, no podrá.
    /// </summary>
    bool canShoot = false;
    /// <summary>
    /// Momento en el que podremos realizar el siguiente disparo.
    /// Su valor se actualiza cada vez que se realiza un disparo.
    /// </summary>
    float nextFire;

    private void OnEnable()
    {
        // Cuando la nave reaparezca, debe estar fuera de movimiento y sin disparar.

        DontAllowMovement();
        DontAllowShoot();
    }

    void Update()
    {
        HandleMoving();

        Shoot();

        if (Input.GetButtonDown("Cancel"))
        {
            GameManager3.manager3.PauseGame();
        }
    }

    /// <summary>
    /// Función que gestiona el movimiento de la nave del jugador.
    /// </summary>
    void HandleMoving()
    {
        if (dontMove)
        {
            StopMoving();
        }

        else
        {
            if (moveLeft)
            {
                MoveLeft();
            }

            else if (!moveLeft)
            {
                MoveRight();
            }
        }
    }

    /// <summary>
    /// Función que permite que el jugador pueda moverse.
    /// Está vinculada a los botones de las flechas en la pantalla.
    /// </summary>
    /// <param name="leftMovement">Verdadero si el movimiento es a la izquierda, falso si es a la derecha.</param>
    public void AllowMovement(bool leftMovement)
    {
        dontMove = false;
        moveLeft = leftMovement;
    }

    /// <summary>
    /// Función que hace que el jugador deje de moverse.
    /// Se activa cuando dejan de pulsarse las flechas de la pantalla.
    /// </summary>
    public void DontAllowMovement()
    {
        dontMove = true;
    }

    /// <summary>
    /// Función que mueve al jugador hacia la izquierda.
    /// </summary>
    void MoveLeft()
    {
        // Si el jugador alcanza el borde izquierdo de la pantalla, no podrá seguir moviéndose en esa dirección.

        if (transform.position.x >= minBound)
        {
            transform.Translate(Vector2.right * -speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Función que mueve al jugador hacia la derecha.
    /// </summary>
    void MoveRight()
    {
        // Si el jugador alcanza el borde derecho de la pantalla, no podrá seguir moviéndose en esa dirección.

        if (transform.position.x <= maxBound)
        {
            transform.Translate(Vector2.right * speed * Time.deltaTime);
        }
    }

    /// <summary>
    /// Función que hace que el jugador deje de moverse.
    /// </summary>
    void StopMoving()
    {
        transform.Translate(Vector2.right * 0 * Time.deltaTime);
    }

    /// <summary>
    /// Función que permite que el jugador pueda disparar.
    /// Se activa al pulsar el botón de disparo en la pantalla.
    /// </summary>
    public void AllowShoot()
    {
        canShoot = true;
    }

    /// <summary>
    /// Función que hace que el jugador deje de disparar.
    /// Se activa cuando se deja de pulsar el botón de disparo.
    /// </summary>
    public void DontAllowShoot()
    {
        canShoot = false;
    }

    /// <summary>
    /// Función que se activa cada vez que el jugador dispara.
    /// </summary>
    void Shoot()
    {
        // No se podrá realizar un nuevo disparo hasta que no pase un tiempo determinado (cadencia).

        if (canShoot && (Time.time > nextFire))
        {
            nextFire = Time.time + cadency;
            GameObject bullet = ObjectPooler.SharedInstance.GetPooledObject("Game3/BulletPlayer");
            if (bullet != null)
            {
                bullet.SetActive(true);
                bullet.transform.position = shootPoint.position;
                bullet.transform.rotation = Quaternion.identity;
            }
            shootAudio.Play();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Si somos alcanzados por una bala enemiga, la nave explotará y perderemos una vida.

        if (other.gameObject.CompareTag("Game3/BulletEnemy"))
        {
            other.gameObject.SetActive(false);
            GameManager3.manager3.LoseHealth(1);
        }
    }
}