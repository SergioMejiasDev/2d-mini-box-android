using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que controlas las funciones principales del Game 07.
/// </summary>
public class GameManager7 : MonoBehaviour
{
    #region Variables
    public static GameManager7 manager7;

    public delegate void Manager7Delegate();
    public static event Manager7Delegate Stop;
    public static event Manager7Delegate Reset;

    /// <summary>
    /// El componente MarioMovement del jugador.
    /// </summary>
    [Header("Player")]
    [SerializeField] MarioMovement player = null;
    /// <summary>
    /// El número de vidas restantes del jugador.
    /// </summary>
    int lifes;
    /// <summary>
    /// Los iconos de las vidas restantes.
    /// </summary>
    [SerializeField] Image[] lifesImages = null;

    /// <summary>
    /// El componente PrincessAnimation de la princesa.
    /// </summary>
    [Header("Princess")]
    [SerializeField] PrincessAnimation princess = null;
    /// <summary>
    /// El cartel de "Help!" que aparece sobre la princesa.
    /// </summary>
    [SerializeField] GameObject help = null;
    /// <summary>
    /// El corazón que aparece cuando el jugador llega hasta la princesa.
    /// </summary>
    [SerializeField] GameObject heart = null;

    /// <summary>
    /// El componente KongMovement del Kong.
    /// </summary>
    [Header("Kong")]
    [SerializeField] KongMovement kong = null;

    /// <summary>
    /// Los diferentes mazos repartidos por el escenario.
    /// </summary>
    [Header("Mallets")]
    [SerializeField] GameObject[] mallets = null;

    /// <summary>
    /// El prefab de las llamas que aparecerán en el bidón.
    /// </summary>
    [Header("Flames")]
    [SerializeField] GameObject flame = null;
    /// <summary>
    /// La posición donde se van a generar las llamas.
    /// </summary>
    [SerializeField] Transform flameSpawnPoint = null;

    /// <summary>
    /// La puntuación del jugador.
    /// </summary>
    [Header("Score")]
    int score;
    /// <summary>
    /// Cartel con la puntuación del jugador.
    /// </summary>
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// Cartel con el número 10 que aparecerá cuando conseguimos estos puntos.
    /// </summary>
    [SerializeField] GameObject number10 = null;
    /// <summary>
    /// Cartel con el número 30 que aparecerá cuando conseguimos estos puntos.
    /// </summary>
    [SerializeField] GameObject number30 = null;
    /// <summary>
    /// Cartel con el número 50 que aparecerá cuando conseguimos estos puntos.
    /// </summary>
    [SerializeField] GameObject number50 = null;
    /// <summary>
    /// Máxima puntuación obtenida por el jugador.
    /// </summary>
    int highScore;
    /// <summary>
    /// Cartel con la máxima puntuación obtenida por el jugador.
    /// </summary>
    [SerializeField] Text highScoreText = null;

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
    /// Panel negro que cubrirá la pantalla en algunas ocasiones.
    /// </summary>
    [SerializeField] GameObject panelBlack = null;
    /// <summary>
    /// Panel que contiene los inputs del jugador.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;

    /// <summary>
    /// Sonido que se reproduce cuando el jugador consigue puntos.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource scoreSound = null;
    /// <summary>
    /// Sonido que se reproduce cuando el jugador llega hasta la princesa.
    /// </summary>
    [SerializeField] AudioSource winSound = null;
    #endregion

    private void Awake()
    {
        manager7 = this;

        LetterBoxer.AddLetterBoxingCamera();
    }

    void Start()
    {
        Time.timeScale = 1;
        LoadHighScore();
    }

    /// <summary>
    /// Función que inicia la nueva partida.
    /// </summary>
    /// <param name="newGame">Verdadero si es una nueva partida, falso si no lo es.</param>
    public void StartGame(bool newGame)
    {
        panelMenu.SetActive(false);
        panelGameOver.SetActive(false);
        panelControllers.SetActive(true);

        // Si es una nueva partida, se resetean las vidas y la puntuación.

        if (newGame)
        {
            score = 0;

            lifes = 3;
            
            for (int i = 0; i < lifesImages.Length; i++)
            {
                lifesImages[i].enabled = true;
            }

            scoreText.text = "Score: 0";
        }

        player.enabled = true;

        heart.SetActive(false);
        princess.enabled = true;

        kong.enabled = true;

        for (int i = 0; i < mallets.Length; i++)
        {
            mallets[i].SetActive(true);
        }

        Reset();
    }

    /// <summary>
    /// Función que inicia la corrutina que indica que hemos ganado.
    /// </summary>
    public void WinGame()
    {
        StartCoroutine(WinningGame());
    }

