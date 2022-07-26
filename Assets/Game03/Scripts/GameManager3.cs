using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Clase que controla las funciones principales del Game 03.
/// </summary>
public class GameManager3 : MonoBehaviour
{
    #region Variables

    public static GameManager3 manager3;

    /// <summary>
    /// Componente PlayerController de la nave del jugador.
    /// </summary>
    [Header("Player")]
    [SerializeField] PlayerController playerController;
    /// <summary>
    /// La nave del jugador.
    /// </summary>
    [SerializeField] GameObject player = null;
    /// <summary>
    /// Los paneles con las vidas restantes del jugador.
    /// </summary>
    [SerializeField] GameObject[] lifes = null;
    /// <summary>
    /// Las bases que hay junto a la nave del jugador.
    /// </summary>
    [SerializeField] GameObject[] bases = null;
    /// <summary>
    /// Número de vidas restantes del jugador.
    /// </summary>
    int remainingLifes;
    /// <summary>
    /// Explosión que aparece cuando la nave del jugador es destruida.
    /// </summary>
    [SerializeField] GameObject playerExplosion = null;
    /// <summary>
    /// Componente AudioSource con el sonido de la explosión de la nave del jugador.
    /// </summary>
    [SerializeField] AudioSource playerExplosionAudio = null;

    /// <summary>
    /// Componente EnemyController de la oleada de enemigos.
    /// </summary>
    [Header("Enemies")]
    [SerializeField] EnemyController enemyController;
    /// <summary>
    /// La oleada de enemigos.
    /// </summary>
    [SerializeField] GameObject enemyHolder = null;
    /// <summary>
    /// Cada uno de los enemigos dentro de la oleada.
    /// </summary>
    [SerializeField] GameObject[] enemies = null;
    /// <summary>
    /// Cantidad de enemigos que hay en la pantalla.
    /// </summary>
    int enemiesInScreen;
    /// <summary>
    /// Explosión que aparece cuando un enemigo es alcanzado por un disparo del jugador.
    /// </summary>
    [SerializeField] GameObject enemyExplosion = null;
    /// <summary>
    /// Componente AudioSource con el sonido de la explosión de los enemigos.
    /// </summary>
    [SerializeField] AudioSource enemyExplosionAudio = null;

    /// <summary>
    /// OVNIs que aparecerán de forma aleatoria en la parte superior de la pantalla.
    /// </summary>
    [Header("UFO")]
    [SerializeField] GameObject ufo = null;
    /// <summary>
    /// Explosión que aparecerá cuando una bala del jugador alcance al OVNI.
    /// </summary>
    [SerializeField] GameObject ufoExplosion = null;
    /// <summary>
    /// Componente AudioSource con el sonido de la explosión del OVNI.
    /// </summary>
    [SerializeField] AudioSource ufoExplosionAudio = null;
    /// <summary>
    /// Probabilidad sobre 100 de que se genere un OVNI tras el correspondiente tiempo de espera.
    /// </summary>
    readonly float ufoProbability = 25;
    /// <summary>
    /// Tiempo de espera en segundos para que pueda (o no) aparecer un nuevo OVNI.
    /// </summary>
    readonly float ufoWaitTime = 10;

    /// <summary>
    /// Puntuación del jugador.
    /// </summary>
    [Header("Score")]
    int score;
    /// <summary>
    /// Panel con la puntuación del jugador.
    /// </summary>
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// Máxima puntuación del jugador guardada en el dispositivo.
    /// </summary>
    int highScore = 0;
    /// <summary>
    /// Panel con la máxima puntuación visible durante la partida.
    /// </summary>
    [SerializeField] Text highScoreText = null;
    /// <summary>
    /// Panel con la máxima puntuación visible en el menú del juego.
    /// </summary>
    [SerializeField] Text highScoreMenu = null;

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
    /// Panel con los inputs del jugador para controlar la nave.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;
    /// <summary>
    /// Panel negro que cubrirá la pantalla en determinados momentos.
    /// </summary>
    [SerializeField] GameObject panelBlack = null;

