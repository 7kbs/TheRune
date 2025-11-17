using UnityEngine;

[CreateAssetMenu(menuName = "ItemStrategy/PotionHP")]
public class PotionHP : ItemBase
{
    [SerializeField] float healRatio = 0.3f;

    public override bool Consumable => true;

    public override void Execute(UserData userData, ItemData itemData) 
    {
        float healAmount = userData.PlayerMaxHp * healRatio;

        if (itemData.UseItem(this)) // 성공적으로 차감했을 때만 효과 적용
        {
            userData.PlayerHp = Mathf.Min(userData.PlayerHp + healAmount, userData.PlayerMaxHp);
            Debug.Log($"PotionMP 사용 → {healAmount} 회복 (최대 {userData.PlayerMaxHp})");
        }
        else
        {
            Debug.LogWarning("포션 수량이 부족합니다!");
        }

        GameMgr.inst.UpdateQuickSlotsCount(this);
    }
}