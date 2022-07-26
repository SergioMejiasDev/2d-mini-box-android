using UnityEngine;

/// <summary>
/// Clase con el delegado que activa los inputs del Game 06.
/// </summary>
public class MobileInputs6 : MonoBehaviour
{
    public delegate void Inputs6Delegate(int direction);
    public static event Inputs6Delegate Button;

    /// <summary>
    /// Función que se activa cuando el jugador pulsa algunos de los inputs en la pantalla.
    /// </summary>
    /// <param name="buttonPressed">1 arriba, 2 derecha, 3 abajo, 4 izquierda.</param>
    public void PressButtons(int buttonPressed)
    {
        if (Button != null)
        {
            Button(buttonPressed);
        }
    }
}