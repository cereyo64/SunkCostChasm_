using System;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "InteractableObject", menuName = "Scriptable Objects/InteractableObject")]
public class InteractableObject : ScriptableObject, IInteractable
{
    [Header("Object Basics")]
    public string ObjectName;
    public ItemBehavior InteractType;
    public enum ItemBehavior
    {
        // 2. Seçenekler: Buraya yazdýðýn her kelime, Inspector panelinde bir seçenek olur.
        TekEtkileþimlik,
        TekEtkileþimBitince,
        TekrardanGelinebilir,
        SürekliEtkileþim,
        SürekliEtkileþimBitince,
    }
    public string canInteractedByTag = "Player";

    [Header("Obje Views")]
    public Sprite ObjectSprite;
    public int SpriteOrderLayer;
    public bool SpriteHaveColor;
    public Color SpriteColor;
    public Collision2D ObjectCollision;

    string IInteractable.InteractionName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    string IInteractable.RequiredItemName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    bool IInteractable.IsProgressedInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    float IInteractable.CompleteTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    bool IInteractable.CanInteract { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    void IInteractable.Interact()
    {
        throw new System.NotImplementedException();
    }

    void IInteractable.InteractCancel()
    {
        throw new System.NotImplementedException();
    }

    void IInteractable.InteractEnd()
    {
        throw new System.NotImplementedException();
    }

    void IInteractable.InteractTick()
    {
        throw new System.NotImplementedException();
    }

    void IInteractable.InteractWithItem(string itemName)
    {
        throw new System.NotImplementedException();
    }
}
