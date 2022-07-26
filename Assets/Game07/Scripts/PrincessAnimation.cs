using System.Collections;
using UnityEngine;

/// <summary>
/// Clase que controla la animación de la princesa.
/// </summary>
public class PrincessAnimation : MonoBehaviour
{
    /// <summary>
    /// El mensaje de "Help!" que aparece sobre la princesa.
    /// </summary>
    [SerializeField] GameObject helpMessage = null;

    /// <summary>
    /// El componente Animator de la princesa.
    /// </summary>
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
    /// Función que se llama a través del delegado para detener la animación de la princesa.
    /// </summary>
    void StopAnimation()
    {
        StopAllCoroutines();

        anim.SetBool("Movement", false);
        helpMessage.SetActive(false);
    }

    /// <summary>
    /// Función que se llama a través del delegado para resetear la escena.
    /// </summary>
    void ResetScene()
    {
        helpMessage.SetActive(false);

        StartCoroutine(Animate());
    }

    /// <summary>
    /// Corrutina que sincroniza las animaciones de la princesa con el cartel de "Help!".
    /// </summary>
    /// <returns></returns>
    IEnumerator Animate()
    {
        while (true)
        {
            yield return new WaitForSeconds(4);

            anim.SetBool("Movement", true);
            helpMessage.SetActive(true);

            yield return new WaitForSeconds(2);

            anim.SetBool("Movement", false);
            helpMessage.SetActive(false);
        }
    }
}