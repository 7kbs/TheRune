using UnityEngine;

public class UI_Minimap : UI_Base
{
    [Header("Common")]
    [SerializeField] GameObject panel;
    [SerializeField] RectTransform playerIcon;
    [SerializeField] Vector2 offset;

    [Header("Scene Objects")]
    [SerializeField] Transform targetPlayer;
    [SerializeField] GameObject bossIcon;
    [SerializeField] GameObject npcIcon;

    bool isOpen;

    void Awake()
    {
        targetPlayer = FindAnyObjectByType<Player>().transform;
        UpdatePlayerPos();
        NpcIconVisable();
    }

    public void Toggle()
    {
        if (PlayerMove.inst.IsInteractionState) return;

        isOpen = !isOpen;
        panel.SetActive(isOpen);

        HandlePlayerState();
        UpdateIcons();
    }

    void HandlePlayerState()
    {
        if (isOpen)
            PlayerMove.inst.ChangeState(new InteractingState());
        else
            PlayerMove.inst.ChangeState(new DefaultState());
    }

    void UpdateIcons()
    {
        bossIcon?.SetActive(!GameMgr.inst.userData.BossDie);
        npcIcon?.SetActive(!GameMgr.inst.userData.PuzzleClear);
    }

    void UpdatePlayerPos()
    {
        //2가지 미니맵 이미지의 가로·세로 비율이 다르기 때문에 스케일보정

        float worldToMapX = GlobalValue.sceneType == SceneType.Game ? 4f : 11f;
        float worldToMapY = GlobalValue.sceneType == SceneType.Game ? 3.2f : 5.75f;

        Vector3 pos = targetPlayer.position;
        playerIcon.anchoredPosition =
            new Vector2(pos.x * worldToMapX, pos.y * worldToMapY) + offset;
    }

    void NpcIconVisable()
    {
        //if (GlobalValue.sceneType == SceneType.Game)
        //{
        //    npcIcon.SetActive(!GameMgr.inst.RunePuzzleClear);
        //}
        //else
        //{
        //    npcIcon.SetActive(GameMgr.inst.RunePuzzleClear);
        //}

        bool isGameScene = GlobalValue.sceneType == SceneType.Game;
        bool isClear = GameMgr.inst.RunePuzzleClear;

        npcIcon.SetActive(isGameScene ? !isClear : isClear);
    }
}