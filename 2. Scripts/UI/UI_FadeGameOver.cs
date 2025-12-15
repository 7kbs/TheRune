using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_FadeGameOver : UI_Base
{
    [Header("------ GameOverPanel ------")]
    public GameObject MinusObj;
    public Text CostText;
    public Text InfoText;
    public Button ReviveBtn;
    public GameObject GoLobbyPanel;

    void Start()
    {
        Init();
    }


    void Init()
    {
        int payment = (int)(GameMgr.inst.userData.GameMoney * 0.3);
        CostText.text = $"{payment}";

        if (GlobalValue.sceneType == SceneType.Battle
            || GlobalValue.sceneType == SceneType.Boss)
        {
            InfoText.text = "다시 도전하시겠습니까?";
            MinusObj.SetActive(false);
        }
        else
        {
            InfoText.text = "소지금의 30%를 지불하고\n\n부활합니다.";
            MinusObj.SetActive(true);
        }
    }

    public void PlayerRevive()
    {
        PlayerMove.inst.ChangeState(new DefaultState());

        int payment = (int)(GameMgr.inst.userData.GameMoney * 0.3);

        if (GlobalValue.sceneType == SceneType.Battle)
        {
            SceneManager.LoadScene("BattleScene");
            SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);
        }
        else if (GlobalValue.sceneType == SceneType.Boss) SceneManager.LoadScene("BossScene");
        else GameMgr.inst.userData.GameMoney -= payment;

        GameMgr.inst.userData.PlayerHp = GameMgr.inst.userData.PlayerMaxHp;
        GameMgr.inst.userData.PlayerMp = GameMgr.inst.userData.PlayerMaxMp;

        Close();
    }

    public void GoLobby()
    {
        SceneManager.LoadScene("LobbyScene");
        SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);

        GameMgr.inst.userData.playerSavePos.x = 0;
        GameMgr.inst.userData.playerSavePos.y = 0;

        Close();
    }
}
