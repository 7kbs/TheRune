using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MoroKhan : MonsterBase
{
    public Image HpBack;
    public bool isDie;

    public GameObject Camera;
    public GameObject BossDieCamera;
    public GameObject BossDieEffect;

    private bool cosmicSpawned = false; // Cosmic이 이미 스폰되었는지 확인하는 플래그

    void Start()
    {
        curHP = maxHP;

        anim = GetComponentInChildren<Animator>();

        HpBar.fillAmount = curHP / maxHP;
        DelayHpbar.fillAmount = curHP / maxHP;
    }

    void Update()
    {
        float targetFill = curHP / maxHP;
        HpBar.fillAmount = targetFill;

        if (DelayHpbar.fillAmount > HpBar.fillAmount)
        {
            DelayHpbar.fillAmount = Mathf.Lerp(DelayHpbar.fillAmount, HpBar.fillAmount, Time.deltaTime * 1.5f);
        }
        else
        {
            DelayHpbar.fillAmount = HpBar.fillAmount; // 회복 시에는 메인 체력바에 즉시 맞춰줌
        }
    }


    protected override void Die()
    {
        if (isDie) return; // 사망 로직이 중복으로 실행되지 않도록 방지

        isDie = true;
        GameMgr.inst.userData.BossDie = true;

        var cq = QuestMgr.inst.CurrentQuest();
        QuestMgr.inst.NextQuestSequence(cq.questID);

        StartCoroutine(BossDieCoroutine());

        // 사망 애니메이션 재생
        anim.SetBool("isDie", true);

        // HP바 비활성화
        HpBar.gameObject.SetActive(false);
        HpBack.gameObject.SetActive(false);
    }


    IEnumerator BossDieCoroutine()
    {
        if (isDie) // boss가 죽었고, cosmic이 아직 스폰되지 않았다면
        {
            Camera.SetActive(false);
            BossDieCamera.SetActive(true);
        }

        yield return new WaitForSeconds(5f);

        gameObject.SetActive(false);

        if (isDie && !cosmicSpawned) // boss가 죽었고, cosmic이 아직 스폰되지 않았다면
        {
            CosmicSpawn();
            cosmicSpawned = true; // Cosmic 스폰 완료 플래그 설정
        }

        Camera.SetActive(true);
        BossDieCamera.SetActive(false);

        BossMapMgr.Inst.ClearObjSet();
    }

    void CosmicSpawn()
    {
        GameObject cosmic = Instantiate(BossDieEffect);
        cosmic.transform.position = new Vector3(0, 0, 0);
    }   


    public override void Attack(Player player)
    {
        
    }
}
