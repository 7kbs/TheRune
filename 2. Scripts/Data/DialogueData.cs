using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public string speakerName;
    [TextArea] public string text;
    public bool showSageIcon;
    public bool enableNext;
    public bool enableExit;
    public bool enableShop;
    public bool puzzleOpen;
}

[CreateAssetMenu(fileName = "NewDialogue", menuName = "Dialogue/DialogueData")]
public class DialogueData : ScriptableObject
{
    public string forUse;   //용도 메모
    public DialogueLine[] lines;
}