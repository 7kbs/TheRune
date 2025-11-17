using UnityEngine;
using UnityEngine.SceneManagement;

public class WarpPointInteractable : Interactable
{
    [SerializeField] string targetScene;
    [SerializeField] GlobalValue.SceneType targetSceneType;

    [SerializeField] QuestData requiredData;
    [SerializeField] QuestData targetData;

    public override void Interact(Player player)
    {
        if (requiredData != null)
        {
            if (!QuestMgr.inst.IsQuestRewarded(requiredData.questID))
            {
                GameMgr.inst.InfoPanelOn("퀘스트를 완료해주세요!");   
                return;
            }
            else QuestMgr.inst.TryCompleteQuest(targetData);
        }

        GameMgr.inst.userData.playerSavePos = Vector3.zero;
        GlobalValue.sceneType = targetSceneType;
        SceneManager.LoadScene(targetScene);
        SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);
    }
}