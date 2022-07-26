using UnityEngine;

/// <summary>
/// Clase encargada de modificar los textos de acuerdo con el idioma activo.
/// </summary>
public class MultilanguageManager : MonoBehaviour
{
    public static MultilanguageManager multilanguageManager;

    /// <summary>
    /// El idioma activo.
    /// </summary>
    public string activeLanguage;

    private void Awake()
    {
        // El objeto que contendrá la clase debe instanciarse una única vez, y se mantendrá en todas las escenas.
        // Si se generara un segundo objeto, se destruirá antes de activar sus funciones.

        GameObject[] objs = GameObject.FindGameObjectsWithTag("Menu/LanguageManager");

        if (objs.Length > 1)
        {
            Destroy(gameObject);
        }

        else
        {
            multilanguageManager = this;

            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        activeLanguage = SaveManager.saveManager.activeLanguage;
    }

    /// <summary>
    /// Función encargada de cambiar el idioma de todos los textos del juego.
    /// </summary>
    /// <param name="newLanguage">El código del idioma que queremos activar.</param>
    public void ChangeLanguage(string newLanguage)
    {
        activeLanguage = newLanguage;

        // Una vez que cambiamos el idioma, lo guardamos para que se active la próxima vez que abramos la aplicación.

        SaveManager.saveManager.activeLanguage = newLanguage;
        SaveManager.saveManager.firstTimeLanguage = true;
        SaveManager.saveManager.SaveOptions();
    }
}