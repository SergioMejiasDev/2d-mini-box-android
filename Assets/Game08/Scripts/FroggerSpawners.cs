using System.Collections;
using System.Linq;
using UnityEngine;

/// <summary>
/// Clase encargada de generar todos los objetos de la escena.
/// </summary>
public class FroggerSpawners : MonoBehaviour
{
    /// <summary>
    /// La posición donde aparecerán los coches (tipo 1).
    /// </summary>
    [Header("Cars")]
    [SerializeField] Vector2[] car1initialSpawn = null;
    /// <summary>
    /// La posición donde aparecerán los coches (tipo 2).
    /// </summary>
    [SerializeField] Vector2[] car2initialSpawn = null;
    /// <summary>
    /// La posición donde aparecerán los coches (tipo 3).
    /// </summary>
    [SerializeField] Vector2[] car3initialSpawn = null;
    /// <summary>
    /// La posición donde aparecerán los coches (tipo 4).
    /// </summary>
    [SerializeField] Vector2[] car4initialSpawn = null;
    /// <summary>
    /// La posición donde aparecerán los coches (tipo 5).
    /// </summary>
    [SerializeField] Vector2[] car5initialSpawn = null;

    /// <summary>
    /// Función que inicia todas las corrutinas para generar los objetos.
    /// </summary>
    public void StartSpawns()
    {
        StopAllCoroutines();

        CleanScene();

        StartCoroutine(SpawnCar1());
        StartCoroutine(SpawnCar2());
        StartCoroutine(SpawnCar3());
        StartCoroutine(SpawnCar4());
        StartCoroutine(SpawnCar5());

        StartCoroutine(SpawnTrunk1());
        StartCoroutine(SpawnTrunk2());
        StartCoroutine(SpawnTrunk3());

        StartCoroutine(SpawnTurtle1());
        StartCoroutine(SpawnTurtle2());
    }

    /// <summary>
    /// Función que elimina todos los objetos de la escena.
    /// </summary>
    void CleanScene()
    {
        GameObject[] car1 = GameObject.FindGameObjectsWithTag("Game8/Car1");
        GameObject[] car2 = GameObject.FindGameObjectsWithTag("Game8/Car2");
        GameObject[] car3 = GameObject.FindGameObjectsWithTag("Game8/Car3");
        GameObject[] car4 = GameObject.FindGameObjectsWithTag("Game8/Car4");
        GameObject[] car5 = GameObject.FindGameObjectsWithTag("Game8/Car5");
        GameObject[] trunk1 = GameObject.FindGameObjectsWithTag("Game8/Trunk1");
        GameObject[] trunk2 = GameObject.FindGameObjectsWithTag("Game8/Trunk2");
        GameObject[] trunk3 = GameObject.FindGameObjectsWithTag("Game8/Trunk3");
        GameObject[] crocodile = GameObject.FindGameObjectsWithTag("Game8/Crocodile");
        GameObject[] turtle1 = GameObject.FindGameObjectsWithTag("Game8/Turtle1");
        GameObject[] turtle2 = GameObject.FindGameObjectsWithTag("Game8/Turtle2");
        GameObject[] turtle3 = GameObject.FindGameObjectsWithTag("Game8/Turtle3");
        GameObject[] turtle4 = GameObject.FindGameObjectsWithTag("Game8/Turtle4");

        GameObject[] allObjects = car1.Concat(car2).Concat(car3).Concat(car4).Concat(car5).
            Concat(trunk1).Concat(trunk2).Concat(trunk3).Concat(crocodile).
            Concat(turtle1).Concat(turtle2).Concat(turtle3).Concat(turtle4).ToArray();

        for (int i = 0; i < allObjects.Length; i++)
        {
            allObjects[i].SetActive(false);
        }
    }

    #region Spawn Coroutines

    /// <summary>
    /// Corrutina que genera continuamente coches (modelo 1).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCar1()
    {
        foreach (Vector2 position in car1initialSpawn)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car1");

            if (car != null)
            {
                car.transform.position = position;
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }
        }

        while (true)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car1");

            if (car != null)
            {
                car.transform.position = new Vector2(3.5f, -2.85f);
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(2f, 3f));
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente coches (modelo 2).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCar2()
    {
        foreach (Vector2 position in car2initialSpawn)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car2");

            if (car != null)
            {
                car.transform.position = position;
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }
        }

