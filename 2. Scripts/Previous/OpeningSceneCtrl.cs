using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class OpeningSceneCtrl : MonoBehaviour
{
    public UserData userData;
    private void Start()
    {
        GlobalValue.sceneType = GlobalValue.SceneType.Opening;
        SoundMgr.inst.BGM_Play(true);
    }

    public void Event_SceneMove()
    {
        GlobalValue.sceneType = GlobalValue.SceneType.Lobby;
        userData.OpeningEnd = true;
        
        SceneManager.LoadScene("LobbyScene");
        SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);
    }
}
