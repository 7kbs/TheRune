using System.Collections.Generic;
using UnityEngine;

public class BasicAttack : MonoBehaviour
{
    Player player;
    public SkillData data;

    int damage;

    [Header("Hit detection")]
    public float attackRange = 1.5f;     // 앞쪽 중심까지 거리
    public float attackRadius = 0.5f;    // 감지 반경
    [Tooltip("공격 시작 지연 (이펙트 시작 전)")]
    public float hitWindowStart = 0.05f; // 공격 판정 시작 (초)
    [Tooltip("공격 판정 유지 시간")]
    public float hitWindowDuration = 0.15f; // 판정 길이 (초)

    float spawnTime;
    bool windowActive = false;
    float windowEndTime;

    // 같은 공격 인스턴스에서 이미 타격한 몬스터 기록 (한 번만 맞게)
    HashSet<MonsterBase> hitSet = new HashSet<MonsterBase>();

    void Awake()
    {
        player = FindAnyObjectByType<Player>();
        Init(player, data != null ? data.damage : 0);

        spawnTime = Time.time;

        // Destroy 전체 수명: (hitWindowStart + hitWindowDuration + 여유)
        Destroy(gameObject, hitWindowStart + hitWindowDuration + 0.05f);
    }

    public void Init(Player player, int dmg)
    {
        this.player = player;
        this.damage = dmg;
    }

    void Update()
    {
        float t = Time.time - spawnTime;

        // 공격 윈도우 시작
        if (!windowActive && t >= hitWindowStart)
        {
            windowActive = true;
            windowEndTime = spawnTime + hitWindowStart + hitWindowDuration;
        }

        // 공격 윈도우 동안만 체크
        if (windowActive && Time.time <= windowEndTime)
        {
            CheckHit();
        }
    }

    void CheckHit()
    {
        if (player == null) return;

        bool facingLeft = player.transform.localScale.x > 0f;
        Vector3 playerPos = player.transform.position;
        Vector2 forward = facingLeft ? Vector2.left : Vector2.right;

        // 씬에 존재하는 모든 몬스터 검색
        MonsterBase[] monsters = FindObjectsOfType<MonsterBase>();

        foreach (var monster in monsters)
        {
            if (monster == null || monster.isDead) continue;

            // 거리 계산
            float dist = Vector2.Distance(playerPos, monster.transform.position);
            if (dist > attackRange) continue; // 사거리 밖

            // 전방 판정 (dot > 0.5 → 약 60도 이내)
            Vector2 dirToMonster = (monster.transform.position - playerPos).normalized;
            float dot = Vector2.Dot(dirToMonster, forward);
            if (dot <= 0.5f) continue; // 뒤쪽이면 패스

            // 이미 공격한 적 체크 (중복 방지)
            if (hitSet.Contains(monster)) continue;

            // 한 번만 데미지 적용
            monster.TakeDamage(damage);
            hitSet.Add(monster);
        }
    }

    // 시각 디버그
    void OnDrawGizmosSelected()
    {
        if (player == null) return;

        bool facingLeft = player.transform.localScale.x > 0f;
        Vector2 center = (Vector2)player.transform.position +
            (facingLeft ? Vector2.left : Vector2.right) * (attackRange * 0.5f);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(center, attackRadius);
    }
}
