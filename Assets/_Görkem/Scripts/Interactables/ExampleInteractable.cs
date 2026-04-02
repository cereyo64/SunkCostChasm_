using UnityEngine;

public class ExampleInteractable : MonoBehaviour, IInteractable
{
    public string InteractionName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string RequiredItemName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsProgressedInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CompleteTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool CanInteract { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Interact()
    {
        throw new System.NotImplementedException();
    }

    public void InteractCancel()
    {
        throw new System.NotImplementedException();
    }

    public void InteractEnd()
    {
        throw new System.NotImplementedException();
    }

    public void InteractTick()
    {
        throw new System.NotImplementedException();
    }

    public void InteractWithItem(string itemName)
    {
        throw new System.NotImplementedException();
    }
}
