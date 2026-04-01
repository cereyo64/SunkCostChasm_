using UnityEngine;

public class Ladder : MonoBehaviour , IInteractable
{

    [SerializeField] private string interactionName = "LadderInteraction";

    public Vector2 PointA;

    public Vector2 PointB;

    public float progressBetweenPoints = 0;

    public float totalTravelTime = 4f;


    public string InteractionName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public string RequiredItemName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsProgressedInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CompleteTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool CanInteract { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    public void Interact()
    {
        //Send New MovementModeEvent to Climbing Ladder
    }

    public void InteractCancel()
    {
        //Send New MovementMode Event to NormalPlayerModel
    }

    public void InteractEnd()
    {
        //Send New MovementModeEvent To NormalPlayerModel.
    }

    public void InteractTick()
    {

    }

    public void InteractWithItem(string itemName)
    {
       

    }

    void Start()
    {
        
    }

   
    void Update()
    {
        
    }
}
