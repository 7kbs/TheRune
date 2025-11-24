using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Leaf")]
public class LeafSkillSO : SkillData
{
    public override void Execute(PlayerCombat player)
    {
        if (player.anim == null) return;
        if (GameMgr.inst.userData.PlayerMp < mpCost) return;
        if (Time.time < lastUsedTime + cooldown) return; // 쿨타임 체크
        lastUsedTime = Time.time;

        // MP 차감
        GameMgr.inst.userData.PlayerMp -= mpCost;

        // 애니메이션 및 사운드
        player.anim.SetTrigger("attack");
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Leaf);

        // 스킬 프리팹 생성
        if (skillPrefab != null)
        {
            Object.Instantiate(skillPrefab, player.shootPos.position, Quaternion.identity);
        }

        // LeafSkill은 지속 업데이트가 필요 없으므로 RegisterRuntimeSkillUpdate 호출 안함
    }
}