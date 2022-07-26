using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// Clase que controla las funciones principales del Game 12.
/// </summary>
public class GameManager12 : MonoBehaviour
{
    public static GameManager12 manager;
    public delegate void Manager12Delegate();
    public static event Manager12Delegate StopMovement;

    /// <summary>
    /// La clase BalloonMovement asignada al globo del jugador.
    /// </summary>
    [Header("Player")]
    [SerializeField] BalloonMovement playerClass = null;
    /// <summary>
    /// Verdadero si está activo el modo magnético, falso si no lo está.
    /// </summary>
    public bool magnetMode = false;
    /// <summary>
    /// Verdadero si está activo el modo invulnerable, falso si no lo está.
    /// </summary>
    public bool colorMode = false;
    /// <summary>
    /// La capa asignada a los objetos.
    /// </summary>
    [SerializeField] LayerMask objetsMask = 0;

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
    /// La puntuación máxima obtenida por el jugador.
    /// </summary>
    int highScore = 0;
    /// <summary>
    /// El panel con la máxima puntuación del jugador visible en el menú del juego.
    /// </summary>
    [SerializeField] Text highScoreMenuText = null;
    /// <summary>
    /// El panel con la máxima puntuación del jugador visible durante la partida.
    /// </summary>
    [SerializeField] Text highScoreText = null;

    /// <summary>
    /// Panel con el menú del juego.
    /// </summary>
    [Header("Panels")]
    [SerializeField] GameObject panelMenu = null;
    /// <summary>
    /// Panel que activa y desactiva el menú de pausa.
    /// </summary>
    [SerializeField] GameObject panelPause = null;
    /// <summary>
    /// Panel que activa y desactiva el menú de Game Over.
    /// </summary>
    [SerializeField] GameObject panelGameOver = null;
    /// <summary>
    /// Panel que activa y desactiva el menú de ayuda.
    /// </summary>
    [SerializeField] GameObject panelHelp = null;
    /// <summary>
    /// Panel que contiene los inputs del jugador.
    /// </summary>
    [SerializeField] GameObject panelControllers = null;

    /// <summary>
    /// Sonido que se reproduce cuando el globo choca con los pinchos.
    /// </summary>
    [Header("Sounds")]
    [SerializeField] AudioSource hitSound = null;

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
    /// Función que inicia una nueva partida.
    /// </summary>
    public void StartGame()
    {
        panelMenu.SetActive(false);
        panelGameOver.SetActive(false);
        panelControllers.SetActive(true);

        score = 0;
        scoreText.text = "0";

        playerClass.enabled = true;
        playerClass.ResetValues();

        magnetMode = false;
        colorMode = false;

        CleanScene();

        StartCoroutine(InstantiateBubbles());
        StartCoroutine(InstantiateSpikes());
    }

    /// <summary>
    /// Función que elimina todos los objetos de la escena.
    /// </summary>
    void CleanScene()
    {
        GameObject[] bubble = GameObject.FindGameObjectsWithTag("Game12/Bubble");
        GameObject[] bubbleColor = GameObject.FindGameObjectsWithTag("Game12/BubbleColor");
        GameObject[] magnet = GameObject.FindGameObjectsWithTag("Game12/Magnet");
        GameObject[] spike1 = GameObject.FindGameObjectsWithTag("Game12/Spike1");
        GameObject[] spike2 = GameObject.FindGameObjectsWithTag("Game12/Spike2");
        GameObject[] spike3 = GameObject.FindGameObjectsWithTag("Game12/Spike3");
        GameObject[] spike4 = GameObject.FindGameObjectsWithTag("Game12/Spike4");
        GameObject[] spike5 = GameObject.FindGameObjectsWithTag("Game12/Spike5");

        GameObject[] allObjects = bubble.Concat(bubbleColor).Concat(magnet).Concat(spike1).Concat(spike2).Concat(spike3).
            Concat(spike4).Concat(spike5).ToArray();

        for (int i = 0; i < allObjects.Length; i++)
        {
            allObjects[i].SetActive(false);
        }
    }

    /// <summary>
    /// Función que activa el Game Over.
    /// </summary>
    public void GameOver()
    {
        hitSound.Play();

        StopMovement();
        StopAllCoroutines();

        SaveHighScore();
        panelGameOver.SetActive(true);
        panelControllers.SetActive(false);
    }

