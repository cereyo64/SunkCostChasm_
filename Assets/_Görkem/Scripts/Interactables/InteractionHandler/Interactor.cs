using UnityEngine;

public class Interactor : MonoBehaviour
{

    public Collider2D interactorCollider;
    public bool canInteract;
    public LayerMask interactorMask;

    public IInteractable sensedInteractable;

    public string currentHeldItem;


    public void Update()
    {
        if(canInteract && Input.GetKeyDown(KeyCode.Return) && sensedInteractable != null)
        {
            sensedInteractable.Interact();
        }

    }
    public void OnTriggerEnter2D(Collider2D collision)
   {
        if (!canInteract) return;

        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
            if (!interactable.CanInteract) return;

            sensedInteractable = interactable;

            
        }

   }

   public void OnTriggerExit2D(Collider2D collision)
   {
        if (collision.TryGetComponent<IInteractable>(out IInteractable interactable))
        {
          sensedInteractable = null;


        }
    }

   public void OnTriggerStay2D(Collider2D collision)
   {
        
        if (sensedInteractable == null) return;

        if (Input.GetKeyDown(KeyCode.Return ) || Input.GetKeyDown(KeyCode.O))
        {
            if(currentHeldItem != "")
            {
                sensedInteractable.InteractWithItem(currentHeldItem);
                //Debug.LogWarning("Interacted With item!");
            }
            else
            {
                sensedInteractable.Interact();
                //Debug.LogWarning("Interacted Without item");
            }
            
        }

   }

   public void ToggleInteractor(bool toggle)
   {
        canInteract = toggle;
        interactorCollider.enabled = toggle;    
   }

   public void AssignNewItem(string ItemName)
   {
     if(ItemName == null) return;    

     if(ItemName == currentHeldItem)
     {
        return;
     }
     else
     {
        currentHeldItem = ItemName;
     }


    
   }

    private void OnGUI()
    {
        if (!canInteract || sensedInteractable == null) return;

        string text = sensedInteractable.InteractionName;
        int width = 300;
        int height = 28;
        Rect rect = new Rect(Screen.width - width - 10, Screen.height - height - 10, width, height);

        GUIStyle style = new GUIStyle(GUI.skin.label);
        style.alignment = TextAnchor.MiddleRight;
        style.fontSize = 14;
        style.normal.textColor = Color.white;

        GUI.Label(rect, text, style);
    }


}
