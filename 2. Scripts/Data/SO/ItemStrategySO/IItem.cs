using UnityEngine;

public interface IItem
{
    void OnExcute(UserData userdata, ItemBase item, ItemDB db);
}
