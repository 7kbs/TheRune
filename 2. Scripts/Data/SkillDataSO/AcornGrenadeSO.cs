using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AcornGrenade")]
public class AcornGrenadeSO : SkillData
{
    public float throwforce = 45;

    public int grenadeCount = 3;
    public float positionOffsetRange = 0.2f;
    public float[] forceMultiplier = { 0.8f, 0.9f, 1f };
}
