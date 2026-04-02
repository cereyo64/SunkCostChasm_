using UnityEngine;

public class Batery : MonoBehaviour, IInteractable, IHaveHealth
{
    public int StartHealth;
    public bool AddedLightBataryAmountIsRandom;
    public float AddedLightBataryAmount;
    public float minimumBataryAmount, maximumBataryAmount, AverageBateryAmount;

    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public string InteractionName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string RequiredItemName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsProgressedInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CompleteTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool CanInteract { get; set; }

    public void Dead()
    {
        throw new System.NotImplementedException();
    }

    public void Heal(int healAmount)
    {
        throw new System.NotImplementedException();
    }

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

    public void TakeDamage(int damageAmount)
    {
        throw new System.NotImplementedException();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        MaxHealth = StartHealth;
        CurrentHealth = MaxHealth;

        if (AddedLightBataryAmountIsRandom)
        {
            AddedLightBataryAmount = Random.Range(minimumBataryAmount, maximumBataryAmount);
        }
        else
        {
            AddedLightBataryAmount = AverageBateryAmount;
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponentInChildren<PlayerLightController>() != null)
        {
            PlayerLightController PLC = other.gameObject.GetComponentInChildren<PlayerLightController>();

            PLC.LightBatary += AddedLightBataryAmount;
            PLC.LightRadius = PLC.LightBatary / 8;

            Destroy(gameObject);
        }
    }
}

