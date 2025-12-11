using UnityEngine;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager inst;

    [SerializeField] Transform rootLayer;

    Stack<UI_Base> uiStack = new Stack<UI_Base>();

    void Awake()
    {
        inst = this;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEscape();

        if (Input.GetKeyDown(KeyCode.I))
            OpenSingleUI("UI_Inventory");

        if (Input.GetKeyDown(KeyCode.K))
            OpenSingleUI("UI_Skill_Re");

        if (Input.GetKeyDown(KeyCode.J))
            OpenSingleUI("UI_Quest");
    }

    // --------------------------------------------------------------------
    // UI 하나만 열기 (중복 방지 + 열기 전 기존 UI 싹 제거)
    // --------------------------------------------------------------------
    public UI_Base OpenSingleUI(string uiName)
    {
        // 기존 UI 모두 닫기
        CloseAllUI();

        // 새 UI 오픈
        return OpenUI(uiName);
    }

    // --------------------------------------------------------------------
    // UI 열기 (스택에 하나 추가)
    // --------------------------------------------------------------------
    public UI_Base OpenUI(string uiName)
    {
        var prefab = Resources.Load<UI_Base>("UI_Path/" + uiName);
        var ui = Instantiate(prefab, rootLayer);

        uiStack.Push(ui);
        ui.OnOpen();
        return ui;
    }

    // --------------------------------------------------------------------
    // UI 닫기 (스택 최상단만 닫도록 설계됨)
    // --------------------------------------------------------------------
    public void CloseUI(UI_Base ui)
    {
        if (uiStack.Count == 0) return;

        if (uiStack.Peek() == ui)
        {
            uiStack.Pop();
            ui.OnClose();
            Destroy(ui.gameObject);
        }
        else
        {
            Debug.LogWarning("UI Stack mismatch");
        }
    }

    // --------------------------------------------------------------------
    // 모든 UI 제거
    // --------------------------------------------------------------------
    public void CloseAllUI()
    {
        while (uiStack.Count > 0)
        {
            var top = uiStack.Pop();
            top.OnClose();
            Destroy(top.gameObject);
        }
    }

    // --------------------------------------------------------------------
    // ESC 핸들링
    // --------------------------------------------------------------------
    void HandleEscape()
    {
        if (uiStack.Count > 0)
        {
            // 스택 최상단 UI 닫기
            uiStack.Peek().Close();
            return;
        }

        // UI가 **완전히 없을 때만** Setting UI 열기
        OpenUI("UI_Setting");
    }
}
