using UnityEngine;

/// <summary>
/// Clase con el delegado que activa los inputs en el juego 01.
/// </summary>
public class MobileInputs1 : MonoBehaviour
{
    /// <summary>
    /// Delegado que activa los inputs en el juego 01.
    /// </summary>
    /// <param name="direction">1 arriba, 2 derecha, 4 izquierda, 5 desactivado.</param>
    public delegate void Inputs1Delegate(int direction);
    /// <summary>
    /// Evento que se activa cuando se pulsan los botones en la pantalla.
    /// </summary>
    public static event Inputs1Delegate Button;

    /// <summary>
    /// Función que se activa cuando se pulsan los botones en la pantalla.
    /// </summary>
    /// <param name="buttonPressed">1 arriba, 2 derecha, 4 izquierda, 5 desactivado.</param>
    public void PressButtons(int buttonPressed)
    {
        if (Button != null)
        {
            Button(buttonPressed);
        }
    }
}
