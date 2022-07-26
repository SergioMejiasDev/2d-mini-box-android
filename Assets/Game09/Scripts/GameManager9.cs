using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que controla las funciones principales del Game 09.
/// </summary>
public class GameManager9 : MonoBehaviour
{
    public static GameManager9 manager;
    public delegate void Manager9Delegate(float newSpeed);
    public static event Manager9Delegate ChangeSpeed;
    public static event Manager9Delegate StopMovement;

    /// <summary>
    /// El dinosaurio.
    /// </summary>
    [Header("Player")]
    [SerializeField] GameObject player = null;
    /// <summary>
    /// El componente Dinosaur asignado al dinosaurio.
    /// </summary>
    [SerializeField] Dinosaur playerClass = null;

    /// <summary>
    /// La velocidad a la que se mueven el suelo y los objetos.
    /// </summary>
    [Header("Movement")]
    public float speed;
    /// <summary>
    /// El tiempo máximo de espera desde que se genera un objeto hasta que se genera otro.
    /// </summary>
    float maxWait;
    /// <summary>
    /// El tiempo mínimo de espera desde que se genera un objeto hasta que se genera otro.
    /// </summary>
    float minWait;

    /// <summary>
    /// La puntuación del jugador.
    /// </summary>
    [Header("Score")]
    int score = 0;
    /// <summary>
    /// El panel con la puntuación del jugador.
    /// </summary>
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// La máxima puntuación del jugador.
    /// </summary>
    int highScore = 0;
    /// <summary>
    /// El panel con la máxima puntuación activo durante la partida.
    /// </summary>
    [SerializeField] Text highScoreText = null;
    /// <summary>
    /// El panel con la máxima puntuación visible en el menú.
    /// </summary>
    [SerializeField] Text highScoreMenuText = null;

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
    /// Sonido que se reproduce cuando aumenta la velocidad.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource increaseSound = null;
    /// <summary>
    /// Sonido que se reproduce con el Game Over.
    /// </summary>
    [SerializeField] AudioSource gameOverSound = null;

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
    /// Función que inicia una nueva partida.
    /// </summary>
    public void StartGame()
    {
        score = 0;
        scoreText.text = "Score: 0";

        panelGameOver.SetActive(false);
        panelMenu.SetActive(false);
        panelControllers.SetActive(true);

        player.SetActive(true);
        playerClass.ResetValues();

        minWait = 1f;
        maxWait = 1.5f;

        CleanScene();

        StartCoroutine(InstantiateCloud());
        StartCoroutine(IncreaseScore());
        StartCoroutine(InstantiateObject());

        speed = 7f;
        ChangeSpeed(speed);
    }

    /// <summary>
    /// Función que aumenta la velocidad de los objetos.
    /// </summary>
    void IncreaseSpeed()
    {
        minWait -= 0.025f;
        maxWait -= 0.025f;

        speed += 0.5f;
        ChangeSpeed(speed);
    }

    /// <summary>
    /// Función que activa el Game Over.
    /// </summary>
    public void GameOver()
    {
        gameOverSound.Play();

        panelGameOver.SetActive(true);
        panelControllers.SetActive(false);

        SaveHighScore();
        
        speed = 0;
        StopMovement(0);

        StopAllCoroutines();
    }

