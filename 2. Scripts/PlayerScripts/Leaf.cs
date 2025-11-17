using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Leaf : MonoBehaviour
{
    Player player;
    public SkillData data;
    
    float leafSpeed = 30.0f;
    Rigidbody2D rigid;

    Vector2 direction;
    int damage;


    public void Init(Player player, int dmg)
    {
        damage = dmg;
        direction = player.transform.localScale.x > 0 ? Vector2.left : Vector2.right;
    }

    void Awake()
    {
        player = FindAnyObjectByType<Player>();
        Init(player, data.damage);

        Destroy(gameObject, 2.0f);
    }

    void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
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
