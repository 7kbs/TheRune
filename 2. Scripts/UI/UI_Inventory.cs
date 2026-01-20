using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UI_Inventory : UI_Base
{
    [SerializeField] ItemDB itemData;
    [SerializeField] Transform slotParent;
    [SerializeField] GameObject partPrefab;
    [SerializeField] Canvas mainCanvas;

    List<Inventory_Parts> parts = new();
    Inventory_Parts selectedPart = null;

    [SerializeField] Image dragIcon;
    [SerializeField] Text dragCountText;
    [SerializeField] RectTransform dragRect;

    [SerializeField] GameObject ghostPrefab; // Image + CountText 포함
    GameObject ghostObj;

    [SerializeField] Text MoneyText;

    public static UI_Inventory inst;

    void Awake()
    {
        inst = this;
        mainCanvas = GameMgr.inst.canvas;

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
        if (!dragIcon.enabled) return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            mainCanvas.transform as RectTransform,
            Input.mousePosition,
            mainCanvas.worldCamera,
            out pos);
        dragRect.anchoredPosition = pos;

        // 슬롯 아닌 곳 클릭 시 실패 처리
        if (Input.GetMouseButtonDown(0))
        {
            if (!IsPointerOverSlot())
                CancelDrag();
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
        foreach (var part in parts)
        {
            if (part.data == null) continue;

            if (itemData.ItemDictionary.TryGetValue(part.data, out int count))
                part.Init(part.data, count);
            else
                part.Init(null, 0);
        }

        foreach (var kv in itemData.ItemDictionary)
        {
            var item = kv.Key;
            var count = kv.Value;
            if (count <= 0) continue;

            if (parts.Any(p => p.data == item)) continue;

            var empty = parts.FirstOrDefault(p => p.data == null);
            if (empty != null)
                empty.Init(item, count);
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
            CancelDrag();
            return;
        }

        Swap(selectedPart, clickedPart);
        EndDrag();
    }

    void Swap(Inventory_Parts a, Inventory_Parts b)
    {
        var tempData = a.data;
        var tempCount = a.count;
        a.Init(b.data, b.count);
        b.Init(tempData, tempCount);
    }

    void StartDragIcon(Inventory_Parts part)
    {
        if (part.icon == null || part.icon.sprite == null) return;

        ghostObj = Instantiate(ghostPrefab, part.transform);
        var ghostIcon = ghostObj.GetComponentInChildren<Image>();
        var ghostText = ghostObj.GetComponentInChildren<Text>();

        ghostIcon.sprite = part.icon.sprite;

        var baseColor = part.icon.color;
        ghostIcon.color = new Color(baseColor.r, baseColor.g, baseColor.b, 0.4f);

        ghostText.text = part.count > 1 ? part.count.ToString() : "";

        ghostObj.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

        dragIcon.sprite = part.icon.sprite;
        dragIcon.color = part.icon.color;
        dragIcon.enabled = true;

        dragCountText.text = part.count > 1 ? part.count.ToString() : "";
        dragCountText.enabled = true;

        part.icon.enabled = false;
        part.countText.text = "";
    }

    void EndDrag()
    {
        if (ghostObj != null)
            Destroy(ghostObj);

        if (selectedPart != null)
        {
            selectedPart.icon.enabled = true;
            selectedPart.countText.text =
                selectedPart.count > 1 ? selectedPart.count.ToString() : "";
        }

        dragIcon.enabled = false;
        dragCountText.enabled = false;
        selectedPart = null;
    }

    void CancelDrag()
    {
        EndDrag(); // 데이터 변화 없이 복귀
    }

    bool IsPointerOverSlot()
    {
        foreach (var p in parts)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(
                p.GetComponent<RectTransform>(),
                Input.mousePosition,
                mainCanvas.worldCamera))
                return true;
        }
        return false;
    }
}