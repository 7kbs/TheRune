using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Stealth")]
public class StealthSkillSO : SkillData
{
    private float stealthEndTime;
    private bool isStealthActive = false;

    public override void Execute(PlayerCombat player)
    {
        if (GameMgr.inst.userData.PlayerMp < mpCost) return;
        if (Time.time < lastUsedTime + cooldown) return; // 쿨타임 체크
        lastUsedTime = Time.time;

        // MP 차감
        GameMgr.inst.userData.PlayerMp -= mpCost;

        // 은신 상태 시작
        player.isStealth = true;
        isStealthActive = true;
        stealthEndTime = Time.time + duration;

        // 투명화 처리
        foreach (var sr in player.GetComponentsInChildren<SpriteRenderer>())
        {
            var c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }

        // PlayerCombat에서 RuntimeUpdate 호출되도록 등록
        player.RegisterRuntimeSkillUpdate(this);
    }

    public override void RuntimeUpdate(PlayerCombat player)
    {
        if (!isStealthActive) return;

        if (Time.time >= stealthEndTime)
        {
            // 은신 종료
            player.isStealth = false;
            isStealthActive = false;

            foreach (var sr in player.GetComponentsInChildren<SpriteRenderer>())
            {
                var c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }
    }
}
