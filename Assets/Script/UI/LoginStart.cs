using System.Collections;
using UnityEngine;

public class LoginStart : MonoBehaviour
{

    void Start()
    {
        UIManager.ShowUI("LoginBg");
        UIManager.ShowUI("LogInYellow");
        StartCoroutine(WaitAllUI());
    }

    IEnumerator WaitAllUI()
    {
        while (!UIManager.IsAllLoaded())
        {
            yield return null;
        }

        EventCenter.Instance.EventTrigger("AllUIReady");
    }
}
