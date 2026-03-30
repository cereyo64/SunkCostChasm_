using System;
using UnityEngine;

public class Doors : MonoBehaviour, IInteractable
{
    public string interactionName;
    public string leadingRoom;
    public string requiredItemName;
    public bool canInteract = true;
    public string RequiredItemName { get => requiredItemName ; set => requiredItemName = value; }
    public bool CanInteract { get => canInteract ; set => canInteract = value; }
    public string InteractionName { get => interactionName ; set => interactionName = value; }

    public void Interact()
    {
        SendPlayerToNextRoom();
       
    }

    public void InteractWithItem(string itemName)
    {
        if (itemName == RequiredItemName)
        {
            SendPlayerToNextRoom();
        }
       
    }
    public void SendPlayerToNextRoom()
    {

        RoomEvents.SwitchToNewRoom(leadingRoom);

        Debug.Log($"Player travels to {leadingRoom}");
    }
   
}
