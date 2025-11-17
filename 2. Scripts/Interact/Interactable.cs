using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public enum InteractableType
{
    None,
    NPC,
    WarpPoint,
    ItemPickup,
    RuneGroup,
    StoneObject
}

public abstract class Interactable : MonoBehaviour
{
    public event Action<Interactable> OnPlayerEnter;
    public event Action<Interactable> OnPlayerExit;
    public InteractableType interactType;
    public string id;

    protected virtual void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            OnPlayerEnter?.Invoke(this);
    }

    protected virtual void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
            OnPlayerExit?.Invoke(this);
    }

    public abstract void Interact(Player player);
}