using UnityEngine;

/// <summary>
/// Clase con el delegado que activa los inputs en el Game 02.
/// </summary>
public class MobileInputs2 : MonoBehaviour
{
    public delegate void Inputs2Delegate(int direction);
    public static event Inputs2Delegate Button;

    /// <summary>
    /// Función que se activa cuando se pulsa algún botón en la pantalla.
    /// </summary>
    /// <param name="buttonPressed">1 arriba, 3 abajo, 5 no se pulsa nada.</param>
    public void PressButtons(int buttonPressed)
    {
        if (Button != null)
        {
            Button(buttonPressed);
        }
    }
}