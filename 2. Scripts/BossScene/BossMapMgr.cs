using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossMapMgr : MonoBehaviour
{
    MoroKhan boss;
    [HideInInspector] public Player player;
    public bool CutSceneOver;

    public GameObject BossStartAnimationGroup;
    public Camera BossRoomCamera;

    //보스 전투시 켜져야하는 목록
    public GameObject BossCanvas;
    //public GameObject Canvas;
    //보스 전투시 켜져야하는 목록

    //전투 종료시 켜져할 오브젝트
    public GameObject RuneStone;
    public GameObject WarpGate;
    public GameObject WarpPoint;

    public static BossMapMgr Inst;

    void Awake()
    {
        Inst = this;
    }

    void Start()
    {
        player = FindAnyObjectByType<Player>();

        if (GlobalValue.BossCutSceneOver)
        {
            BossRoomCamera.gameObject.SetActive(true);
            BossStartAnimationGroup.SetActive(false);
            PlayerMove.inst.ChangeState(new DefaultState());
            CutSceneOver = true;

            BossCanvas.SetActive(true);
            //Canvas.SetActive(true);
            player.transform.position = new Vector3(-15f, -1.3f, 0.0f);
        }
        else
            player.transform.position = new Vector3(-104.0f, -1.3f, 0.0f);

        boss = FindAnyObjectByType<MoroKhan>();
        GlobalValue.sceneType = SceneType.Boss;
    }

    void Update()
    {
        BossSceneWalk();
    }

    void StartAnimStop()
    {
        if (boss.isDie)
            return;

        BossRoomCamera.gameObject.SetActive(true);
        BossStartAnimationGroup.SetActive(false);
        CutSceneOver = true;

        PlayerMove.inst.ChangeState(new DefaultState());
        BossCanvas.SetActive(true);
        //Canvas.SetActive(true);
    }

    public void BossSceneWalk()
    {
        if (CutSceneOver || GlobalValue.BossCutSceneOver)
            return;

        if (player.transform.position.x >= -11f)
        {
            BossRoomCamera.gameObject.SetActive(false);
            BossStartAnimationGroup.SetActive(true);
            BossCanvas.SetActive(false);
            //Canvas.SetActive(false);

            Invoke("StartAnimStop", 25f);
            PlayerMove.inst.ChangeState(new InteractingState());
            GlobalValue.BossCutSceneOver = true;
        }
    }

    public void ClearObjSet()
    {
        GameMgr.inst.InfoPanelOn("포탈이 열렸습니다!", 3.0f);
        RuneStone.SetActive(true);
        WarpGate.transform.SetParent(WarpPoint.transform);
        WarpGate.transform.localPosition = Vector3.zero;
    }
}