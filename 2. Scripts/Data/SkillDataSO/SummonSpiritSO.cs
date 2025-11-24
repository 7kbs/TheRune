using UnityEngine;

[CreateAssetMenu(menuName = "Skill/SummonSpirit")]
public class SummonSpiritSO : SkillData
{
    public override void Execute(PlayerCombat player)
    {
        if (GameMgr.inst.userData.PlayerMp < mpCost) return;
        if (Time.time < lastUsedTime + cooldown) return; // 쿨타임 체크
        lastUsedTime = Time.time;

        // MP 차감
        GameMgr.inst.userData.PlayerMp -= mpCost;

        // 스킬 프리팹 생성
        if (skillPrefab != null)
        {
            GameObject spirit = Instantiate(skillPrefab, player.transform.position, Quaternion.identity);
            spirit.SetActive(true);
        }

        // 효과음
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Fairy);

        // 필요 시 PlayerCombat에서 RuntimeUpdate 호출하도록 등록
        // 현재 SummonSpirit은 즉시 효과형이라 RuntimeUpdate 필요 없음
        // player.RegisterRuntimeSkillUpdate(this); // 필요시 활성화
    }
}
