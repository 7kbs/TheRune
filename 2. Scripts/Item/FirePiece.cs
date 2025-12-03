using UnityEngine;

public class FirePiece : MonoBehaviour, IItem
{
    public void OnExcute(UserData userdata, ItemBase item, ItemDB db)
    {
        var data = (FirePieceSO)item;
        userdata.PlayerMaxHp += data.maxHpIncrease;

        Debug.Log("FirePiece Excute");
    }
}
