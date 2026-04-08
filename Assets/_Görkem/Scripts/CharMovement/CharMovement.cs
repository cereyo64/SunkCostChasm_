using System;
using System.Collections.Generic;
using UnityEngine;

public class CharMovement : MonoBehaviour
{
    [Header("Physics References")]
    
    public Rigidbody2D m_rb2D;
    public Collider2D m_collider2D;

    public float movementSpeed = 15f;

    [Header("Ground Check")]
    public bool isGrounded;
    public float groundCheckRadius = 0.5f;
    public LayerMask groundLayer;
    public LayerMask wallLayer;

    [SerializeField] private Vector2 lastCachedPosition;
    [SerializeField] private float movement;
    public float downOffset = 1 ;
    public float rayDistance  = 20f;

    [SerializeField] private float movementLerpAlpha = 0.2f;
    [SerializeField] private float wallCheckDistance = 2f;

    [Header(" Movement Settings ")]
    public MovementType currentMovementType = MovementType.normal;

    [Header("Ladder Settings")]

    [SerializeField] Ladder currentLadder;

    [SerializeField] private float climbingSpeed;
    public enum MovementType
    {
        normal,
        ladder,
        swim
    }

    #region Str , Updt , FxdUpdt

    public void Start()
    {
        movement = 0;
        currentMovementType = MovementType.normal;
        currentLadder = null;
    }
    public void Update()
    {
        switch (currentMovementType)
        { 
         
          case MovementType.normal:

              float x;
              x = Input.GetAxisRaw("Horizontal");

              movement = x;

              break;

            case MovementType.ladder:

                x = Input.GetAxisRaw("Vertical");

                movement = x;

                if (Input.GetKey(KeyCode.Space))
                {
                   SwitchMovementType(MovementType.normal);
                }

            break;
       
        }
    }
    public void FixedUpdate()
    {
        switch (currentMovementType)
        {
            case MovementType.normal:
              NormalMovementFixedUpdate();
            break;

            case MovementType .ladder:
              LadderMovementFixedUpdate();
            break;
        }
    }

    public void OnEnable()
    {
        MovementEvents.OnGrabLadder += MovementEvents_OnGrabLadder;
    }
    public void OnDisable()
    {
        MovementEvents.OnGrabLadder -= MovementEvents_OnGrabLadder;
    }
    #endregion
    private void SwitchMovementType(MovementType newMovementType)
    {
        if(currentMovementType == newMovementType)
        {
            Debug.LogWarning(" You tried to assign the same movementType ");
            return;
        }

        switch (newMovementType)
        {
            case MovementType.normal:

                Debug.Log("Switched To Normal Movement");
                

            break;

            case MovementType.ladder:

                Debug.Log(" Switched To Ladder Movement ");
                

            break;

            case MovementType.swim:

                Debug.Log(" Switched To swimming Movement ");

            break;
        }
        

        currentMovementType = newMovementType;
    }

    #region Movement Fixed Updates

    private void LadderMovementFixedUpdate()
    {
        Vector2 movementDir = movement * climbingSpeed * Time.fixedDeltaTime * currentLadder.direction;

        Vector2 lerpedMovement = Vector2.Lerp((Vector2)transform.position , (Vector2)transform.position + movementDir, movementLerpAlpha);

        RaycastHit2D ladderCheck = Physics2D.CircleCast(transform.position, 0.2f, movementDir);


        if(!ladderCheck.collider.CompareTag("Ladder"))
        {
            SwitchMovementType(MovementType.normal);
        }

        if (movementDir != Vector2.zero)
        {
            m_rb2D.MovePosition(lerpedMovement);
        }


    }
    private void NormalMovementFixedUpdate()
    {
        RaycastHit2D middleGroundCast = Physics2D.Raycast((Vector2)transform.position, Vector2.down, rayDistance, groundLayer);

        RaycastHit2D forwardCast = Physics2D.Raycast((Vector2)transform.position, transform.right, wallCheckDistance, wallLayer);

        RaycastHit2D backwardCast = Physics2D.Raycast((Vector2)transform.position, -transform.right, wallCheckDistance, wallLayer);

        if (forwardCast.collider != null)
        {
            movement = Mathf.Clamp(movement, -1, 0);
        }
        else if (backwardCast.collider != null)
        {

            movement = Mathf.Clamp(movement, 1, 0);
        }

        Vector2 calcMovement = ((Vector2)transform.right * movement * movementSpeed * Time.fixedDeltaTime) + Vector2.up;

        Vector2 LerpedMovement;

        LerpedMovement = Vector2.Lerp((Vector2)transform.position, middleGroundCast.point + calcMovement, movementLerpAlpha);

        m_rb2D.MovePosition(LerpedMovement);
    }

    #endregion

    #region Movement Event Handling
    private void MovementEvents_OnGrabLadder(object sender, Ladder grabbedLadder)
    {
        if(grabbedLadder != null)
        {
            currentLadder = grabbedLadder;
        }

        SwitchMovementType(MovementType.ladder);


    }
    
    #endregion

    public void OnDrawGizmos()
    {
        RaycastHit2D hit2D = Physics2D.Raycast((Vector2)transform.position, Vector2.down, rayDistance, groundLayer);

        RaycastHit2D middleGroundCast = Physics2D.Raycast((Vector2)transform.position, Vector2.down, rayDistance, groundLayer);

        RaycastHit2D forwardCast = Physics2D.Raycast((Vector2)transform.position , transform.right, wallCheckDistance , wallLayer);

        RaycastHit2D backwardCast = Physics2D.Raycast((Vector2)transform.position , -transform.right, wallCheckDistance , wallLayer);

        if (forwardCast.collider != null)
        {
            movement = Mathf.Clamp(movement, -1, 0);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(forwardCast.point,0.5f);


        }
        else if (backwardCast.collider != null)
        {

             movement = Mathf.Clamp(movement, 1, 0);
             Gizmos.color = Color.red;
             Gizmos.DrawWireSphere(backwardCast.point, 0.5f);

        }

        Vector2 calcMovement = ((Vector2)transform.right * movement * movementSpeed * Time.fixedDeltaTime) + Vector2.up;

        Gizmos.color = Color.maroon;
        Gizmos.DrawWireSphere(hit2D.point, 0.5f);
        
        Gizmos.color = Color.forestGreen;
        Gizmos.DrawWireSphere(hit2D.point + calcMovement,0.3f);

     
       
    }
    
}
