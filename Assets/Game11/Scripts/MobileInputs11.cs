using UnityEngine;

/// <summary>
/// Clase con el delegado que activa los inputs en el Game 11.
/// </summary>
public class MobileInputs11 : MonoBehaviour
{
    public delegate void Inputs11Delegate(int direction);
    public static event Inputs11Delegate Button;

    /// <summary>
    /// Función que se activa al pulsar los botones en la pantalla.
    /// </summary>
    /// <param name="buttonPressed">1 arriba, 2 derecha, 3 abajo, 4 izquierda, 5 y 6 dejar de pulsar los botones.</param>
    public void PressButtons(int buttonPressed)
    {
        if (Button != null)
        {
            Button(buttonPressed);
        }
    }
}