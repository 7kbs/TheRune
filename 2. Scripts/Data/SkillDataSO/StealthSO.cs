using UnityEngine;

[CreateAssetMenu(menuName = "Skill/Stealth")]
public class StealthSkillSO : SkillData, ISkillBehaviour
{
    public void OnExecute(PlayerCombat player, SkillData data,SkillRuntime state)
    {
        if (GameMgr.inst.userData.PlayerMp < mpCost) return;

        // MP 차감
        GameMgr.inst.userData.PlayerMp -= mpCost;

        // 은신 시작
        player.isStealth = true;
        state.isActive = true;
        state.startTime = Time.time;

        // 투명화
        foreach (var sr in player.GetComponentsInChildren<SpriteRenderer>())
        {
            var c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }
    }

    //public override void RuntimeUpdate(PlayerCombat player, SkillRuntime state)
    //{
    //    if (!state.isActive) return;

    //    if (Time.time - state.startTime >= duration)
    //    {
    //        // 은신 종료
    //        player.isStealth = false;
    //        state.isActive = false;

    //        foreach (var sr in player.GetComponentsInChildren<SpriteRenderer>())
    //        {
    //            var c = sr.color;
    //            c.a = 1f;
    //            sr.color = c;
    //        }
    //    }
    //}
}
