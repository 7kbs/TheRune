using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class DialogueMgr : MonoBehaviour
{
    public GameObject dialogueBox;
    [SerializeField] Text nameText;
    [SerializeField] Text dialogueText;
    [SerializeField] GameObject sageIcon;
    [SerializeField] Button nextBtn;

    public Button exitBtn;
    public Button shopBtn;
    public GameObject shopFrame;

    DialogueData currentDialogue;

    int dialogueIndex;

    public event Action OnPuzzleTriggered;    // 퍼즐 오픈 이벤트 (옵저버 패턴)
    public event Action OnDialogueEnded; // 대화 끝났을 때 발생하는 이벤트 (옵저버 패턴)

    public static DialogueMgr inst;

    void Awake() => inst = this;

    public void StartDialogue(DialogueData data)
    {
        dialogueBox.SetActive(true);
        currentDialogue = data;
        dialogueIndex = 0;
        ShowLine();

        PlayerMove.inst.ChangeState(new InteractingState());
    }

    public void OnNextClicked()
    {
        dialogueIndex++;
        if (dialogueIndex < currentDialogue.lines.Length) ShowLine();
        else EndDialogue();
    }

    private void ShowLine()
    {
        DialogueLine line = currentDialogue.lines[dialogueIndex];
        nameText.text = $"[{line.speakerName}]";
        dialogueText.text = line.text;

        sageIcon.SetActive(line.showSageIcon);
        nextBtn.gameObject.SetActive(line.enableNext);
        exitBtn.gameObject.SetActive(line.enableExit);

        if (line.puzzleOpen) OnPuzzleTriggered?.Invoke();
    }

    public void EndDialogue()
    {
        Debug.Log("끝났음");
        currentDialogue = null;
        dialogueBox.SetActive(false);

        OnDialogueEnded?.Invoke();    // 이벤트 호출
        PlayerMove.inst.ChangeState(new DefaultState());
    }
}