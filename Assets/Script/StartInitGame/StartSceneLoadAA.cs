using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;
using UnityEngine.Localization.Metadata;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartSceneLoadAA : MonoBehaviour
{
    public Image startBg;
    public TextMeshProUGUI progress_percent_text;
    public TextMeshProUGUI progress_text;
    public Image load_progress_bar;

    public LocalizedStringTable m_StringTable;

    private StringTable _table;
    private long _totalDownloadSize = 0;
    private DateTime _lastTime;
    private long _lastBytesDownloaded = 0;

    //void OnEnable()
    //{
    //    m_StringTable.TableChanged += LoadStrings;
    //}

    //void OnDisable()
    //{
    //    m_StringTable.TableChanged -= LoadStrings;
    //}

    //void LoadStrings(StringTable stringTable)
    //{
    //    m_TranslatedStringHello = GetLocalizedString(stringTable, "Hello");
    //    m_TranslatedStringGoodbye = GetLocalizedString(stringTable, "Goodbye");
    //    m_TranslatedStringThisIsATest = GetLocalizedString(stringTable, "This is a test");
    //}

    IEnumerator Start()
    {
        var zhLocale = LocalizationSettings.AvailableLocales.GetLocale("zh-Hans");
        LocalizationSettings.SelectedLocale = zhLocale;

        var tableOp = m_StringTable.GetTableAsync();
        yield return tableOp;
        _table = tableOp.Result;
        if (_table == null)
        {
            Debug.LogError("多语言表加载失败，请检查 m_StringTable 的配置！");
            progress_text.text = "语言表加载失败";
            yield break;
        }

        yield return Addressables.InitializeAsync();

        progress_text.text = GetLocalizedString(_table, "检测版本更新");

        var checkHandle = Addressables.CheckForCatalogUpdates(false);
        yield return checkHandle;
        if (checkHandle.Result.Count > 0)
        {
            var updateHandle = Addressables.UpdateCatalogs(checkHandle.Result);
            yield return updateHandle;
            Addressables.Release(updateHandle);
        }
        Addressables.Release(checkHandle);

        List<string> allLabels = new List<string> { "Image", "Dll", "Scene" };
        var locHandle = Addressables.LoadResourceLocationsAsync(allLabels, Addressables.MergeMode.Union);
        yield return locHandle;
        IList<IResourceLocation> locations = locHandle.Result;
        Addressables.Release(locHandle);

        var sizeHandle = Addressables.GetDownloadSizeAsync(locations);
        yield return sizeHandle;
        _totalDownloadSize = sizeHandle.Result;
        Addressables.Release(sizeHandle);

        if (_totalDownloadSize == 0)
        {
            progress_percent_text.text = "100%";
            load_progress_bar.fillAmount = 1f;
            progress_text.text = GetLocalizedString(_table, "已经是最新版本");
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            float sizeMB = _totalDownloadSize / (1024f * 1024f);
            progress_text.text = $"{GetLocalizedString(_table, "本次更新大小")} {sizeMB:F2} MB";

            var downloadHandle = Addressables.DownloadDependenciesAsync(locations);
            _lastTime = DateTime.Now;
            _lastBytesDownloaded = 0;

            while (!downloadHandle.IsDone)
            {
                var status = downloadHandle.GetDownloadStatus();
                float p = status.Percent;

                load_progress_bar.fillAmount = p;
                progress_percent_text.text = $"{p * 100:F1}%";

                float speed = 0;
                var now = DateTime.Now;
                if ((now - _lastTime).TotalSeconds >= 1)
                {
                    long diff = status.DownloadedBytes - _lastBytesDownloaded;
                    speed = diff / 1024f;
                    _lastBytesDownloaded = status.DownloadedBytes;
                    _lastTime = now;
                }

                progress_text.text = $"{GetLocalizedString(_table, "本次更新大小")} {sizeMB:F2} MB  " +
                                    $"{GetLocalizedString(_table, "当前下载速度")} {speed:F1} KB/s";

                yield return null;
            }

            if (downloadHandle.Status == AsyncOperationStatus.Succeeded)
            {
                progress_percent_text.text = "100%";
                load_progress_bar.fillAmount = 1f;
                progress_text.text = GetLocalizedString(_table, "已经是最新版本");
                yield return new WaitForSeconds(0.5f);
            }
            else
            {
                progress_text.text = GetLocalizedString(_table, "下载异常");
            }

            Addressables.Release(downloadHandle);
        }

        string sceneAddress = "Assets/Scenes/login.unity";
        var sceneLoadHandle = Addressables.LoadSceneAsync(
            sceneAddress,
            LoadSceneMode.Single,
            true 
        );
        yield return sceneLoadHandle;

        if (sceneLoadHandle.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogError("登录场景加载失败：" + sceneLoadHandle.OperationException);
        }
    }

    static string GetLocalizedString(StringTable table, string entryName)
    {

        var entry = table.GetEntry(entryName);
        if (entry == null)
        {
            Debug.LogError($"多语言表中未找到 Key：{entryName}，请检查拼写或表配置");
            return entryName;
        }

        var comment = entry.GetMetadata<Comment>();

        if (comment != null)
        {
            Debug.Log($"Found metadata comment for {entryName} - {comment.CommentText}");
        }

        return entry.GetLocalizedString();
    }
}