using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/RuneStone")]
public class RuneStone : ItemBase
{
    public override bool QuestItem => true;

    public override void Execute(UserData userData, ItemData itemData)
    {
        
    }
}
