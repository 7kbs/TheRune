using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/IcePiece")]
public class IcePiece : ItemBase
{
    [SerializeField] int maxMpIncrease = 50;

    // 유저 데이터 적용
    public override void Execute(UserData userData, ItemData itemData)
    {
        userData.PlayerMaxMp += maxMpIncrease;

        Debug.Log("IcePiece Excute");

    }
}