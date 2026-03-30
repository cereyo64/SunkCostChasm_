

public interface IInteractable 
{
    public string InteractionName {  get; set; }
    public string RequiredItemName { get; set;}
    public bool IsProgressedInteractable { get; set; }
    public float CompleteTime { get; set; }
    public bool CanInteract {  get; set; }
    public void Interact();
    public void InteractTick();
    public void InteractEnd();
    public void InteractCancel();

    public void InteractWithItem(string itemName);
   






}
