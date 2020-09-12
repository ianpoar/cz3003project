using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UI : MonoBehaviour
{
    // Start is called before the first frame update
    protected virtual void Awake()
    {
        if (GameObject.Find("StaticObjects") == null)
        {
            GameObject obj = GameObject.Instantiate(Resources.Load("LoadedPrefabs/StaticObjects") as GameObject);
            obj.name = "StaticObjects";
            GameObject.DontDestroyOnLoad(obj);
        }
    }
}
