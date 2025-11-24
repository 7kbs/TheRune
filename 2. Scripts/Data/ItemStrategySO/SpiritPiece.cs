using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/SpiritPiece")]
public class SpiritPiece : ItemBase
{
    [SerializeField] int defaultReward = 50;

    public override void Execute(UserData userData, ItemData itemData)
    {
        userData.GameMoney += defaultReward;

        GameMgr.inst.GoldText.text = $"{userData.GameMoney}";

        Debug.Log("SpiritPiece Excute");
    }
}