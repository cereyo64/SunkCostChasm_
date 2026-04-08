using System;
using UnityEngine;

public class Doors : MonoBehaviour, IInteractable
{
    public string interactionName;
    public string leadingRoom;
    public string requiredItemName;

    [Header("Kapý seçenekleri")]
    public bool canInteract = true;
    public bool KeyRequired;
    public bool DestroyKeyOnUse;

    [Header("Oda deðiþtiriyor mu ?")]
    public Sprite RoomChangerDoorSprite;
    public Sprite NormalDoorSprite;
    public bool IsRoomChangerDoor;
    public Color AfterOpenedDoorColor;

    public string RequiredItemName { get => requiredItemName ; set => requiredItemName = value; }
    public bool CanInteract { get => canInteract ; set => canInteract = value; }
    public string InteractionName { get => interactionName ; set => interactionName = value; }
    public bool IsProgressedInteractable { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public float CompleteTime { get => 0; set => value = 0; }

    public void Interact()
    {
        if (KeyRequired)
        {
            InteractWithItem(requiredItemName);
        }
        else
        {
            SendPlayerToNextRoom();
        }
        
    }
    public void InteractCancel()
    {
       
    }
    public void InteractEnd()
    {
        
    }
    public void InteractTick()
    {
        
    }

    public void InteractWithItem(string itemName)
    {
        if (KeyRequired)
        {
            if (PlayerInventory.Inventory.Keys.Contains(itemName))
            {
                SendPlayerToNextRoom();
                KeyRequired = false;
                if (DestroyKeyOnUse)
                {
                    PlayerInventory.Inventory.Keys.Remove(itemName);
                    //Ses efekti kullanýlacaksa burada çalýnabilir.
                }

            }
            else
            {
                print("You need the correct key to open this door.");
            }
        }
        
       
    }
    public void SendPlayerToNextRoom()
    {
        if (IsRoomChangerDoor)
        {
            RoomEvents.SwitchToNewRoom(leadingRoom);

            Debug.Log($"Player travels to {leadingRoom}");
        }
        else
        {
            GetComponent<BoxCollider2D>().isTrigger = true;
            CanInteract = false;
            GetComponent<SpriteRenderer>().sortingOrder = 3;
            GetComponent<SpriteRenderer>().color = AfterOpenedDoorColor;
            //Kapý açýlma sesi konulabilir.
        }
        
    }

    public void Start()
    {
        if(requiredItemName == "")
        {
            KeyRequired = false;
            DestroyKeyOnUse = false;
        }
        else
        {
            KeyRequired = true;
        }

        if(IsRoomChangerDoor)
        {
            GetComponent<SpriteRenderer>().sprite = RoomChangerDoorSprite;
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = NormalDoorSprite;
            GetComponent<BoxCollider2D>().isTrigger = false;
        }
    }
}
