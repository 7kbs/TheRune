using UnityEngine;
using UnityEngine.UI;

public class StoneObjNPC : NPCInteractable
{
    [Header("Child")]
    UserData ud;
    public QuestData firstData;
    public QuestData requireData;
    public DialogueData Sanctom;


    public override void Interact(Player player)
    {
        Debug.Log("StoneObj Á¶»ç!");
        FistContatct();

        var userData = GameMgr.inst.userData;
        var cq = QuestMgr.inst.CurrentQuest();

        if (cq == QuestMgr.inst.SearchQuest(requireData.questID).questSO &&!userData.PuzzleClear)
        {
            UIManager.inst.OpenUI("UI_Puzzle");
        }
        else if (QuestMgr.inst.IsQuestInProgress("main04"))
        {
            DialogueMgr.inst.StartDialogue(Sanctom);
        }
    }
    void Start()
    {
        ud = GameMgr.inst.userData;
    }

    void FistContatct()
    {
        var rq = QuestMgr.inst.SearchQuest(firstData.questID);

        if (QuestMgr.inst.IsQuestInProgress(rq.questSO.questID))
        {
            QuestMgr.inst.TryCompleteQuest(rq.questSO);
            DialogueMgr.inst.StartDialogue(Sanctom);
        }
    }
}