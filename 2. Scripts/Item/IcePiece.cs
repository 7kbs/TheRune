using UnityEngine;

public class IcePiece : MonoBehaviour, IItem
{
    public void OnExcute(UserData userdata, ItemBase item, ItemDB db)
    {
        var data = (IcePieceSO)item;
        userdata.PlayerMaxMp += data.maxMpIncrease;

        Debug.Log("IcePiece Excute");
    }
}
