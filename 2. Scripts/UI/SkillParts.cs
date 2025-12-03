using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SkillParts : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public SkillData skillData; // 직접 드래그
    public Text skill_Name;
    public Text skill_Info;

    private RectTransform dragRect;
    private Image dragImg;
    private SkillUIMgr mgr;

    void Start()
    {
        mgr = SkillUIMgr.inst;
        if (mgr?.DragIconObj != null)
        {
            dragRect = mgr.DragIconObj.GetComponent<RectTransform>();
            if (mgr.DragIconObj.transform.childCount > 0)
                dragImg = mgr.DragIconObj.transform.GetChild(0).GetComponent<Image>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (dragImg == null || skillData == null) return;

        mgr.DragIconObj.SetActive(true);
        dragImg.sprite = skillData.skillIcon;
        dragImg.enabled = true;
        UpdateDragPosition();
    }

    public void OnDrag(PointerEventData eventData) => UpdateDragPosition();

    public void OnEndDrag(PointerEventData eventData)
    {
        if (skillData == null || mgr?.DragIconImages == null) return;
        mgr.DragIconObj.SetActive(false);

        Vector2 mousePos = Input.mousePosition;

        for (int i = 0; i < mgr.DragIconImages.Length; i++)
        {
            var img = mgr.DragIconImages[i];
            if (img == null) continue;

            if (RectTransformUtility.RectangleContainsScreenPoint(img.rectTransform, mousePos))
            {
                // 기존 슬롯 중복 제거
                for (int j = 0; j < mgr.userData.SkillSlots.Length; j++)
                    if (mgr.userData.SkillSlots[j] == skillData) mgr.userData.SkillSlots[j] = null;

                // 신규 배치
                if (i < mgr.userData.SkillSlots.Length)
                {
                    mgr.userData.SkillSlots[i] = skillData;
                    img.sprite = skillData.skillIcon;
                    img.enabled = true;
                    mgr.InitUI();
                }
                break;
            }
        }
    }

    void UpdateDragPosition()
    {
        if (dragRect != null) dragRect.position = Input.mousePosition;
    }

    public void ShowSkillInfo()
    {
        if (skill_Name == null || skill_Info == null || skillData == null) return;

        if (!mgr.userData.LearnedSkills.Contains(skillData))
        {
            skill_Name.text = "???";
            skill_Info.text = "????\n??????";
            return;
        }

        skill_Name.gameObject.SetActive(true);
        skill_Info.gameObject.SetActive(true);
        skill_Name.text = skillData.skillName;
        skill_Info.text =
            $"Damage: {skillData.damage}\n" +
            $"Cooldown: {skillData.cooldown}s\n" +
            (skillData.duration > 0 ? $"Duration: {skillData.duration}s\n" : "") +
            $"MP Cost: {skillData.mpCost}\n\n" +
            $"{skillData.skillDescription}";
    }
}
