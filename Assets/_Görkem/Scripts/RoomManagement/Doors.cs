using JetBrains.Annotations;
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
    public Color AfterClosedDoorColor;

    [Header("Jeneratör baðlantýsý")]
    public string ConnectedGeneratorName;
    public bool ThisDoorIsReversedDoorForGenerator; // Eðer bu kapý jeneratörle baðlantýlýysa ve jeneratör kapalýyken açýk, jeneratör açýlýnca kapanacaksa true yap
    [HideInInspector] public bool IsConnectedToGenerator;
    public bool DoorIsOpened;


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
        else if (IsConnectedToGenerator)
        {
            // Jeneratör açýksa geç, kapalýysa mesaj ver
            if (DoorIsOpened)
            {
                SendPlayerToNextRoom();
            }
            else
            {
                print("Bu kapý bir jeneratöre baðlý, önce jeneratörü aç!");
            }
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
                }
            }
            else
            {
                print("You need the correct key to open this door.");
            }
        }
        else
        {
            print("This door doesn't require a key or Generator Connections");
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
            OpenedDoor();
        }
        
    }

    public void OpenedDoor()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
        GetComponent<SpriteRenderer>().color = AfterOpenedDoorColor;
        DoorIsOpened = true;
    }

    public void ClosedDoor()
    {
        GetComponent<BoxCollider2D>().isTrigger = false;
        GetComponent<SpriteRenderer>().color = AfterClosedDoorColor;
        DoorIsOpened = false;
    }

    public void Start()
    {
        if (requiredItemName == "")
        {
            KeyRequired = false;
            DestroyKeyOnUse = false;
        }
        else
        {
            KeyRequired = true;
        }

        if (IsRoomChangerDoor)
        {
            GetComponent<SpriteRenderer>().sprite = RoomChangerDoorSprite;
            GetComponent<BoxCollider2D>().isTrigger = true;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = NormalDoorSprite;
            GetComponent<BoxCollider2D>().isTrigger = false;
        }

        if (!DoorIsOpened)
        {
            ClosedDoor();
        }
        else
        {
            OpenedDoor();
        }

        if (ConnectedGeneratorName == "")
        {
            IsConnectedToGenerator = false;
        }
        else
        {
            IsConnectedToGenerator = true;
            RequiredItemName = ConnectedGeneratorName;
            KeyRequired = false;

            // Yeni Scene'e geçildiðinde: bu kapýnýn jeneratörü daha önce açýldýysa kapýyý aç
            if (PlayerInventory.Inventory != null &&
                PlayerInventory.Inventory.OpenedGenerators.Contains(ConnectedGeneratorName))
            {
                OpenedDoor();
            }
        }
    }
}
