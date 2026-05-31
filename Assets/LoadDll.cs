using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class LoadDll : MonoBehaviour
{
    // 随便填一个你确定存在的 key
    private const string TestKey = "Assets/Image/start_load_bg 1.png";

    private AsyncOperationHandle _initHandle;

    //IEnumerator Start()
    //{
    //    Debug.Log("=== 开始测试远端服务器是否可访问 ===");

    //    // 1. 初始化（不保存句柄，不手动 Release）
    //    yield return Addressables.InitializeAsync();
    //    Debug.Log("初始化完成，准备下载测试资源");

    //    // 2. 用 Completed 回调，完全不读 .Status（最安全）
    //    AsyncOperationHandle downloadHandle = Addressables.DownloadDependenciesAsync(TestKey, true);
    //    downloadHandle.Completed += OnDownloadDone;
    //    yield return downloadHandle;
    //}

    //void OnDownloadDone(AsyncOperationHandle handle)
    //{
    //    if (handle.Status == AsyncOperationStatus.Succeeded)
    //    {
    //        Debug.Log("<color=green>✅ 远端服务器访问成功！</color>");
    //    }
    //    else
    //    {
    //        Debug.LogError($"❌ 下载失败：{handle.OperationException}");
    //    }

    //    // 释放
    //    Addressables.Release(handle);
    //}
}