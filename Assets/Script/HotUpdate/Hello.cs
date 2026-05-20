using System.Collections;
using UnityEngine;

public class Hello
{
    public static void Run()
    {
        Debug.Log("我是旧版Hello, HybridCLR");
    }
    public void Run(GameObject obj)
    {
        Debug.Log("我是旧版Hello, HybridCLR");
    }

    public GameObject RunReturn(GameObject obj)
    {
        Debug.Log("我是旧版Hello, HybridCLR");
        return obj;
    }
}