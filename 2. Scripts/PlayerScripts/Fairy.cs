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

    public void OnExecute(PlayerCombat caster, SkillData skillData)
    {
        player = caster;

        if (data.skillPrefab == null)
        {
            Debug.LogWarning("Fairy prefab missing");
            return;
        }

        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Fairy);
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