using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : MonoBehaviour, ISkillBehaviour
{
    PlayerCombat player;
    public SkillData data;

    public GameObject fairyAttackObj;

    private Queue<GameObject> attackTargets = new Queue<GameObject>();
    private float attackTimer = 0f;

    void Awake()
    {
        Destroy(gameObject, data.duration);
    }

    public void OnExecute(PlayerCombat caster, SkillData skillData, SkillRuntime state)
    {
        player = caster;

        if (data.skillPrefab == null)
        {
            Debug.LogWarning("Fairy prefab missing");
            state.isActive = false;
            return;
        }

        // Prefab 인스턴스화
        //var fairyInstance = Instantiate(data.skillPrefab, caster.player.transform.position + new Vector3(0, 3.5f, 0), Quaternion.identity);

        // 스폰된 인스턴스의 Fairy 스크립트 가져오기
        //var fairyScript = GetComponent<Fairy>();
        //if (fairyScript != null)
        //{
        //    fairyScript.player = player;
        //    fairyScript.data = data;
        //    fairyScript.fairyAttackObj = fairyAttackObj;
        //}

        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Fairy);

        // 즉시형 스킬 → state 종료
        state.isActive = false;

        // Destroy는 Prefab 인스턴스에만
        //Object.Destroy(fairyInstance, data.duration);
    }

    void Update()
    {
        // 플레이어 머리 위 따라다님
        transform.position = player.transform.position + new Vector3(0, 3.5f, 0);

        // 공격
        if (attackTargets.Count > 0)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                FireAttack();
                attackTimer = 1f;
            }
        }
    }

    void FireAttack()
    {
        GameObject atk = Instantiate(fairyAttackObj, transform.position, Quaternion.identity);
        FairyAttack fa = atk.GetComponent<FairyAttack>();
        if (fa != null)
        {
            fa.monsterTr = attackTargets.Peek();
            fa.damage = data.damage;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") || collision.name == "Boss")
        {
            attackTargets.Enqueue(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Monster") || collision.name == "Boss")
        {
            if (attackTargets.Count > 0)
                attackTargets.Dequeue();
        }
    }
}