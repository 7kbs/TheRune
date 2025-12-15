using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SkillShopProduct : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("스킬 정보")]
    public SkillData skillData;  // 직접 참조 (이전 SkillType 대체)

    [Header("UI 요소")]
    public Button skillBtn;
    public Text skillDes;
    public Text price;
    public Image skillIcon;
    public Image fillImage;

    private float fillSpeed = 0.5f;
    private bool isFill = false;

    void Start()
    {
        if (skillData == null)
        {
            Debug.LogError($"{name} : SkillData가 연결되지 않았습니다!");
            return;
        }

        // UI 초기화
        skillDes.text = $"[{skillData.skillName}]\n<size=21>{skillData.skillDescription}</size>";
        price.text = $"{skillData.price}";
        skillIcon.sprite = skillData.skillIcon;

        // 이미 배운 스킬이면 구매 불가 처리
        if (GameMgr.inst.userData.LearnedSkills.Contains(skillData))
        {
            fillImage.fillAmount = 1.0f;
            skillBtn.interactable = false;
        }
    }

    void Update()
    {
        if (isFill)
        {
            fillImage.fillAmount += fillSpeed * Time.deltaTime;

            if (fillImage.fillAmount >= 1.0f)
            {
                fillImage.fillAmount = 1.0f;
                isFill = false;
                skillBtn.interactable = false;

                // 구매 처리
                GameMgr.inst.userData.LearnedSkills.Add(skillData);
                GameMgr.inst.userData.GameMoney -= skillData.price;

                // 스킬 획득 반영
                SkillUIMgr.inst.AcquireSkills();

                UIManager.inst.GetToast().Init("ESC키를 눌러 새로운 스킬을 등록하세요!", Color.white);
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (skillData == null) return;

        if (GameMgr.inst.userData.LearnedSkills.Contains(skillData))
            return;

        if (GameMgr.inst.userData.GameMoney < skillData.price)
        {
            UIManager.inst.GetToast().Init("실바론이 숲의 힘을 더 필요로 합니다.", Color.white);
            return;
        }

        isFill = true;
        UIManager.inst.GetToast().Init("길게 눌러 스킬을 구매하세요!", Color.white);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (skillData == null) return;

        if (GameMgr.inst.userData.LearnedSkills.Contains(skillData))
            fillImage.fillAmount = 1.0f;
        else
            fillImage.fillAmount = 0.0f;

        isFill = false;
    }
}
