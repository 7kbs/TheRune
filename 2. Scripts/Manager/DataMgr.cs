using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DataMgr : MonoBehaviour
{
    public UserData userData;
    public ItemData itemData;

    public static DataMgr inst;

    private void Awake()
    {
        if (inst == null) inst = this;
        else Destroy(gameObject);
    }

    public void SaveData()
    {
        string user_json = JsonUtility.ToJson(userData, true);
        string item_json = JsonUtility.ToJson(itemData, true);

        string user_Path = Path.Combine(Application.persistentDataPath, "userData.json");
        string item_Path = Path.Combine(Application.persistentDataPath, "itemData.json");

        File.WriteAllText(user_Path, user_json);
        File.WriteAllText(item_Path, item_json);
    }

    public void LoadData()
    {
        string user_Path = Path.Combine(Application.persistentDataPath, "userData.json");
        string item_Path = Path.Combine(Application.persistentDataPath, "itemData.json");

        if (File.Exists(user_Path) && File.Exists(item_Path))
        {
            string user_json = File.ReadAllText(user_Path);
            string item_json = File.ReadAllText(item_Path);

            JsonUtility.FromJsonOverwrite(user_json, userData);
            JsonUtility.FromJsonOverwrite(item_json, itemData);

            // List > Dictionary 복원 (런타임 사용용)
            itemData.SyncDictFromList();

            QuestMgr.inst.RefreshCurrentQuest();
        }
        else
        {
            // 최초 실행 시: Dictionary 초기화
            userData.InitData();

            // ItemManager에서 관리하는 모든 전략 리스트 전달
            var allItems = ItemManager.inst != null
            ? new List<ItemBase>(ItemManager.inst.GetAllItems())
            : new List<ItemBase>();

            itemData.InitData();
        }
    }
}