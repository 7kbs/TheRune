using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour, ISkill
{
    PlayerCombat player;
    public BasicAttackSO skillData;

    float spawnTime;
    HashSet<MonsterBase> hitSet = new HashSet<MonsterBase>();

    void Awake()
    {
        Destroy(gameObject, skillData.fxStartDelay + skillData.fxDuration + 0.05f);
    }

    // Execute 호출 시 초기화
    public void OnExecute(PlayerCombat player, SkillData skillData)
    {
        Init(player, (BasicAttackSO)skillData);
    }

    public void Init(PlayerCombat player, BasicAttackSO skillData)
    {
        this.player = player;
        this.skillData = skillData;
        spawnTime = Time.time;

        // 애니메이션
        if (player.anim != null)
            player.anim.SetTrigger("attack");

        // 사운드
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Sword);
    }

    void Update()
    {
        if (player == null || skillData == null) return;

        transform.position = player.shootPos.position;
        transform.localScale = player.transform.localScale;

        float elapsed = Time.time - spawnTime;

        // 중력 초기화
        if (elapsed >= skillData.gravityResetDelay)
            player.rb.gravityScale = 15f;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var monster = other.GetComponent<MonsterBase>();
        if (monster == null || monster.isDead || hitSet.Contains(monster)) return;

        monster.TakeDamage(skillData.damage);
        hitSet.Add(monster);
    }

    //void CheckHit()
    //{
    //    if (playerCombat == null) return;

    //    bool facingLeft = playerCombat.transform.localScale.x < 0f;
    //    Vector3 playerPos = playerCombat.transform.position;
    //    Vector2 forward = facingLeft ? Vector2.right : Vector2.left;

    //    MonsterBase[] monsters = FindObjectsOfType<MonsterBase>();
    //    foreach (var monster in monsters)
    //    {
    //        if (monster == null || monster.isDead) continue;

    //        float dist = Vector2.Distance(playerPos, monster.transform.position);
    //        if (dist > skillData.attackRange) continue;

    //        Vector2 dirToMonster = (monster.transform.position - playerPos).normalized;
    //        float dot = Vector2.Dot(dirToMonster, forward);
    //        if (dot <= 0.5f) continue;

    //        if (hitSet.Contains(monster)) continue;

    //        monster.TakeDamage(skillData.damage);
    //        hitSet.Add(monster);
    //    }
    //}
}