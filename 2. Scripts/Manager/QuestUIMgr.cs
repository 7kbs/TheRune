using UnityEngine;
using UnityEngine.UI;

public class QuestUIMgr : MonoBehaviour
{
    [Header("UI Elements")]
    public Text mainQuestTitleText;
    public Text questTitleText;
    public Text detailTitleText;
    public Text detailText;
    public Text rewardText;
    public Text goldText;

    public GameObject outClearImage;
    public GameObject outProgressImage;
    public GameObject clearImage;
    public GameObject progressImage;
    public GameObject questClearBtn;
    public GameObject questClearObjects;

    public GameObject[] clearQuest; // 완료된 퀘스트 목록 표시용

    private UserData userData => GameMgr.inst.userData;

    public static QuestUIMgr inst;

    void Awake()
    {
        inst = this;
    }

    private void OnEnable()
    {
        RefreshUI();
    }

    public void RefreshUI()
    {
        var currentQuest = QuestMgr.inst.CurrentQuest();
        if (currentQuest == null) return;

        var q = QuestMgr.inst.SearchQuest(currentQuest.questID);

        bool showClear = q.progress == QuestProgress.Completed;
        bool showRewarded = q.progress == QuestProgress.GetReward;
        bool showInProgress = q.progress == QuestProgress.InProgress;

        // 상태별 UI 표시
        outClearImage.SetActive(showClear);
        clearImage.SetActive(showClear);

        outProgressImage.SetActive(showInProgress || showRewarded);
        progressImage.SetActive(showInProgress || showRewarded);

        questClearBtn.SetActive(showClear); // 보상 버튼은 Completed 상태에서만 보임

        // 텍스트 갱신
        mainQuestTitleText.text = currentQuest.questTitle;
        questTitleText.text = currentQuest.questTitle;
        detailTitleText.text = currentQuest.questTitle;
        detailText.text = currentQuest.description;

        rewardText.text = $"{currentQuest.reward}";
        goldText.text = $"{userData.GameMoney}";

        // 클리어된 퀘스트들 UI 표시
        for (int i = 0; i < clearQuest.Length; i++)
        {
            clearQuest[i].SetActive(i < userData.questProgressList.FindAll(x => x.progress == QuestProgress.GetReward).Count);
        }

        questClearObjects.SetActive(true);
    }

    public void OnClickOwnQuest(GameObject obj)
    {
        Text txt = obj.GetComponentInChildren<Text>();
        string title = txt.text;

        foreach (var q in userData.questProgressList)
        {
            if (q.questSO.questTitle == title)
            {
                detailTitleText.text = q.questSO.questTitle;
                detailText.text = q.questSO.description;
                questClearObjects.SetActive(false);
            }
        }
    }

    public void OnClickReceiveReward()
    {
        var cq = QuestMgr.inst.CurrentQuest();
        if (cq == null) return;

        QuestMgr.inst.ReceiveReward(cq);
        RefreshUI();
    }
}
