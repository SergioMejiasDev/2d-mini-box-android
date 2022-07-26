using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que gestiona las funciones principales del Game 10.
/// </summary>
public class GameManager10 : MonoBehaviour
{
    public static GameManager10 manager;

    public delegate void Manager10Delegate();
    public static event Manager10Delegate StopMovement;

    /// <summary>
    /// El jugador.
    /// </summary>
    [Header("Player")]
    [SerializeField] GameObject player = null;
    /// <summary>
    /// La clase QBertMovement asignada al jugador.
    /// </summary>
    [SerializeField] QBertMovement playerClass = null;
    /// <summary>
    /// Las vidas restantes del jugador.
    /// </summary>
    int remainingLifes = 3;
    /// <summary>
    /// Los iconos de las vidas restantes.
    /// </summary>
    [SerializeField] Image[] lifes = null;

    /// <summary>
    /// Cada uno de los bloques de la pirámide.
    /// </summary>
    [Header("Block")]
    [SerializeField] QBertBlocks[] blocks = null;
    /// <summary>
    /// Los bloques restantes de la pirámide (los que aun no son amarillos).
    /// </summary>
    int remainingBlocks = 28;

    /// <summary>
    /// El disco a la izquierda de la pirámide.
    /// </summary>
    [Header("Discs")]
    [SerializeField] GameObject disc1 = null;
    /// <summary>
    /// El disco a la derecha de la pirámide.
    /// </summary>
    [SerializeField] GameObject disc2 = null;
    /// <summary>
    /// El collider que sustituirá al disco de la izquierda cuando desaparezca.
    /// </summary>
    [SerializeField] GameObject discCollider1 = null;
    /// <summary>
    /// El collider que sustituirá al disco de la derecha cuando desaparezca.
    /// </summary>
    [SerializeField] GameObject discCollider2 = null;

    /// <summary>
    /// La puntuación del jugador.
    /// </summary>
    [Header("Score")]
    int score = 0;
    /// <summary>
    /// Panel con la puntuación del jugador.
    /// </summary>
    [SerializeField] Text scoreText = null;
    /// <summary>
    /// La máxima puntuación del jugador.
    /// </summary>
    int highScore = 0;
    /// <summary>
    /// Panel con la máxima puntuación del jugador.
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
    /// Panel con los inputs del jugador.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;
    /// <summary>
    /// Panel negro que aparecerá en determinadas ocasiones.
    /// </summary>
    [SerializeField] GameObject panelBlack = null;

    /// <summary>
    /// Sonido que se reproduce cuando el jugador cae de la pirámide.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource fallSound = null;
    /// <summary>
    /// Sonido que se reproduce cuando el jugador es golpeado por un enemigo.
    /// </summary>
    [SerializeField] AudioSource speechSound = null;
    /// <summary>
    /// Sonido que se reproduce cuando una serpiente cae de la pirámide.
    /// </summary>
    [SerializeField] AudioSource fallSnakeSound = null;
    /// <summary>
    /// Sonido que se reproduce cuando el jugador gana la partida.
    /// </summary>
    [SerializeField] AudioSource winSound = null;

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

        disc1.transform.position = new Vector2(-2.46f, -1.15f);
        disc1.SetActive(true);
        disc2.transform.position = new Vector2(2.46f, -1.15f);
        disc2.SetActive(true);
        discCollider1.SetActive(false);
        discCollider2.SetActive(false);

        panelMenu.SetActive(false);
        panelGameOver.SetActive(false);
        panelControllers.SetActive(true);

        player.transform.position = new Vector2(0, 2.76f);
        player.SetActive(true);

        remainingLifes = 3;

        for (int i = 0; i < lifes.Length; i++)
        {
            lifes[i].enabled = true;
        }

        StartCoroutine(GenerateObject());

