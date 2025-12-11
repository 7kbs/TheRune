using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Base
{
    [SerializeField] private ItemDB itemData;     //긁어올 item DataBase Storage
    [SerializeField] private Transform slotParent; // 슬롯 50개 존재
    [SerializeField] private GameObject partPrefab;
    [SerializeField] private Canvas mainCanvas; // 마우스 아이콘 표시용

    private List<Inventory_Parts> parts = new();
    private Inventory_Parts selectedPart = null;

    [SerializeField] private Image dragIcon;
    [SerializeField] private Text dragCountText;
    [SerializeField] private RectTransform dragRect;

    [SerializeField] Text MoneyText;

    public static UI_Inventory inst;

    void Awake()
    {
        inst = this;

        for (int i = 0; i < slotParent.childCount; i++)
        {
            var slot = slotParent.GetChild(i);
            var partObj = Instantiate(partPrefab, slot);
            var part = partObj.GetComponent<Inventory_Parts>();
            part.Init(null, 0);
            parts.Add(part);
        }
    }

    void Update()
    {
        if (dragIcon.enabled)
        {
            Vector2 pos;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                mainCanvas.transform as RectTransform,
                Input.mousePosition,
                mainCanvas.worldCamera,
                out pos);
            dragRect.anchoredPosition = pos;
        }
    }

    void Init()
    {
        Refresh();
        MoneyText.text = GameMgr.inst.userData.GameMoney.ToString();
    }

    void OnEnable()
    {
        itemData.OnInventoryChanged += Refresh;
        Init();
    }

    void OnDisable()
    {
        itemData.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        // 1) 모든 슬롯 일단 sync만 한다 (데이터 유지)
        foreach (var part in parts)
        {
            if (part.data == null) continue;

            if (itemData.ItemDictionary.TryGetValue(part.data, out int count))
            {
                part.Init(part.data, count);
            }
            else
            {
                // 해당 아이템이 dict에 없다 = 0개
                part.Init(null, 0);
            }
        }

        // 2) dict에 있는데 UI에 없는 아이템을 추가
        foreach (var kv in itemData.ItemDictionary)
        {
            var item = kv.Key;
            var count = kv.Value;

            if (count <= 0) continue;

            // Skip: 이미 슬롯에 존재하는 아이템
            bool exists = parts.Any(p => p.data == item);
            if (exists) continue;

            // Empty slot 찾아서 배치
            var empty = parts.FirstOrDefault(p => p.data == null);
            if (empty != null)
            {
                empty.Init(item, count);
            }
        }
    }

    public void OnSlotClicked(Inventory_Parts clickedPart)
    {
        if (selectedPart == null)
        {
            if (clickedPart.IsEmpty) return;
            selectedPart = clickedPart;
            StartDragIcon(clickedPart);
            return;
        }

        if (clickedPart == selectedPart)
        {
            StopDragIcon();
            selectedPart = null;
            return;
        }

        Swap(selectedPart, clickedPart);

        StopDragIcon();
        selectedPart = null;
    }

    private void Swap(Inventory_Parts a, Inventory_Parts b)
    {
        var tempData = a.data;
        var tempCount = a.count;

        a.Init(b.data, b.count);
        b.Init(tempData, tempCount);
    }


    // 선택 시 아이콘 마우스에 붙이고 슬롯에선 숨김
    void StartDragIcon(Inventory_Parts part)
    {
        if (part.icon == null || part.icon.sprite == null) return;

        dragIcon.sprite = part.icon.sprite;
        dragIcon.enabled = true;

        dragCountText.text = part.count > 1 ? part.count.ToString() : "";
        dragCountText.enabled = true;

        part.icon.enabled = false;
        part.countText.text = "";       
    }


    // 선택 해제 시 복원
    private void StopDragIcon()
    {
        if (selectedPart != null)
        {
            if (selectedPart.icon != null)
                selectedPart.icon.enabled = true;

            if (selectedPart.countText != null)
            {
                selectedPart.countText.text =
                    selectedPart.count > 1 ? selectedPart.count.ToString() : "";
            }
        }

        dragIcon.enabled = false;
        dragCountText.enabled = false;
    }
}