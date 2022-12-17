using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool
{
    // The prefab for the objects in the pool
    private GameObject prefab;

    // The list of objects in the pool
    private List<GameObject> objects;

    // The transform to parent the objects to
    private Transform parent;

    public ObjectPool(GameObject prefab, int initialSize, Transform parent)
    {
        this.prefab = prefab;
        this.parent = parent;
        objects = new List<GameObject>();

        // Populate the object pool with the initial number of objects
        for(int i = 0; i < initialSize; i++)
        {
            GameObject obj = CreateObject();
            obj.SetActive(false);
            objects.Add(obj);
        }
    }

    // Get an available object from the pool
    public GameObject GetObject()
    {
        // Look for an inactive object in the pool
        foreach(GameObject obj in objects)
        {
            if(!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // If there are no inactive objects, create a new object
        GameObject newObj = CreateObject();
        objects.Add(newObj);
        return newObj;
    }

    // Create a new object and set its parent
    private GameObject CreateObject()
    {
        GameObject obj = GameObject.Instantiate(prefab);
        obj.transform.parent = parent;
        return obj;
    }
}