    /// <summary>
    /// Función que inicia una corrutina cuando el jugador muere.
    /// </summary>
    public void GameOver()
    {
        StartCoroutine(LosingGame());
    }

    /// <summary>
    /// Función que detiene todas las animaciones del juego.
    /// </summary>
    public void StopGame()
    {
        Stop();
    }

    /// <summary>
    /// Función que elimina los barriles y llamas de la escena.
    /// </summary>
    public void CleanScene()
    {
        GameObject[] activeBarrels = GameObject.FindGameObjectsWithTag("Game7/Barrel");

        if (activeBarrels != null)
        {
            for (int i = 0; i < activeBarrels.Length; i++)
            {
                activeBarrels[i].SetActive(false);
            }
        }

        GameObject[] activeFlames = GameObject.FindGameObjectsWithTag("Game7/Flame");

        if (activeFlames != null)
        {
            for (int i = 0; i < activeFlames.Length; i++)
            {
                Destroy(activeFlames[i]);
            }
        }
    }

    /// <summary>
    /// Función que se activa cuando el jugador salta sobre un barril.
    /// </summary>
    /// <param name="barrelPosition">La posición del barril que el jugador ha saltado.</param>
    public void JumpBarrel(Vector2 barrelPosition)
    {
        UpdateScore(10);

        scoreSound.Play();

        Destroy(Instantiate(number10, barrelPosition, Quaternion.identity), 1);
    }

    /// <summary>
    /// Función que se activa cuando el jugador rompe un barril con el mazo.
    /// </summary>
    /// <param name="barrelPosition">La posición del barril que se ha destruido.</param>
    public void DestroyBarrel(Vector2 barrelPosition)
    {
        UpdateScore(30);

        scoreSound.Play();

        Destroy(Instantiate(number30, barrelPosition, Quaternion.identity), 1);
    }

    /// <summary>
    /// Función que se activa cuando el jugador golpea una llama con el mazo.
    /// </summary>
    /// <param name="flamePosition">La posición de la llama que se ha destruido.</param>
    public void DestroyFlame(Vector2 flamePosition)
    {
        UpdateScore(50);

        scoreSound.Play();

        Destroy(Instantiate(number50, flamePosition, Quaternion.identity), 1);
    }

    /// <summary>
    /// Función que se activa cuando un barril golpea el bidón.
    /// </summary>
    /// <param name="isVertical">Verdadero si el barril caía hacia abajo. Falso si el barril ha llegado llorando.</param>
    public void SpawnFlame(bool isVertical)
    {
        if (isVertical || Random.value > 0.5f)
        {
            Instantiate(flame, flameSpawnPoint.position, Quaternion.identity);
        }
    }

    /// <summary>
    /// Función que se activa cuando aumenta la puntuación.
    /// </summary>
    /// <param name="increase">La cantidad en que aumenta la puntuación.</param>
    void UpdateScore(int increase)
    {
        score += increase;
        scoreText.text = "Score: " + score.ToString();
    }

    /// <summary>
    /// Función que carga la máxima puntuación del jugador guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score7;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que guarda en el dispositivo la máxima puntuación obtenida por el jugador.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score7 = score;
            SaveManager.saveManager.SaveScores();
        }
    }

    /// <summary>
    /// Función que activa y desactiva el menú de pausa.
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
    /// Función que activa y desactiva el menú de ayuda.
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
    /// Corrutina que se activa cuando el jugador alcanza a la princesa, deteniendo las animaciones y reiniciando la partida (sin perder la puntuación).
    /// </summary>
    /// <returns></returns>
    IEnumerator WinningGame()
    {
        StopGame();

        player.enabled = false;
        princess.enabled = false;
        kong.enabled = false;

        help.SetActive(false);
        heart.SetActive(true);

        CleanScene();

        yield return new WaitForSeconds(0.25f);

        winSound.Play();

        yield return new WaitForSeconds(3);

        UpdateScore(100);
        panelBlack.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        panelBlack.SetActive(false);
        StartGame(false);
        Reset();
    }

    /// <summary>
    /// Corrutina que se inicia cuando el jugador muere, reiniciando la escena o activando el Game Over según las vidas restantes.
    /// </summary>
    /// <returns></returns>
    IEnumerator LosingGame()
    {
        if (lifes > 0)
        {
            lifes -= 1;

            lifesImages[lifes].enabled = false;

            panelBlack.SetActive(true);

            yield return new WaitForSeconds(0.5f);

            panelBlack.SetActive(false);
            StartGame(false);
            Reset();
        }

        else
        {
            SaveHighScore();

            panelGameOver.SetActive(true);
            panelControllers.SetActive(false);
        }
    }
}