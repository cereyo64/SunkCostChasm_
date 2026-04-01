using UnityEngine;

public class MovementTypeBase : MonoBehaviour 
{

    public Rigidbody2D rb2D;

    public virtual void EnterMovement(Rigidbody2D rigidBody2D)
    {
        if(rb2D != null)
        {
            rb2D = rigidBody2D;
        }
        else
        {
            Debug.LogError("You assigned A null rigidbody");
        }

    }

    public virtual void ExitMovement(Rigidbody2D rigidBody2D)
    {
        if (rb2D != null)
        {
            rb2D = rigidBody2D;
        }
        else
        {
            Debug.LogError("You assigned A null rigidbody");
        }

    }

    public virtual void Update()
    {

    }

    public virtual void Movement()
    {

    }




}
