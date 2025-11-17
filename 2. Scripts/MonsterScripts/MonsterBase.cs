using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class MonsterBase : MonoBehaviour
{
    [Header("공통 설정")]
    [Space(5f)]
    [Header("Stats")]
    public float maxHP = 100f;
    public float curHP;
    public int AttackPower = 20;
    public bool isDead = false;

    [Header("UI")]
    public Image HpBar;
    public Image DelayHpbar;

    [HideInInspector] public Animator anim;


    public void Initialize()
    {
        curHP = maxHP;
        HpBar.fillAmount = curHP / maxHP;
        isDead = false;
    }


    protected virtual void Die()
    {
        
    }


    public void TakeDamage(float dmg)
    {
        if (isDead) return;

        GameMgr.inst.DamageTextSpawn(dmg, transform.position, Color.red);
        curHP -= dmg;

        if (curHP <= 0) Die();
    }


    public abstract void Attack(Player player);
}