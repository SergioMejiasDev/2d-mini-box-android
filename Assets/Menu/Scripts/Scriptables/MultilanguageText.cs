using UnityEngine;

/// <summary>
/// Asset que nos va a permitir escribir los textos del juego en diferentes idiomas.
/// </summary>
[CreateAssetMenu(menuName = "My Assets/Multilanguage Text")]
public class MultilanguageText : ScriptableObject
{
    [TextArea(14, 10)] public string english = null;
    [TextArea(14, 10)] public string spanish = null;
}