        while (true)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car2");

            if (car != null)
            {
                car.transform.position = new Vector2(-3.5f, -2.35f);
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(2f, 3f));
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente coches (modelo 3).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCar3()
    {
        foreach (Vector2 position in car3initialSpawn)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car3");

            if (car != null)
            {
                car.transform.position = position;
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }
        }

        while (true)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car3");

            if (car != null)
            {
                car.transform.position = new Vector2(3.5f, -1.85f);
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(2f, 3f));
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente coches (modelo 4).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCar4()
    {
        foreach (Vector2 position in car4initialSpawn)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car4");

            if (car != null)
            {
                car.transform.position = position;
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }
        }

        while (true)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car4");

            if (car != null)
            {
                car.transform.position = new Vector2(-3.5f, -1.35f);
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(1f, 3f));
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente coches (modelo 5).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnCar5()
    {
        foreach (Vector2 position in car5initialSpawn)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car5");

            if (car != null)
            {
                car.transform.position = position;
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }
        }

        while (true)
        {
            GameObject car = ObjectPooler.SharedInstance.GetPooledObject("Game8/Car5");

            if (car != null)
            {
                car.transform.position = new Vector2(3.5f, -0.85f);
                car.transform.rotation = Quaternion.identity;
                car.SetActive(true);
            }

            yield return new WaitForSeconds(3);
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente troncos (modelo 1).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTrunk1()
    {
        while (true)
        {
            GameObject trunk = ObjectPooler.SharedInstance.GetPooledObject("Game8/Trunk1");

            if (trunk != null)
            {
                trunk.transform.position = new Vector2(-4.5f, 0.65f);
                trunk.transform.rotation = Quaternion.identity;
                trunk.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(3f, 4f));
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente troncos (modelo 2).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTrunk2()
    {
        while (true)
        {
            GameObject trunk;

            if (Random.value < 0.75f)
            {
                trunk = ObjectPooler.SharedInstance.GetPooledObject("Game8/Trunk2");
            }

            else
            {
                trunk = ObjectPooler.SharedInstance.GetPooledObject("Game8/Crocodile");
            }

            if (trunk != null)
            {
                trunk.transform.position = new Vector2(-4.5f, 2.15f);
                trunk.transform.rotation = Quaternion.identity;
                trunk.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(3f, 5f));
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente troncos (modelo 3).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTrunk3()
    {
        while (true)
        {
            GameObject trunk = ObjectPooler.SharedInstance.GetPooledObject("Game8/Trunk3");

            if (trunk != null)
            {
                trunk.transform.position = new Vector2(-4.5f, 1.15f);
                trunk.transform.rotation = Quaternion.identity;
                trunk.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(3f, 6f));
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente tortugas (modelo 1).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTurtle1()
    {
        while (true)
        {
            GameObject turtle;

            if (Random.value < 0.75f)
            {
                turtle = ObjectPooler.SharedInstance.GetPooledObject("Game8/Turtle1");
            }

            else
            {
                turtle = ObjectPooler.SharedInstance.GetPooledObject("Game8/Turtle3");
            }

            if (turtle != null)
            {
                turtle.transform.position = new Vector2(4.5f, 0.15f);
                turtle.transform.rotation = Quaternion.identity;
                turtle.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(1.5f, 2f));
        }
    }

    /// <summary>
    /// Corrutina que genera continuamente tortugas (modelo 2).
    /// </summary>
    /// <returns></returns>
    IEnumerator SpawnTurtle2()
    {
        while (true)
        {
            GameObject turtle;

            if (Random.value < 0.75f)
            {
                turtle = ObjectPooler.SharedInstance.GetPooledObject("Game8/Turtle2");
            }

            else
            {
                turtle = ObjectPooler.SharedInstance.GetPooledObject("Game8/Turtle4");
            }

            if (turtle != null)
            {
                turtle.transform.position = new Vector2(4.5f, 1.65f);
                turtle.transform.rotation = Quaternion.identity;
                turtle.SetActive(true);
            }

            yield return new WaitForSeconds(Random.Range(1.5f, 2f));
        }
    }

    #endregion
}