using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Clase que controla el cronómetro del Game 11.
/// </summary>
public class Timer11 : MonoBehaviour
{
    /// <summary>
    /// El momento en el que empieza a contar el cronómetro.
    /// </summary>
    float startTime;

    /// <summary>
    /// El tiempo transcurrido desde que se puso en marcha el cronómetro.
    /// </summary>
    float timerControl;
    /// <summary>
    /// Los segundos del contador.
    /// </summary>
    string secs;
    /// <summary>
    /// Los milisegundos del contador.
    /// </summary>
    string millisecs;

    /// <summary>
    /// El panel donde aparecen los valores del cronómetro.
    /// </summary>
    [SerializeField] Text timerText = null;

    void OnEnable()
    {
        startTime = Time.time;
    }

    void Update()
    {
        timerControl = Time.time - startTime;
        secs = ((int)timerControl).ToString();
        millisecs = ((int)(timerControl * 100) % 100).ToString("00");

        timerText.text = string.Format("{00}:{01}", secs, millisecs);
    }

    /// <summary>
    /// Función que reinicia el cronómetro tras cada vuelta.
    /// </summary>
    public void ResetTimer()
    {
        GameManager11.manager.SaveHighScore(int.Parse(secs), int.Parse(millisecs));

        startTime = Time.time;
    }
}