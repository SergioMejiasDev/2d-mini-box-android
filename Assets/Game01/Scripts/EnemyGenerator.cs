using System.Collections;
using UnityEngine;

/// <summary>
/// Clase usada por el generador de enemigos (tortugas).
/// </summary>
public class EnemyGenerator : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(Spawner());
    }

    /// <summary>
    /// Función encargada de generar enemigos (tortugas).
    /// </summary>
    void GenerateEnemy()
    {
        GameObject turtle = ObjectPooler.SharedInstance.GetPooledObject("Game1/Enemy");

        if (turtle != null)
        {
            turtle.transform.position = transform.position;
            turtle.transform.rotation = Quaternion.identity;
            turtle.SetActive(true);
        }
    }

    /// <summary>
    /// Corrutina que genera una nueva tortuga cada pocos segundos.
    /// </summary>
    /// <returns></returns>
    IEnumerator Spawner()
    {
        while (true)
        {
            GenerateEnemy();

            yield return new WaitForSeconds(Random.Range(3, 6));
        }
    }
}