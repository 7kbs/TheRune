using System;
using UnityEngine;

public abstract class ItemBase : ScriptableObject
{
    public string ItemName;
    public string Description;
    public Sprite icon;
    public int amount;

    public GameObject reward;

    public virtual bool Consumable => false;
    public virtual bool QuestItem => false;
}