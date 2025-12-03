using UnityEngine;

public interface ISkill
{
    void OnExecute(PlayerCombat player, SkillData data);
    //virtual void OnUpdate(PlayerCombat player, SkillData data) { }
}