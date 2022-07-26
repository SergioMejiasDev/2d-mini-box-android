using UnityEngine;

/// <summary>
/// Clase que controla el movimiento de las piezas del juego (tetrominós).
/// </summary>
public class Tetromino : MonoBehaviour
{
    /// <summary>
    /// La letra correspondiente al tetrominó.
    /// </summary>
    [SerializeField] string letter;
    /// <summary>
    /// Cada una de las 4 posiciones que puede alcanzar cada pieza como máximo a partir de su rotación.
    /// </summary>
    int rotationMode = 1;
    /// <summary>
    /// Hace referencia al segundo en el que la pieza activa ha bajado por última vez.
    /// </summary>
    float lastFall = 0;

    /// <summary>
    /// Sonido que se reproduce cuando una pieza tropieza con el suelo o con otra pieza.
    /// </summary>
    [SerializeField] AudioSource fallingSound;

    private void OnEnable()
    {
        MobileInputs6.Button += ActivateInputs;
    }

    private void OnDisable()
    {
        MobileInputs6.Button -= ActivateInputs;
    }

    void Start()
    {
        if (!IsValidPosition())
        {
            GameManager6.manager6.GameOver();
            Destroy(gameObject);
        }
    }

    private void Update()
    {
        if (Time.time - lastFall >= 1)
        {
            ActivateInputs(3);
        }
    }

    /// <summary>
    /// Función que indica si la pieza pasa a formar parte de la rejilla tras moverse.
    /// Esto significa que la pieza a tropezado por abajo, ya sea con otra pieza o con el suelo.
    /// </summary>
    /// <returns>Verdadero si la pieza está en la rejilla, falso si no lo está.</returns>
    bool IsValidPosition()
    {
        foreach (Transform child in transform)
        {
            Vector2 v = GameManager6.manager6.RoundVec2(child.position);

            if (!GameManager6.manager6.InsideBorder(v))
            {
                return false;
            }

            if (GameManager6.grid[(int)v.x, (int)v.y] != null && GameManager6.grid[(int)v.x, (int)v.y].parent != transform)
            {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Función que se activa cuando el jugador pulsa los inputs en la pantalla.
    /// </summary>
    /// <param name="direction">1 arriba, 2 derecha, 3 abajo, 4 izquierda.</param>
    void ActivateInputs(int direction)
    {
        if (direction == 4) // La pieza se mueve a la izquierda.
        {
            transform.position += new Vector3(-1, 0, 0);

            if (IsValidPosition())
            {
                UpdateGrid();
            }

            else
            {
                transform.position += new Vector3(1, 0, 0);
            }
        }

        else if (direction == 2) // La pieza se mueve a la derecha.
        {
            transform.position += new Vector3(1, 0, 0);

            if (IsValidPosition())
            {
                UpdateGrid();
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
            }
        }

        else if (direction == 1) // La pieza rota hacia la derecha.
        {
            if ((letter == "I") || (letter == "S") || (letter == "Z"))
            {
                // Los tetrominós "I", "S" y "Z" solo tienen dos posiciones.
                // Esto se debe a que al girar 180º son simétricos.

                if (rotationMode == 1)
                {
                    transform.Rotate(0, 0, -90);

                    if (IsValidPosition())
                    {
                        UpdateGrid();
                    }

                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }

                    rotationMode = 2;
                }

                else if (rotationMode == 2)
                {
                    transform.Rotate(0, 0, 90);

                    if (IsValidPosition())
                    {
                        UpdateGrid();
                    }

                    else
                    {
                        transform.Rotate(0, 0, -90);
                    }

                    rotationMode = 1;
                }
            }

            else if (letter == "L" || letter == "J" || letter == "T")
            {
                // Los tetrominós "L", "J" y "T" tienen cuatro posiciones.
                // Esto se debe a que al girar 180º no se consiguen posiciones simétricas.

                if ((rotationMode == 1) || (rotationMode == 2))
                {
                    transform.Rotate(0, 0, -90);

                    if (letter == "T")
                    {
                        transform.localScale = new Vector2(1, 1);
                    }

                    if (IsValidPosition())
                    {
                        UpdateGrid();
                    }

                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }

                    switch (rotationMode)
                    {
                        case 1:
                            rotationMode = 2;
                            break;
                        case 2:
                            rotationMode = 3;
                            break;
                    }
                }

                else if ((rotationMode == 3) || (rotationMode == 4))
                {
                    transform.Rotate(0, 0, -90);

                    if (letter == "T")
                    {
                        transform.localScale = new Vector2(-1, 1);
                    }

                    if (IsValidPosition())
                    {
                        UpdateGrid();
                    }

                    else
                    {
                        transform.Rotate(0, 0, 90);
                    }

                    switch (rotationMode)
                    {
                        case 3:
                            rotationMode = 4;
                            break;
                        case 4:
                            rotationMode = 1;
                            break;
                    }
                }
            }
        }

        else if (direction == 3) // La pieza desciende un nivel hacia abajo.
        {
            transform.position += new Vector3(0, -1, 0);

            if (IsValidPosition())
            {
                UpdateGrid();
            }

            else
            {
                transform.position += new Vector3(0, 1, 0);

                fallingSound.Play();

                GameManager6.manager6.UpdateScore(1);

                GameManager6.manager6.DeleteFullRows();

                GameObject.FindGameObjectWithTag("Game6/Spawner").GetComponent<TetrominoSpawner>().Spawn();

                enabled = false;
            }

            lastFall = Time.time;
        }
    }

    /// <summary>
    /// Función que actualiza la rejilla con los datos de la pieza actual.
    /// </summary>
    void UpdateGrid()
    {
        for (int y = 0; y < GameManager6.height; ++y)
        {
            for (int x = 0; x < GameManager6.width; ++x)
            {
                if (GameManager6.grid[x, y] != null)
                {
                    if (GameManager6.grid[x, y].parent == transform)
                    {
                        GameManager6.grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform child in transform)
        {
            Vector2 v = GameManager6.manager6.RoundVec2(child.position);
            GameManager6.grid[(int)v.x, (int)v.y] = child;
        }
    }
}