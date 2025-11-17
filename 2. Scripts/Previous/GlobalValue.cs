using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalValue
{
    public enum SceneType
    {
        Title,
        Opening,
        Lobby,
        Game,
        Boss,
        Battle
    }

    public static SceneType sceneType;
    //public static bool isPlayerStop;
    //public static bool DialogueBoxOn;
    //public static bool PuzzlePanelOn;
    public static bool BossCutSceneOver;

    //public static float BGM_Value = 1.0f;
    //public static float SFX_Value = 1.0f;
}
