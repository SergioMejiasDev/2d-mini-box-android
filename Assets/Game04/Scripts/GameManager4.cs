using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que controla las funciones principales del Game 04.
/// </summary>
public class GameManager4 : MonoBehaviour
{
    #region Variables
    public static GameManager4 manager4;
    public delegate void Manager4Delegate();
    public static event Manager4Delegate ResetPositions;
    public static event Manager4Delegate PlayerWin;

    /// <summary>
    /// El jugador (Pacman).
    /// </summary>
    [Header("Player")]
    [SerializeField] GameObject player = null;
    /// <summary>
    /// Los iconos con las vidas restantes del jugador.
    /// </summary>
    [SerializeField] GameObject[] lifes = null;
    /// <summary>
    /// Número de vidas restantes del jugador.
    /// </summary>
    int remainingLifes = 4;
    /// <summary>
    /// Componente PacmanMovement del jugador.
    /// </summary>
    [SerializeField] PacmanMovement pacmanMovement = null;

    /// <summary>
    /// Los fantasmas que se moverán alrededor del escenario.
    /// </summary>
    [Header("Enemies")]
    [SerializeField] GameObject[] enemies = null;
    /// <summary>
    /// Número de enemigos azules restantes (no han sido comidos por el jugador) mientras esté activa una ronda del modo azul.
    /// </summary>
    public int enemiesInScreen = 4;
    /// <summary>
    /// Número 20 que aparecerá cuando consigamos esta puntuación.
    /// </summary>
    [SerializeField] GameObject number20 = null;
    /// <summary>
    /// Número 40 que aparecerá cuando consigamos esta puntuación.
    /// </summary>
    [SerializeField] GameObject number40 = null;
    /// <summary>
    /// Número 80 que aparecerá cuando consigamos esta puntuación.
    /// </summary>
    [SerializeField] GameObject number80 = null;
    /// <summary>
    /// Número 160 que aparecerá cuando consigamos esta puntuación.
    /// </summary>
    [SerializeField] GameObject number160 = null;
    /// <summary>
    /// Componente AudioSource con el sonido que se activará cada vez que el jugador se coma a un fantasma.
    /// </summary>
    [SerializeField] AudioSource enemyEatenSound = null;

    /// <summary>
    /// Cada uno de los puntos que hay repartidos por el escenario.
    /// </summary>
    [Header("Dots")]
    [SerializeField] GameObject[] dots = null;
    /// <summary>
    /// Número de puntos restantes en el escenario.
    /// </summary>
    int dotsInScreen = 325;
    /// <summary>
    /// Cada uno de los puntos grandes que hay en el escenario.
    /// </summary>
    [SerializeField] GameObject[] bigDots = null;

    /// <summary>
    /// Cada una de las posibles frutas que pueden aparecer en el escenario.
    /// </summary>
    [Header("Fruits")]
    [SerializeField] GameObject[] fruits = null;
    /// <summary>
    /// Componente AudioSource con el sonido que se reproducirá al comer una pieza de fruta.
    /// </summary>
    [SerializeField] AudioSource fruitSound = null;

    /// <summary>
    /// Puntuación del jugador.
    /// </summary>
    [Header("Score")]
    int score = 0;
    /// <summary>
    /// Panel que muestra la puntuación del jugador.
    /// </summary>
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// Máxima puntuación obtenida por el jugador.
    /// </summary>
    int highScore = 0;
    /// <summary>
    /// Panel que muestra la máxima puntuación obtenida por el jugador.
    /// </summary>
    [SerializeField] Text highScoreText = null;

    /// <summary>
    /// Panel del menu principal.
    /// </summary>
    [Header("Panels")]
    [SerializeField] GameObject panelMenu = null;
    /// <summary>
    /// Panel con el menú de ayuda.
    /// </summary>
    [SerializeField] GameObject panelHelp = null;
    /// <summary>
    /// Panel con el menú de pausa.
    /// </summary>
    [SerializeField] GameObject panelPause = null;
    /// <summary>
    /// Panel con el menú de Game Over.
    /// </summary>
    [SerializeField] GameObject panelGameOver = null;
    /// <summary>
    /// Panel con los inputs del jugador.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;
    /// <summary>
    /// Panel negro que cubrirá la pantalla en determinados momentos.
    /// </summary>
    [SerializeField] GameObject panelBlack = null;
    #endregion

