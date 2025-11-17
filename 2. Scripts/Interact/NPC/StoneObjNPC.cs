using UnityEngine;
using UnityEngine.UI;

public class StoneObjNPC : NPCInteractable
{
    [Header("Child")]
    public QuestData firstData;
    public QuestData requireData;
    public DialogueData Sanctom;

    public GameObject RunePuzzlePanel;
    public GameObject PuzzlePieceSet;
    public Button RunePuzzleEscBtn;

    public override void Interact(Player player)
    {
        Debug.Log("StoneObj Á¶»ç!");
        FistContatct();

        var userData = DataMgr.inst.userData;
        var cq = QuestMgr.inst.CurrentQuest();

        if (cq == QuestMgr.inst.SearchQuest(requireData.questID).questSO)
        {
            if (!GameMgr.inst.RunePuzzleClear) RunePuzzlePanel.SetActive(true);
            PuzzlePieceSet.SetActive(true);
        }
        else if (QuestMgr.inst.IsQuestInProgress("main04"))
        {
            DialogueMgr.inst.StartDialogue(Sanctom);
        }
    }

    void Start()
    {
        if (RunePuzzleEscBtn != null) RunePuzzleEscBtn.onClick.AddListener(() =>
        {
            Sanctom stoneCtrl = GameObject.Find("StoneObject").GetComponent<Sanctom>();

            if (GameMgr.inst.RunePuzzleClear) stoneCtrl.BossPortalOpen();
            RunePuzzlePanel.SetActive(false);
        });
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