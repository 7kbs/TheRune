using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameSceneMgr : MonoBehaviour
{
    public GameObject[] Traps;      //준보스 전투씬 들어갈 트리거 오브젝트
    public GameObject[] RunePiece;
    public GameObject VineObj;  //도토리 폭탄으로만 뚫을 수 있는 기믹

    public Button NextBtn;

    public GameObject PuzzlePanel;
    public GameObject Puzzle_1_BoxObject;
    public GameObject Puzzle_2_BoxObject;
    public Button NextStageBtn;
    public Button ClearBtn;
    public GameObject[] ClearPanel;

    PuzzleBoxCtrl[] boxCtrl_1;
    PuzzleBoxCtrl[] boxCtrl_2;

    public GameObject Terragon;
    public GameObject bossportal;
    public Transform bossportalpos;

    int DialogueIndex;
    int puzzleCount = 0;
    public static GameSceneMgr inst;

    private void Awake()
    {
        inst = this;
    }

    void Start()
    {
        DialogueIndex = 0;
        GlobalValue.sceneType = SceneType.Game;

        Terragon.SetActive(!GameMgr.inst.userData.TerragonPuzzleClear);

        boxCtrl_1 = Puzzle_1_BoxObject.GetComponentsInChildren<PuzzleBoxCtrl>(true);
        boxCtrl_2 = Puzzle_2_BoxObject.GetComponentsInChildren<PuzzleBoxCtrl>(true);

        //준보스가 죽었는지 안죽었는지 체크해서 Trap설치한 부분 액티브 꺼놓기
        if (GameMgr.inst.userData.UnderBossDie) Traps[0].SetActive(false);

        if (NextBtn != null)
            NextBtn.onClick.AddListener(() =>
            {
                DialogueMgr.inst.OnNextClicked();
            });


        if (ClearBtn != null)  ClearBtn.onClick.AddListener(CheckPuzzle);

        if (GameMgr.inst.userData.VineDestroy)  VineObj.SetActive(false);
        else  VineObj.SetActive(true);

        for (int i = 0; i < RunePiece.Length; i++)
        {
            if (GameMgr.inst.userData.uniqueItem.Contains(RunePiece[i].GetComponent<FieldItem>().uniqueId))
                Destroy(RunePiece[i].gameObject);
        }

        if (GameMgr.inst.userData.BossPortalOpen) SpawnBossPortal();
    }


    void CheckPuzzle()
    {
        if (Puzzle_1_BoxObject.activeSelf == true)
        {
            for (int i = 0; i < boxCtrl_1.Length; i++)
            {
                if (boxCtrl_1[i].isOn == true)
                {                  
                    puzzleCount++;
                    if (puzzleCount == 16)
                    {
                        //퍼즐클리어                        
                        ClearPanel[0].SetActive(true);
                        puzzleCount = 0;
                        NextStageBtn.gameObject.SetActive(true);
                        Puzzle_1_BoxObject.gameObject.SetActive(false);
                        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.usePotion);
                    }
                }
                else
                {
                    puzzleCount = 0;
                }
            }
        }

        if (Puzzle_2_BoxObject.activeSelf == true)
        {
            for (int i = 0; i < boxCtrl_2.Length; i++)
            {
                if (boxCtrl_2[i].isOn == true)
                {
                    puzzleCount++;
                    if (puzzleCount == 24)
                    {
                        //퍼즐클리어
                        ClearPanel[1].SetActive(true);
                        RunePiece[2].SetActive(true);

                        PuzzlePanel.SetActive(false);
                        DialogueMgr.inst.dialogueBox.SetActive(true);
                        
                        GameMgr.inst.userData.TerragonPuzzleClear = true;
                        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.usePotion);
                    }
                }
                else
                {
                    puzzleCount = 0;
                }
            }
        }
    }

    public void SpawnBossPortal()
    {
        var bp = Instantiate(bossportal, bossportalpos);
    }
}
