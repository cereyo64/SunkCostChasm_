using UnityEngine;

public class Ladder : MonoBehaviour , IInteractable
{

    [SerializeField] private string interactionName = "LadderInteraction";

    [SerializeField] private bool canInteract;

    public Transform PointA;

    public Transform PointB;

    public Vector2 direction;

    public float progressBetweenPoints = 0;

    public float climbTime = 4f;

    public string InteractionName { get => interactionName ; set => interactionName = value; }
    public string RequiredItemName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool IsProgressedInteractable { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float CompleteTime { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public bool CanInteract { get => canInteract ; set => canInteract = value; }

    public void Start()
    {
        direction = ((Vector2)PointB.position - (Vector2)PointA.position). normalized;
    }
    public void Interact()
    {
        MovementEvents.GrabLadder(this);
    }
    public void InteractCancel()
    {
        //Send New MovementMode Event to NormalPlayerModel
    }
    public void InteractEnd()
    {
        
    }
    public void InteractTick()
    {

    }
    public void InteractWithItem(string itemName)
    {
       

    }
    public void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(PointA.position, 0.2f);

        Gizmos.color = Color.bisque;
        Gizmos.DrawWireSphere(PointB.position, 0.2f);


        direction = ((Vector2)PointB.position - (Vector2)PointA.position).normalized;

        Gizmos.DrawRay(PointA.position, direction);
    }



}
