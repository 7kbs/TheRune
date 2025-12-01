using UnityEngine;

public class SkillRuntime
{
    public SkillData data;
    public float lastUsedTime;
    public bool isActive;  // 런타임 로직 필요한 스킬만 true
    public float startTime; // 공격 시작 등 시간 기록용
    public GameObject currentFx;

    public SkillRuntime(SkillData data)
    {
        this.data = data;
        lastUsedTime = -999f;
        isActive = false;
        startTime = 0f;
        currentFx = null;
    }
}