using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que controla la animación y las acciones del Kong.
/// </summary>
public class KongMovement : MonoBehaviour
{
    /// <summary>
    /// Posición donde se van a generar los barriles que ruedan hacia la derecha.
    /// </summary>
    [Header("Barrels")]
    [SerializeField] Transform instantiateArea = null;

    /// <summary>
    /// Posición donde se van a generar los barriles que caen hacia abajo.
    /// </summary>
    [Header("Vertical Barrels")]
    [SerializeField] Transform instantiateVerticalArea = null;
    /// <summary>
    /// El prefab de los barriles que el Kong lanzará hacia abajo.
    /// </summary>
    [SerializeField] GameObject verticalBarrel = null;
    /// <summary>
    /// Verdadero si el Kong ha lanzado el primer barril hacia abajo. Falso si no lo ha lanzado aún.
    /// </summary>
    bool firstThrow = false;

    /// <summary>
    /// Componente Animator del Kong.
    /// </summary>
    [Header("Components")]
    [SerializeField] Animator anim;

    private void OnEnable()
    {
        GameManager7.Stop += StopAnimation;
        GameManager7.Reset += ResetScene;
    }

    private void OnDisable()
    {
        GameManager7.Stop -= StopAnimation;
        GameManager7.Reset -= ResetScene;
    }

    /// <summary>
    /// Función que se activa para generar un nuevo barril a la derecha del Kong.
    /// </summary>
    public void InstantiateBarrel()
    {
        GameObject barrel = ObjectPooler.SharedInstance.GetPooledObject("Game7/Barrel");

        if (barrel != null)
        {
            barrel.transform.position = instantiateArea.position;
            barrel.transform.rotation = Quaternion.identity;
            barrel.SetActive(true);
        }
    }

    /// <summary>
    /// Función que se activa para generar un nuevo barril debajo del Kong.
    /// </summary>
    public void InstantiateVerticalBarrel()
    {
        if (!firstThrow)
        {
            firstThrow = true;

            StartCoroutine(ThrowVerticalBarrel());

            return;
        }

        if (Random.value > 0.05f)
        {
            return;
        }

        StartCoroutine(ThrowVerticalBarrel());
    }

    /// <summary>
    /// Función a la que se llama desde el delegado para desactivar la animación del Kong.
    /// </summary>
    void StopAnimation()
    {
        StopAllCoroutines();

        anim.SetBool("ThrowBarrels", false);
    }

    /// <summary>
    /// Función a la que se llama desde el delegado para activar la animación del Kong.
    /// </summary>
    void ResetScene()
    {
        anim.SetBool("ThrowBarrels", true);

        firstThrow = false;
    }

    /// <summary>
    /// Corrutina que sincroniza la animación con el lanzamiento de los barriles hacia abajo.
    /// </summary>
    /// <returns></returns>
    IEnumerator ThrowVerticalBarrel()
    {
        anim.SetBool("ThrowBarrels", false);

        Instantiate(verticalBarrel, instantiateVerticalArea.position, Quaternion.identity);

        yield return new WaitForSeconds(0.25f);

        anim.SetBool("ThrowBarrels", true);
    }
}