    /// <summary>
    /// Función que calcula una posición aleatoria donde los objetos se van a generar.
    /// </summary>
    /// <returns>La posición aleatoria donde se generará el objeto.</returns>
    Vector2 RandomPosition()
    {
        float x = Random.Range(-3.6f, 3.6f);

        if (Physics2D.OverlapCircle(new Vector2(x, 6.0f), 1.0f, objetsMask))
        {
            // Si hay otro objeto demasiado cerca, buscamos otra posición.

            return RandomPosition();
        }

        else
        {
            return new Vector2(x, 6.0f);
        }
    }

    /// <summary>
    /// Corrutina que genera objetos constantemente.
    /// </summary>
    /// <returns></returns>
    IEnumerator InstantiateBubbles()
    {
        while (true)
        {
            if (Random.value < 0.7)
            {
                EnableBubble();
            }

            else if (Random.value < 0.9f)
            {
                EnableBubble();
                EnableBubble();
            }

            else if (Random.value < 0.91f)
            {
                EnableBubbleColor();
            }

            else if (Random.value < 0.92f)
            {
                EnableMagnet();
            }

            yield return new WaitForSeconds(0.2f);
        }
    }

    /// <summary>
    /// Función que genera una burbuja normal.
    /// </summary>
    void EnableBubble()
    {
        GameObject bubble = ObjectPooler.SharedInstance.GetPooledObject("Game12/Bubble");

        if (bubble != null)
        {
            bubble.transform.position = RandomPosition();
            bubble.transform.rotation = Quaternion.identity;
            bubble.SetActive(true);
        }
    }

    /// <summary>
    /// Función que genera una burbuja de color.
    /// </summary>
    void EnableBubbleColor()
    {
        if (colorMode)
        {
            return;
        }

        GameObject bubble = ObjectPooler.SharedInstance.GetPooledObject("Game12/BubbleColor");

        if (bubble != null)
        {
            bubble.transform.position = RandomPosition();
            bubble.transform.rotation = Quaternion.identity;
            bubble.SetActive(true);
        }
    }

    /// <summary>
    /// Función que genera un imán.
    /// </summary>
    void EnableMagnet()
    {
        if (magnetMode)
        {
            return;
        }

        GameObject magnet = ObjectPooler.SharedInstance.GetPooledObject("Game12/Magnet");

        if (magnet != null)
        {
            magnet.transform.position = RandomPosition();
            magnet.transform.rotation = Quaternion.identity;
            magnet.SetActive(true);
        }
    }

    /// <summary>
    /// Corrutina que genera constantemente pinchos.
    /// </summary>
    /// <returns></returns>
    IEnumerator InstantiateSpikes()
    {
        while (true)
        {
            EnableSpike();

            yield return new WaitForSeconds(0.5f);
        }
    }

    /// <summary>
    /// Función que genera los pinchos.
    /// </summary>
    void EnableSpike()
    {
        GameObject spike = RandomSpike();

        if (spike != null)
        {
            spike.transform.position = RandomPosition();
            spike.transform.rotation = Quaternion.identity;
            spike.SetActive(true);
        }
    }

    /// <summary>
    /// Función que decide aleatoriamente uno de los posibles tipos de pinchos que se van a generar.
    /// </summary>
    /// <returns>Uno de los cinco posibles tipos de pinchos.</returns>
    GameObject RandomSpike()
    {
        switch (Random.Range(1, 6))
        {
            case 1:
                return ObjectPooler.SharedInstance.GetPooledObject("Game12/Spike1");
            case 2:
                return ObjectPooler.SharedInstance.GetPooledObject("Game12/Spike2");
            case 3:
                return ObjectPooler.SharedInstance.GetPooledObject("Game12/Spike3");
            case 4:
                return ObjectPooler.SharedInstance.GetPooledObject("Game12/Spike4");
            default:
                return ObjectPooler.SharedInstance.GetPooledObject("Game12/Spike5");
        }
    }

    /// <summary>
    /// Función que aumenta la puntuación.
    /// </summary>
    /// <param name="increase">La cantidad en que aumenta la puntuación.</param>
    public void UpdateScore(int increase)
    {
        score += increase;
        scoreText.text = score.ToString();
    }

    /// <summary>
    /// Función que carga la máxima puntuación guardada en el dispositivo.
    /// </summary>
    void LoadHighScore()
    {
        highScore = SaveManager.saveManager.score12;
        highScoreText.text = highScore.ToString();
        highScoreMenuText.text = "High Score: " + highScore.ToString();
    }

    /// <summary>
    /// Función que guarda la máxima puntuación en el dispositivo.
    /// </summary>
    public void SaveHighScore()
    {
        if (score > highScore)
        {
            SaveManager.saveManager.score12 = score;
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

}