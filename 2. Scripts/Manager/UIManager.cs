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
        if (Input.GetKeyDown(KeyCode.Tab)) HandleTabPress();

        if (Input.GetKeyDown(KeyCode.Escape))
            HandleEscape();

        if (Input.GetKeyDown(KeyCode.I))
            OpenUI("UI_Inventory");

        if (Input.GetKeyDown(KeyCode.K))
            OpenUI("UI_Skill_Re");

        if (Input.GetKeyDown(KeyCode.J))
            OpenUI("UI_Quest");
    }

    void HandleTabPress()
    {
        switch (GlobalValue.sceneType)
        {
            case SceneType.Game:
                OpenUI("UI_GameMap");
                break;
            case SceneType.Lobby:
                OpenUI("UI_LobbyMap");
                break;
        }
    }

    public UI_Base OpenUI(string uiName)
    {
        CloseAllUI();

        var prefab = Resources.Load<UI_Base>("UI_Path/" + uiName);
        var ui = Instantiate(prefab, rootLayer);

        uiStack.Push(ui);
        ui.OnOpen();
        return ui;
    }

    public void CloseUI(UI_Base ui)
    {
        if (uiStack.Count == 0) return;

        if (uiStack.Peek() == ui)
        {
            uiStack.Pop();
            ui.OnClose();
            Destroy(ui.gameObject);

            PlayerMove.inst.ChangeState(new DefaultState());
        }
        else
        {
            Debug.LogWarning("UI Stack mismatch");
        }
    }

    public void CloseAllUI()
    {
        while (uiStack.Count > 0)
        {
            var top = uiStack.Pop();
            top.OnClose();
            Destroy(top.gameObject);
        }
    }

    void HandleEscape()
    {
        if (uiStack.Count > 0)
        {
            // 스택 최상단 UI 닫기
            uiStack.Peek().Close();
            return;
        }

        // UI가 완전히 없을 때만 Setting UI 열기
        OpenUI("UI_Setting");
    }


    public UI_Toast GetToast()
    {
        return Instantiate(Resources.Load<UI_Toast>("UI_Path/" + "UI_Toast"), transform);
    }
}