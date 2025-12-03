using UnityEngine;

public class Stealth : MonoBehaviour, ISkill
{
    PlayerCombat player;
    StealthSkillSO data;
    SpriteRenderer[] renderers;
    float startTime;

    public void OnExecute(PlayerCombat player, SkillData skillData)
    {
        this.player = player;
        this.data = (StealthSkillSO)skillData;

        renderers = player.GetComponentsInChildren<SpriteRenderer>();
        startTime = Time.time;

        // 은신 시작
        player.isStealth = true;
        foreach (var r in renderers)
        {
            var c = r.color;
            c.a = data.transparentAlpha;
            r.color = c;
        }
    }

    void Update()
    {
        if (player == null || data == null) return;

        if (Time.time - startTime >= data.stealthDuration)
        {
            EndStealth();
        }
    }

    private void EndStealth()
    {
        if (player != null)
            player.isStealth = false;

        if (renderers != null)
        {
            foreach (var r in renderers)
            {
                var c = r.color;
                c.a = 1f;
                r.color = c;
            }
        }

        Destroy(gameObject);
    }
}