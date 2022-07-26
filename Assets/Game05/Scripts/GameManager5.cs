using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que controla las funciones principales del Game 05.
/// </summary>
public class GameManager5 : MonoBehaviour
{
    #region Variables
    public static GameManager5 manager5;

    /// <summary>
    /// La cabeza de la serpiente.
    /// </summary>
    [Header("Player")]
    [SerializeField] GameObject head = null;
    /// <summary>
    /// Componente SnakeMovement de la cabeza de la serpiente.
    /// </summary>
    [SerializeField] SnakeMovement snakeMovement;

    /// <summary>
    /// Prefab de la comida.
    /// </summary>
    [Header("Food")]
    [SerializeField] GameObject food = null;
    /// <summary>
    /// Prefab de la comida roja.
    /// </summary>
    [SerializeField] GameObject redFood = null;
    /// <summary>
    /// AudioSource con el sonido que indica la aparición de una nueva comida roja.
    /// </summary>
    [SerializeField] AudioSource newRedFoodSound = null;
    /// <summary>
    /// Capa asignada a la cabeza de la serpiente y los diferentes bloques de la cola.
    /// </summary>
    [SerializeField] LayerMask snakeMask = 0;

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
    /// Máxima puntuación conseguida por el jugador.
    /// </summary>
    int highScore = 0;
    /// <summary>
    /// Panel con la máxima puntuación del jugador.
    /// </summary>
    [SerializeField] Text highScoreText = null;
    /// <summary>
    /// Panel con la máxima puntuación del jugador visible durante la partida.
    /// </summary>
    [SerializeField] Text highScoreText2 = null;

    /// <summary>
    /// Posición del borde superior del escenario.
    /// </summary>
    [Header("Borders")]
    [SerializeField] Transform borderTop = null;
    /// <summary>
    /// Posición del borde inferior del escenario.
    /// </summary>
    [SerializeField] Transform borderBottom = null;
    /// <summary>
    /// Posición del borde izquierdo del escenario.
    /// </summary>
    [SerializeField] Transform borderLeft = null;
    /// <summary>
    /// Posición del borde derecho del escenario.
    /// </summary>
    [SerializeField] Transform borderRight = null;

    /// <summary>
    /// Panel con el menú principal.
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
    /// Panel con el menú de ayuda.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;
    #endregion

    private void Awake()
    {
        manager5 = this;

        LetterBoxer.AddLetterBoxingCamera();
    }

    void Start()
    {
        Time.timeScale = 1;
        LoadHighScore();
    }

    /// <summary>
    /// Función que inicia una nueva partida.
    /// </summary>
    public void StartGame()
    {
        score = 0;

        GameObject[] activeFood = GameObject.FindGameObjectsWithTag("Game5/Food");

        if (activeFood != null)
        {
            for (int i = 0; i < activeFood.Length; i++)
            {
                Destroy(activeFood[i]);
            }
        }

        GameObject[] activeRedFood = GameObject.FindGameObjectsWithTag("Game5/RedFood");

        if (activeRedFood != null)
        {
            for (int i = 0; i < activeRedFood.Length; i++)
            {
                Destroy(activeRedFood[i]);
            }
        }

        panelControllers.SetActive(true);
        panelMenu.SetActive(false);
        panelGameOver.SetActive(false);

        head.transform.position = Vector2.zero;
        head.SetActive(true);
        snakeMovement.enabled = true;

        Spawn();
        SpawnRed();
    }

    /// <summary>
    /// Función que busca una posición aleatoria del escenario que esté vacía.
    /// </summary>
    /// <returns>El vector de posición vacío donde se va a instanciar la comida.</returns>
    Vector2 SpawnVector()
    {
        int x = (int)Random.Range(borderLeft.position.x + 1, borderRight.position.x - 1);

        int y = (int)Random.Range(borderBottom.position.y + 1, borderTop.position.y - 1);

        if (!Physics2D.OverlapCircle(new Vector2(x, y), 0.5f, snakeMask))
        {
            // Si la posición está vacía, nos quedamos con el vector de posición aleatorio obtenido.

            return new Vector2(x, y);
        }

        else
        {
            // Si la posición no está vacía, volvemos a iniciar la función.

            return SpawnVector();
        }
    }

    /// <summary>
    /// Función que genera una nueva pieza de comida.
    /// </summary>
    public void Spawn()
    {
        Instantiate(food, SpawnVector(), Quaternion.identity);
    }

    /// <summary>
    /// Function que genera una nueva pieza de comida roja.
    /// </summary>
    public void SpawnRed()
    {
        StopAllCoroutines();

        // La comida roja desaparecerá automáticamente si no la cogemos antes de unos segundos.

        StartCoroutine(SpawnRedFood());
    }

    /// <summary>
    /// Función que aumenta la puntuación.
    /// </summary>
    /// <param name="scoreValue">La cantidad en que aumenta la puntuación.</param>
    public void UpdateScore(int scoreValue)
    {
        score += scoreValue;
        scoreText.text = "SCORE: " + score.ToString();
    }

    /// <summary>
    /// Función que activa la pantalla de Game Over.
    /// </summary>
    public void GameOver()
    {
        snakeMovement.enabled = false;
        panelGameOver.SetActive(true);
        panelControllers.SetActive(false);
        StopAllCoroutines();
        SaveHighScore();
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score5;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
        highScoreText2.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que guarda en el dispositivo la máxima puntuación obtenida.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score5 = score;
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
    /// Función que abre y cierra el menú de ayuda.
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

    /// <summary>
    /// Corrutina que inicia una cuenta atrás antes de generar una nueva pieza de comida roja.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnRedFood()
    {
        yield return new WaitForSeconds(Random.Range(20, 40));

        GameObject activeRedFood = Instantiate(redFood, SpawnVector(), Quaternion.identity);

        newRedFoodSound.Play();

        StartCoroutine(DestroyRedFood(activeRedFood));
    }

    /// <summary>
    /// Corrutina que inicia una cuenta atrás para hacer desaparecer una pieza de comida roja.
    /// </summary>
    /// <param name="activeRedFood">La pieza de comida roja que va a ser destruida.</param>
    /// <returns></returns>
    IEnumerator DestroyRedFood(GameObject activeRedFood)
    {
        yield return new WaitForSeconds(20);

        if (activeRedFood != null)
        {
            Destroy(activeRedFood);
        }

        SpawnRed();
    }
}