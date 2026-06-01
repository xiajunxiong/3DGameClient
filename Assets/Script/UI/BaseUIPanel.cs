using UnityEngine;

public class BaseUIPanel : MonoBehaviour
{
    public string uiName;
    public UILayer layer;

    public virtual void Show() { gameObject.SetActive(true); }
    public virtual void Hide() { gameObject.SetActive(false); }
    public virtual void Die() {Destroy(gameObject); }
}
public enum UILayer
{
    Background,   // 背景层（最底下）
    Normal,       // 普通界面（默认）
    Popup,        // 弹窗（在普通上面）
    Top           // 最顶层（提示、Loading）
}