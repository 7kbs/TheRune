using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour
{
    [SerializeField] private ItemData itemData;
    [SerializeField] private Transform slotParent; // 슬롯 50개 존재
    [SerializeField] private GameObject partPrefab;
    [SerializeField] private Canvas mainCanvas; // 마우스 아이콘 표시용

    private List<Inventory_Parts> parts = new();
    private Inventory_Parts selectedPart = null;

    private Image dragIcon;
    private RectTransform dragRect;

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

        // 드래그용 아이콘
        GameObject iconObj = new GameObject("DragIcon");
        iconObj.transform.SetParent(mainCanvas.transform, false);
        dragRect = iconObj.AddComponent<RectTransform>();
        dragIcon = iconObj.AddComponent<Image>();
        dragIcon.raycastTarget = false;
        dragIcon.enabled = false;
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

    void OnEnable()
    {
        itemData.OnInventoryChanged += Refresh;
        Refresh();
    }

    void OnDisable()
    {
        itemData.OnInventoryChanged -= Refresh;
    }

    public void Refresh()
    {
        foreach (var part in parts)
            part.Init(null, 0);

        int i = 0;
        foreach (var kv in itemData.ItemDict)
        {
            if (kv.Key is SpiritPiece) continue;
            if (i >= parts.Count) break;

            parts[i].Init(kv.Key, kv.Value);
            i++;
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
    private void StartDragIcon(Inventory_Parts part)
    {
        if (part.icon == null || part.icon.sprite == null) return;

        dragIcon.sprite = part.icon.sprite;
        dragIcon.enabled = true;

        part.icon.enabled = false; // 슬롯 이미지 숨기기
    }


    // 선택 해제 시 복원
    private void StopDragIcon()
    {
        if (selectedPart != null && selectedPart.icon != null)
            selectedPart.icon.enabled = true; // 슬롯 이미지 복구

        dragIcon.enabled = false;
    }
}