using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoBehaviour
{
    public enum SFX_Sound
    {
        Sword,  // 기본공격
        Leaf,   //나뭇잎
        Stealth, //은신
        Fairy,   //정령
        Bomb,    //폭탄
        BossAttack, //보스 공격사운드
        usePotion, //포션사용 효과음
        Dash,
        Warp
    }


    public enum UI_Sound
    {
        Button,
        ItemGet,
        Dialogue
    }


    public AudioSource[] audioSourceObj;

    public AudioClip[] BGM_Clip;
    public AudioClip[] SFX_Clip;
    public AudioClip[] UI_Clip;

    public static float BGM_Value = 1.0f;
    public static float SFX_Value = 1.0f;

    public static SoundMgr inst;

    private void Awake()
    {
        inst = this;
    }

    public void BGM_Play(bool isOn)
    {
        audioSourceObj[0].volume = BGM_Value;

        if (GlobalValue.sceneType == GlobalValue.SceneType.Title)
        {
            audioSourceObj[0].clip = BGM_Clip[0];
        }

        if (GlobalValue.sceneType == GlobalValue.SceneType.Opening)
        {
            audioSourceObj[0].clip = BGM_Clip[1];
        }

        if (GlobalValue.sceneType == GlobalValue.SceneType.Lobby)
        {
            audioSourceObj[0].clip = BGM_Clip[2];
        }

        if (GlobalValue.sceneType == GlobalValue.SceneType.Game)
        {
            audioSourceObj[0].clip = BGM_Clip[3];
        }

        if (GlobalValue.sceneType == GlobalValue.SceneType.Boss)
        {
            audioSourceObj[0].volume = BGM_Value * 0.7f;            
            audioSourceObj[0].clip = BGM_Clip[4];
        }

        if (GlobalValue.sceneType == GlobalValue.SceneType.Battle)
        {
            audioSourceObj[0].clip = BGM_Clip[5];
        }

        audioSourceObj[0].Play();
    }

    public void SFX_Play(int index)
    {
        audioSourceObj[1].volume = SFX_Value;

        if (index == (int)SFX_Sound.Bomb)
        {
            Invoke(nameof(BombSoundOn), 2.0f);
        }
        else
        {            
            audioSourceObj[1].clip = SFX_Clip[index];
            audioSourceObj[1].Play();
        }
    }

    public void UI_Play(int index)
    {
        audioSourceObj[2].volume = SFX_Value;

        audioSourceObj[2].clip = UI_Clip[index];
        audioSourceObj[2].Play();
    }

    void BombSoundOn()
    {
        audioSourceObj[1].volume = SFX_Value;
        audioSourceObj[1].clip = SFX_Clip[(int)SFX_Sound.Bomb];
        audioSourceObj[1].Play();
    }

    public void BGM_Volume_Set(float value)
    {
        BGM_Value = value;
        audioSourceObj[0].volume = BGM_Value;
    }

    public void SFX_Volume_Set(float value)
    {
        SFX_Value = value;
        audioSourceObj[1].volume = SFX_Value;
        audioSourceObj[2].volume = SFX_Value;
    }
}
