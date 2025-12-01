using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCombat : MonoBehaviour
{
    [Header("References")]
    public Player player;
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody2D rb;

    [Header("Skill Settings")]
    public Transform shootPos;
    public Transform hand;
    public Image[] skillCooldownImages;
    public KeyCode[] skillKeys = { KeyCode.Z, KeyCode.X, KeyCode.C };

    [Header("State")]
    public bool isStealth;
    public bool isAttacking;

    // 런타임 상태 관리
    private Dictionary<SkillData, SkillRuntime> skillStates = new();

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        // 스킬 사용
        for (int i = 0; i < skillKeys.Length; i++)
        {
            if (Input.GetKeyDown(skillKeys[i]))
                UseSlotSkill(i);
        }

        // 런타임 스킬 업데이트
        //foreach (var runtime in skillStates.Values)
        //{
        //    if (runtime.isActive)
        //        runtime.data.RuntimeUpdate(this, runtime);
        //}

        UpdateCooldownUI();
    }

    void UpdateCooldownUI()
    {
        for (int i = 0; i < skillCooldownImages.Length; i++)
        {
            var slotSkill = GameMgr.inst.userData.SkillSlots[i];
            if (slotSkill == null || skillCooldownImages[i] == null) continue;

            if (!skillStates.ContainsKey(slotSkill))
                skillStates[slotSkill] = new SkillRuntime(slotSkill);

            var state = skillStates[slotSkill];
            float elapsed = Time.time - state.lastUsedTime;
            skillCooldownImages[i].fillAmount = Mathf.Clamp01(1 - (elapsed / slotSkill.cooldown));
        }
    }

    void UseSlotSkill(int slotIndex)
    {
        var slotSkill = GameMgr.inst.userData.SkillSlots[slotIndex];
        if (slotSkill == null) return;

        if (!skillStates.ContainsKey(slotSkill))
            skillStates[slotSkill] = new SkillRuntime(slotSkill);

        var state = skillStates[slotSkill];

        // 쿨타임 체크
        if (Time.time < state.lastUsedTime + slotSkill.cooldown) return;

        state.lastUsedTime = Time.time;

        //slotSkill.Execute(this, state);
        if (slotSkill.mpCost >= 0)
        {
            if (GameMgr.inst.userData.PlayerMp < slotSkill.mpCost)
            {
                state.isActive = false;
                return;
            }
            GameMgr.inst.userData.PlayerMp -= slotSkill.mpCost;

            var obj = Instantiate(slotSkill.skillPrefab);
            obj.GetComponent<ISkillBehaviour>().OnExecute(this, slotSkill, state);
        }

        player.MpBar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;
    }
}