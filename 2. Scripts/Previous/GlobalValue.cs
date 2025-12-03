using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SceneType
{
    Title,
    Opening,
    Lobby,
    Game,
    Boss,
    Battle
}

public class GlobalValue
{
    public static SceneType sceneType;

    public static bool BossCutSceneOver;
}
