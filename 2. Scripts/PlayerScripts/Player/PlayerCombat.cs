using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    Player player;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody2D rb;

    [Space(20f)]
    #region Skill Property
    [Header("스킬 Property")]
    public Transform shootPos;
    public Transform hand;
    [Header("스킬 쿨타임 시각 UI")]
    public Image[] skillCooldownImages;

    [Space(20f)]
    public bool isStealth;
    [Space(20f)]

    [Header("Skill Keys")]
    public KeyCode[] skillKeys = { KeyCode.Z, KeyCode.X, KeyCode.C };

    // SkillData 기반 딕셔너리
    List<SkillData> runtimeSkills = new List<SkillData>();
    #endregion

    public void RegisterRuntimeSkillUpdate(SkillData sd)
    {
        if (!runtimeSkills.Contains(sd))
            runtimeSkills.Add(sd);
    }

    public bool IsPressingDown => Input.GetKey(KeyCode.DownArrow);

    void Awake()
    {
        player = GetComponent<Player>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        for (int i = 0; i < skillKeys.Length; i++)
        {
            if (Input.GetKeyDown(skillKeys[i]))
                UseSlotSkill(i);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1)) UseQuickSlot(0);
        if (Input.GetKeyDown(KeyCode.Alpha2)) UseQuickSlot(1);

        foreach (var sd in runtimeSkills.ToList())
        {
            if (sd is BasicAttackSO ba) ba.RuntimeUpdate(this);
            else if (sd is StealthSkillSO ss) ss.RuntimeUpdate(this);
            // 다른 스킬(Stealth, Grenade 등)도 같은 방식으로 확장
        }

        UpdateCooldownUI(); // 매 프레임 UI 업데이트
    }

    void UpdateCooldownUI()
    {
        for (int i = 0; i < skillCooldownImages.Length; i++)
        {
            var slotSkill = GameMgr.inst.userData.SkillSlots[i];
            if (slotSkill == null) continue;
            if (skillCooldownImages[i] == null) continue;

            // lastUsedTime과 cooldown은 SO 내부에 있으므로 계산
            float elapsed = Time.time - slotSkill.lastUsedTime;
            float fill = Mathf.Clamp01(1 - (elapsed / slotSkill.cooldown));
            skillCooldownImages[i].fillAmount = fill;
        }
    }

    void UseSlotSkill(int slotIndex)
    {
        var slotSkill = GameMgr.inst.userData.SkillSlots[slotIndex];
        if (slotSkill == null) return;
        //if (!skills.ContainsKey(slotSkill)) return;

        slotSkill.Execute(this);

        player.MpBar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;
    }


    void UseQuickSlot(int index)
    {
        var userData = GameMgr.inst.userData;
        var itemData = GameMgr.inst.itemData;

        if (index < 0 || index >= userData.quickSlots.Length)
            return;

        ItemBase potion = userData.quickSlots[index].potion;

        if (potion == null) return;

        // 해당 SO 실행
        potion.Execute(userData, itemData);

        // 수량 감소
        if (itemData.ItemDictionary.ContainsKey(potion))
        {
            if (itemData.ItemDictionary[potion] <= 0)
            {
                itemData.ItemDictionary[potion] = 0;
                userData.quickSlots[index].potion = null;
            }
        }

        // UI 반영
        GameMgr.inst.UpdateQuickSlotsCount(potion);
    }
}