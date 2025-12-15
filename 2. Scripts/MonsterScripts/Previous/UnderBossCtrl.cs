using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnderBossCtrl : MonoBehaviour
{
    public enum BossType
    {
        UnderBoss1,
        UnderBoss2
    }

    public BossType bossType;

    float Hp = 0;
    float MaxHp = 0;
    float MoveSpeed = 3.0f;
    bool isInvincible;

    bool isDie = false;
    [HideInInspector] public bool isgroggy = false;
    float groggyTime = 0.0f;

    [Header("--- UnderBoss 1 ---")]
    public GameObject Bullet;
    public GameObject ShootPos;
    public GameObject HitParticle;
    public Image HpBar;
    public Image DelayBar;
    public Image groggyBar;
    public Canvas BossCanvas;

    Animator anim;
    Player player;

    Vector3 Dir = Vector3.zero;

    float AttackTimer = 0.0f;
    float Attack2Timer = 3.0f;
    int playerHitCount = 0;

    public GameObject Warpgate;


    void Start()
    {
        GlobalValue.sceneType = SceneType.Battle;

        player = FindAnyObjectByType<Player>();
        anim = GetComponent<Animator>();

        //if문으로 Boss타입이 1이면 체력 ??? / Boss타입이 2면 체력 ??? 정해주고 시작

        if(bossType == BossType.UnderBoss1)
        {
            MaxHp = 2500;
            //MaxHp = 100; Test
            Hp = MaxHp;
        }

        HpBar.fillAmount = Hp / MaxHp;
        DelayBar.fillAmount = Hp / MaxHp;
    }


    void Update()
    {
        if (GameMgr.inst.player.isDie || !PlayerMove.inst.IsDefaultState)
        {
            return;
        }

        BossCanvas.gameObject.SetActive(true);

        Trace();

        if (playerHitCount >= 20 && isgroggy == false)
        {
            isgroggy = true;
            playerHitCount = 0;
            groggyTime = 5.0f;
        }

        if (groggyTime > 0.0f)
        {
            groggyTime -= Time.deltaTime;
            Groggy();
            if (groggyTime <= 0.0f)
            {
                groggyTime = 0.0f;
                isgroggy = false;
                anim.SetBool("groggy", false);
            }
        }

        float targetFill = Hp / MaxHp;
        HpBar.fillAmount = targetFill;

        if (DelayBar.fillAmount > HpBar.fillAmount)
        {
            DelayBar.fillAmount = Mathf.Lerp(DelayBar.fillAmount, HpBar.fillAmount, Time.deltaTime * 1.5f);
        }
        else
        {
            DelayBar.fillAmount = HpBar.fillAmount; // 회복 시에는 메인 체력바에 즉시 맞춰줌
        }
    }

    void Trace()
    {
        Dir = player.transform.position - transform.position;
        Dir.y = 0.0f;

        if (isgroggy || isDie || player.GetComponent<Player>().isStealth) return;

        if (20.0f > Dir.magnitude && Dir.magnitude > 15.0f) //플레이어와 간격 좀 벌어지면 멈추고 총알공격
        {            
            Attack("Attack1");
            return;
        }

        if (0.0f <= Dir.magnitude && Dir.magnitude < 3.0f)
        {            
            Attack("Attack2");
            return;
        }

        transform.Translate(Dir.normalized * MoveSpeed * Time.deltaTime);

        Vector3 localScale = transform.localScale;

        if (Dir.x > 0)
        {
            localScale.x = Mathf.Abs(localScale.x) * -1;
        }
        else
        {
            localScale.x = Mathf.Abs(localScale.x);
        }
        

        transform.localScale = localScale;        
    }

    void Attack(string triggerName)
    {
        if (isDie || player.GetComponent<Player>().isStealth) return;

        if(triggerName == "Attack1")
        {
            anim.SetTrigger(triggerName);
            //AttackDir방향으로 총알 발사 (시간으로 공격 주기 만들기)
            AttackTimer -= Time.deltaTime;

            if(AttackTimer <= 0.0f)
            {
                GameObject BulletObj = Instantiate(Bullet);
                BulletObj.transform.position = ShootPos.transform.position;
                AttackTimer = 0.7f;
            }
        }

        if (triggerName == "Attack2")
        {
            Attack2Timer -= Time.deltaTime;
            if(Attack2Timer <= 0.0f)
            {
                anim.SetTrigger(triggerName);
                Attack2Timer = 3.0f;
            }
        }     
    }

    void Groggy()
    {
        anim.SetBool("groggy", true);
        groggyBar.fillAmount -= Time.deltaTime * 0.2f;
    }

    void TakeDamage(float Damage)
    {
        if (!isInvincible)
        {
            HitParticle.gameObject.SetActive(true);
            Invoke(nameof(HitParticleOff), 0.5f);
            if (isgroggy == false)
            {
                playerHitCount++;
                groggyBar.fillAmount = (float)playerHitCount / 20;
            }

            GameMgr.inst.DamageTextSpawn(-Damage, transform.position, Color.red);
            Hp -= Damage;            
            //HpBar.fillAmount = Hp / MaxHp;   
            
            if(Hp <= 0.0f)
            {
                UnderBossDie();
            }

            // 무적 상태 시작
            StartCoroutine(InvincibilityCoroutine());
        }
    }

    // 무적 상태를 위한 Coroutine
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;  // 무적 상태 설정
        yield return new WaitForSeconds(0.1f);
        isInvincible = false;  // 무적 상태 해제
    }

    void UnderBossDie()
    {
        if (bossType == BossType.UnderBoss1)
        {
            anim.SetBool("groggy", true);
            BoxCollider2D coll = GetComponentInChildren<BoxCollider2D>();
            coll.enabled = false; //콜라이더 해제
        }

        GameMgr.inst.userData.UnderBossDie = true;
        BossCanvas.gameObject.SetActive(false);
        Warpgate.SetActive(true);
        isDie = true;
        UIManager.inst.GetToast().Init("포탈이 열렸습니다!", Color.white);
        Destroy(gameObject, 3.0f);
    }

    void HitParticleOff()
    {
        HitParticle.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "PlayerAttack")
        {
            //플레이어 공격에 의해 피해를 입음.
            if (collision.gameObject.name.Contains("Leaf"))
            {
                TakeDamage(30f);
                Destroy(collision.gameObject);
            }
            else if (collision.gameObject.name.Contains("Fairy"))
            {
                TakeDamage(20.0f);
            }
            else if (collision.gameObject.name.Contains("Bomb"))
            {
                TakeDamage(50f);
            }
            else TakeDamage(25f);
        }
    }
}
