using UnityEngine;


[CreateAssetMenu(menuName = "ItemStrategy/FirePiece")]
public class FirePiece : ItemBase
{
    [SerializeField] int maxHpIncrease = 50;

    public override void Execute(UserData userData, ItemData itemData)
    {
        userData.PlayerMaxHp += maxHpIncrease;

        Debug.Log("FirePiece Excute");
    }
}