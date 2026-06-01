using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using System;

public static class UIManager
{
    public static Transform UIBackground;
    public static Transform UINormal;
    public static Transform UIPopup;
    public static Transform UITop;

    private static Dictionary<string, BaseUIPanel> _uiCache = new();
    private static Queue<string> _uiLoadQueue = new();
    private static bool _isLoading = false;

    private const string UI_PREFAB_PATH = "Assets/Prefab/UI/";

    public static Action OnAllUILoaded;

    public static Transform GetLayer(UILayer layer)
    {
        return layer switch
        {
            UILayer.Background => UIBackground,
            UILayer.Popup => UIPopup,
            UILayer.Top => UITop,
            _ => UINormal
        };
    }

    public static void ShowUI(string uiName)
    {
        if (_uiCache.ContainsKey(uiName))
        {
            _uiCache[uiName].Show();
            return;
        }

        _uiLoadQueue.Enqueue(uiName);

        if (!_isLoading)
            ProcessLoadQueue();
    }

    private static async void ProcessLoadQueue()
    {
        if (_uiLoadQueue.Count == 0)
        {
            _isLoading = false;
            OnAllUILoaded?.Invoke();
            return;
        }

        _isLoading = true;
        string uiName = _uiLoadQueue.Dequeue();
        string fullAddress = $"{UI_PREFAB_PATH}{uiName}.prefab";

        try
        {
            var handle = Addressables.LoadAssetAsync<GameObject>(fullAddress);
            await handle.Task;

            var prefab = handle.Result;
            var newPanel = GameObject.Instantiate(prefab).GetComponent<BaseUIPanel>();
            var parent = GetLayer(newPanel.layer);
            newPanel.transform.SetParent(parent, false);
            newPanel.Show();

            _uiCache[uiName] = newPanel;
        }
        catch (Exception e)
        {
            Debug.LogError($"¼ÓÔØUIÊ§°Ü: {uiName} -> {e.Message}");
        }

        ProcessLoadQueue();
    }

    public static bool IsAllLoaded() => _uiLoadQueue.Count == 0 && !_isLoading;
    public static bool IsUILoaded(string uiName) => _uiCache.ContainsKey(uiName);

    public static void HideUI(string uiName)
    {
        if (_uiCache.TryGetValue(uiName, out var panel))
            panel.Hide();
    }
    public static void DestroyUI(string uiName)
    {
        if (_uiCache.TryGetValue(uiName, out var panel))
        {
            panel.Die();
            _uiCache.Remove(uiName);
        }
    }
    public static void ClearCache()
    {
        foreach (var panel in _uiCache.Values)
            panel.Die();
        _uiCache.Clear();
        _uiLoadQueue.Clear();
        _isLoading = false;
    }
}