    #endregion

    void Awake()
    {
        manager3 = this;

        LetterBoxer.AddLetterBoxingCamera();
    }

    void Start()
    {
        Time.timeScale = 1;
        LoadHighScore();
        highScoreMenu.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que inicia una nueva partida.
    /// </summary>
    public void StartGame()
    {
        panelMenu.SetActive(false);
        panelGameOver.SetActive(false);
        panelControllers.SetActive(true);

        // El jugador comienza teniendo 4 vidas.

        for (int i = 0; i < lifes.Length; i++)
        {
            lifes[i].SetActive(true);
        }
        remainingLifes = 4;

        // El número inicial de enemigos es 55.

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetActive(true);
        }
        enemyHolder.SetActive(true);
        enemiesInScreen = 55;

        // Habrá 4 bases al inicio de la partida, todas con la salud al máximo.

        for (int i = 0; i < bases.Length; i++)
        {
            bases[i].SetActive(true);
            bases[i].GetComponent<BaseHealth>().Restart();
        }

        // Se reinicia la puntuación.

        scoreText.enabled = true;
        highScoreText.enabled = true;
        score = 0;
        scoreText.text = "SCORE: " + score.ToString();
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();

        // Se activan el movimiento del jugador y la corrutina del OVNI.

        player.SetActive(true);
        enemyController.StartPlay(true);
        StartCoroutine(WaitForUFO());
    }

