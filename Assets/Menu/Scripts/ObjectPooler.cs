using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Clase encargada de gestionar el pooling de los objetos en la escena.
/// </summary>
public class ObjectPooler : MonoBehaviour
{
    [System.Serializable]
    public class ObjectPoolItem
    {
        /// <summary>
        /// Número de veces que se va a generar el objeto.
        /// </summary>
        public int amountToPool;
        /// <summary>
        /// Objeto que se va a generar.
        /// </summary>
        public GameObject objectToPool;
        /// <summary>
        /// Verdadero si se pueden generar nuevas copias del objeto.
        /// </summary>
        public bool shouldExpand;
    }

    public static ObjectPooler SharedInstance;
    public List<GameObject> pooledObjects;
    public List<ObjectPoolItem> itemsToPool;

    public GameObject GetPooledObject(string tag)
    {
        for (int i = 0; i < pooledObjects.Count; i++)
        {
            if (!pooledObjects[i].activeInHierarchy && pooledObjects[i].CompareTag(tag))
            {
                return pooledObjects[i];
            }
        }

        foreach (ObjectPoolItem item in itemsToPool)
        {
            if (item.objectToPool.CompareTag(tag))
            {
                if (item.shouldExpand)
                {
                    GameObject obj = Instantiate(item.objectToPool);
                    obj.SetActive(false);
                    pooledObjects.Add(obj);

                    return obj;
                }
            }
        }

        return null;
    }

    void Awake()
    {
        SharedInstance = this;
    }

    private void Start()
    {
        pooledObjects = new List<GameObject>();
        foreach (ObjectPoolItem item in itemsToPool)
        {
            for (int i = 0; i < item.amountToPool; i++)
            {
                GameObject obj = (GameObject)Instantiate(item.objectToPool);
                obj.SetActive(false);
                pooledObjects.Add(obj);
            }
        }
    }
}