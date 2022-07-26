using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que gestiona las funciones principales del Game 11.
/// </summary>
public class GameManager11 : MonoBehaviour
{
    public static GameManager11 manager;

    /// <summary>
    /// El coche del jugador.
    /// </summary>
    [Header("Player")]
    [SerializeField] GameObject player = null;

    /// <summary>
    /// El componente Timer11 del cronómetro.
    /// </summary>
    [Header("Timer")]
    [SerializeField] Timer11 timer = null;

    /// <summary>
    /// Panel con la puntuación del jugador.
    /// </summary>
    [Header("Score")]
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// Máxima puntuación del jugador.
    /// </summary>
    int[] highScore = new int[2];
    /// <summary>
    /// Panel con la máxima puntuación del jugador activo durante la partida.
    /// </summary>
    [SerializeField] Text highScoreText = null;
    /// <summary>
    /// Panel con la máxima puntuación del jugador activo en el menú.
    /// </summary>
    [SerializeField] Text highScoreTextMenu = null;

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
    /// Panel con el menú de ayuda.
    /// </summary>
    [SerializeField] GameObject panelHelp = null;
    /// <summary>
    /// Panel con los inputs del jugador.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;

    /// <summary>
    /// Sonido que se reproduce cuando el jugador cruza la meta.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource goalSound = null;

    private void Awake()
    {
        manager = this;

        LetterBoxer.AddLetterBoxingCamera();
    }

    void Start()
    {
        Time.timeScale = 1;
        LoadHighScore();
    }

    /// <summary>
    /// Función que inicia la partida.
    /// </summary>
    public void StartGame()
    {
        panelMenu.SetActive(false);
        panelControllers.SetActive(true);

        scoreText.enabled = true;
        highScoreText.enabled = true;

        player.SetActive(true);

        timer.enabled = true;
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score11;
        highScoreText.text = string.Format("{00}:{01}", highScore[0], highScore[1].ToString("00"));
        highScoreTextMenu.text = string.Format("High Score: {00}:{01}", highScore[0], highScore[1].ToString("00"));
    }

    /// <summary>
    /// Función que guarda la máxima puntuación en el dispositivo.
    /// </summary>
    public void SaveHighScore(int secs, int millisecs)
    {
        goalSound.Play();

        if (secs < highScore[0] ||
            (secs == highScore[0] && millisecs < highScore[1]) ||
            (highScore[0] == 0 && highScore[1] == 0))
        {
            SaveManager.saveManager.score11 = new int[2] { secs, millisecs };
            SaveManager.saveManager.SaveScores();

            LoadHighScore();
        }
    }

    /// <summary>
    /// Función que activa y desactiva el menú de pausa.
    /// </summary>
    public void PauseGame()
    {
        if (!panelPause.activeSelf)
        {
            Time.timeScale = 0;
            panelPause.SetActive(true);
            panelControllers.SetActive(false);
        }

        else if (panelPause.activeSelf)
        {
            Time.timeScale = 1;
            panelPause.SetActive(false);
            panelControllers.SetActive(true);
        }
    }

    /// <summary>
    /// Función que activa y desactiva el menú de ayuda.
    /// </summary>
    public void Help()
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
    /// Función que cambia de escena.
    /// </summary>
    /// <param name="buildIndex">Número de la escena que se va a cargar.</param>
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}