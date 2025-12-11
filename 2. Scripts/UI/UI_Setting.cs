using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_Setting : UI_Base
{
    public Button SoundBtn;
    public Button KeyGuideBtn;
    public Button ExitBtn;

    [Header("--- Sound UI ---")]
    public Slider BGM_Slider;
    public Slider SFX_Slider;
    public Text BGM_Value;
    public Text SFX_Value;

    [Header("--- Exit UI ---")]
    public Button YesBtn;

    // Start is called before the first frame update
    void Start()
    {
        YesBtn.onClick.AddListener(UserDataSave);
        BGM_Slider.value = SoundMgr.BGM_Value;
        SFX_Slider.value = SoundMgr.SFX_Value;
        BGM_Value.text = $"{BGM_Slider.value * 100f:F0}";
        SFX_Value.text = $"{SFX_Slider.value * 100f:F0}";
    }

    // Update is called once per frame
    void Update()
    {
        if(BGM_Slider.value != SoundMgr.BGM_Value)
        {
            SoundMgr.inst.BGM_Volume_Set(BGM_Slider.value);
            BGM_Value.text = $"{BGM_Slider.value * 100f:F0}";
        }

        if(SFX_Slider.value != SoundMgr.SFX_Value)
        {
            SoundMgr.inst.SFX_Volume_Set(SFX_Slider.value);
            SFX_Value.text = $"{SFX_Slider.value * 100f:F0}";
        }
    }

    public void Event_ButtonStay(int index)
    {
        switch (index)
        {
            case 0: //사운드 버튼클릭
                Outline soundText = SoundBtn.GetComponentInChildren<Outline>();
                soundText.enabled = true;
                break;

            case 1: //키가이드 버튼 클릭
                Outline keyText = KeyGuideBtn.GetComponentInChildren<Outline>();
                keyText.enabled = true;
                break;

            case 2: //exit버튼 클릭
                Outline exitText = ExitBtn.GetComponentInChildren<Outline>();
                exitText.enabled = true;
                break;
        }
    }
    public void Event_ButtonExit(int index)
    {
        switch (index)
        {
            case 0: //사운드 버튼클릭
                Outline soundText = SoundBtn.GetComponentInChildren<Outline>();
                soundText.enabled = false;
                break;

            case 1: //키가이드 버튼 클릭
                Outline keyText = KeyGuideBtn.GetComponentInChildren<Outline>();
                keyText.enabled = false;
                break;

            case 2: //exit버튼 클릭
                Outline exitText = ExitBtn.GetComponentInChildren<Outline>();
                exitText.enabled = false;
                break;
        }
    }

    void UserDataSave()
    {
        //여기서 유저 정보를 저장
        //Debug.Log("세이브버튼 클릭");
        Vector3 playerPos = GameObject.FindGameObjectWithTag("Player").transform.position;

        if (GlobalValue.sceneType == SceneType.Lobby) //마을씬
        {           
            GameMgr.inst.userData.sceneType = SceneType.Lobby;
            GameMgr.inst.userData.playerSavePos = playerPos;
        }

        if (GlobalValue.sceneType == SceneType.Game) //게임씬
        {
            GameMgr.inst.userData.sceneType = SceneType.Game;
            GameMgr.inst.userData.playerSavePos = playerPos;
        }

        DataMgr.inst.SaveData();
        SceneManager.LoadScene("TitleScene");
    }
}
