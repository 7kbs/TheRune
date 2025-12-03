using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/RuneStonePiece")]
public class RuneStonePieceSO : ItemBase
{
    public QuestData questData;

    public override bool QuestItem => true;
}
