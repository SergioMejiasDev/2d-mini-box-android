using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que controla las funciones principales de Game 06.
/// </summary>
public class GameManager6 : MonoBehaviour
{
    public static GameManager6 manager6;

    /// <summary>
    /// La anchura del campo de juego.
    /// </summary>
    public static int width = 10;
    /// <summary>
    /// La altura del campo de juego.
    /// </summary>
    public static int height = 20;
    /// <summary>
    /// Matriz con todas las posibles posiciones que pueden ocupar los cuadros de los tetrominós.
    /// </summary>
    public static Transform[,] grid = new Transform[width, height];

    /// <summary>
    /// Componente TetrominoSpawner del generador de piezas.
    /// </summary>
    [SerializeField] TetrominoSpawner spawner = null;

    /// <summary>
    /// Puntuación del jugador.
    /// </summary>
    [Header("Score")]
    int score = 0;
    /// <summary>
    /// Panel con la puntuación del jugador.
    /// </summary>
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// Máxima puntuación del jugador.
    /// </summary>
    int highScore = 0;
    /// <summary>
    /// Panel con la máxima puntuación del jugador.
    /// </summary>
    [SerializeField] Text highScoreText = null;

    /// <summary>
    /// Sonido que se reproducirá cada vez que se complete una fila.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource deleteRowSound = null;
    /// <summary>
    /// Sonido que se reproducirá cuando haya un Game Over.
    /// </summary>
    [SerializeField] AudioSource gameOverSound = null;

    /// <summary>
    /// Panel con el menú principal del juego.
    /// </summary>
    [Header("Panels")]
    [SerializeField] GameObject panelMenu = null;
    /// <summary>
    /// Panel con el menú de pausa.
    /// </summary>
    [SerializeField] GameObject panelPause = null;
    /// <summary>
    /// Panel con el menú de Game Over.
    /// </summary>
    [SerializeField] GameObject panelGameOver = null;
    /// <summary>
    /// Panel con el menú de ayuda.
    /// </summary>
    [SerializeField] GameObject panelHelp = null;
    /// <summary>
    /// Panel con los botones de los inputs.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;


    #region Booleans
    /// <summary>
    /// Función que redondea los valores de un vector a los valores enteros más cercanos.
    /// </summary>
    /// <param name="v">El vector que se va a redondear.</param>
    /// <returns>El vector con los valores redondeados a partir del vector original.</returns>
    public Vector2 RoundVec2(Vector2 v)
    {
        return new Vector2(Mathf.Round(v.x), Mathf.Round(v.y));
    }

    /// <summary>
    /// Función que comprueba si el vector introducido está dentro de la cuadrícula de juego.
    /// </summary>
    /// <param name="position">El vector que queremos comprobar si se encuentra en la cuadrícula.</param>
    /// <returns>Verdadero si la posición introducida está en la cuadrícula, falso si no lo está.</returns>
    public bool InsideBorder(Vector2 position)
    {
        return position.x >= 0 && position.x < width && position.y >= 0;
    }

    /// <summary>
    /// Función que comprueba si una fila está completa.
    /// </summary>
    /// <param name="y">La posición en el eje Y de la fila que queremos comprobar.</param>
    /// <returns>Verdadero si la fila está completa, falso si no lo está.</returns>
    public bool IsRowFull(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }
    #endregion

    void Awake()
    {
        manager6 = this;

        LetterBoxer.AddLetterBoxingCamera();
    }

    private void Start()
    {
        Time.timeScale = 1;
        LoadHighScore();
    }

    /// <summary>
    /// Función que inicia una nueva partida.
    /// </summary>
    public void StartGame()
    {
        panelMenu.SetActive(false);
        panelGameOver.SetActive(false);
        panelControllers.SetActive(true);

        score = 0;
        scoreText.text = "SCORE: " + score.ToString();

        spawner.enabled = true;
        spawner.Spawn();
    }

    /// <summary>
    /// Función que elimina una fila al completo.
    /// </summary>
    /// <param name="y">La posición en el eje Y de la fila que se va a eliminar.</param>
    public void DeleteRow(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            Destroy(grid[x, y].gameObject);
            grid[x, y] = null;
        }
    }

    /// <summary>
    /// Función que hace que una fila completa descienda un nivel.
    /// </summary>
    /// <param name="y">La posición en el eje Y de la fila que va a descender.</param>
    public void DecreaseRow(int y)
    {
        for (int x = 0; x < width; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;

                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    /// <summary>
    /// Función que hace que todas las filas superiores a la actual desciendan un nivel.
    /// </summary>
    /// <param name="y">La posición en el eje Y de la fila a tomar como referencia.</param>
    public void DecreaseRowsAbove(int y)
    {
        for (int i = y; i < height; ++i)
        {
            DecreaseRow(i);
        }
    }

    /// <summary>
    /// Función que comprueba si existe alguna fila completa y, si es el caso, la elimina.
    /// </summary>
    public void DeleteFullRows()
    {
        for (int y = 0; y < height; ++y)
        {
            if (IsRowFull(y))
            {
                deleteRowSound.Play();

                UpdateScore(50);
                DeleteRow(y);
                DecreaseRowsAbove(y + 1);
                --y;
            }
        }
    }

    /// <summary>
    /// Función que aumenta la puntuación.
    /// </summary>
    /// <param name="scoreIncrease">La cantidad en que aumenta la puntuación.</param>
    public void UpdateScore(int scoreIncrease)
    {
        score += scoreIncrease;
        scoreText.text = "SCORE: " + score.ToString();
    }

    /// <summary>
    /// Función que se activa cuando se pierde la partida.
    /// </summary>
    public void GameOver()
    {
        gameOverSound.Play();
        spawner.enabled = false;
        panelGameOver.SetActive(true);
        panelControllers.SetActive(false);

        GameObject[] activeTetrominos = GameObject.FindGameObjectsWithTag("Game6/Tetromino");

        if (activeTetrominos != null)
        {
            for (int i = 0; i < activeTetrominos.Length; i++)
            {
                Destroy(activeTetrominos[i]);
            }
        }

        SaveHighScore();
    }

    /// <summary>
    /// Función que carga la puntuación más alta guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score6;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que guarda la máxima puntuación del jugador en el dispositivo.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score6 = score;
            SaveManager.saveManager.SaveScores();
        }
    }

    /// <summary>
    /// Función que abre y cierra el menú de pausa.
    /// </summary>
    public void PauseGame()
    {
        if (!panelPause.activeSelf)
        {
            panelPause.SetActive(true);
            panelControllers.SetActive(false);
            Time.timeScale = 0;
        }

        else
        {
            panelPause.SetActive(false);
            panelControllers.SetActive(true);
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Función que abre y cierra el penú de ayuda.
    /// </summary>
    public void OpenHelp()
    {
        if (!panelHelp.activeSelf)
        {
            panelHelp.SetActive(true);
        }

        else
        {
            panelHelp.SetActive(false);
        }
    }

    /// <summary>
    /// Función que carga una nueva escena.
    /// </summary>
    /// <param name="buildIndex">La escena que se va a cargar.</param>
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}