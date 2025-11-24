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

    public static bool BossCutSceneOver;

}
