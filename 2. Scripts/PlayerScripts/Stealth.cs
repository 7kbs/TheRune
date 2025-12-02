using UnityEngine;

public class Stealth : MonoBehaviour, ISkillBehaviour
{
    PlayerCombat player;
    StealthSkillSO data;
    SkillRuntime runtime;
    SpriteRenderer[] renderers;

    public void OnExecute(PlayerCombat player, SkillData data, SkillRuntime runtime)
    {
        this.player = player;
        this.data = (StealthSkillSO)data;
        this.runtime = runtime;

        runtime.isActive = true;
        runtime.startTime = Time.time;

        renderers = player.GetComponentsInChildren<SpriteRenderer>();

        // ½ÃÀÛ
        player.isStealth = true;
        foreach (var r in renderers)
        {
            var c = r.color;
            c.a = this.data.transparentAlpha;
            r.color = c;
        }
    }

    void Update()
    {
        if (!runtime.isActive)
            return;

        if (Time.time - runtime.startTime >= data.stealthDuration)
        {
            EndStealth();
        }
    }

    void EndStealth()
    {
        runtime.isActive = false;
        player.isStealth = false;

        foreach (var r in renderers)
        {
            var c = r.color;
            c.a = 1f;
            r.color = c;
        }

        Destroy(gameObject);
    }
}