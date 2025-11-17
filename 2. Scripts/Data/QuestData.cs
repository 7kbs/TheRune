using UnityEngine;

public enum QuestType
{
    Talk,
    Kill,
    Destination,
    Collect,
    PuzzleClear
}

[CreateAssetMenu(fileName = "QuestData", menuName = "ScriptableObject/QuestData")]
public class QuestData : ScriptableObject
{
    public string questID;       // 고유 ID
    public string questTitle;
    [TextArea] public string description;
    public int reward;           // 보상

    [Space(5f)]
    [Header ("완료 조건")]
    public QuestType questType;  // 어떤 종류인지 (대화, 처치, 수집)
    public int targetCount = 1;  // 필요한 개수 (대화는 1, 처치/수집은 여러 개 가능)
}
