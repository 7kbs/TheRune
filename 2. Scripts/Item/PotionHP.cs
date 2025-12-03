using UnityEngine;

public class PotionHP : MonoBehaviour, IItem
{
    public void OnExcute(UserData userdata, ItemBase item, ItemDB db)
    {
        var data = (PotionHPSO)item;

        float healAmount = userdata.PlayerMaxMp * data.healRatio;

        if (db.UseItem(item)) // 성공적으로 차감했을 때만 효과 적용
        {
            userdata.PlayerHp = Mathf.Min(userdata.PlayerHp + healAmount, userdata.PlayerMaxHp);
            Debug.Log($"PotionMP 사용 → {healAmount} 회복 (최대 {userdata.PlayerMaxHp})");
        }
        else
        {
            Debug.LogWarning("포션 수량이 부족합니다!");
        }


        GameMgr.inst.UpdateQuickSlotsCount(item);
    }
}
