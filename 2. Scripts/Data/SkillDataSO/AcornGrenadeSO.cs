using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AcornGrenade")]
public class AcornGrenadeSO : SkillData
{
    [Header("Grenade Settings")]
    public int grenadeCount = 3;
    public float positionOffsetRange = 0.2f;
    public float[] forceMultiplier = { 0.8f, 0.9f, 1f };

    public override void Execute(PlayerCombat player)
    {
        if (player.anim == null) return;
        if (GameMgr.inst.userData.PlayerMp < mpCost) return;
        if (Time.time < lastUsedTime + cooldown) return; // 쿨타임 체크
        lastUsedTime = Time.time;

        // MP 차감
        GameMgr.inst.userData.PlayerMp -= mpCost;

        // 공격 애니메이션 & 사운드
        player.anim.SetTrigger("attack");
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Bomb);

        // Spawn Grenades
        for (int i = 0; i < grenadeCount; i++)
        {
            // 랜덤 위치 흔들림
            Vector3 spawnPos =
                player.shootPos.position +
                new Vector3(Random.Range(-positionOffsetRange, positionOffsetRange), 0, 0);

            // 생성
            GameObject grenade = Instantiate(skillPrefab, spawnPos, player.shootPos.rotation);

            // Force 적용
            Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
            Vector2 throwDir =
                new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;

            float force = throwforce;
            if (i < forceMultiplier.Length)
                force *= forceMultiplier[i];

            rb.AddForce(throwDir * force, ForceMode2D.Impulse);
        }

        // 이 스킬은 지속 업데이트 필요 없음 (따라서 등록 X)
        // player.RegisterRuntimeSkillUpdate(this);  // 필요 없음
    }
}
