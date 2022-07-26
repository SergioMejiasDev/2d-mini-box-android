using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que controla las funciones principales del Game 08.
/// </summary>
public class GameManager8 : MonoBehaviour
{
    #region Variables

    public static GameManager8 manager8;

    /// <summary>
    /// La rana del jugador.
    /// </summary>
    [Header("Player")]
    [SerializeField] GameObject player = null;
    /// <summary>
    /// El componente FrogMovement de la rana.
    /// </summary>
    [SerializeField] FrogMovement playerClass = null;
    /// <summary>
    /// Las vidas restantes del jugador.
    /// </summary>
    int remainingLifes;
    /// <summary>
    /// Los iconos de las vidas restantes.
    /// </summary>
    [SerializeField] Image[] lifes = null;

    /// <summary>
    /// El generador de los objetos de la escena.
    /// </summary>
    [Header("Spawner")]
    [SerializeField] FroggerSpawners spawner = null;

    /// <summary>
    /// Las ranas restantes por rescatar.
    /// </summary>
    [Header("Rescued Frogs")]
    int remainingFrogs;
    /// <summary>
    /// Los iconos de las ranas que se pueden rescatar.
    /// </summary>
    [SerializeField] GameObject[] rescuedFrogs = null;

    /// <summary>
    /// La puntuación del jugador.
    /// </summary>
    [Header("Score")]
    int score;
    /// <summary>
    /// El panel con la puntuación del jugador.
    /// </summary>
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// La máxima puntuación del jugador.
    /// </summary>
    int highScore;
    /// <summary>
    /// El panel con la máxima puntuación del jugador.
    /// </summary>
    [SerializeField] Text highScoreText = null;
    /// <summary>
    /// Los colliders que permiten que aumente la puntuación al ascender la rana en el escenario.
    /// </summary>
    [SerializeField] GameObject[] scoreLines = null;

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
    /// Panel con los inputs del jugador.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;
    /// <summary>
    /// Panel negro que cubrirá la pantalla en algunas ocasiones.
    /// </summary>
    [SerializeField] GameObject panelBlack = null;

    /// <summary>
    /// El sonido que se reproduce al rescatar a una rana.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource rescuedFrogSound = null;

    #endregion

    void Awake()
    {
        manager8 = this;

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
    /// <param name="newGame">Verdadero si es una nueva partida.</param>
    public void StartGame(bool newGame)
    {
        // Si es una nueva partida, se reinicia la puntuación y las vidas.

        if (newGame)
        {
            score = 0;
            scoreText.text = "Score: 0";

            remainingLifes = 3;

            for (int i = 0; i < lifes.Length; i++)
            {
                lifes[i].enabled = true;
            }
        }

        for (int i = 0; i < scoreLines.Length; i++)
        {
            scoreLines[i].SetActive(true);
        }

        remainingFrogs = 5;

        for (int i = 0; i < rescuedFrogs.Length; i++)
        {
            rescuedFrogs[i].GetComponent<SpriteRenderer>().enabled = false;
            rescuedFrogs[i].layer = 0;
        }

        spawner.StartSpawns();

        player.SetActive(true);
        playerClass.ResetPosition();

        panelMenu.SetActive(false);
        panelGameOver.SetActive(false);
        panelControllers.SetActive(true);
    }

    /// <summary>
    /// Función que se activa cuando se rescata a una rana.
    /// </summary>
    /// <param name="frog">La rana que se ha rescatado.</param>
    public void FrogRescued(GameObject frog)
    {
        frog.GetComponent<SpriteRenderer>().enabled = true;
        frog.layer = 10;

        for (int i = 0; i < scoreLines.Length; i++)
        {
            scoreLines[i].SetActive(true);
        }

        remainingFrogs -= 1;
        rescuedFrogSound.Play();

        UpdateScore(50);

        if (remainingFrogs <= 0)
        {
            StartCoroutine(WinGame());
        }
    }

    /// <summary>
    /// Función que activa la corrutina con las funciones correspondientes cuando el jugador muere.
    /// </summary>
    public void IsDie()
    {
        StartCoroutine(IsDying());
    }

    /// <summary>
    /// Función que aumenta la puntuación.
    /// </summary>
    /// <param name="increase">La cantidad en que aumenta la puntuación.</param>
    public void UpdateScore(int increase)
    {
        score += increase;
        scoreText.text = "Score: " + score.ToString();
    }

    /// <summary>
    /// Función que carga la máxima puntuación del jugador guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score8;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que guarda la máxima puntuación en el dispositivo.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score8 = score;
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
    /// <param name="buildIndex">La escena que queremos cargar.</param>
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    /// <summary>
    /// Corrutina que reinicia la escena después de morir la rana.
    /// </summary>
    /// <returns></returns>
    IEnumerator IsDying()
    {
        if (remainingLifes > 0)
        {
            remainingLifes -= 1;
            lifes[remainingLifes].enabled = false;
        }

        else
        {
            panelGameOver.SetActive(true);
            panelControllers.SetActive(false);
            SaveHighScore();

            yield break;
        }

        panelBlack.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        player.SetActive(true);
        playerClass.ResetPosition();

        panelBlack.SetActive(false);
    }

    /// <summary>
    /// Corrutina que reinicia la escena después de rescatar a todas las ranas.
    /// </summary>
    /// <returns></returns>
    IEnumerator WinGame()
    {
        player.SetActive(false);

        yield return new WaitForSeconds(2);

        panelBlack.SetActive(true);
        UpdateScore(100);

        yield return new WaitForSeconds(0.5f);

        StartGame(false);
        panelBlack.SetActive(false);
    }
}