using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialMgr : MonoBehaviour
{
    public Player player;
    public Text tutorialText;
    public Image tutorialPanel;
    public Button skipBtn;

    private Queue<string> tutorialSteps;
    private bool isTutorialActive = true;
    private bool isBtnClicked = false;


    void Start()
    {
        if (GameMgr.inst.userData.TutorialClear == false)
        {
            tutorialSteps = new Queue<string>();
            tutorialPanel.gameObject.SetActive(true);
            InitializeTutorialSteps();
            StartCoroutine(ShowTutorial());

            if (skipBtn != null)
                skipBtn.onClick.AddListener(() =>
                {
                    SkipTutorial();
                });
        }
    }

    void InitializeTutorialSteps()
    {
        tutorialSteps.Enqueue("왼쪽 화살표(←)와 오른쪽 화살표(→)를 사용하여 좌우로 이동할 수 있습니다.");
        tutorialSteps.Enqueue("스페이스바를 눌러 점프할 수 있습니다. 공중에서 한 번 더 누르면 이중 점프가 가능합니다!");
        tutorialSteps.Enqueue("이동키와 왼쪽 Shift 키를 눌러 빠르게 대시할 수 있습니다.");
        tutorialSteps.Enqueue("Esc를 눌러 하단의 스킬창에 보유한 스킬을 드래그해서 등록 할 수 있습니다.");
        tutorialSteps.Enqueue("좌측 상단의 버튼을 눌러 스킬창을 종료 할 수 있습니다.");
        tutorialSteps.Enqueue("Z, X, C 키를 사용하여 다양한 공격을 할 수 있습니다.\n 아래 방향키와 함께 누르면 하단 공격이 가능합니다.");
        tutorialSteps.Enqueue("간단한 조작법은 끝입니다. 여정을 시작하세요!");
    }

    IEnumerator ShowTutorial()
    {
        while (tutorialSteps.Count > 0 && isTutorialActive)
        {
            string currentStep = tutorialSteps.Dequeue();
            tutorialText.text = currentStep;
            //tutorialPanel.gameObject.SetActive(true);
            tutorialText.gameObject.SetActive(true);

            yield return StartCoroutine(WaitForPlayerAction(currentStep));

            //tutorialPanel.gameObject.SetActive(false);
            tutorialText.gameObject.SetActive(false);
            yield return new WaitForSeconds(1f);
        }

        EndTutorial();
    }

    IEnumerator WaitForPlayerAction(string step)
    {
        bool actionPerformed = false;

        while (!actionPerformed)
        {
            if (step.Contains("화살표") && (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.RightArrow)))
            {
                actionPerformed = true;
            }
            else if (step.Contains("스페이스바") && Input.GetKeyDown(KeyCode.Space))
            {
                actionPerformed = true;
            }
            else if (step.Contains("Shift") && Input.GetKeyDown(KeyCode.LeftShift))
            {
                actionPerformed = true;
            }
            else if (step.Contains("Esc") && Input.GetKeyDown(KeyCode.Escape))
            {
                actionPerformed = true;
            }
            else if (step.Contains("좌측") && IsBtnClick())
            {
                actionPerformed = true;
            }
            else if (step.Contains("Z, X, C") && (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.X) || Input.GetKeyDown(KeyCode.C)))
            {
                actionPerformed = true;
            }
            else if (step.Contains("간단한"))
            {
                actionPerformed = true;
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);
    }

    void EndTutorial()
    {
        isTutorialActive = false;
        tutorialPanel.gameObject.SetActive(false);
        //튜토리얼 종료후 상태 추가가능
        GameMgr.inst.userData.TutorialClear = true;
    }

    // 필요한 경우 튜토리얼을 강제로 종료하는 메서드
    public void SkipTutorial()
    {
        StopAllCoroutines();
        EndTutorial();
    }

    public void OnBtnClick()
    {
        SetBtnClick(true);
    }

    public void SetBtnClick(bool clicked)
    {
        isBtnClicked = clicked;
    }

    public bool IsBtnClick()
    {
        return isBtnClicked;
    }

}
