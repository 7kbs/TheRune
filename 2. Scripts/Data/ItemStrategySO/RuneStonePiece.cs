using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/RuneStonePiece")]
public class RuneStonePiece : ItemBase
{
    public QuestData questData;

    public override bool QuestItem => true;

    public override void Execute(UserData userData, ItemData itemData)
    {
        QuestMgr.inst.TryCompleteQuest(questData);
    }
}
