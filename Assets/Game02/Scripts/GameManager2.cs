using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que controla las funciones principales del Game 02.
/// </summary>
public class GameManager2 : MonoBehaviour
{
    public static GameManager2 manager;

    /// <summary>
    /// La pelota que se pasarán los dos jugadores.
    /// </summary>
    [Header("Ball")]
    [SerializeField] GameObject ball = null;
    /// <summary>
    /// La clase que contiene las funciones de movimiento de la pelota.
    /// </summary>
    [SerializeField] Ball ballScript = null;

    /// <summary>
    /// La pala del jugador.
    /// </summary>
    [Header("Player 1")]
    [SerializeField] GameObject player1 = null;
    /// <summary>
    /// La clase que contiene las funciones de movimiento de la pala del jugador.
    /// </summary>
    [SerializeField] Paddle paddle1 = null;

    /// <summary>
    /// La pala de la IA.
    /// </summary>
    [Header("Player AI")]
    [SerializeField] GameObject playerAI = null;
    /// <summary>
    /// La clase que contiene las funciones de movimiento de la pala de la IA.
    /// </summary>
    [SerializeField] ComputerAI paddleAI = null;

    /// <summary>
    /// Texto donde aparece la puntuación del jugador.
    /// </summary>
    [Header("Score")]
    [SerializeField] Text player1Text = null;
    /// <summary>
    /// Texto donde aparece la puntuación de la IA.
    /// </summary>
    [SerializeField] Text player2Text = null;
    /// <summary>
    /// Puntuación del jugador.
    /// </summary>
    int player1Score = 0;
    /// <summary>
    /// Puntuación de la IA.
    /// </summary>
    int player2Score = 0;
    /// <summary>
    /// Panel donde aparece la máxima puntuación.
    /// </summary>
    [SerializeField] Text highScoreText = null;
    /// <summary>
    /// Máxima puntuación obtenida por el jugador.
    /// </summary>
    int[] highScore = new int[2];

    /// <summary>
    /// Panel del menú principal.
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
    /// Panel con los botones de los inputs.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;

    private void Awake()
    {
        manager = this;

        LetterBoxer.AddLetterBoxingCamera();
    }

    private void Start()
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

        ball.SetActive(true);
        player1.SetActive(true);
        playerAI.SetActive(true);
    }

    /// <summary>
    /// Función que resetea las posiciones de las palas y la pelota.
    /// </summary>
    private void ResetPosition()
    {
        paddle1.ResetPosition();

        paddleAI.ResetPosition();

        ballScript.ResetPosition();
    }

    /// <summary>
    /// Función encargada de aumentar la puntuación.
    /// </summary>
    /// <param name="playerNumber">Jugador que ha puntuado (1 para el jugador 1, 2 para la IA).</param>
    public void UpdateScore(int playerNumber)
    {
        switch (playerNumber)
        {
            case 1: // Puntúa el jugador 1.
                player1Score += 1;
                player1Text.text = player1Score.ToString();
                break;
            case 2: // Puntúa la IA.
                player2Score += 1;
                player2Text.text = player2Score.ToString();
                break;
        }

        ResetPosition();
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    public void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score2;
        highScoreText.text = "HIGH SCORE: " + highScore[0].ToString() + " - " + highScore[1].ToString();
    }

    /// <summary>
    /// Función encargada de guardar la máxima puntuación en el dispositivo cuando se haya superado la anterior.
    /// </summary>
    public void SaveHighScore()
    {
        // He considerado como puntuación la diferencia de puntos entre el jugador y la IA.
        // Si la diferencia de puntos es superior a la anterior, se actualiza la máxima puntuación.
        // Por ejemplo, una puntuación de (5 - 1) será mejor que una de (10 - 8) ya que la diferencia de puntos es mayor.

        if ((player1Score - player2Score) > (highScore[0] - highScore[1]))
        {
            SaveManager.saveManager.score2 = new int[] { player1Score, player2Score };
            SaveManager.saveManager.SaveScores();
        }
    }

    /// <summary>
    /// Función encargada de pausar y reanudar la partida.
    /// </summary>
    public void PauseGame()
    {
        if (!panelPause.activeSelf) // Si la partida no está pausada, se activa la pausa.
        {
            panelPause.SetActive(true);
            panelControllers.SetActive(false);
            Time.timeScale = 0;
        }

        else if (panelPause.activeSelf) // Si la partida está pausada, se reanuda.
        {
            panelPause.SetActive(false);
            panelControllers.SetActive(true);
            Time.timeScale = 1;
        }
    }

    /// <summary>
    /// Función encargada de activar y desactivar el panel de ayuda.
    /// </summary>
    public void Help()
    {
        if (!panelHelp.activeSelf) // Si el panel está desactivado, se activa.
        {
            panelHelp.SetActive(true);
        }

        else // Si el panel está activado, se desactiva.
        {
            panelHelp.SetActive(false);
        }
    }

    /// <summary>
    /// Función encargada de cambiar de escena.
    /// </summary>
    /// <param name="buildIndex">Número de la escena que se quiere cargar.</param>
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }
}