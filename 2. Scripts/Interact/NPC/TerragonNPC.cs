using System.Collections.Generic;
using UnityEngine;

public class TerragonNPC : NPCInteractable
{
    [SerializeField] List<QuestDialogueSet> questDialogues;
    public QuestData main10;
    public QuestData main11;

    public override void Interact(Player player)
    {
        Debug.Log("Terragon");

        if (!GameMgr.inst.userData.TerragonPuzzleClear)
            DialogueMgr.inst.OnPuzzleTriggered += OpenPuzzlePanel;

        if (QuestMgr.inst.IsQuestInProgress(main11.questID))
        {
            DialogueMgr.inst.shopFrame.gameObject.SetActive(true);
            return;
        }

        foreach (var set in questDialogues)
        {
            if (QuestMgr.inst.IsQuestInProgress(set.quest.questID))
            {
                HandleQuestDialogue(set);
                // 대화 종료 시점 처리
                DialogueMgr.inst.OnDialogueEnded += () => OnQuestDialogueEnded(set.quest);
                return;
            }
        }
    }

    void OpenPuzzlePanel()
    {
        DialogueMgr.inst.dialogueBox.SetActive(false);
        GameSceneMgr.inst.PuzzlePanel.SetActive(true);

        DialogueMgr.inst.OnPuzzleTriggered -= OpenPuzzlePanel;
    }

    void OnQuestDialogueEnded(QuestData data)
    {
        // 이벤트 중복 방지
        DialogueMgr.inst.OnDialogueEnded -= () => OnQuestDialogueEnded(data);

        // 퀘스트 완료 처리
        QuestMgr.inst.TryCompleteQuest(data);

        // main10이라면 엔딩 연출 호출
        if (data == main10)
        {
            LobbyMgr.inst.EndingPanelOn();
        }
    }
}