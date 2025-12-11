using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ItemManager : MonoBehaviour
{
    public UserData userData;
    public ItemDB itemData;

    [SerializeField] private List<ItemBase> itemAssets; // Inspector에 드래그 (SO들)

    public static ItemManager inst { get; private set; }

    void Awake()
    {
        inst = this;

        //// 인벤토리 초기화 (등록된 아이템 SO 기준)
        //itemData.InitData(itemAssets);
    }

    public void GetItem(ItemBase item)
    {
        if (item == null) return;

        // 수량 증가 (ItemData에서 카운트 관리)
        itemData.AddItem(item, item.amount);

        // 아이템 효과 실행
        if (!item.Consumable)
        {
            var iitem = item.reward.GetComponent<IItem>();
            iitem.OnExcute(userData, item, itemData);
        }
        else GameMgr.inst.UpdateQuickSlotsCount(item);

        // 저장
        //DataMgr.inst.SaveData();
    }


    // 아이템 SO 전체 반환
    public IEnumerable<ItemBase> GetAllItems()
    {
        return itemAssets;
    }
}