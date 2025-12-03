using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;


[CreateAssetMenu(fileName = "PlayerItemData", menuName = "ScriptableObject/Item")]
public class ItemDB : ScriptableObject
{
    [Header("게임에 존재하는 모든 아이템 데이터")]
    public List<ItemBase> allItems = new();
    [SerializeField] string resourcesPath = "Item SO";

    [Header("인벤토리 데이터")]
    public List<InventoryItem> invenItems = new(); // 플레이어가 가진 아이템, 인스펙터 표시
    [NonSerialized] public Dictionary<ItemBase, int> ItemDictionary = new(); // 런타임용도

    public event Action OnInventoryChanged;

    // 초기화: 모든 아이템 풀을 기반으로 inventoryItems 초기화
    public void InitData()
    {
        ItemDictionary.Clear();
        invenItems.Clear();
        allItems = Resources.LoadAll<ItemBase>(resourcesPath).ToList();

        foreach (var item in allItems)
        {
            ItemDictionary[item] = 0;
            invenItems.Add(new InventoryItem { item = item, count = 0 });
        }

        OnInventoryChanged?.Invoke();
    }

    // 리스트 → 딕셔너리 동기화
    public void SyncDictFromList()
    {
        ItemDictionary.Clear();
        foreach (var inv in invenItems)
        {
            if (inv.item != null)
                ItemDictionary[inv.item] = inv.count;
        }
    }

    // 아이템 추가
    public void AddItem(ItemBase item, int amount = 1)
    {
        if (item == null) return;

        if (!ItemDictionary.ContainsKey(item))
        {
            ItemDictionary[item] = amount;
            invenItems.Add(new InventoryItem { item = item, count = amount });
        }
        else
        {
            ItemDictionary[item] += amount;
            var inv = invenItems.Find(i => i.item == item);
            if (inv != null)
                inv.count = ItemDictionary[item];
        }

        OnInventoryChanged?.Invoke();
    }

    // 아이템 사용
    public bool UseItem(ItemBase item, int amount = 1)
    {
        if (item == null || !ItemDictionary.ContainsKey(item) || ItemDictionary[item] < amount)
            return false;

        ItemDictionary[item] -= amount;
        var inv = invenItems.Find(i => i.item == item);
        if (inv != null)
            inv.count = ItemDictionary[item];

        if (ItemDictionary[item] <= 0)
        {
            ItemDictionary.Remove(item);
            invenItems.RemoveAll(i => i.item == item);
        }

        OnInventoryChanged?.Invoke();
        return true;
    }

    // 아이템 제거
    public void RemoveItem(ItemBase item)
    {
        if (item == null) return;

        if (ItemDictionary.ContainsKey(item))
        {
            ItemDictionary.Remove(item);
            invenItems.RemoveAll(i => i.item == item);
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