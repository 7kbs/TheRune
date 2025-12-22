using UnityEngine;

public class RuneStonePiece : MonoBehaviour, IItem
{
    public void OnExcute(UserData userdata, ItemBase item, ItemDB db)
    {
        var data = (RuneStonePieceSO)item;

        if (db.ItemDictionary[item] >= data.questData.targetCount)
            QuestMgr.inst.CompleteQuest(data.questData.questID);
    }
}
