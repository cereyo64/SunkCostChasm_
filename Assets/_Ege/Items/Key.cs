using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.Universal;


public class Key : MonoBehaviour, IInteractable, IHaveHealth
{
    [Header("Saðlýklar")]
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }
    public int StartedHealth;

    [Header("Anahtar Özellikleri")]
    public string KeyNane;
    public string KeyCodes;
    public Sprite KeyLooks;

    [Header("Key Effects")]
    public ParticleSystem KeyCollectEffect;


    public string InteractionName { get ; set; }
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
        PlayerInventory.Inventory.Keys.Add(KeyCodes);
        Destroy(gameObject);
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
        gameObject.GetComponent<SpriteRenderer>().sprite = KeyLooks;

        InteractionName = "CollectKey";
        CanInteract = true;
        MaxHealth = StartedHealth;
        CurrentHealth = MaxHealth;


    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerInventory>() != null)
        {
            KeyCollectEffect.Play();
        }
    }

    public void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.GetComponent<PlayerInventory>() != null)
        {
            KeyCollectEffect.Stop();
        }
    }
}
