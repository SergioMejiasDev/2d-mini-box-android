using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Clase que controla las funciones del menú principal.
/// </summary>
public class GameManagerMenu : MonoBehaviour
{
    #region Variables

    /// <summary>
    /// Los paneles que pueden abrirse dentro del menú.
    /// </summary>
    [Header("Panels")]
    [SerializeField] GameObject[] panels = null;

    /// <summary>
    /// Paneles con los nombres de los juegos disponibles,
    /// </summary>
    [Header("Games")]
    [SerializeField] GameObject[] games = null;
    /// <summary>
    /// El juego que está seleccionado.
    /// </summary>
    int activeGame = 0;

    /// <summary>
    /// Volumen del juego.
    /// </summary>
    [Header("Volume")]
    int volume;
    /// <summary>
    /// Texto que indica el volumen actual del juego.
    /// </summary>
    [SerializeField] Text volumeText = null;
    /// <summary>
    /// Flecha izquierda para controlar el volumen.
    /// </summary>
    [SerializeField] GameObject volumeLeftArrow = null;
    /// <summary>
    /// Flecha derecha para controlar el volumen.
    /// </summary>
    [SerializeField] GameObject volumeRightArrow = null;

    /// <summary>
    /// Los diferentes paneles que se van a abrir durante los créditos.
    /// </summary>
    [Header("Credits")]
    [SerializeField] GameObject[] creditsPanels = null;
    /// <summary>
    /// El panel de los créditos que está activo en este momento.
    /// </summary>
    int activeCredits = 0;

    #endregion

    private void Start()
    {
        LetterBoxer.AddLetterBoxingCamera();

        CheckVolume();

        // Si es la primera vez que se inicia la aplicación, se lanza un panel para elegir el idioma.

        if (!SaveManager.saveManager.firstTimeLanguage)
        {
            OpenPanel(panels[5]);
        }
    }

    /// <summary>
    /// Función llamada para cargar un nuevo juego (escena).
    /// </summary>
    /// <param name="buildIndex">Número de la escena que se va a cargar.</param>
    public void LoadGame(int buildIndex)
    {
        SceneManager.LoadScene(buildIndex);
    }

    /// <summary>
    /// Función que abre un juego aleatorio.
    /// </summary>
    public void LoadRandomGame()
    {
        SceneManager.LoadScene(Random.Range(1, SceneManager.sceneCountInBuildSettings));
    }

    /// <summary>
    /// Función usada para navegar entre los paneles del menú.
    /// </summary>
    /// <param name="panel">El panel que se va a abrir.</param>
    public void OpenPanel(GameObject panel)
    {
        for (int i = 0; i < panels.Length; i++)
        {
            panels[i].SetActive(false);
        }
        
        panel.SetActive(true);
    }

    /// <summary>
    /// Función que cambia el juego activo en el menú.
    /// Se llama a través de las flechas junto al nombre del juego.
    /// </summary>
    /// <param name="leftArrow">Verdadero si es la flecha izquierda, falso si es la flecha derecha.</param>
    public void ArrowGames(bool leftArrow)
    {
        if (leftArrow) // Flecha izquierda.
        {
            activeGame -= 1;

            if (activeGame < 0)
            {
                activeGame = games.Length - 1;
            }
        }

        else // Flecha derecha.
        {
            activeGame += 1;

            if (activeGame >= games.Length)
            {
                activeGame = 0;
            }
        }

        for (int i = 0; i < games.Length; i++)
        {
            games[i].SetActive(false);
        }

        games[activeGame].SetActive(true);
    }

    /// <summary>
    /// Cierra la aplicación completamente.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    /// <summary>
    /// Función a la que se llama para comprobar el volumen del juego.
    /// </summary>
    void CheckVolume()
    {
        LoadOptions();

        AudioListener.volume = (volume / 100f);

        if (volume > 0) // Si el volumen es superior a 0, aparecerá el porcentaje en la pantalla.
        {
            volumeText.text = volume.ToString() + "%";

            if (volume >= 100)
            {
                volumeRightArrow.SetActive(false);
            }
        }

        else // Si el volumen es 0, en lugar del porcentaje aparecerá el mensaje de "OFF".
        {
            volumeLeftArrow.SetActive(false);
            volumeText.text = "OFF";
        }
    }

    /// <summary>
    /// Función que modifica el volumen del juego.
    /// </summary>
    /// <param name="leftArrow">Verdadero si estamos bajando el volumen (flecha izquierda).</param>
    public void VolumeManager(bool leftArrow)
    {
        if (leftArrow) // Flecha izquierda (baja el volumen).
        {
            volume -= 5;
            AudioListener.volume = (volume / 100f);
            volumeRightArrow.SetActive(true);

            if (volume > 0)
            {
                volumeText.text = volume.ToString() + "%";
            }

            else
            {
                volumeLeftArrow.SetActive(false);
                volumeText.text = "OFF";
            }
        }

        else // Flecha derecha (sube el volumen).
        {
            volume += 5;
            AudioListener.volume = (volume / 100f);
            volumeLeftArrow.SetActive(true);
            volumeText.text = volume.ToString() + "%";


            if (volume >= 100)
            {
                volumeRightArrow.SetActive(false);
            }
        }
    }

    /// <summary>
    /// Función que carga los datos guardados relativos al volumen.
    /// </summary>
    void LoadOptions()
    {
        volume = SaveManager.saveManager.gameVolume;
    }

    /// <summary>
    /// Función que guarda los datos relativos al volumen.
    /// </summary>
    public void SaveOptions()
    {
        SaveManager.saveManager.gameVolume = volume;
        SaveManager.saveManager.SaveOptions();
    }

    /// <summary>
    /// Función que permite cambiar el idioma desde el menú de opciones.
    /// </summary>
    /// <param name="newLanguage">El código del idioma que queremos activar.</param>
    public void ChangeLanguage(string newLanguage)
    {
        MultilanguageManager.multilanguageManager.ChangeLanguage(newLanguage);
    }

    /// <summary>
    /// Función encargada de borrar todas las puntuaciones guardadas.
    /// </summary>
    public void ClearHighScores()
    {
        SaveManager.saveManager.DeleteScores();
    }

    /// <summary>
    /// Función que nos permite desplazarnos por los paneles de los créditos.
    /// </summary>
    public void ArrowCredits()
    {
        activeCredits += 1;

        for (int i = 0; i < creditsPanels.Length; i++)
        {
            creditsPanels[i].SetActive(false);
        }

        if (activeCredits == 3)
        {
            activeCredits = 0;
            OpenPanel(panels[0]);
        }

        creditsPanels[activeCredits].SetActive(true);
    }

    /// <summary>
    /// Función llamada para abrir un link externo.
    /// </summary>
    /// <param name="link">Link que queremos abrir, dentro de los que aparecen en la función.</param>
    public void OpenURL(string link)
    {
        switch (link)
        {
            case "GooglePlay": // Enlace de la aplicación en la Google Play Store.
                Application.OpenURL("https://play.google.com/store/apps/details?id=com.SergioMejias.MiniBox2D");
                break;
            case "ItchIo": // Enlace de la aplicación en Itch.
                Application.OpenURL("https://sergiomejias.itch.io/2d-mini-box");
                break;
            case "GitLab": // Enlace al código fuente de la aplicación en GitLab.
                Application.OpenURL("https://gitlab.com/SergioMejiasDev/2d-mini-box-android");
                break;
        }
    }
}