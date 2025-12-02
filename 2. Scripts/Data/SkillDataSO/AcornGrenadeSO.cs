using UnityEngine;

[CreateAssetMenu(menuName = "Skill/AcornGrenade")]
public class AcornGrenadeSO : SkillData
{
    public float throwforce = 45;

    public int grenadeCount = 3;
    public float positionOffsetRange = 0.2f;
}
