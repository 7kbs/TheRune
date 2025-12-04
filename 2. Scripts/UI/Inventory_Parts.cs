using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Inventory_Parts : MonoBehaviour, IPointerClickHandler
{
    public Image icon;
    public Text countText;
    [SerializeField] private Sprite emptySprite; // 빈 슬롯용 스프라이트 (투명 혹은 회색)

    public ItemBase data { get; private set; }
    public int count { get; private set; }

    // 슬롯이 빈 상태인가
    public bool IsEmpty => data == null;

    // 초기화 또는 데이터 갱신: 슬롯 오브젝트 자체는 항상 존재
    public void Init(ItemBase item, int amount)
    {
        data = item;
        count = amount;

        if (item == null)
        {
            icon.sprite = emptySprite;
            countText.text = "";
            icon.color = new Color(0, 0, 0, 0); // 빈칸은 반투명으로 표시 (원하면 0)
        }
        else
        {
            icon.sprite = item.icon;
            countText.text = amount > 1 ? amount.ToString() : "";
            icon.color = Color.white;
        }
    }

    // UI에서 보여줄 아이콘 스프라이트(외부에서 필요)
    public Sprite GetIcon() => icon.sprite;

    // 슬롯 클릭: UI_Inventory에 위임
    public void OnPointerClick(PointerEventData eventData)
    {
        UI_Inventory.inst.OnSlotClicked(this);
    }
}