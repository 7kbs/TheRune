using UnityEngine;

public interface ISkillBehaviour
{
    void OnExecute(PlayerCombat player, SkillData data);
    virtual void OnUpdate(PlayerCombat player, SkillData data) { }
}