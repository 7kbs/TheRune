using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// 대사 리팩토링
[Serializable]
public class DialogueRecord
{
    public string dialogueID;
    public bool played;
}
/// 대사 리팩토링


public enum QuestProgress { NoneStart, InProgress, Completed, GetReward };
[Serializable]
public class QuestList
{
    public QuestData questSO;      // 참조 SO
    public QuestProgress progress;
    public bool isCleared = false; // 완료 여부

    public int currentCount;    // 진행 상황 (킬/수집 카운트 등)
}


[CreateAssetMenu(fileName = "Player", menuName = "ScriptableObject/UserData")]
public class UserData : ScriptableObject
{
    public float PlayerMaxHp;  //플레이어 최대 체력
    public float PlayerMaxMp;   //플레이어 최대 마력
    public float PlayerHp;      //현재 체력 (껐다가 켜도 유지되도록..)
    public float PlayerMp;      //현재 마나 (껐다가 켜도 유지되도록..)
    public int GameMoney;

    public int sceneType;       //유저가 껐다가 킬때 저장될 SceneType
    public Vector3 playerSavePos;   //유저가 껐다가 켤때 저장된 플레이어 위치변수

    public List<SkillData> allSkills = new List<SkillData>();   // 게임 내 전체 스킬 목록 (Resources/Skill)
    public List<SkillData> LearnedSkills = new List<SkillData>(); // 유저가 배운 스킬
    public SkillData[] SkillSlots = new SkillData[3];

    [Header("퀵슬롯")]
    public QuickSlotData[] quickSlots;

    /// 대사 리팩토링
    public List<DialogueRecord> dialogueRecord = new List<DialogueRecord>();
    /// 대사 리팩토링


    [Header("퀘스트 관리")]
    public List<QuestList> questProgressList = new List<QuestList>();
    [NonSerialized] public QuestList currentQuest = null;
    [Header("현재 진행중인 퀘스트 (읽기 전용)")]
    public QuestData currentQuestSO;


    public bool OpeningEnd = false;     //오프닝씬 한번만 나오도록 하는 변수
    public bool TutorialClear = false;  //튜토리얼 한번만 나오도록 하는 변수   

    public bool[] GetStonePiece;        //룬스톤을 3개꽂았는지 아닌지 확인하는 변수
    public HashSet<string> uniqueItem = new HashSet<string>();

    public bool BossPortalOpen = false; //보스포탈이 열렸는지 안열렸는지 확인하는 변수
    public bool UnderBossDie;         //준보스가 죽었는지 안죽었는지 체크할 변수
    public bool BossDie = false;        //보스가 죽었는지 안죽었는지 확인하는 변수

    public bool PuzzleClear = false; //테라곤 퍼즐클리어 했는지 안했는지
    public bool VineDestroy = false;  //지하입구 장애물 부쉈는지 안부쉈는지


    //데이터 초기화 함수 >> 타이틀에 버튼하나 만들어서 넣던지..
    public void InitData()
    {
        PlayerMaxHp = 200;
        PlayerMaxMp = 100;
        PlayerHp = 200;
        PlayerMp = 100;
        GameMoney = 5000;

        sceneType = 0;
        playerSavePos = Vector3.zero;

        // 전체 스킬 불러오기
        allSkills = Resources.LoadAll<SkillData>("Skills").ToList();

        // 기본 스킬 지정 (이름 또는 ID로 필터)
        LearnedSkills.Clear();
        SkillData basicSkill = allSkills.FirstOrDefault(s => s.skillID == "BasicAttack");
        if (basicSkill != null)
            LearnedSkills.Add(basicSkill);
        else
            Debug.LogWarning("기본 스킬(Basic)을 찾을 수 없습니다. Skills 폴더 확인 요망.");

        // 스킬 슬롯 초기화
        for (int i = 0; i < SkillSlots.Length; i++) SkillSlots[i] = null;
        for (int i = 0; i < quickSlots.Length; i++) quickSlots[i] = null;

        dialogueRecord.Clear();
        ///ItemData로 분리로 인한 주석처리

        QuestData[] questSOList = Resources.LoadAll<QuestData>("Quest");
        questProgressList.Clear();
        foreach (var qSO in questSOList)
        {
            questProgressList.Add(new QuestList
            {
                questSO = qSO,
                progress = QuestProgress.NoneStart,
                isCleared = false
            });
        }

        QuestMgr.inst.RefreshCurrentQuest();

        OpeningEnd = false;
        TutorialClear = false;

        uniqueItem.Clear();
        for (int i = 0; i < GetStonePiece.Length; i++) GetStonePiece[i] = false;
        
        BossPortalOpen = false;
        UnderBossDie = false;
        BossDie = false;
        PuzzleClear = false;
        VineDestroy = false;
    }


    ///대사 리팩토링
    public bool IsDialoguePlayed(string id)
    {
        var ds = dialogueRecord.Find(x => x.dialogueID == id);
        return ds != null && ds.played;
    }

    public void SetDialoguePlayed(string id)
    {
        var ds = dialogueRecord.Find(x => x.dialogueID == id);
        if (ds == null)
        {
            ds = new DialogueRecord { dialogueID = id, played = true };
            dialogueRecord.Add(ds);
        }
        else
        {
            ds.played = true;
        }
        DataMgr.inst.SaveData(); // 저장
    }
    ///대사 리팩토링
}