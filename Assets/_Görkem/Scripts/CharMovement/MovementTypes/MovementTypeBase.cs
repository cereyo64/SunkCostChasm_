using UnityEngine;

public class MovementTypeBase : MonoBehaviour 
{
    public string movementName;
    public Rigidbody2D rb2D;
    public Collider2D col2D;
    public virtual void EnterMovement(Rigidbody2D rigidBody2D,Collider2D collider2D)
    {
        if(rigidBody2D != null)
        {
            rb2D = rigidBody2D;
        }
        else
        {
            Debug.LogError("You assigned A null rigidbody");
        }

        if(collider2D != null)
        {
            col2D = collider2D;
        }
    }
    public virtual void ExitMovement()
    {
        Debug.Log("Exited this movementType");

    }
    public virtual void UpdateCache()
    {

    }
    public virtual void Movement()
    {

    }
    public string GetMovementName()
    {
        return movementName;
    }
    

}
