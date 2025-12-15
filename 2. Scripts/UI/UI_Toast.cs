using UnityEngine;
using UnityEngine.UI;

public class UI_Toast : MonoBehaviour
{
    [SerializeField] Text ToastText;

    public void Init(string temp, Color color)
    {
        SoundMgr.inst.UI_Play((int)SoundMgr.UI_Sound.Dialogue);

        ToastText.color = color;
        ToastText.text = temp;

        Destroy(gameObject, 2.0f);
    }
}
