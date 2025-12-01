using UnityEngine;

public interface ISkillBehaviour
{
    void OnExecute(PlayerCombat player, SkillData data, SkillRuntime runtime);
    virtual void OnUpdate(PlayerCombat player, SkillData data, SkillRuntime runtime) { }
}