using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class PlayerInventory : MonoBehaviour
{
    [Header("Seviye keþif objeleri")]
    public List<string> Keys;

    [Header("Saðlýk Objeler")]
    public int HealthKitCount;
    public int HealthKitHealAmount;
    public int EnergyBarCount;

    public static PlayerInventory Inventory;

    // ===================================================== HEALTH KIT =====================================================

    public void Start()
    {
        if (Inventory == null)
        {
            Inventory = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddHealthKit()
    {
        HealthKitCount++;
    }

    public void UseHealthKit()
    {
        if (HealthKitCount > 0)
        {
            HealthKitCount--;
            GetComponent<IHaveHealth>().Heal(25);
        }
        else
        {
            Debug.Log("Yeterli saðlýk kiti yok!");
        }
    }
}
