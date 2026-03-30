

public interface IInteractable 
{
    public string InteractionName {  get; set; }
    public string RequiredItemName { get; set;}
    public bool CanInteract {  get; set; }
    public void Interact();
    public void InteractWithItem(string itemName);
   






}
