using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class OpeningMgr : MonoBehaviour
{
    public UserData data;
    public Button skipbtn;

    void Start()
    {
        skipbtn.onClick.AddListener(() => 
        {
            data.OpeningEnd = true;
            SceneManager.LoadScene($"LobbyScene");
            SceneManager.LoadScene($"PlayerScene", LoadSceneMode.Additive);
        });
    }
}
