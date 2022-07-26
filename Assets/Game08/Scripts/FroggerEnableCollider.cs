using UnityEngine;

/// <summary>
/// Clase que se encarga de activar y desactivar los colliders de los cocodrilos y las tortugas.
/// Estos son los colliders que pueden matar al jugador.
/// </summary>
public class FroggerEnableCollider : MonoBehaviour
{
    /// <summary>
    /// El collider al que afectará la clase.
    /// </summary>
    [SerializeField] GameObject boxCollider = null;

    /// <summary>
    /// Función que activa y desactiva los colliders.
    /// </summary>
    /// <param name="enable">1 para activar los colliders, otro valor para desactivarlos.</param>
    public void EnableColliders(int enable)
    {
        if (boxCollider == null)
        {
            return;
        }

        if (enable == 1)
        {
            boxCollider.SetActive(true);

        }

        else
        {
            boxCollider.SetActive(false);
        }
    }
}