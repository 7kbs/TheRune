using UnityEngine;

public enum SkillCastType
{
    SpawnObject,   // 투사체/설치물 스킬
    LogicOnly      // 은신/버프/즉발/상태변화
}

[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObject/SkillData")]
public abstract class SkillData : ScriptableObject
{
    public SkillCastType castType;
    public string skillID;

    public string skillName;
    [TextArea] public string skillDescription;
    public float cooldown;

    public int damage;
    public int mpCost;
    public float duration;
    public int price;
    public Sprite skillIcon;

    public GameObject skillPrefab;
}