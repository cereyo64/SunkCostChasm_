using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CharMovement : MonoBehaviour
{
    public Rigidbody2D rb2D;
    public Collider2D col2D;

    
    public Dictionary<string,MovementTypeBase> movementTypes = new Dictionary<string,MovementTypeBase>();

    public MovementTypeBase currentMovementType;

    public void Start()
    {
        if(rb2D == null)
        {
            rb2D = GetComponent<Rigidbody2D>();
        }

        if(col2D == null)
        {
            col2D = GetComponent<Collider2D>();
        }
        currentMovementType = GetComponent<BaseCharacterMovement>();
        currentMovementType.EnterMovement(rb2D,col2D);
    }

    public void Update()
    {
        currentMovementType.UpdateCache();
    }

    public void FixedUpdate()
    {
        currentMovementType.Movement();
    }


    public void RegisterNewMovementType(string movementName, MovementTypeBase newMovement)
    {

    }
    public void SwitchToNewMovementType(string movementName)
    {

        currentMovementType.ExitMovement();

    }

    /*
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

    [Header("Jump")]

    [SerializeField] private bool canJump;
    [SerializeField] public float JumpForce;

    public Matrix4x4 transformMatrix;

    [SerializeField] private float movement;
    public float downOffset = 1 ;
    public float rayDistance  = 20f;

    [SerializeField] private float movementLerpAlpha = 0.2f;
    [SerializeField] private float wallCheckDistance = 2f;

    public void Start()
    {
        movement = 0;
       
    }

    public void Update()
    {
        float x = Input.GetAxisRaw("Horizontal");

        movement = x;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

    }

    public void FixedUpdate()
    {
        RaycastHit2D middleGroundCast = Physics2D.Raycast((Vector2)transform.position, Vector2.down, rayDistance, groundLayer);

        RaycastHit2D forwardCast = Physics2D.Raycast((Vector2)transform.position , transform.right, wallCheckDistance, wallLayer);

        RaycastHit2D backwardCast = Physics2D.Raycast((Vector2)transform.position , -transform.right, wallCheckDistance, wallLayer);

        if(forwardCast.collider != null)
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
        canJump = isGrounded;
    }
    public void Jump()
    {
        if (canJump)
        {
            m_rb2D.AddForceY(Mathf.Abs(JumpForce));
        }
    }

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
    */
}
