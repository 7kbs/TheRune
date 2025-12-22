using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/RuneStone")]
public class RuneStoneSO : ItemBase, IItem
{
    public override bool QuestItem => true;

    public void OnExcute(UserData userdata, ItemBase item, ItemDB db)
    {

    }
}
