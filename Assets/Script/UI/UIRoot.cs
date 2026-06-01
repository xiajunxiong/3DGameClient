using UnityEngine;
using UnityEngine.UIElements;

public class UIRoot : MonoBehaviour
{
    public static UIRoot Instance;
    void Awake()
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

        UIManager.UIBackground = transform.Find("UIBackground");
        UIManager.UINormal = transform.Find("UINormal");
        UIManager.UIPopup = transform.Find("UIPopup");
        UIManager.UITop = transform.Find("UITop");
    }
}