        remainingBlocks = 28;
        
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].ResetSprite();
        }
    }

    /// <summary>
    /// Función que reinicia el escenario después de ganar la partida.
    /// </summary>
    void ContinuePlaying()
    {
        disc1.transform.position = new Vector2(-2.46f, -1.15f);
        disc1.SetActive(true);
        disc2.transform.position = new Vector2(2.46f, -1.15f);
        disc2.SetActive(true);
        discCollider1.SetActive(false);
        discCollider2.SetActive(false);

        player.transform.position = new Vector2(0, 2.76f);
        player.SetActive(true);

        StartCoroutine(GenerateObject());

        remainingBlocks = 28;

        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].ResetSprite();
        }
    }

    /// <summary>
    /// Función que elimina todos los objetos de la escena.
    /// </summary>
    void CleanScene()
    {
        GameObject[] green = GameObject.FindGameObjectsWithTag("Game10/GreenBall");
        GameObject[] red = GameObject.FindGameObjectsWithTag("Game10/RedBall");
        GameObject[] purple = GameObject.FindGameObjectsWithTag("Game10/PurpleBall");

        GameObject[] allObjects = green.Concat(red).Concat(purple).ToArray();

        for (int i = 0; i < allObjects.Length; i++)
        {
            allObjects[i].SetActive(false);
        }
    }

    /// <summary>
    /// Función que reinicia la posición del jugador.
    /// </summary>
    public void Restart()
    {
        player.transform.position = new Vector2(0, 2.76f);
        player.SetActive(true);

        StartCoroutine(GenerateObject());
    }

    /// <summary>
    /// Función que se activa cuando el jugador golpea a un enemigo.
    /// </summary>
    public void DeathHit()
    {
        if (StopMovement != null)
        {
            StopMovement();
        }

        speechSound.Play();

        StopAllCoroutines();

        StartCoroutine(WaitForRespawn());
    }

    /// <summary>
    /// Función que se activa cuando el jugador cae de la pirámide.
    /// </summary>
    public void DeathFall()
    {
        fallSound.Play();

        StopAllCoroutines();

        StartCoroutine(WaitForRespawn());
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
    /// Función que activa y desactiva los colliders de los bloques.
    /// </summary>
    /// <param name="enable">Verdadero para activarlos, falso para desactivarlos.</param>
    public void EnableBlocks(bool enable)
    {
        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].EnableOrDisable(enable);
        }
    }

    /// <summary>
    /// Función que reduce el número de bloques restantes para ganar la partida.
    /// Se activa cuando un bloque se vuelve amarillo.
    /// </summary>
    public void ReduceBlocks()
    {
        remainingBlocks -= 1;

        if (remainingBlocks == 0)
        {
            StopAllCoroutines();

            StartCoroutine(WinGame());
        }
    }

    /// <summary>
    /// Función que se activa cuando una serpiente cae de la pirámide.
    /// </summary>
    public void FallSnake()
    {
        fallSnakeSound.Play();

        UpdateScore(100);
    }

    /// <summary>
    /// Corrutina encargada de generar bolas cada pocos segundos.
    /// </summary>
    /// <returns></returns>
    IEnumerator GenerateObject()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(3f, 5f));

            GameObject randomObject = RandomObject();

            if (randomObject != null)
            {
                randomObject.transform.position = SpawnPoint();
                randomObject.transform.rotation = Quaternion.identity;
                randomObject.SetActive(true);
            }
        }
    }

    /// <summary>
    /// Corrutina que se activa cuando el jugador muere.
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForRespawn()
    {
        yield return new WaitForSeconds(3);

        remainingLifes -= 1;

        if (remainingLifes >= 0)
        {
            lifes[remainingLifes].enabled = false;
        }

        else
        {
            SaveHighScore();
            CleanScene();
            player.SetActive(false);
            panelGameOver.SetActive(true);
            panelControllers.SetActive(false);

            yield break;
        }

        panelBlack.SetActive(true);

        CleanScene();
        player.SetActive(false);

        yield return new WaitForSeconds(0.5f);

        panelBlack.SetActive(false);

        Restart();
    }

    /// <summary>
    /// Corrutina que se activa cuando el jugador gana la partida (todos los bloques son amarillos).
    /// </summary>
    /// <returns></returns>
    IEnumerator WinGame()
    {
        if (StopMovement != null)
        {
            StopMovement();
        }

        playerClass.Invulnerable();

        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].EnableAnimator(true);
        }

        winSound.Play();

        yield return new WaitForSeconds(3);

        for (int i = 0; i < blocks.Length; i++)
        {
            blocks[i].EnableAnimator(false);
        }

        if (disc1.activeSelf)
        {
            disc1.SetActive(false);

            UpdateScore(25);

            yield return new WaitForSeconds(1);
        }

        if (disc2.activeSelf)
        {
            disc2.SetActive(false);

            UpdateScore(25);

            yield return new WaitForSeconds(1);
        }

        yield return new WaitForSeconds(0.5f);

        panelBlack.SetActive(true);

        CleanScene();

        player.SetActive(false);

        UpdateScore(250);

        SaveHighScore();

        yield return new WaitForSeconds(0.5f);

        ContinuePlaying();

        panelBlack.SetActive(false);
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score10;
        highScoreText.text = "HIGH SCORE: " + highScore.ToString();
    }

    /// <summary>
    /// Función que guarda la máxima puntuación en el dispositivo.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score10 = score;
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
    /// Función que activa el menú de ayuda.
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
    /// Función que decide donde se va a generar una nueva bola.
    /// </summary>
    /// <returns>Una de las dos posibles posiciones donde aparecerá la bola.</returns>
    Vector2 SpawnPoint()
    {
        if (Random.value < 0.5f)
        {
            return new Vector2(-0.5f, 2.68f);
        }

        else
        {
            return new Vector2(0.5f, 2.68f);
        }
    }

    /// <summary>
    /// Función que decide de forma aleatoria que bola se va a generar.
    /// </summary>
    /// <returns>Una de las tres posibles bolas (roja, morada o verde).</returns>
    GameObject RandomObject()
    {
        float randomNumber = Random.value;

        if (randomNumber < 0.05f)
        {
            return ObjectPooler.SharedInstance.GetPooledObject("Game10/PurpleBall");
        }

        else if (randomNumber < 0.15f)
        {
            return ObjectPooler.SharedInstance.GetPooledObject("Game10/GreenBall");
        }

        else
        {
            return ObjectPooler.SharedInstance.GetPooledObject("Game10/RedBall");
        }
    }
}