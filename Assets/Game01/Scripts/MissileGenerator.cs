using System.Collections;
using UnityEngine;

/// <summary>
/// Clase usada por el generador de misiles.
/// </summary>
public class MissileGenerator : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(SpawnMissiles());
    }

    /// <summary>
    /// Función que alcula una posición aleatoria donde generar los misiles.
    /// </summary>
    /// <returns>Vector 2 aleatorio donde se generarán los misiles.</returns>
    Vector2 SpawnPosition()
    {
        switch (Random.Range(1, 4))
        {
            case 1:
                return new Vector2(11f, 2.23f);
            case 2:
                return new Vector2(11f, -3.5f);
            case 3:
                return new Vector2(-11f, 2.23f);
            default:
                return new Vector2(-11f, -3.5f);
        }
    }

    /// <summary>
    /// Función que genera un misil en alguno de los lados de la pantalla.
    /// </summary>
    void GenerateMissiles()
    {
        GameObject missile = ObjectPooler.SharedInstance.GetPooledObject("Game1/Missile");

        if (missile != null)
        {
            missile.transform.position = SpawnPosition();
            missile.transform.rotation = Quaternion.identity;
            missile.SetActive(true);
        }
    }

    /// <summary>
    /// Corrutina que genera un nuevo misil cada pocos segundos.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnMissiles()
    {
        yield return new WaitForSeconds(3);

        while (true)
        {
            GenerateMissiles();

            yield return new WaitForSeconds(Random.Range(4, 7));
        }
    }
}