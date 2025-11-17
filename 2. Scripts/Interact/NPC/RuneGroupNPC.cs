using UnityEngine;

public class RuneGroupNPC : NPCInteractable
{
    public QuestData main08;

    public override void Interact(Player player)
    {
        if (QuestMgr.inst.IsQuestInProgress(main08.questID))
            LobbyMgr.inst.RuneStoneAnimOn();
    }
}
