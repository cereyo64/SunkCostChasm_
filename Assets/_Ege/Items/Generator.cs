using JetBrains.Annotations;
using UnityEngine;
using System.Collections.Generic;

public class Generator : MonoBehaviour, IInteractable
{
    [Header("Jeneratör")]
    public Sprite GeneratorLetherClose;
    public Sprite GeneratorLetherOpen;
    public bool IsSwitchOn;
    public string GeneratorName;
    public List<Doors> ConnectedDoors;
    public List<Doors> ConnectedDoorsReverse;

    public string InteractionName { get; set; }
    public string RequiredItemName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsProgressedInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CompleteTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool CanInteract { get; set; }

    public void Interact()
    {
        if (CanInteract)
        {
            // Her etkileþimde kapýlarý taze bul
            RefreshConnectedDoors();

            if (IsSwitchOn == true)
            {
                CloseGenerator();
            }
            else
            {
                OpenGenerator();
            }
        }
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

    void Start()
    {
        if (IsSwitchOn)
        {
            GetComponent<SpriteRenderer>().sprite = GeneratorLetherOpen;
        }
        else
        {
            GetComponent<SpriteRenderer>().sprite = GeneratorLetherClose;
        }
        CanInteract = true;
    }

    public void CloseGenerator()
    {
        GetComponent<SpriteRenderer>().sprite = GeneratorLetherClose;
        ConnectedDoors.ForEach(door => door.ClosedDoor());
        ConnectedDoorsReverse.ForEach(door => door.OpenedDoor());
        PlayerInventory.Inventory.OpenedGenerators.Remove(GeneratorName);
        IsSwitchOn = false;
    }

    public void OpenGenerator()
    {
        GetComponent<SpriteRenderer>().sprite = GeneratorLetherOpen;

        for (int i = 0; i < ConnectedDoors.Count; i++)
        {
            if (ConnectedDoors[i] == null)
            {
                Debug.LogError($"ConnectedDoors[{i}] NULL! Inspector'da boþ slot var.");
                continue;
            }
            ConnectedDoors[i].OpenedDoor();
        }
        for (int i = 0; i < ConnectedDoorsReverse.Count; i++)
        {
            if (ConnectedDoorsReverse[i] == null)
            {
                Debug.LogError($"ConnectedDoorsReverse[{i}] NULL! Inspector'da boþ slot var.");
                continue;
            }
            ConnectedDoorsReverse[i].ClosedDoor();
        }

        PlayerInventory.Inventory.OpenedGenerators.Add(GeneratorName);
        IsSwitchOn = true;
    }

    public void RefreshConnectedDoors()
    {
        ConnectedDoors.Clear();
        ConnectedDoorsReverse.Clear(); // Eksikti

        Doors[] allDoors = FindObjectsByType<Doors>(FindObjectsInactive.Include); // Inactive kapýlarý da bul

        foreach (Doors door in allDoors)
        {
            if (door.ConnectedGeneratorName == GeneratorName)
            {
                if (door.ThisDoorIsReversedDoorForGenerator) // Reverse kapý
                {
                    ConnectedDoorsReverse.Add(door);
                    Debug.Log($"Reverse kapý eklendi: {door.gameObject.name}");
                }
                else // Normal kapý
                {
                    ConnectedDoors.Add(door);
                    Debug.Log($"Normal kapý eklendi: {door.gameObject.name}");
                }
            }
        }

        Debug.Log($"ConnectedDoors: {ConnectedDoors.Count} | ConnectedDoorsReverse: {ConnectedDoorsReverse.Count}");
    }
}
