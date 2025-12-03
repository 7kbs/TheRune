using UnityEngine;

[CreateAssetMenu(menuName = "Skill/BasicAttack")]
public class BasicAttackSO : SkillData
{
    [Header("FX/Runtime Object")]

    public float attackRange = 2.0f;
    public float fxStartDelay = 0.2f;
    public float fxDuration = 0.1f;
    public float gravityResetDelay = 1.0f;
}
