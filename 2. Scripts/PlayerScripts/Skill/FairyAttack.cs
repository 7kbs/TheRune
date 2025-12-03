using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyAttack : MonoBehaviour
{
    Player player;

    [HideInInspector]public GameObject monsterTr;
    public SkillData data;

    float AttackSpeed = 3.0f;
    [HideInInspector] public int damage;

    Vector3 Dir = Vector3.zero;
    Vector3 monsterPos = Vector3.zero;

    void Awake()
    {
        player = FindAnyObjectByType<Player>();
        Init(player, data.damage);

        Destroy(gameObject, 2.0f);
    }

    public void Init(Player player, int dmg)
    {
        damage = dmg;
    }

    void Start()
    {
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Fairy);
    }

    void Update()
    {
        if (monsterTr != null)
        {
            monsterPos = new Vector3(monsterTr.transform.position.x,
                monsterTr.transform.position.y + 0.5f, 0.0f);

            Dir = monsterPos - transform.position;
        }
        transform.position += Dir * Time.deltaTime * AttackSpeed;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        var monster = collision.GetComponent<MonsterBase>();
        if (monster != null)
        {
            monster.TakeDamage(damage);
            Destroy(gameObject); // Ãæµ¹ ÈÄ ¼Ò¸ê
        }
    }
}
