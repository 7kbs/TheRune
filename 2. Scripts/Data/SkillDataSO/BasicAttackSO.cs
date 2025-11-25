using UnityEngine;

[CreateAssetMenu(menuName = "Skill/BasicAttack")]
public class BasicAttackSO : SkillData
{
    [Header("Basic Attack Settings")]
    public GameObject[] attSlash;
    public float fxStartDelay = 0.2f;
    public float fxDuration = 0.1f;
    public float gravityResetDelay = 1.0f;

    private int fxIndex = 0;
    private GameObject currentFx;
    private float attackStartTime;

    public override void Execute(PlayerCombat player)
    {
        if (player.isAttacking) return;
        if (player.anim == null) return;

        // 기본 공격 애니메이션
        player.anim.SetTrigger("attack");
        fxIndex = 0;

        // FX 생성
        var fx = Instantiate(attSlash[fxIndex], player.shootPos);
        fx.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        currentFx = fx;

        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Sword);

        player.isAttacking = true;
        attackStartTime = Time.time;

        // 플레이어가 SkillSO Update를 호출할 수 있게 연결
        player.RegisterRuntimeSkillUpdate(this);
    }


    public override void RuntimeUpdate(PlayerCombat player)
    {
        if (!player.isAttacking) return;

        float elapsed = Time.time - attackStartTime;

        // FX 삭제
        if (elapsed >= fxStartDelay + fxDuration)
        {
            if (currentFx != null)
            {
                Destroy(currentFx);
                currentFx = null;
            }
            player.isAttacking = false;
        }

        // 아래 공격이라면 중력 복구
        if (fxIndex == 1 && elapsed >= gravityResetDelay)
            player.rb.gravityScale = 15.0f;
    }
}
