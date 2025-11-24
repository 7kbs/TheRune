using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class SkillUIMgr : MonoBehaviour
{
    public static SkillUIMgr inst;

    [Header("References")]
    public UserData userData;

    [Header("UI Elements")]
    public GameObject DragIconObj;
    public Image[] DragIconImages;
    public Image[] SkillIconsUI;
    public Text GoldText;

    void Awake()
    {
        inst = this;
    }

    void Start()
    {
        AcquireSkills();
        InitUI();
    }

    public void InitUI()
    {
        for (int i = 0; i < DragIconImages.Length; i++)
        {
            var skill = userData.SkillSlots[i];
            if (skill != null)
            {
                DragIconImages[i].sprite = skill.skillIcon;
                DragIconImages[i].enabled = true;
            }
            else
            {
                DragIconImages[i].sprite = null;
                DragIconImages[i].enabled = false;
            }
        }
    }

    public void AcquireSkills()
    {
        GoldText.text = userData.GameMoney.ToString();

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