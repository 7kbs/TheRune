using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObject/SkillData")]
public abstract class SkillData : ScriptableObject
{
    public string skillID;

    public string skillName;
    [TextArea] public string skillDescription;
    public float cooldown;
    [HideInInspector] public float lastUsedTime = -999f;

    public int damage;
    public int mpCost;
    public float duration;
    public float throwforce;
    public int price;
    public Sprite skillIcon;

    public GameObject skillPrefab;
    public abstract void Execute(PlayerCombat player);
    public virtual void RuntimeUpdate(PlayerCombat player) { }
}