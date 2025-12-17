using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SkillUIMgr : UI_Base
{
    public static SkillUIMgr inst;
    [HideInInspector] public PlayerCombat pc;

    [Header("References")]
    public UserData userData;

    [Header("UI Elements")]
    public GameObject DragIconObj;
    public Image[] SkillIconsUI;

    void Awake()
    {
        inst = this;

        pc = FindAnyObjectByType<PlayerCombat>();
    }

    public override void OnOpen()
    {
        AcquireSkills();
    }

    public void AcquireSkills()
    {
        // SkillIconsUI 배열에 연결된 SkillParts 컴포넌트 기준으로 UI 활성화
        for (int i = 0; i < SkillIconsUI.Length; i++)
        {
            if (SkillIconsUI[i] == null) continue;

            var skillPart = SkillIconsUI[i].GetComponent<SkillParts>();
            if (skillPart == null || skillPart.skillData == null) continue;

            // LearnedSkills에 있는지 확인 후 활성화
            SkillIconsUI[i].gameObject.SetActive(userData.LearnedSkills.Contains(skillPart.skillData));
        }
    }
}