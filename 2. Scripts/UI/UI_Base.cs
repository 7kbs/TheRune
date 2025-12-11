using UnityEngine;

public abstract class UI_Base : MonoBehaviour
{
    public virtual void OnOpen() { }
    public virtual void OnClose() { }

    // UI 닫을 때 UIManager에 자동 보고
    public void Close()
    {
        UIManager.inst.CloseUI(this);
    }
}