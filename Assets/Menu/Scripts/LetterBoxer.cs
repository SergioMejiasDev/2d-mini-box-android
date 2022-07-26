using UnityEngine;

/// <summary>
/// Clase que añade letterboxes al juego y corrige la resolución de la pantalla.
/// </summary>
public static class LetterBoxer
{
    /// <summary>
    /// Función que crea una cámara diferente para mostrar los letterboxes.
    /// </summary>
    public static void AddLetterBoxingCamera()
    {
        Camera letterBoxerCamera = new GameObject().AddComponent<Camera>();
        letterBoxerCamera.backgroundColor = Color.black;
        letterBoxerCamera.cullingMask = 0;
        letterBoxerCamera.depth = -100;
        letterBoxerCamera.farClipPlane = 1;
        letterBoxerCamera.useOcclusionCulling = false;
        letterBoxerCamera.allowHDR = false;
        letterBoxerCamera.allowMSAA = false;
        letterBoxerCamera.clearFlags = CameraClearFlags.Color;
        letterBoxerCamera.name = "Letter Boxer Camera";

        PerformSizing();
    }

    /// <summary>
    /// Función que adapta el tamaño de la cámara principal a la resolución indicada.
    /// </summary>
    static void PerformSizing()
    {
        Camera mainCamera = Camera.main;

        float targetRatio = 16.0f / 9.0f;

        float windowaspect = (float)Screen.width / (float)Screen.height;

        float scaleheight = windowaspect / targetRatio;

        if (scaleheight < 1.0f)
        {
            Rect rect = mainCamera.rect;

            rect.width = 1.0f;
            rect.height = scaleheight;
            rect.x = 0;
            rect.y = (1.0f - scaleheight) / 2.0f;

            mainCamera.rect = rect;
        }

        else
        {
            float scalewidth = 1.0f / scaleheight;

            Rect rect = mainCamera.rect;

            rect.width = scalewidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scalewidth) / 2.0f;
            rect.y = 0;

            mainCamera.rect = rect;
        }
    }
}