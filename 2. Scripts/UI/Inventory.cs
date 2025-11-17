using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


[Serializable]
public class QuickSlotData
{
    public ItemBase potion; // 등록된 포션 SO만 저장
}


[Serializable]
public class QuickSlot
{
    public Image icon;
    public Text countText;

    public void Assign(ItemBase potion, int count)
    {
        if (potion == null)
        {
            Clear();
            return;
        }

        // 아이콘 교체
        icon.sprite = potion.icon;
        icon.gameObject.SetActive(true);

        // 포션 타입에 따라 색상 변경
        if (potion.ItemName.Contains("HP"))
            icon.color = new Color32(255, 34, 34, 255);
        else if (potion.ItemName.Contains("MP"))
            icon.color = new Color32(75, 186, 255, 255);
        else
            icon.color = Color.white;

        // 수량 반영
        countText.text = count.ToString();
        countText.gameObject.SetActive(true);
    }

    public void UpdateCount(int count)
    {
        countText.text = count.ToString();
    }

    public void Clear()
    {
        icon.gameObject.SetActive(false);
        countText.gameObject.SetActive(false);
    }
}


public class Inventory : MonoBehaviour
{
    [Header("슬롯 UI")]
    [SerializeField] GameObject[] slots;
    [SerializeField] Image[] slotIcons;
    [SerializeField] Text[] slotCounts;

    [Header("설명 UI")]
    [SerializeField] Text itemNameText;
    [SerializeField] Text itemDescText;
    [SerializeField] Button registerButton;

    int selectedQuickSlot = -1;

    UserData userData;
    ItemData itemData;

    ItemBase selectedPotion = null;
    public bool registerMode = false;

    void OnEnable()
    {
        itemData = GameMgr.inst.itemData;
        userData = GameMgr.inst.userData;

        itemNameText.gameObject.SetActive(false);
        itemDescText.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);

        RefreshInventoryUI();
    }


    void RefreshInventoryUI()
    {
        int slotIndex = 0;

        foreach (var kvp in itemData.ItemDict)
        {
            var item = kvp.Key;
            int count = kvp.Value;

            if (count <= 0) continue;
            if (item is SpiritPiece) continue; // 표시 제외

            if (slotIndex >= slots.Length) break;

            slots[slotIndex].SetActive(true);
            slotIcons[slotIndex].gameObject.SetActive(true);
            slotIcons[slotIndex].sprite = item.icon;
            slotCounts[slotIndex].gameObject.SetActive(true);
            slotCounts[slotIndex].text = count.ToString();

            slotIndex++;
        }

        // 남는 슬롯 비우기
        for (int i = slotIndex; i < slots.Length; i++)
        {
            slots[i].SetActive(true);
            slotIcons[i].gameObject.SetActive(false);
            slotCounts[i].gameObject.SetActive(false);
        }
    }


    public void OnSlotClick(int slotIndex)
    {
        if (slotIndex >= itemData.ItemDict.Count) return;

        var itemArray = itemData.ItemDict.ToArray();
        var item = itemArray[slotIndex].Key;
        int count = itemArray[slotIndex].Value;

        if (count <= 0) return;

        selectedPotion = item;

        itemNameText.gameObject.SetActive(true);
        itemDescText.gameObject.SetActive(true);
        itemNameText.text = selectedPotion.ItemName;
        itemDescText.text = selectedPotion.Description;

        registerButton.gameObject.SetActive(selectedPotion.Consumable);
        registerButton.onClick.RemoveAllListeners();
        registerButton.onClick.AddListener(() => { registerMode = true; });
    }


    public void OnQuickSlotClick(int slotIndex)
    {
        if (!registerMode)
        {
            Debug.Log("등록 모드가 아닙니다.");
            selectedQuickSlot = slotIndex; // 슬롯 선택만
            return;
        }

        if (selectedPotion == null)
        {
            Debug.LogWarning("선택된 포션이 없습니다.");
            return;
        }

        if (slotIndex < 0 || slotIndex >= GameMgr.inst.userData.quickSlots.Length)
        {
            Debug.LogWarning("퀵슬롯 범위를 벗어났습니다.");
            return;
        }

        RegisterPotionToSlot(slotIndex, selectedPotion);

        // 등록 후 초기화
        registerMode = false;
        selectedPotion = null;
        selectedQuickSlot = -1;
        itemNameText.gameObject.SetActive(false);
        itemDescText.gameObject.SetActive(false);
        registerButton.gameObject.SetActive(false);
    }


    void RegisterPotionToSlot(int slotIndex, ItemBase potion)
    {
        // 중복 제거
        for (int i = 0; i < userData.quickSlots.Length; i++)
        {
            if (userData.quickSlots[i].potion == potion)
            {
                userData.quickSlots[i].potion = null;
                GameMgr.inst.quickSlotUIs[i].Clear();
                Debug.Log($"퀵슬롯 {i + 1}에서 중복 제거 완료");
            }
        }

        int count = itemData.ItemDict.ContainsKey(potion) ? itemData.ItemDict[potion] : 0;

        // UserData에 SO 저장
        userData.quickSlots[slotIndex].potion = potion;

        // UI 업데이트
        GameMgr.inst.quickSlotUIs[slotIndex].Assign(potion, count);

        Debug.Log($"퀵슬롯 {slotIndex + 1}에 {potion.ItemName} 등록 완료");
    }
}