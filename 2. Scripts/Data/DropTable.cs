using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "DropTable")]
public class DropTable : ScriptableObject
{
    [System.Serializable]
    public class DropEntry
    {
        [Tooltip("드랍할 아이템 SO")]
        public ItemBase item; // IItemStrategy 대신 ItemBase

        [Range(0f, 1f)]
        public float dropRate;
    }

    public List<DropEntry> entries = new List<DropEntry>();

    // 랜덤 드랍
    public ItemBase GetDrop()
    {
        float roll = Random.value;
        float cumulative = 0f;

        foreach (var entry in entries)
        {
            cumulative += entry.dropRate;
            if (roll <= cumulative)
                return entry.item; // 캐스팅 불필요
        }

        return null;
    }
}