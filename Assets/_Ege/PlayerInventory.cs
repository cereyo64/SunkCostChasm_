using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    [Header("Seviye keþif objeleri")]
    public bool HaveImportentKey;
    public int BasicKeyCount;

    [Header("Saðlýk Objeler")]
    public int HealthKitCount;
    public int HealthKitHealAmount;
    public int EnergyBarCount;

    // ===================================================== BASIC KEY =====================================================

    public void AddBasicKey()
    {
        BasicKeyCount++;
    }

    public void useBasicKey()
    {
        if (BasicKeyCount > 0)
        {
            BasicKeyCount--;
        }
        else
        {
            Debug.Log("Yeterli anahtar yok!");
        }
    }

    // ===================================================== IMPROTENT KEY =====================================================

    public void GetHaveImportentKey()
    {
        if(HaveImportentKey)
        {
            Debug.Log("Zaten önemli anahtar var!");
            return;
        }
        HaveImportentKey = true;
        print("Artýk önemli kapý açýlacak");
    }

    public void UseHaveImportentKey()
    {
        HaveImportentKey = false;
        print("Önemli kapý açýldý");
    }

    // ===================================================== HEALTH KIT =====================================================

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
