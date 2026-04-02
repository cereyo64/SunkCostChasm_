using JetBrains.Annotations;
using System;
using UnityEngine;
using UnityEngine.Events;
using static InteractableObject;

public class BasicObject : MonoBehaviour, IInteractable, IHaveHealth
{
    [SerializeField] private InteractableObject interactableObject;
    string ObjectName;
    public int WantedMaxHealth;
    public GameObject Target;

    public string InteractionName { get => ((IInteractable)interactableObject).InteractionName; set => ((IInteractable)interactableObject).InteractionName = value; }
    public string RequiredItemName { get => ((IInteractable)interactableObject).RequiredItemName; set => ((IInteractable)interactableObject).RequiredItemName = value; }
    public bool IsProgressedInteractable { get => ((IInteractable)interactableObject).IsProgressedInteractable; set => ((IInteractable)interactableObject).IsProgressedInteractable = value; }
    public float CompleteTime { get => ((IInteractable)interactableObject).CompleteTime; set => ((IInteractable)interactableObject).CompleteTime = value; }
    public bool CanInteract { get => ((IInteractable)interactableObject).CanInteract; set => ((IInteractable)interactableObject).CanInteract = value; }
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }



    public void Interact()
    {
        Heal(15);
    }

    public void InteractCancel()
    {

    }

    public void InteractEnd()
    {
        if (interactableObject.InteractType == ItemBehavior.TekEtkileþimlik)
        {

        }
    }

    public void InteractTick()
    {

    }

    public void InteractWithItem(string itemName)
    {

    }

    void Start()
    {
        ObjectName = interactableObject.ObjectName;
        Debug.Log(ObjectName);
        gameObject.GetComponent<SpriteRenderer>().sprite = interactableObject.ObjectSprite;
        gameObject.GetComponent<SpriteRenderer>().sortingOrder = interactableObject.SpriteOrderLayer;
        if (interactableObject.SpriteHaveColor)
        {
            gameObject.GetComponent<SpriteRenderer>().color = interactableObject.SpriteColor;
        }
        MaxHealth = WantedMaxHealth;
        CurrentHealth = MaxHealth;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            TakeDamage(10);
        }

        if (Input.GetKeyDown(KeyCode.Q))
        {
            Heal(10);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Target = collision.gameObject;
        Target.GetComponent<IInteractable>()?.Interact();
    }

    public void TakeDamage(int damageAmount)
    {
        CurrentHealth -= damageAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, -1, MaxHealth);
        print("Eþyanýn caný: " + CurrentHealth);
        if (CurrentHealth <= 0) Dead();
    }

    public void Heal(int healAmount)
    {
        if (CurrentHealth >= MaxHealth) return;
        CurrentHealth += healAmount;
        print("Eþyanýn caný: " + CurrentHealth);
        CurrentHealth = Mathf.Clamp(CurrentHealth, -1, MaxHealth);
    }

    public void Dead()
    {
        Destroy(gameObject);
    }
}
