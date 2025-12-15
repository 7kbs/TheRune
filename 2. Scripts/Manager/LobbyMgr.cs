using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class LobbyMgr : MonoBehaviour
{
    public GameObject RuneStone;
    public GameObject Item_Flower;

    public GameObject NPC_Char;
    public GameObject Sylvaron;
    public GameObject Sylvaron_Stone;

    public GameObject EndingPanel;
    public GameObject EndingCredit;

    public GameObject animGroup;
    Animation anim;
    public static LobbyMgr inst;

    int DialogueIndex = 0;


    private void Awake()
    {
        inst = this;
    }


    void Start()
    {
        GlobalValue.sceneType = SceneType.Lobby;

        anim = animGroup.GetComponent<Animation>();

        if (GameMgr.inst.userData.PuzzleClear) NPC_Char.SetActive(true);
        else NPC_Char.SetActive(false);

        if (QuestMgr.inst.IsQuestCompleted("main08"))
        {
            RuneStone.SetActive(true);
            Sylvaron_Stone.SetActive(true);
            Sylvaron.SetActive(false);
        }

        if (QuestMgr.inst.IsQuestCompleted("main09")) Sylvaron_Stone.SetActive(false);

        DataMgr.inst.SaveData();
    }


    public void RuneStoneAnimOn() //보스퇴치 후 플레이어가 룬스톤에 다가갔을 때 연출할 애니메이션
    {
        if (!QuestMgr.inst.IsQuestInProgress("main08"))
        {
            UIManager.inst.GetToast().Init("퀘스트를 완료해주세요!", Color.white);
            return;
        }

        var runeStone = GameMgr.inst.itemData.ItemDictionary.Keys.FirstOrDefault(e => e is RuneStoneSO);

        if (runeStone != null)
        {
            GameMgr.inst.itemData.UseItem(runeStone, 1);
        }

        Sylvaron.SetActive(false);
        PlayerMove.inst.ChangeState(new InteractingState());
        RuneStone.SetActive(true);
        Invoke(nameof(EndingAnimOn), 5.0f);
    }

    void EndingAnimOn()
    {
        animGroup.SetActive(true);
        Invoke(nameof(EndingAnimOff), 8.5f);
    }

    void EndingAnimOff()
    {
        PlayerMove.inst.ChangeState(new DefaultState());
        animGroup.SetActive(false);

        var cq = QuestMgr.inst.CurrentQuest();
        QuestMgr.inst.TryCompleteQuest(cq);
        UIManager.inst.GetToast().Init("퀘스트를 완료하고 보상을 얻으세요!", Color.white);

        Sylvaron_Stone.SetActive(true);
    }

    public void EndingPanelOn()
    {
        EndingPanel.SetActive(true);
        PlayerMove.inst.ChangeState(new InteractingState());

        Invoke(nameof(EndingPanelOff), 5.0f);
    }

    public void EndingPanelOff()
    {
        EndingPanel.SetActive(false);
        EndingCredit.SetActive(true);

        Invoke(nameof(EndingCreditOff), 21.0f);
    }

    void EndingCreditOff()
    {
        EndingCredit.SetActive(false);
        PlayerMove.inst.ChangeState(new DefaultState());

        var main10 = QuestMgr.inst.SearchQuest("main10");
        QuestMgr.inst.TryCompleteQuest(main10.questSO);
    }
}