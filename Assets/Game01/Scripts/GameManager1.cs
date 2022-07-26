using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Clase que controla las funciones principales del Game 01.
/// </summary>
public class GameManager1 : MonoBehaviour
{
    #region Variables
    public static GameManager1 manager;

    /// <summary>
    /// Puntuación del jugador.
    /// </summary>
    [Header("Score")]
    int score;
    /// <summary>
    /// Cartel con la puntuación del jugador.
    /// </summary>
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// Puntuación más alta del jugador.
    /// </summary>
    int highScore = 0;
    /// <summary>
    /// Cartel con la puntuación más alta.
    /// </summary>
    [SerializeField] Text highScoreText = null;

    /// <summary>
    /// Panel con el menú principal del juego.
    /// </summary>
    [Header("Panels")]
    [SerializeField] GameObject panelMenu = null;
    /// <summary>
    /// Panel con la pantalla de Game Over.
    /// </summary>
    [SerializeField] GameObject panelGameOver = null;
    /// <summary>
    /// Panel con la pantalla de Pausa.
    /// </summary>
    [SerializeField] GameObject panelPause = null;
    /// <summary>
    /// Panel con la pantalla de ayuda.
    /// </summary>
    [SerializeField] GameObject panelHelp = null;
    /// <summary>
    /// Panel con los botones para controlar al personaje.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;

    /// <summary>
    /// Jugador 1.
    /// </summary>
    [Header("Player")]
    [SerializeField] GameObject player1 = null;

    /// <summary>
    /// Generadores activos durante el juego, tanto de monedas como de enemigos.
    /// </summary>
    [Header("Spawns")]
    [SerializeField] GameObject[] generators = null;

    /// <summary>
    /// AudioSource que reproduce el sonido de las monedas cada vez que se consigue alguna.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource coindSound = null;
    #endregion

    void Awake()
    {
        manager = this;

        // Aplicamos el Letterboxing para ajustar la pantalla.

        LetterBoxer.AddLetterBoxingCamera();
    }

    private void Start()
    {
        // Al iniciar la escena debemos poner el tiempo a 1 por si previamente estaba detenido (juego pausado).
        // También debemos actualizar la máxima puntuación en la pantalla.

        Time.timeScale = 1;
        LoadHighScore();
    }

    /// <summary>
    /// Función que inicia una nueva partida.
    /// </summary>
    public void StartGame()
    {
        // Limpiamos todos los enemigos y monedas que pueda haber en la pantalla antes de jugar de nuevo.

        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Game1/Enemy");
        if (enemies != null)
        {
            for (int i = 0; i < enemies.Length; i++)
            {
                enemies[i].SetActive(false);
            }
        }

        GameObject[] coins = GameObject.FindGameObjectsWithTag("Game1/Coin");
        if (coins != null)
        {
            for (int i = 0; i < coins.Length; i++)
            {
                coins[i].SetActive(false);
            }
        }

        GameObject[] missiles = GameObject.FindGameObjectsWithTag("Game1/Missile");
        if (missiles != null)
        {
            for (int i = 0; i < missiles.Length; i++)
            {
                missiles[i].SetActive(false);
            }
        }

        // Desactivamos todos los paneles que no sean necesarios (menú, pausa y Game Over) y activamos el panel con los botones.

        panelMenu.SetActive(false);
        panelPause.SetActive(false);
        panelGameOver.SetActive(false);
        panelControllers.SetActive(true);

        // Activamos al jugador 1 y los generadores (monedas y enemigos).

        player1.SetActive(true);

        for (int i = 0; i < generators.Length; i++)
        {
            generators[i].SetActive(true);
        }

        // Ponemos la puntuación a 0.

        score = 0;
        scoreText.text = "SCORE: 0";
    }

    /// <summary>
    /// Función que activa el Game Over.
    /// </summary>
    public void GameOver()
    {
        panelControllers.SetActive(false);
        panelGameOver.SetActive(true);

        // Desactivamos los generadores para que no sigan funcionando mientras el juego está parado.

        for (int i = 0; i < generators.Length; i++)
        {
            generators[i].SetActive(false);
        }

        // Si hemos superado la puntuación más alta, la actualizamos.

        SaveHighScore();
    }

    /// <summary>
    /// Función que activa y desactiva el menú de pausa.
    /// </summary>
    public void PauseGame()
    {
        // Si no está activa la pausa, la activamos.

        if (!panelPause.activeSelf)
        {
            panelPause.SetActive(true);
            panelControllers.SetActive(false);
            Time.timeScale = 0;
        }

        // Si está activa la pausa, la desactivamos.

        else if (panelPause.activeSelf)
        {
            panelPause.SetActive(false);
            panelControllers.SetActive(true);
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Función que actualiza la puntuación cada vez que cogemos una moneda.
    /// </summary>
    public void UpdateScore()
    {
        // La puntuación aumenta en un punto y se actualiza el cartel con la nueva puntuación.

        score += 1;
        scoreText.text = "SCORE: " + score.ToString();

        // Reproducimos el sonido que indica que hemos cogido la moneda.

        coindSound.Play();
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    public void LoadHighScore()
    {
        // Se coge la puntuación del SaveManager y se actualiza el cartel con la puntuación.

        highScore = SaveManager.saveManager.score1;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que guarda la máxima puntuación en el dispositivo para después poder recuperarla.
    /// </summary>
    public void SaveHighScore()
    {
        // Si la puntuación obtenida es inferior a la puntuación máxima, no será necesario continuar con la función.

        if ((score) > (highScore))
        {
            SaveManager.saveManager.score1 = score;
            SaveManager.saveManager.SaveScores();
        }
    }

    /// <summary>
    /// Función que activa y desactiva el panel de ayuda.
    /// </summary>
    public void Help()
    {
        // Si el panel de ayuda está desactivado, lo activamos.

        if (!panelHelp.activeSelf)
        {
            panelHelp.SetActive(true);
        }

        // Si está acticado, lo desactivamos.

        else
        {
            panelHelp.SetActive(false);
        }
    }

    /// <summary>
    /// Función que cambia la escena del juego.
    /// </summary>
    /// <param name="buildIndex">Escena que queremos cargar.</param>
    public void LoadGame(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}
