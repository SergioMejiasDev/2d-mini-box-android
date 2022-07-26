using System.Collections;
using UnityEngine;

/// <summary>
/// Clase usada por el generador de monedas.
/// </summary>
public class CoinGenerator : MonoBehaviour
{
    void OnEnable()
    {
        StartCoroutine(SpawnCoins());
    }


    /// <summary>
    /// Calcula una posición aleatoria donde generar la moneda dentro de los rangos indicados.
    /// </summary>
    /// <returns>Vector2 con una posición aleatoria dentro del escenario.</returns>
    Vector2 SpawnPosition()
    {
        switch (Random.Range(1, 7))
        {
            case 1:
                return new Vector2(Random.Range(-7.65f, -3.13f), 4.5f);
            case 2:
                return new Vector2(Random.Range(3.13f, 7.65f), 4.5f);
            case 3:
                return new Vector2(Random.Range(-1.75f, 1.75f), 2f);
            case 4:
                return new Vector2(Random.Range(-8.8f, -3f), -1f);
            case 5:
                return new Vector2(Random.Range(3f, 8.8f), -1f);
            case 6:
                return new Vector2(-1.75f, -3.5f);
            default:
                return new Vector2(1.75f, -3.5f);
        }
    }

    /// <summary>
    /// Función encargada de generar las monedas.
    /// </summary>
    void GenerateCoin()
    {
        GameObject coin = ObjectPooler.SharedInstance.GetPooledObject("Game1/Coin");

        if (coin != null)
        {
            coin.transform.position = SpawnPosition();
            coin.transform.rotation = Quaternion.identity;
            coin.SetActive(true);
        }
    }

    /// <summary>
    /// Corrutina que genera una nueva moneda tras pasar unos segundos.
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCoins()
    {
        yield return new WaitForSeconds(1);

        while (true)
        {
            GenerateCoin();

            yield return new WaitForSeconds(Random.Range(5, 10));
        }
    }
}