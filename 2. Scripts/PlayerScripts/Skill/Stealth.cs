using UnityEngine;

public class Stealth : MonoBehaviour, ISkill
{
    PlayerCombat pc;
    StealthSkillSO data;
    SpriteRenderer[] renderers;
    float startTime;

    public void OnExecute(PlayerCombat pc, SkillData skillData)
    {
        this.pc = pc;
        this.data = (StealthSkillSO)skillData;

        renderers = pc.GetComponentsInChildren<SpriteRenderer>();
        startTime = Time.time;

        // 은신 시작
        pc.player.isStealth = true;
        foreach (var r in renderers)
        {
            var c = r.color;
            c.a = data.transparentAlpha;
            r.color = c;
        }
    }

    void Update()
    {
        if (pc == null || data == null) return;

        if (Time.time - startTime >= data.stealthDuration)
        {
            EndStealth();
        }
    }

    private void EndStealth()
    {
        if (pc != null)
            pc.player.isStealth = false;

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