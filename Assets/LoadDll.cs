using System;
using System.Collections;
using System.Reflection;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadDll : MonoBehaviour
{
    public string dllKey = "Assets/HybridCLRGenerate/HotUpdateDll/HotUpdate.dll.bytes";

    IEnumerator Start()
    {
        AsyncOperationHandle<TextAsset> handle = Addressables.LoadAssetAsync<TextAsset>(dllKey);
        yield return handle;

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            byte[] dllBytes = handle.Result.bytes;
            Assembly.Load(dllBytes);
        }
        else
        {
            Debug.LogError($"热更 DLL 加载失败: {handle.OperationException}");
        }
    }
}