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

    [Header("Jump")]

    [SerializeField] private bool canJump;
    [SerializeField] public float JumpForce;

    Matrix4x4 transformMatrix;
    [SerializeField] private float movement;
    public float downOffset;

    public void Start()
    {
        movement = 0;
    }


    public void Update()
    {
        float x = Input.GetAxis("Horizontal");

        movement = x;


        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

    }

    public void FixedUpdate()
    {
        if (Mathf.Abs(movement) > 0)
        {
            Vector2 calcMovement = transform.position + transform.right * movement * movementSpeed * Time.fixedDeltaTime;

            m_rb2D.MovePosition(calcMovement);
        }

        transformMatrix = transform.localToWorldMatrix;

        isGrounded = Physics2D.CircleCast(transformMatrix.GetPosition() ,groundCheckRadius, Vector3.down,downOffset,groundLayer);

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
        Gizmos.matrix = transformMatrix;


        if (isGrounded)
        {
            Gizmos.color = Color.yellow;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawWireSphere(Vector3.zero + Vector3.down * downOffset, groundCheckRadius);



    }






}