    private void Awake()
    {
        manager4 = this;

        LetterBoxer.AddLetterBoxingCamera();
    }

    private void Start()
    {
        Time.timeScale = 1;
        dots = GameObject.FindGameObjectsWithTag("Game4/Dot");
        LoadHighScore();
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que inicia una nueva partida.
    /// </summary>
    public void StartGame()
    {
        panelMenu.SetActive(false);
        panelControllers.SetActive(true);

        player.SetActive(true);
        
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetActive(true);
        }

        GenerateFruit();
    }

    /// <summary>
    /// Función que reinicia la partida tras un Game Over.
    /// </summary>
    public void RestartGame()
    {
        panelGameOver.SetActive(false);
        panelControllers.SetActive(true);

        score = 0;
        scoreText.text = "SCORE: 0";

        remainingLifes = 4; // El número de vidas inicial será de 4.

        for (int i = 0; i < lifes.Length; i++)
        {
            lifes[i].SetActive(true);
        }

        dotsInScreen = 325; // El número de puntos en la pantalla será de 325 al inicio de la partida.

        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(true);
        }

        for (int i = 0; i < bigDots.Length; i++)
        {
            bigDots[i].SetActive(true);
        }

        if (ResetPositions != null)
        {
            ResetPositions();
        }

        StopAllCoroutines();

        GameObject[] activeFruit = GameObject.FindGameObjectsWithTag("Game4/Fruit");

        if (activeFruit != null)
        {
            for (int i = 0; i < activeFruit.Length; i++)
            {
                Destroy(activeFruit[i]);
            }
        }

        GenerateFruit();

