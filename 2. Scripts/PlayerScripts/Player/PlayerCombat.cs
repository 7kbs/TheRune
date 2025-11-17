using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 공통 Skill 추상 클래스
public abstract class Skill
{
    protected Player player;
    protected SkillData data;
    protected float lastUsedTime;

    public Skill(Player player, SkillData data)
    {
        this.player = player;
        this.data = data;
        lastUsedTime = -data.cooldown;
    }

    public bool CanUse() => Time.time >= lastUsedTime + data.cooldown;

    public void Use()
    {
        if (!CanUse()) return;
        if (OnUse()) lastUsedTime = Time.time;
    }

    protected abstract bool OnUse();
    public virtual void Update() { }

    public float CooldownRatio
    {
        get
        {
            float elapsed = Time.time - lastUsedTime;
            return Mathf.Clamp01(1 - (elapsed / data.cooldown));
        }
    }
}

public class BasicAttackSkill : Skill
{
    Rigidbody2D rb;
    Animator anim;
    GameObject[] attSlash;
    Transform hand;

    int fxIndex = 0;
    bool isAttacking = false;
    float attackStartTime;

    readonly float fxStartDelay = 0.2f;
    readonly float fxDuration = 0.1f;
    readonly float gravityResetDelay = 1.0f;

    GameObject currentFx;

    public BasicAttackSkill(Player player, SkillData data, Rigidbody2D rb, Animator anim, Transform hand, GameObject[] attSlash)
        : base(player, data)
    {
        this.rb = rb;
        this.anim = anim;
        this.hand = hand;
        this.attSlash = attSlash;
    }

    protected override bool OnUse()
    {
        if (isAttacking) return false;

        //if (Input.GetKey(KeyCode.DownArrow))
        //{
        //    rb.gravityScale = 15.0f;
        //    anim.SetTrigger("downAttack");
        //    fxIndex = 1;

        //    var effect = Object.Instantiate(attSlash[fxIndex], hand);
        //    effect.transform.SetParent(hand);
        //    effect.transform.localPosition = effect.transform.localPosition;
        //}
        //else
        //{
            anim.SetTrigger("attack");
            fxIndex = 0;

            var effect = Object.Instantiate(attSlash[fxIndex], hand);
            effect.transform.SetParent(hand);
            effect.transform.localPosition = effect.transform.localPosition;
        //}

        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Sword);

        isAttacking = true;
        attackStartTime = Time.time;

        return true;
    }

    public override void Update()
    {
        if (!isAttacking) return;

        float elapsed = Time.time - attackStartTime;

        // FX 지속시간이 끝나면 삭제
        if (elapsed >= fxStartDelay + fxDuration)
        {
            if (currentFx != null)
            {
                Object.Destroy(currentFx);
                currentFx = null;
            }
            isAttacking = false;
        }

        // 아래공격일 때 중력 복원
        if (fxIndex == 1 && elapsed >= gravityResetDelay)
            rb.gravityScale = 15.0f;
    }
}


public class LeafSkill : Skill
{
    Animator anim;
    Transform shootPos;

    public LeafSkill(Player player, SkillData data, Animator anim, Transform shootPos)
        : base(player, data)
    {
        this.anim = anim;
        this.shootPos = shootPos;
    }

    protected override bool OnUse()
    {
        if (GameMgr.inst.userData.PlayerMp < data.mpCost) return false;
        GameMgr.inst.userData.PlayerMp -= data.mpCost;

        var leaf = Object.Instantiate(data.skillPrefab, shootPos.position, Quaternion.identity);

        anim.SetTrigger("attack");
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Leaf);

        return true;
    }
}

// ---------------------------

public class StealthSkill : Skill
{
    float stealthEndTime;

    public StealthSkill(Player player, SkillData data) : base(player, data) { }

    protected override bool OnUse()
    {
        if (GameMgr.inst.userData.PlayerMp < data.mpCost) return false;

        GameMgr.inst.userData.PlayerMp -= data.mpCost;

        player.isStealth = true;
        stealthEndTime = Time.time + data.duration;  // duration 동안 은신 유지
        foreach (var sr in player.GetComponentsInChildren<SpriteRenderer>())
        {
            var c = sr.color;
            c.a = 0.5f;
            sr.color = c;
        }

        return true;
    }

    public override void Update()
    {
        if (player.isStealth && Time.time >= stealthEndTime)
        {
            player.isStealth = false;
            foreach (var sr in player.GetComponentsInChildren<SpriteRenderer>())
            {
                var c = sr.color;
                c.a = 1f;
                sr.color = c;
            }
        }
    }
}

public class SummonSpiritSkill : Skill
{
    public SummonSpiritSkill(Player player, SkillData data) : base(player, data) { }

    protected override bool OnUse()
    {
        if (GameMgr.inst.userData.PlayerMp < data.mpCost) return false;

        GameMgr.inst.userData.PlayerMp -= data.mpCost;

        // 스킬 프리팹 인스턴스 생성
        GameObject spirit = Object.Instantiate(data.skillPrefab, player.transform.position, Quaternion.identity);
        spirit.SetActive(true);
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Fairy);

        return true;
    }
}

public class GrenadeSkill : Skill
{
    Animator anim;
    Transform shootPos;

