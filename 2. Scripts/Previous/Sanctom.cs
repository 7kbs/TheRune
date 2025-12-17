using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sanctom : MonoBehaviour
{
    public GameObject[] EffObj;
    public GameObject BossPortalEff;
    public Camera SubCam;

    public QuestData requireQuest;

    void Start()
    {
        if (GameMgr.inst.userData.BossPortalOpen && GameMgr.inst.userData.BossDie == false)
        {
            EffObj[0].SetActive(true);
            EffObj[1].SetActive(true);
            EffObj[2].SetActive(true);
        }
    }

    public void StoneInteration()
    {
        for (int i = 0; i < EffObj.Length; i++)
        {
            EffObj[i].SetActive(true);
        }
    }

    public void BossPortalOpen()
    {
        PlayerMove.inst.ChangeState(new InteractingState());
        GameSceneMgr.inst.SpawnBossPortal();
        GameMgr.inst.userData.BossPortalOpen = true;
        BossPortalEff.SetActive(true);
        Invoke(nameof(SubCamOn), 8.0f);
    }

    void SubCamOn()
    {
        BossPortalEff.SetActive(false);
        SubCam.gameObject.SetActive(true);
        Invoke(nameof(SubCamOff), 8.0f);
    }

    void SubCamOff()
    {
        PlayerMove.inst.ChangeState(new DefaultState());
        SubCam.gameObject.SetActive(false);

        var cq = QuestMgr.inst.CurrentQuest();
        
        if (cq == requireQuest)
        {

            QuestMgr.inst.TryCompleteQuest(cq);
            UIManager.inst.GetToast().Init("퀘스트를 완료하고 보상을 얻으세요!", Color.white);
        }
    }

}