    /// <summary>
    /// Función que hace reaparecer a los enemigos tras defender a una oleada completa.
    /// La principal diferencia con la función de StartGame() es que no se recuperan las vidas ni se restauran las bases.
    /// </summary>
    public void ContinuePlaying()
    {
        playerController.enabled = true;
        
        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].SetActive(true);
        }
        enemiesInScreen = 55;
        
        enemyController.StartPlay(true);
        StopAllCoroutines(); // Detenemos la corrutina del OVNI antes de iniciar una nueva, evitando que se inicie múltiples veces.
        StartCoroutine(WaitForUFO());
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score3;
    }

    /// <summary>
    /// Función que guarda la máxima puntuación obtenida en el dispositivo.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score3 = score;
            SaveManager.saveManager.SaveScores();
        }

        LoadHighScore();
    }

    /// <summary>
    /// Función que actualiza la puntuación.
    /// </summary>
    /// <param name="scoreValue">Valor que se añadirá a la puntuación actual.</param>
    void UpdateScore(int scoreValue)
    {
        score += scoreValue;
        scoreText.text = "SCORE: " + score.ToString();
    }

    /// <summary>
    /// Función que se activa cuando el jugador es alcanzado por una bala del enemigo.
    /// Se perderá una vida y se activará una animación donde la nave explota.
    /// </summary>
    /// <param name="lifesLost">Número de vidas que va a perder el jugador.</param>
    public void LoseHealth(int lifesLost)
    {
        remainingLifes -= lifesLost;
        StartCoroutine(Death());
    }

    /// <summary>
    /// Función que hace que aparezca un OVNI en la parte superior derecha de la pantalla.
    /// </summary>
    void GenerateUFO()
    {
        Instantiate(ufo, new Vector2(9.75f, 4f), Quaternion.identity);
    }

    /// <summary>
    /// Función que se activa cuando el jugador destruye un OVNI.
    /// </summary>
    /// <param name="ufoScore">Puntuación que nos otorgará el OVNI.</param>
    /// <param name="location">Posición donde el OVNI ha sido destruido.</param>
    public void UFODeath(int ufoScore, Vector2 location)
    {
        Destroy(Instantiate(ufoExplosion, location, Quaternion.identity), 2);
        ufoExplosionAudio.Play();
        UpdateScore(ufoScore);
    }

    /// <summary>
    /// Función que se activa cuando el jugador destruye algún enemigo.
    /// </summary>
    /// <param name="enemyScore">Puntuación que nos otorgará el enemigo.</param>
    /// <param name="location">Posición donde el enemigo ha sido destruido.</param>
    public void EnemyDeath(int enemyScore, Vector2 location)
    {
        Destroy(Instantiate(enemyExplosion, location, Quaternion.identity), 0.5f);
        enemyExplosionAudio.Play();
        enemyController.DecreaseWaitTime();

        UpdateScore(enemyScore);

        enemiesInScreen -= 1;
        if (enemiesInScreen <= 0)
        {
            StartCoroutine(Victory());
        }
    }

    /// <summary>
    /// Función que activa y desactiva la pausa.
    /// </summary>
    public void PauseGame()
    {
        if (!panelPause.activeSelf)
        {
            panelPause.SetActive(true);
            panelControllers.SetActive(false);
            Time.timeScale = 0;
            AudioListener.volume = 0;
        }

        else if (panelPause.activeSelf)
        {
            panelPause.SetActive(false);
            panelControllers.SetActive(true);
            Time.timeScale = 1;
            AudioListener.volume = 1;
        }
    }

    /// <summary>
    /// Función que activa y desactiva el panel de ayuda.
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
    /// Función encargada de cambiar de escena.
    /// </summary>
    /// <param name="buildIndex">Escena que se quiere cargar.</param>
    public void LoadScene(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    /// <summary>
    /// Corrutina que se inicia cuando el jugador muere.
    /// </summary>
    /// <returns></returns>
    IEnumerator Death()
    {
        enemyController.stopMoving = true;
        player.SetActive(false);

        if (remainingLifes > 0) // Si al jugador le queda alguna vida, la partida continuará.
        {
            for (int i = remainingLifes - 1; i < lifes.Length; i++)
            {
                lifes[i].SetActive(false);
            }
            Destroy(Instantiate(playerExplosion, player.transform.position, Quaternion.identity), 2);
            playerExplosionAudio.Play();
            yield return new WaitForSeconds(2);
            player.SetActive(true);
            player.transform.position = new Vector2(0, -4);
            enemyController.StartPlay(false);
        }

        else // Si al jugador no le quedan vidas, la partida terminará.
        {
            for (int i = 0; i < lifes.Length; i++)
            {
                lifes[i].SetActive(false);
            }

            Destroy(Instantiate(playerExplosion, player.transform.position, Quaternion.identity), 2);
            playerExplosionAudio.Play();

            GameObject[] ufos = GameObject.FindGameObjectsWithTag("Game3/UFO");
            if (ufos != null)
            {
                for (int i = 0; i < ufos.Length; i++)
                {
                    Destroy(ufos[i]);
                }
            }
            
            panelGameOver.SetActive(true);
            panelControllers.SetActive(false);
            SaveHighScore();
            LoadHighScore();
            StopAllCoroutines();
        }
    }

    /// <summary>
    /// Corrutina que se inicia cuando el jugador elimina a una oleada completa de enemigos.
    /// </summary>
    /// <returns></returns>
    IEnumerator Victory()
    {
        playerController.enabled = false;
        enemyController.stopMoving = true;

        SaveHighScore();
        LoadHighScore();

        yield return new WaitForSeconds(1.5f);

        panelBlack.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        panelBlack.SetActive(false);
        player.transform.position = new Vector2(0, -4);

        ContinuePlaying();
    }

    /// <summary>
    /// Corrutina que llama de forma aleatoria a la función para generar OVNIs.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForUFO()
    {
        while (true)
        {
            yield return new WaitForSeconds(ufoWaitTime);

            if ((enemiesInScreen <= 0) || (remainingLifes <= 0))
            {
                yield break;
            }

            else if (Random.value < (ufoProbability / 100))
            {
                GenerateUFO();
            }
        }
    }
}