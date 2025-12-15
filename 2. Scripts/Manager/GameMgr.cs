using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public UserData userData;       //유저가 저장한 정보를 얻어오는 데이터
    public ItemDB itemData;

    [HideInInspector] public Player player;   
    public GameObject LoadingPanel; //페이드인 판넬

    public GameObject SylvaronStoenObj;

    //[Header("--- MiniMap UI ---")]
    //public GameObject MiniMapPanel; //미니맵 판넬
    //public Transform Player;        //미니맵에 들어갈 플레이어 아이콘을 위한 플레이어 위치변수
    //public RectTransform MiniMapPlayerIcon; //미니맵 플레이어 아이콘의 위치변수
    //public Button minimapPortal;
    //private bool MiniMapOnOff = false;      //미니맵이 켜졌는지 아닌지 판별할 변수
    //public GameObject GameMapNPC_Char;

    //public Vector2 minimapOffset = Vector2.zero;  //플레이어아이콘의 위치추적변수

    //[Header("--- LobbyMap UI ---")]
    //public GameObject LobbyMapPanel;
    //public Transform LobbyPlayer;
    //public RectTransform LobbyMapPlayerIcon;
    //private bool LobbyMapOnOff = false;
    //public GameObject BossPortalIcon;
    //public GameObject NPC_Char;

    //public Vector2 LobbyMapOffset = Vector2.zero;
    //public Button lobbymapPortal;

    [Header("------ Damage Text ------")]
    public GameObject DamageTextRoot = null;
    public Transform Damage_Canvas = null;
    //--- 캐릭터 메리위에 데미지 띄우기용 변수 선언

    //퀘스트 보상 수치 관리할 변수 추가 예정..
    public bool RunePuzzleClear;

    [Header("퀵슬롯")]
    public QuickSlot[] quickSlotUIs;  // 캔버스에 배치한 퀵슬롯 UI들

    public static GameMgr inst; 

    private void Awake()
    {
        inst = this;
    }


    void Start()
    {
        LoadingPanel.SetActive(true);
        Invoke(nameof(LoadingPanelOff), 3.0f);
        PlayerMove.inst.ChangeState(new DefaultState());

        SoundMgr.inst.BGM_Play(true);
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Warp);

        player = GameObject.Find("Player").GetComponent<Player>();

        //로비씬 지도 On
        //if (GlobalValue.sceneType == SceneType.Lobby)
        //{
        //    if (LobbyMapPanel != null)
        //    {
        //        LobbyMapPanel.SetActive(false);
        //        UpdateLobbyPlayerPos();
        //    }

        //    if (lobbymapPortal != null)
        //        lobbymapPortal.onClick.AddListener(() =>
        //        {
        //            LobbyPlayer.transform.position = new Vector2(-10.0f, 1.0f);
        //        });
        //}

        //게임씬 지도 On
        //if (GlobalValue.sceneType == SceneType.Game)
        //{
        //    if (MiniMapPanel != null)
        //    {
        //        MiniMapPanel.SetActive(false);
        //        UpdatePlayerPos();
        //    }

        //    if (minimapPortal != null)
        //        minimapPortal.onClick.AddListener(() =>
        //    {
        //        Player.transform.position = new Vector2(0.0f, 0.2f);
        //    });
        //}

        InitQuickSlots();
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab)) HandleTabPress();

        //미니맵 온오프 변수가 true일때 플레이어 위치 추적
        //if (MiniMapOnOff) UpdatePlayerPos();
        //if (LobbyMapOnOff) UpdateLobbyPlayerPos();
    }


    void HandleTabPress()
    {
        switch (GlobalValue.sceneType)
        {
            case SceneType.Game:
                UIManager.inst.OpenUI("MiniMapPanel");
                break;
            case SceneType.Lobby:
                UIManager.inst.OpenUI("LobbyMapPanel");
                break;
        }
    }

    //void ToggleGameMap()
    //{
    //    if (PlayerMove.inst.IsInteractionState) return;

    //    // 상태 전환
    //    if (PlayerMove.inst.IsDefaultState) PlayerMove.inst.ChangeState(new InteractingState());
    //    else if (PlayerMove.inst.IsInteractionState) PlayerMove.inst.ChangeState(new DefaultState());

    //    MiniMapOnOff = !MiniMapOnOff;
    //    MiniMapPanel.SetActive(MiniMapOnOff);

    //    BossPortalIcon.SetActive(!userData.BossDie);
    //    GameMapNPC_Char.SetActive(!userData.PuzzleClear);
    //}

    //void ToggleLobbyMap()
    //{
    //    if (PlayerMove.inst.IsInteractionState) return;

    //    // 상태 전환
    //    if (PlayerMove.inst.IsDefaultState) PlayerMove.inst.ChangeState(new InteractingState());
    //    else if (PlayerMove.inst.IsInteractionState) PlayerMove.inst.ChangeState(new DefaultState());

    //    LobbyMapOnOff = !LobbyMapOnOff;
    //    LobbyMapPanel.SetActive(LobbyMapOnOff);

    //    NPC_Char.SetActive(userData.PuzzleClear);
    //}


    //void UpdatePlayerPos() //플레이어 이동관련 함수
    //{
    //    Vector3 playerPos = Player.transform.position;

    //    Vector2 minimapPos = new Vector2(playerPos.x * 4.0f, playerPos.y * 3.2f) + minimapOffset;

    //    MiniMapPlayerIcon.anchoredPosition = minimapPos;
    //}

    //void UpdateLobbyPlayerPos() //플레이어 이동관련 함수
    //{
    //    Vector3 playerPos = LobbyPlayer.transform.position;

    //    Vector2 minimapPos = new Vector2(playerPos.x * 11.0f, playerPos.y * 5.75f) + LobbyMapOffset;

    //    LobbyMapPlayerIcon.anchoredPosition = minimapPos;
    //}


    public void DamageTextSpawn(float dmg, Vector3 pos, Color color)
    {
        GameObject dmgClone = Instantiate(DamageTextRoot, Damage_Canvas);
        DamageTextControl DamageText = dmgClone.GetComponent<DamageTextControl>();
        DamageText.InitDamage(dmg, color);
        Vector3 StartPos = new Vector3(pos.x, pos.y + 2.25f, 0.0f);
        dmgClone.transform.position = StartPos;
    }


    public void LoadingPanelOff()
    {
        LoadingPanel.SetActive(false);
    }


    ///퀵슬롯 
    public void UpdateQuickSlotsCount(ItemBase potion)
    {
        int count = itemData.ItemDictionary.ContainsKey(potion) ? itemData.ItemDictionary[potion] : 0;

        for (int i = 0; i < userData.quickSlots.Length; i++)
        {
            if (userData.quickSlots[i].potion == potion)
                quickSlotUIs[i].UpdateCount(count);
        }
    }


    public void InitQuickSlots()
    {
        for (int i = 0; i < quickSlotUIs.Length; i++)
        {
            var slotData = userData.quickSlots[i];

            if (slotData.potion != null)
            {
                int count = itemData.ItemDictionary.ContainsKey(slotData.potion) ? itemData.ItemDictionary[slotData.potion] : 0;
                quickSlotUIs[i].Assign(slotData.potion, count);
            }
            else
            {
                quickSlotUIs[i].Clear();
            }
        }
    }
    ///퀵슬롯 
}