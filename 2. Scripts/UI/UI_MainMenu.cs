using UnityEngine;
using UnityEngine.UI;

public class UI_MainMenu : MonoBehaviour
{
    public Transform subLayer;

    private GameObject currentSubUI;

    [SerializeField] Button skillBtn;
    [SerializeField] Button questBtn;
    [SerializeField] Button setBtn;
    [SerializeField] Button escBtn;

    void Start()
    {
        
        skillBtn.onClick.AddListener(() => OpenSubUI("Skill_UI_Re"));
        questBtn.onClick.AddListener(() => OpenSubUI("Quest_UI"));
        setBtn.onClick.AddListener(() => OpenSubUI("Setting_UI"));
        escBtn.onClick.AddListener(() => Destroy(gameObject));
    }

    private void OpenSubUI(string uiName)
    {
        // 기존 서브 UI 삭제
        if (currentSubUI != null)
        {
            Destroy(currentSubUI);
            currentSubUI = null;
        }

        // 새로운 서브 UI 스폰
        var prefab = Resources.Load<GameObject>("UI_Path/" + uiName);
        currentSubUI = Instantiate(prefab, subLayer);
    }

    void OnDestroy()
    {
        // 메인 메뉴 종료 시 서브 UI도 정리
        if (currentSubUI != null)
            Destroy(currentSubUI);
    }
}
