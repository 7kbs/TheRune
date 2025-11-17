using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleMgr : MonoBehaviour
{
    public UserData userData;
    public ItemData itemData;

    public Button GameStartBtn;
    public Button ExitBtn;
    public Image FadeObject;
    public Button ResetBtn;

    void Start()
    {
        DataMgr.inst.LoadData();
        GlobalValue.sceneType = GlobalValue.SceneType.Title;

        if (GameStartBtn != null)
            GameStartBtn.onClick.AddListener(StartBtnClick);

        if (ExitBtn != null)
            ExitBtn.onClick.AddListener(() =>
            {
                Application.Quit();
            });

        SoundMgr.inst.BGM_Play(true);
    }

    void StartBtnClick()
    {
        if (userData.OpeningEnd == false)
        {
            GlobalValue.sceneType = GlobalValue.SceneType.Opening;
            SceneManager.LoadScene("OpeningScene");
        }
        else
        {
            if (userData.sceneType == 0)
            {
                GlobalValue.sceneType = GlobalValue.SceneType.Lobby;
                SceneManager.LoadScene("LobbyScene");
                SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);
            }
            if(userData.sceneType == 1)
            {
                GlobalValue.sceneType = GlobalValue.SceneType.Game;
                SceneManager.LoadScene("GameScene");
                SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);
            }
        }
    }

    public void ResetBtnClick()
    {
        // 1) 데이터 초기화
        DataMgr.inst.userData.InitData();

        // 2) 퀘스트 목록 지연 초기화
        QuestMgr.inst.EnsureQuestList();

        // 3) 현재 퀘스트 세팅/승격
        QuestMgr.inst.CurrentQuest();

        // 4) 아이템 초기화
        var allItems = ItemManager.inst != null
            ? new List<ItemBase>(ItemManager.inst.GetAllItems())
            : new List<ItemBase>();

        itemData.InitData();

        DataMgr.inst.SaveData();
    }
}
