using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolerMono : MonoBehaviour
{
    public static PoolerMono Instance { get; private set; }

    Dictionary<string, List<GameObject>> objectPool = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void Push(GameObject target, int count = 5)
    {
        if (count < 1)
        {
            Debug.Log("<color=red>ERROR</color> Cant add less than 1 element");
            return;
        }

        var targetName = target.name;
        if (!objectPool.ContainsKey(targetName))
        {
            var objectsList = new List<GameObject>();

            for (int i = 0; i < count; i++)
            {
                objectsList.Add(Instantiate(target));
                objectsList[i].SetActive(false);
            }

            objectPool.Add(targetName, objectsList);
        }
    }

    public GameObject Pull(GameObject target)
    {
        GameObject obj = null;
        var targetName = target.name;

        if (objectPool.ContainsKey(targetName))
        {
            bool found = false;

            for (int i = 0; i < objectPool[targetName].Count; i++)
            {
                if (!objectPool[targetName][i].activeInHierarchy)
                {
                    obj = objectPool[targetName][i];
                    found = true;

                    break;
                }
            }

            if (!found)
                obj = IncreasePoolSize(targetName);
        }
        else
        {
            Debug.Log("<color=red>ERROR</color> No specified object in the pool");
        }

        return obj;
    }

    private GameObject IncreasePoolSize(string targetName)
    {
        GameObject obj = Instantiate(objectPool[targetName][0]);
        objectPool[targetName].Add(obj);
        obj.SetActive(false);

        return obj;
    }

    public void DeactivatePool(GameObject target)
    {
        var targetName = target.name;

        if (objectPool.ContainsKey(targetName))
        {
            foreach (var item in objectPool[targetName])
                item.SetActive(false);
        }
    }
}
