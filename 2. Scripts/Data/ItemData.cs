using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerItemData", menuName = "ScriptableObject/Item")]
public class ItemData : ScriptableObject
{
    [Header("Item Pool (Inspector)")]
    public List<ItemBase> allItems = new(); // 전체 아이템 풀, 에셋에서 등록

    [Header("Player Inventory")]
    public List<InventoryItem> inventoryItems = new(); // 플레이어가 가진 아이템, 인스펙터 표시
    [NonSerialized] public Dictionary<ItemBase, int> ItemDict = new(); // 런타임 최적화

    public event Action OnInventoryChanged;

    // 초기화: 모든 아이템 풀을 기반으로 inventoryItems 초기화
    public void InitData()
    {
        ItemDict.Clear();
        inventoryItems.Clear();

        foreach (var item in allItems)
        {
            ItemDict[item] = 0;
            inventoryItems.Add(new InventoryItem { item = item, count = 0 });
        }

        OnInventoryChanged?.Invoke();
    }

    // 리스트 → 딕셔너리 동기화
    public void SyncDictFromList()
    {
        ItemDict.Clear();
        foreach (var inv in inventoryItems)
        {
            if (inv.item != null)
                ItemDict[inv.item] = inv.count;
        }
    }

    // 아이템 추가
    public void AddItem(ItemBase item, int amount = 1)
    {
        if (item == null) return;

        if (!ItemDict.ContainsKey(item))
        {
            ItemDict[item] = amount;
            inventoryItems.Add(new InventoryItem { item = item, count = amount });
        }
        else
        {
            ItemDict[item] += amount;
            var inv = inventoryItems.Find(i => i.item == item);
            if (inv != null)
                inv.count = ItemDict[item];
        }

        OnInventoryChanged?.Invoke();
    }

    // 아이템 사용
    public bool UseItem(ItemBase item, int amount = 1)
    {
        if (item == null || !ItemDict.ContainsKey(item) || ItemDict[item] < amount)
            return false;

        ItemDict[item] -= amount;
        var inv = inventoryItems.Find(i => i.item == item);
        if (inv != null)
            inv.count = ItemDict[item];

        if (ItemDict[item] <= 0)
        {
            ItemDict.Remove(item);
            inventoryItems.RemoveAll(i => i.item == item);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    // 아이템 제거
    public void RemoveItem(ItemBase item)
    {
        if (item == null) return;

        if (ItemDict.ContainsKey(item))
        {
            ItemDict.Remove(item);
            inventoryItems.RemoveAll(i => i.item == item);
            OnInventoryChanged?.Invoke();
        }
    }
}

[Serializable]
public class InventoryItem
{
    public ItemBase item;
    public int count;
}