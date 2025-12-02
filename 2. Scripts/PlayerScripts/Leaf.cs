using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour, ISkillBehaviour
{
    public SkillData data;
    
    float leafSpeed = 30.0f;

    Vector2 direction;
    int damage;

    public void OnExecute(PlayerCombat player, SkillData data)
    {
        player.anim.SetTrigger("attack");

        transform.position = player.shootPos.transform.position;

        Init(player, data.damage);
    }


    public void Init(PlayerCombat player, int dmg)
    {
        damage = dmg;
        direction = player.transform.localScale.x > 0 ? Vector2.left : Vector2.right;
    }

    void Awake()
    {
        Destroy(gameObject, 2.0f);
    }

    void Update()
    {
        transform.Translate(direction * leafSpeed * Time.deltaTime, Space.World);
        transform.Rotate(new Vector3(0, 0, 2000 * Time.deltaTime));
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        var monster = collision.GetComponent<MonsterBase>();
        if (monster != null)
        {
            monster.TakeDamage(damage);
            Destroy(gameObject); // Ãæµ¹ ÈÄ ¼Ò¸ê
        }
    }
}
