using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NPCInteractable : Interactable
{
    public override void Interact(Player player)
    {

    }

    public virtual void HandleQuestDialogue(QuestDialogueSet set)
    {
        PlayerMove.inst.ChangeState(new InteractingState());

        var userData = DataMgr.inst.userData;

        if (!userData.IsDialoguePlayed(set.dialogue.forUse))
        {
            DialogueMgr.inst.StartDialogue(set.dialogue);
            userData.SetDialoguePlayed(set.dialogue.forUse);
        }
    }
}
