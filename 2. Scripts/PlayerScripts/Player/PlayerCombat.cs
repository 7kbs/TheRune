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
    public bool isStealth;
    public Image[] skillCooldownImages;
    public KeyCode[] skillKeys = { KeyCode.Z, KeyCode.X, KeyCode.C };

    // 쿨타임만 관리
    Dictionary<SkillData, float> lastUsedTimes = new();

    void Awake()
    {
        anim = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        for (int i = 0; i < skillKeys.Length; i++)
        {
            if (Input.GetKeyDown(skillKeys[i]))
                UseSlotSkill(i);
        }

        UpdateCooldownUI();
    }

    // 스킬 쿨타임 UI 갱신
    void UpdateCooldownUI()
    {
        for (int i = 0; i < skillCooldownImages.Length; i++)
        {
            var slotSkill = GameMgr.inst.userData.SkillSlots[i];
            if (slotSkill == null || skillCooldownImages[i] == null) continue; // 슬롯 또는 이미지가 없으면 건너뜀

            // 마지막 사용 시간 가져오기 (없으면 -999f로 초기화)
            float lastUsed = lastUsedTimes.ContainsKey(slotSkill) ? lastUsedTimes[slotSkill] : -999f;
            float elapsed = Time.time - lastUsed; // 경과 시간 계산

            // UI 이미지 fillAmount 업데이트 (0~1 범위로 제한)
            skillCooldownImages[i].fillAmount = Mathf.Clamp01(1 - (elapsed / slotSkill.cooldown));
        }
    }

    // 슬롯 인덱스에 해당하는 스킬 사용
    void UseSlotSkill(int slotIndex)
    {
        var slotSkill = GameMgr.inst.userData.SkillSlots[slotIndex];
        if (slotSkill == null) return; // 스킬이 없으면 종료

        // 마지막 사용 시간 체크
        float lastUsed = lastUsedTimes.ContainsKey(slotSkill) ? lastUsedTimes[slotSkill] : -999f;
        if (Time.time < lastUsed + slotSkill.cooldown) return; // 쿨타임 안끝났으면 종료

        lastUsedTimes[slotSkill] = Time.time; // 사용 시간 갱신

        // MP 부족 체크
        if (GameMgr.inst.userData.PlayerMp < slotSkill.mpCost) return;
        GameMgr.inst.userData.PlayerMp -= slotSkill.mpCost; // MP 차감

        // 스킬 Prefab 인스턴스화 후 실행
        var obj = Instantiate(slotSkill.skillPrefab);
        var behaviour = obj.GetComponent<ISkillBehaviour>();
        if (behaviour != null) behaviour.OnExecute(this, slotSkill); // 인터페이스 통해 스킬 실행

        // MP 바 UI 업데이트
        player.MpBar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;
    }
}