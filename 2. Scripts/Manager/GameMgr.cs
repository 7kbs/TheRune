using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameMgr : MonoBehaviour
{
    public UserData userData;       //유저가 저장한 정보를 얻어오는 데이터
    public ItemDB itemData;

    [HideInInspector] public Player player;   
    public GameObject FadePanel; //페이드인 판넬

    public GameObject SylvaronStoenObj;

    [Header("------ Damage Text ------")]
    public GameObject DamageTextRoot = null;
    public Transform Damage_Canvas = null;
    //--- 캐릭터 메리위에 데미지 띄우기용 변수 선언

    [Header("퀵슬롯")]
    public QuickSlot[] quickSlotUIs;  // 캔버스에 배치한 퀵슬롯 UI들

    public static GameMgr inst; 

    private void Awake()
    {
        inst = this;
    }


    void Start()
    {
        FadePanel.SetActive(true);
        Invoke(nameof(FadePanelOff), 3.0f);
        PlayerMove.inst.ChangeState(new DefaultState());

        SoundMgr.inst.BGM_Play(true);
        SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Warp);

        player = GameObject.Find("Player").GetComponent<Player>();

        InitQuickSlots();
    }


    public void DamageTextSpawn(float dmg, Vector3 pos, Color color)
    {
        GameObject dmgClone = Instantiate(DamageTextRoot, Damage_Canvas);
        DamageTextControl DamageText = dmgClone.GetComponent<DamageTextControl>();
        DamageText.InitDamage(dmg, color);
        Vector3 StartPos = new Vector3(pos.x, pos.y + 2.25f, 0.0f);
        dmgClone.transform.position = StartPos;
    }


    public void FadePanelOff()
    {
        FadePanel.SetActive(false);
    }


    ///퀵슬롯 
    public void UpdateQuickSlotsCount(ItemBase potion)
    {
        int count = itemData.ItemDictionary.ContainsKey(potion) ? itemData.ItemDictionary[potion] : 0;

        for (int i = 0; i < userData.quickSlots.Length; i++)
        {
            if (userData.quickSlots[i].potion == potion)
                quickSlotUIs[i].UpdateCount(count);
        }
    }


    public void InitQuickSlots()
    {
        for (int i = 0; i < quickSlotUIs.Length; i++)
        {
            var slotData = userData.quickSlots[i];

            if (slotData.potion != null)
            {
                int count = itemData.ItemDictionary.ContainsKey(slotData.potion) ? itemData.ItemDictionary[slotData.potion] : 0;
                quickSlotUIs[i].Assign(slotData.potion, count);
            }
            else
            {
                quickSlotUIs[i].Clear();
            }
        }
    }
    ///퀵슬롯 
}