    int grenadeCount = 3;
    float positionOffsetRange = 0.2f;
    float[] forceMultiplier = { 0.8f, 0.9f, 1f };

    public GrenadeSkill(Player player, SkillData data, Animator anim, Transform shootPos)
        : base(player, data)
    {
        this.anim = anim;
        this.shootPos = shootPos;
    }

    protected override bool OnUse()
    {
        if (GameMgr.inst.userData.PlayerMp < data.mpCost) return false;

        anim.SetTrigger("attack");
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Bomb);

        GameMgr.inst.userData.PlayerMp -= data.mpCost;

        for (int i = 0; i < grenadeCount; i++)
        {
            Vector3 spawnPos = shootPos.position + new Vector3(Random.Range(-positionOffsetRange, positionOffsetRange), 0, 0);

            GameObject grenade = Object.Instantiate(data.skillPrefab, spawnPos, shootPos.rotation);

            Rigidbody2D rb = grenade.GetComponent<Rigidbody2D>();
            Vector2 throwDir = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;
            rb.AddForce(throwDir * data.throwforce * forceMultiplier[i], ForceMode2D.Impulse);
        }

        return true;
    }
}

// PlayerCombat은 단순 호출자
public class PlayerCombat : MonoBehaviour
{
    Player player;

    #region Skill Property
    [Header("Skill Objects")]
    public Transform shootPos;
    public Transform hand;
    public GameObject SpiritPrefab;

    public Image[] skillCooldownImages; // Z, X, C 슬롯 UI
    enum ZxcSlot { z, x, c };

    // SkillData 기반 딕셔너리
    public Dictionary<SkillData, Skill> skills = new Dictionary<SkillData, Skill>();
    #endregion

    public bool IsPressingDown => Input.GetKey(KeyCode.DownArrow);

    void Awake()
    {
        player = GetComponent<Player>();
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        Animator anim = GetComponent<Animator>();

        // SkillMgr에 등록된 모든 SkillData를 기반으로 Skill 객체 생성
        foreach (var skillData in SkillMgr.inst.allSkills)
        {
            if (skillData == null) continue;

            Skill skill = null;
            string id = skillData.skillID;  // 소문자로 통일

            // skillData에 skillID(문자열) 필드 있다고 가정
            switch (id)
            {
                case "BasicAttack":
                    skill = new BasicAttackSkill(player, skillData, rb, anim, hand, player.AttSlash);
                    break;
                case "Leaf":
                    skill = new LeafSkill(player, skillData, anim, shootPos);
                    break;
                case "Stealth":
                    skill = new StealthSkill(player, skillData);
                    break;
                case "SummonSpirit":
                    skill = new SummonSpiritSkill(player, skillData);
                    break;
                case "AcornGrenade":
                    skill = new GrenadeSkill(player, skillData, anim, shootPos);
                    break;
                default:
                    Debug.LogWarning($"정의되지 않은 스킬ID: {skillData.skillID}");
                    break;
            }

            if (skill != null)
                skills[skillData] = skill;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Z)) UseSlotSkill((int)ZxcSlot.z);
        if (Input.GetKeyDown(KeyCode.X)) UseSlotSkill((int)ZxcSlot.x);
        if (Input.GetKeyDown(KeyCode.C)) UseSlotSkill((int)ZxcSlot.c);
        if (Input.GetKeyDown(KeyCode.Alpha1)) UseQuickSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseQuickSlot(1);

        foreach (var kvp in skills)
            kvp.Value?.Update();

        UpdateCooldownUI();
    }

    void UseSlotSkill(int slotIndex)
    {
        var slotSkill = GameMgr.inst.userData.SkillSlots[slotIndex];
        if (slotSkill == null) return;
        if (!skills.ContainsKey(slotSkill)) return;

        skills[slotSkill].Use();

        player.MpBar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;
    }

    void UpdateCooldownUI()
    {
        for (int i = 0; i < skillCooldownImages.Length; i++)
        {
            var slotSkill = GameMgr.inst.userData.SkillSlots[i];
            if (slotSkill == null) continue;
            if (!skills.ContainsKey(slotSkill)) continue;
            if (skillCooldownImages[i] == null) continue;

            skillCooldownImages[i].fillAmount = skills[slotSkill].CooldownRatio;
        }
    }

    void UseQuickSlot(int index)
    {
        var userData = GameMgr.inst.userData;
        var itemData = GameMgr.inst.itemData;

        if (index < 0 || index >= userData.quickSlots.Length)
            return;

        ItemBase potion = userData.quickSlots[index].potion;

        if (potion == null)
        {
            Debug.Log($"퀵슬롯 {index + 1}에 포션이 등록되지 않았습니다.");
            return;
        }

        // 해당 SO 실행
        potion.Execute(userData, itemData);

        // 수량 감소
        if (itemData.ItemDict.ContainsKey(potion))
        {
            if (itemData.ItemDict[potion] <= 0)
            {
                itemData.ItemDict[potion] = 0;
                userData.quickSlots[index].potion = null;
                Debug.Log($"{potion.ItemName} 다 써서 퀵슬롯 {index + 1} 비워짐");
            }
        }

        // UI 반영
        GameMgr.inst.UpdateQuickSlotsCount(potion);
    }
}