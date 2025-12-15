using UnityEngine;

public class SpritPiece : MonoBehaviour, IItem
{
    public void OnExcute(UserData userdata, ItemBase item, ItemDB db)
    {
        var data = (SpiritPieceSO)item;
        userdata.GameMoney += data.defaultReward;

        Debug.Log("SpiritPiece Excute");
    }
}
