using System;
using UnityEngine;

//public interface IItemStrategy
//{
//    string ItemName { get; }     // ½Äº°¿ë
//    void Execute(UserData userData, ItemData itemData);
//}


public abstract class ItemBase : ScriptableObject/*, IItemStrategy*/
{
    public string ItemName;
    public string Description;
    public Sprite icon;
    public int amount;

    //string IItemStrategy.ItemName => throw new NotImplementedException();

    public GameObject reward;

    public virtual bool Consumable => false;
    public virtual bool QuestItem => false;
    public abstract void Execute(UserData userData, ItemData itemData);
}