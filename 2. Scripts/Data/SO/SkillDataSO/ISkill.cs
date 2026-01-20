using UnityEngine;

public interface ISkill
{
    void OnExecute(PlayerCombat player, SkillData data);
}