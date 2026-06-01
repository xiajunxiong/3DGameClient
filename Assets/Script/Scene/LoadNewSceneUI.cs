using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public struct SceneLoadState
{
    // 场景是否加载完成
    public bool sceneLoaded;

    // UI是否全部加载完成
    public bool uiReady;


    public bool IsAllReady => sceneLoaded && uiReady;
}

public class LoadNewSceneUI : MonoBehaviour
{
    public Image load_progress_bar;
    public TextMeshProUGUI percentage_text;
    public GameObject Canvas;
    public GameObject MainCamera;


    public float minNormalTime = 5f;
    public float maxNormalTime = 10f;
    public float fastFillTime = 0.5f;

    public static string targetSceneName;
    public static LoadNewSceneUI Instance;
    private AsyncOperationHandle<SceneInstance> _transSceneHandle;
    private SceneLoadState _loadState;

    private float _currentProgress;
    private bool _isFastFilling;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void OnEnable()
    {
        ResetState();
        targetSceneName = SceneLoadParams.targetSceneName;

        EventCenter.Instance.AddEventListener("AllUIReady", OnAllUIReady);
        StartLoading();
    }

    private void OnDisable()
    {
        EventCenter.Instance.RemoveEventListener("AllUIReady", OnAllUIReady);
    }

    private void StartLoading()
    {
        StartCoroutine(LoadSceneAsync());
        StartCoroutine(NormalProgressBar());
    }

    private IEnumerator LoadSceneAsync()
    {
        string sceneAddress = $"Assets/Scenes/{targetSceneName}.unity";
        _transSceneHandle = Addressables.LoadSceneAsync(sceneAddress, LoadSceneMode.Additive, true);
        yield return _transSceneHandle;

        _loadState.sceneLoaded = true;
        CheckAllReady();
    }

    private IEnumerator NormalProgressBar()
    {
        float totalTime = Random.Range(minNormalTime, maxNormalTime);
        float timer = 0;

        while (timer < totalTime && !_isFastFilling)
        {
            timer += Time.deltaTime;
            _currentProgress = timer / totalTime;
            UpdateProgressUI();
            yield return null;
        }
    }


    void OnAllUIReady()
    {
        _loadState.uiReady = true;
        CheckAllReady();
    }

    private void CheckAllReady()
    {
        if (_loadState.IsAllReady)
        {
            StopAllCoroutines();
            StartCoroutine(FastFillProgress());
        }
    }

    private IEnumerator FastFillProgress()
    {
        _isFastFilling = true;
        float from = _currentProgress;
        float timer = 0;

        while (timer < fastFillTime)
        {
            timer += Time.deltaTime;
            _currentProgress = Mathf.Lerp(from, 1f, timer / fastFillTime);
            UpdateProgressUI();
            yield return null;
        }

        _currentProgress = 1f;
        UpdateProgressUI();

        yield return _transSceneHandle.Result.ActivateAsync();
        // 隐藏当前场景

        yield return new WaitForEndOfFrame();
        MainCamera.SetActive(false);
        Canvas.SetActive(false);
    }

    private void UpdateProgressUI()
    {
        load_progress_bar.fillAmount = _currentProgress;
        percentage_text.text = $"{Mathf.FloorToInt(_currentProgress * 100)}%";
    }

    private void ResetState()
    {
        _loadState = new SceneLoadState { sceneLoaded = false, uiReady = false };
        MainCamera.SetActive(true);
        Canvas.SetActive(true);
    }
}