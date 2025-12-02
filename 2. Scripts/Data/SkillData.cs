using UnityEngine;

[CreateAssetMenu(fileName = "NewSkill", menuName = "ScriptableObject/SkillData")]
public abstract class SkillData : ScriptableObject
{
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