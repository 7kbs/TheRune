using UnityEngine;
using System.Collections.Generic;


public class SylvaroneNPC : NPCInteractable
{
    [SerializeField] List<QuestDialogueSet> questDialogues;

    public override void Interact(Player player)
    {
        foreach (var set in questDialogues)
        {
            if (QuestMgr.inst.IsQuestInProgress(set.quest.questID))
            {
                HandleQuestDialogue(set);
                QuestMgr.inst.TryCompleteQuest(set.quest);
                return;
            }
        }

        // 상점 잠그기
        if (QuestMgr.inst.IsQuestRewarded("main07")) return;

        if (!PlayerMove.inst.IsInteractionState)
        {
            PlayerMove.inst.ChangeState(new InteractingState());
            DialogueMgr.inst.shopFrame.gameObject.SetActive(true);
        }
    }
}