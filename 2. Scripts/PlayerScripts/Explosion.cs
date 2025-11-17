using UnityEngine;

public class Explosion : MonoBehaviour
{
    int damage;

    void Awake()
    {
        Destroy(gameObject, 3f);
    }

    public void Init(Player player, int dmg)
    {
        damage = dmg;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        var monster = other.GetComponent<MonsterBase>();
        if (monster != null) monster.TakeDamage(damage);
    }
}
