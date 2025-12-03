using UnityEngine;

public class RuneStonePiece : MonoBehaviour, IItem
{
    public void OnExcute(UserData userdata, ItemBase item, ItemDB db)
    {
        var data = (RuneStonePieceSO)item;
        QuestMgr.inst.TryCompleteQuest(data.questData);
    }
}
