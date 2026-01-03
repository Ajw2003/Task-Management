using UnityEngine;
using System.Collections.Generic;
using System;
using Code.Scripts.Singleton;
using TMPro;
using Unity.VisualScripting;

public class InstantiateObjectManagers : SingletonBase<InstantiateObjectManagers>
{
    public GameObject objectToInstantiate;
    public Transform objectToInstantiateTransform;
    public Dictionary<string, GameObject> _objectsInstantiated = new Dictionary<string, GameObject>();
    
    public void Instantiate()
    {
        if (objectToInstantiate != null)
        {
            var obj = Instantiate(objectToInstantiate, objectToInstantiateTransform);
            _objectsInstantiated.TryAdd(obj.name, obj);
        }
        
    }

    public void AddToDictonary(string Name, GameObject obj)
    {
        _objectsInstantiated.TryAdd(Name, obj);
    }

    public void RemoveFromDictonary(string Name)
    {
        _objectsInstantiated.Remove(Name);
    }
}