    /// <summary>
    /// Función que elimina todos los objetos de la escena.
    /// </summary>
    void CleanScene()
    {
        GameObject[] cactus1 = GameObject.FindGameObjectsWithTag("Game9/Cactus1");
        GameObject[] cactus2 = GameObject.FindGameObjectsWithTag("Game9/Cactus2");
        GameObject[] cactus3 = GameObject.FindGameObjectsWithTag("Game9/Cactus3");
        GameObject[] cactus4 = GameObject.FindGameObjectsWithTag("Game9/Cactus4");
        GameObject[] cactus5 = GameObject.FindGameObjectsWithTag("Game9/Cactus5");
        GameObject[] cactus6 = GameObject.FindGameObjectsWithTag("Game9/Cactus6");
        GameObject[] bird = GameObject.FindGameObjectsWithTag("Game9/Bird");
        GameObject[] cloud = GameObject.FindGameObjectsWithTag("Game9/Cloud");

        GameObject[] allObjects = cactus1.Concat(cactus2).Concat(cactus3).Concat(cactus4).Concat(cactus5).
            Concat(cactus6).Concat(bird).Concat(cloud).ToArray();

        for (int i = 0; i < allObjects.Length; i++)
        {
            allObjects[i].SetActive(false);
        }
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score9;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
        highScoreMenuText.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que guarda la máxima puntuación en el dispositivo.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score9 = score;
            SaveManager.saveManager.SaveScores();

            LoadHighScore();
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
    /// Corrutina que genera nubes de forma aleatoria.
    /// </summary>
    /// <returns></returns>
    IEnumerator InstantiateCloud()
    {
        while (true)
        {
            GameObject cloud = ObjectPooler.SharedInstance.GetPooledObject("Game9/Cloud");

            if (cloud != null)
            {
                cloud.transform.position = new Vector2(8, Random.Range(0f, 2.75f));
                cloud.transform.rotation = Quaternion.identity;
                cloud.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(5f, 15f));
        }
    }

    /// <summary>
    /// Corrutina que aumenta la puntuación de forma constante.
    /// </summary>
    /// <returns></returns>
    IEnumerator IncreaseScore()
    {
        int temporalScore = 0;

        while (true)
        {
            score += 1;
            scoreText.text = "Score: " + score.ToString();

            temporalScore += 1;

            if (temporalScore >= 100)
            {
                temporalScore = 0;

                increaseSound.Play();

                IncreaseSpeed();
            }

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// Corrutina que genera objetos de forma aleatoria (cactus o pájaros).
    /// </summary>
    /// <returns></returns>
    IEnumerator InstantiateObject()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minWait, maxWait));

            if (Random.value > 0.1f)
            {
                GameObject cactus = RandomCactus();

                if (cactus != null)
                {
                    cactus.transform.position = new Vector2(8f, -1.48f);
                    cactus.transform.rotation = Quaternion.identity;
                    cactus.SetActive(true);
                }
            }

            else
            {
                GameObject bird = ObjectPooler.SharedInstance.GetPooledObject("Game9/Bird");

                if (bird != null)
                {
                    bird.transform.position = BirdPosition();
                    bird.transform.rotation = Quaternion.identity;
                    bird.SetActive(true);
                }
            }
        }
    }

    /// <summary>
    /// Función que decide qué tipo de cactus se va a generar.
    /// </summary>
    /// <returns>Uno de los seis posibles tipos de cactus.</returns>
    GameObject RandomCactus()
    {
        int randomNumber = Random.Range(1, 7);

        switch (randomNumber)
        {
            case 1:
                return ObjectPooler.SharedInstance.GetPooledObject("Game9/Cactus1");
            case 2:
                return ObjectPooler.SharedInstance.GetPooledObject("Game9/Cactus2");
            case 3:
                return ObjectPooler.SharedInstance.GetPooledObject("Game9/Cactus3");
            case 4:
                return ObjectPooler.SharedInstance.GetPooledObject("Game9/Cactus4");
            case 5:
                return ObjectPooler.SharedInstance.GetPooledObject("Game9/Cactus5");
            default:
                return ObjectPooler.SharedInstance.GetPooledObject("Game9/Cactus6");
        }
    }

    /// <summary>
    /// Función que decide en que posición se va a generar un pájaro.
    /// </summary>
    /// <returns>Una de las tres posibles posiciones.</returns>
    Vector2 BirdPosition()
    {
        int randomNumber = Random.Range(1, 4);

        switch (randomNumber)
        {
            case 1:
                return new Vector2(8f, -1.38f);
            case 2:
                return new Vector2(8f, -0.63f);
            default:
                return new Vector2(8f, 0.85f);
        }
    }
}