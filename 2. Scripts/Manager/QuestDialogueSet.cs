using UnityEngine;

[System.Serializable]
public class QuestDialogueSet
{
    public QuestData quest;
    public DialogueData dialogue;

    //나중에 "이 대사는 퀘스트 완료 후만 나오게 한다" 같은 조건이 필요하면 여기에 필드 추가하면 끝.
    //ex) public enum DialogueTriggerType { OnProgress, OnComplete, OnRewarded }
    //    public DialogueTriggerType triggerType;
}
