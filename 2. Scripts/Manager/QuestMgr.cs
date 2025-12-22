using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class QuestMgr : MonoBehaviour
{
    public UserData userData;
    [SerializeField] string resourcesPath = "Quest SO"; // Resources 경로
    public static QuestMgr inst;

    void Awake()
    {
        inst = this;
    }

    // 현재 퀘스트 갱신
    public void RefreshCurrentQuest()
    {
        if (GameMgr.inst == null || GameMgr.inst.userData == null) return;
        if (GameMgr.inst.userData.questProgressList == null || GameMgr.inst.userData.questProgressList.Count == 0) return;

        userData.currentQuestSO = null;
        foreach (var q in GameMgr.inst.userData.questProgressList)
        {
            if (q.progress == QuestProgress.NoneStart)
            {
                q.progress = QuestProgress.InProgress;
                userData.currentQuestSO = q.questSO;
                DataMgr.inst.SaveData();
                break;
            }
            else if (q.progress == QuestProgress.InProgress)
            {
                userData.currentQuestSO = q.questSO;
                break;
            }
        }
    }

    /// 퀘스트 리스트가 없으면 Resources에서 SO를 읽어 채운다
    public void EnsureQuestList()
    {
        if (userData == null) return;

        if (userData.questProgressList == null)
            userData.questProgressList = new List<QuestList>();

        if (userData.questProgressList.Count > 0) return;

        // Resources/5.Data/Quest 내부의 모든 QuestData로드
        QuestData[] questSOList = Resources.LoadAll<QuestData>(resourcesPath);
        foreach (var so in questSOList)
        {
            if (so == null) continue;
            if (userData.questProgressList.Any(e => e.questSO == so)) continue;

            userData.questProgressList.Add(new QuestList
            {
                questSO = so,
                progress = QuestProgress.NoneStart,
                isCleared = false
            });
        }

        DataMgr.inst?.SaveData();
    }

    /// 현재 퀘스트 가져오기 (필요 시 첫 퀘스트를 InProgress로 변경)
    public QuestData CurrentQuest()
    {
        // userData 보정
        if (userData == null)
            userData = DataMgr.inst != null ? DataMgr.inst.userData : GameMgr.inst?.userData;

        if (userData == null)
        {
            Debug.LogError("[QuestMgr] userData is null. CurrentQuest aborted.");
            return null;
        }

        // 퀘스트 리스트 초기화
        EnsureQuestList();
        if (userData.questProgressList == null || userData.questProgressList.Count == 0)
        {
            Debug.LogWarning("[QuestMgr] questProgressList is empty.");
            return null;
        }

        // 보상 미수령 Completed 퀘스트 우선 처리
        var pendingRewardQuest = userData.questProgressList
            .FirstOrDefault(x => x.progress == QuestProgress.Completed);

        if (pendingRewardQuest != null)
        {
            userData.currentQuestSO = pendingRewardQuest.questSO;
            return userData.currentQuestSO;
        }

        // 진행 중 또는 시작 전 퀘스트 검색
        var nextQuest = userData.questProgressList
            .FirstOrDefault(x => x.progress == QuestProgress.InProgress || x.progress == QuestProgress.NoneStart);

        if (nextQuest == null)
            return null;

        // NoneStart > InProgress 승격
        if (nextQuest.progress == QuestProgress.NoneStart)
            nextQuest.progress = QuestProgress.InProgress;

        userData.currentQuestSO = nextQuest.questSO;
        return userData.currentQuestSO;
    }

    // 특정 ID로 퀘스트 찾기
    public QuestList SearchQuest(string questID)
    {
        return userData.questProgressList
            .FirstOrDefault(q => q.questSO != null && q.questSO.questID == questID);
    }

    // 퀘스트 완료
    public void CompleteQuest(string questID)
    {
        var q = SearchQuest(questID);
        if (q != null && q.progress == QuestProgress.InProgress)
        {
            q.progress = QuestProgress.Completed;
            DataMgr.inst.SaveData();
        }
    }

    // 보상 수령
    public void ReceiveReward(QuestData data)
    {
        var q = SearchQuest(data.questID);
        if (q != null && q.progress == QuestProgress.Completed)
        {
            userData.GameMoney += q.questSO.reward;
            q.progress = QuestProgress.GetReward;
            q.isCleared = true;

            RefreshCurrentQuest();
            DataMgr.inst.SaveData();
        }
    }

    //퀘스트 완료 검사
    public void TryCompleteQuest(QuestData data)
    {
        var q = SearchQuest(data.questID);
        if (q == null || q.progress != QuestProgress.InProgress) return;

        var questData = q.questSO;
        if (questData == null) return;

        q.currentCount++;
        if (q.currentCount >= questData.targetCount)
        {
            q.progress = QuestProgress.Completed;
            Debug.Log($"Quest {data.questID} completed!");
            DataMgr.inst.SaveData();
        }
    }


    // 상태 체크 헬퍼
    public bool IsQuestInProgress(string questID)
    {
        var q = SearchQuest(questID);
        return q != null && q.progress == QuestProgress.InProgress;
    }

    public bool IsQuestRewarded(string questID)
    {
        var q = SearchQuest(questID);
        return q != null && q.progress == QuestProgress.GetReward;
    }

    public bool IsQuestCompleted(string questID)
    {
        var q = SearchQuest(questID);
        return q != null && q.progress == QuestProgress.Completed;
    }


    public void NextQuestSequence(string questID)
    {
        var q = SearchQuest(questID);
        q.progress = QuestProgress.GetReward;
        q.currentCount++;
        RefreshCurrentQuest();
    }
}