        player.transform.position = new Vector2(14, 11);
        player.SetActive(true);
        pacmanMovement.ResetPosition();
    }

    /// <summary>
    /// Función a la que se llama para aumentar la puntuación.
    /// </summary>
    /// <param name="scoreValue">La cantidad en que va a aumentar la puntuación.</param>
    public void UpdateScore(int scoreValue)
    {
        score += scoreValue;
        scoreText.text = "SCORE: " + score.ToString();
    }

    /// <summary>
    /// Función que se activa cada vez que el jugador se come un punto.
    /// </summary>
    public void DotEaten()
    {
        dotsInScreen -= 1;

        UpdateScore(1);

        // Si no quedan puntos en la pantalla, habremos ganado la partida.

        if (dotsInScreen == 0)
        {
            StartCoroutine(ContinuePlaying());
        }
    }

    /// <summary>
    /// Función para iniciar la corrutina que genera la fruta.
    /// </summary>
    public void GenerateFruit()
    {
        StartCoroutine(WaitForFruit());
    }

    /// <summary>
    /// Función que reproduce un sonido cuando el jugador se come una fruta.
    /// </summary>
    public void FruitSound()
    {
        fruitSound.Play();
    }

    /// <summary>
    /// Función que se activa cada vez que el jugador se come a un fantasma.
    /// </summary>
    /// <param name="enemyPosition">La posición donde el fantasma ha sido comido.</param>
    public void EnemyEaten(Vector2 enemyPosition)
    {
        enemyEatenSound.Play();

        // Cuantos menos fantasmas restantes queden, mayor será la puntuación.
        // Si se termina el modo donde los fantasmas son vulnerables (modo azul), se volverá a contar desde el principio.

        switch (enemiesInScreen)
        {
            case 4:
                UpdateScore(20);
                Destroy(Instantiate(number20, enemyPosition, Quaternion.identity), 1);
                break;
            case 3:
                UpdateScore(40);
                Destroy(Instantiate(number40, enemyPosition, Quaternion.identity), 1);
                break;
            case 2:
                UpdateScore(80);
                Destroy(Instantiate(number80, enemyPosition, Quaternion.identity), 1);
                break;
            case 1:
                UpdateScore(160);
                Destroy(Instantiate(number160, enemyPosition, Quaternion.identity), 1);
                break;
        }

        enemiesInScreen -= 1;
    }

    /// <summary>
    /// Función que se activa cuando el jugador es alcanzado por un fantasma que no pueda ser comido.
    /// </summary>
    public void PlayerDeath()
    {
        remainingLifes -= 1;

        // Si queda alguna vida restante, la partida continúa.

        if (remainingLifes > 0)
        {
            for (int i = remainingLifes - 1; i < lifes.Length; i++)
            {
                lifes[i].SetActive(false);
            }

            StartCoroutine(ResetPosition());
        }

        // Si no quedan vidas restantes, la partida termina.

        else
        {
            panelGameOver.SetActive(true);
            panelControllers.SetActive(false);
            SaveHighScore();
        }
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score4;
    }

    /// <summary>
    /// Función que guarda la máxima puntuación obtenida en el dispositivo.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score4 = score;
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
    /// Corrutina que devuelve al jugador y los fantasmas a su posición inicial.
    /// </summary>
    /// <returns></returns>
    IEnumerator ResetPosition()
    {
        yield return new WaitForSeconds(1);

        panelBlack.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        panelBlack.SetActive(false);

        if (ResetPositions != null)
        {
            ResetPositions();
        }

        player.transform.position = new Vector2(14, 11);
        player.SetActive(true);
        pacmanMovement.ResetPosition();
    }

    /// <summary>
    /// Corrutina que devuelve al escenario a su estado inicial tras ganar el jugador la partida.
    /// </summary>
    /// <returns></returns>
    IEnumerator ContinuePlaying()
    {
        pacmanMovement.enabled = false;
        PlayerWin();

        yield return new WaitForSeconds(2);
        
        panelBlack.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        panelBlack.SetActive(false);

        dotsInScreen = 325;

        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(true);
        }

        for (int i = 0; i < bigDots.Length; i++)
        {
            bigDots[i].SetActive(true);
        }

        StopCoroutine(WaitForFruit());

        GameObject[] activeFruit = GameObject.FindGameObjectsWithTag("Game4/Fruit");

        if (activeFruit != null)
        {
            for (int i = 0; i < activeFruit.Length; i++)
            {
                Destroy(activeFruit[i]);
            }
        }

        GenerateFruit();

        if (ResetPositions != null)
        {
            ResetPositions();
        }

        player.transform.position = new Vector2(14, 11);
        pacmanMovement.enabled = true;
        pacmanMovement.ResetPosition();
    }

    /// <summary>
    /// Corrutina que genera una pieza de fruta aleatoria tras unos segundos de espera.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForFruit()
    {
        yield return new WaitForSeconds(Random.Range(30, 60));

        // La probabilidad de que aparezca una pieza de fruta será menor cuanto mayor sea su puntuación.

        float randomNumber = Random.value;
        GameObject fruitToGenerate;

        if (randomNumber <= 0.01f)
        {
            fruitToGenerate = fruits[7];
        }

        else if (randomNumber <= 0.03f)
        {
            fruitToGenerate = fruits[6];
        }

        else if (randomNumber <= 0.07f)
        {
            fruitToGenerate = fruits[5];
        }

        else if (randomNumber <= 0.13f)
        {
            fruitToGenerate = fruits[4];
        }

        else if (randomNumber <= 0.21f)
        {
            fruitToGenerate = fruits[3];
        }

        else if (randomNumber <= 0.37f)
        {
            fruitToGenerate = fruits[2];
        }

        else if (randomNumber <= 0.61f)
        {
            fruitToGenerate = fruits[1];
        }

        else
        {
            fruitToGenerate = fruits[0];
        }

        Instantiate(fruitToGenerate, new Vector2(14, 14), Quaternion.identity);
    }
}