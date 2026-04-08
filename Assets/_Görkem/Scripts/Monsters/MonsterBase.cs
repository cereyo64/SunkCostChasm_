using System;
using System.Collections;
using UnityEngine;

public class MonsterBase : MonoBehaviour
{

    [Header("Combat Related ")]
    public MonsterHealth health;
    public float monsterDamage;

    [Header("Attrebuites")]
    public float monsterHealth;
    public float monsterSpeed;

    [Header("Physics")]
    public Rigidbody2D rb2D;
    public Collider2D col2D;

    [SerializeField] private Vector2 targetPoint = Vector2.zero;
    [SerializeField] private Vector2 targetVelocity;
    [SerializeField] private float minimumDistance;
    [SerializeField] private bool timerStarted;
    [Header(" Visuals ")]

    [SerializeField] private Transform headVisual;
    [SerializeField] private Transform BodyVisual;

    [SerializeField] private float lookAtLerpTime;

    [Header("Behaviour")]

    [SerializeField] private MovementState currentMovementState = MovementState.Idle;

    public enum MovementState
    {
        Idle ,
        Interested,
        Chasing,
        Stunned ,
        Lost

    }
    public void Start()
    {
        currentMovementState = MovementState.Idle;
        Invoke("FindRandomMovePoint", 0.3f);

        if (timerStarted)
        {

        }
    }
    public void Update()
    {
        switch (currentMovementState)
        {
            case MovementState.Idle:

                IdleUpdate();

            break;

            case MovementState.Interested:

                InterestedUpdate();


            break;

            case MovementState.Chasing:

                ChasingUpdate();

            break;

            
        }
    }

    public void FixedUpdate()
    {
        switch (currentMovementState)
        {
            case MovementState.Idle:

                IdleFixedUpdate();

                break;

            case MovementState.Interested:

                InterestedFixedUpdate();
                break;

            case MovementState.Chasing:

                ChasingFixedUpdate();

            break;

            case MovementState.Stunned:

                
            break;

            case MovementState.Lost:

                LostFixedUpdate();

            break;

        }
    }


    #region FixedUpdateMethods
    private void IdleFixedUpdate()
    {

        Vector3 mouseScreen = Input.mousePosition;
        Camera cam = Camera.main;
        if (cam == null) return;

        // Dünya düzlemi z = 0 üzerinde göstermek için kamera ile dünya arasındaki z farkını kullanıyoruz
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, -cam.transform.position.z));

        Vector2 dir = (Vector2)mouseWorld - (Vector2)transform.position;

        Vector2 velocity = monsterSpeed * Time.fixedDeltaTime * dir;

        rb2D.AddForce(velocity,ForceMode2D.Force);

        /*
        //Return if we havent reached the target position Yet
        if (Vector2.Distance((Vector2)transform.position, targetPoint) <= minimumDistance)
        {
           StopCoroutine(ReachedCheck());
           FindRandomMovePoint();
           StartCoroutine(ReachedCheck());
        }
        if (!timerStarted)
        {
            StartCoroutine(ReachedCheck());
        }
        rb2D.AddRelativeForce(Time.fixedDeltaTime * monsterSpeed * targetVelocity ,ForceMode2D.Force);
        */
    }

    private IEnumerator ReachedCheck()
    {
        timerStarted = true;
        yield return new WaitForSeconds(3f);
        timerStarted = false;
        FindRandomMovePoint() ;
    }
    private void FindRandomMovePoint()
    {

        System.Random rnd = new System.Random();

        float x = rnd.Next(0, 360);

        float cosX = Mathf.Cos(x * Mathf.Rad2Deg);

        float y = rnd.Next(0, 360);

        float sinX = Mathf.Cos(y * Mathf.Rad2Deg);

        System.Random lenghtRandom = new System.Random();

        float lenght = lenghtRandom.Next(0, 20);

        Vector2 randomDirection = (Vector2)transform.position + ((Vector2)transform.forward * cosX) + (Vector2)transform.right * sinX;

        Vector2 finalPoint = lenght * randomDirection;

        targetPoint = finalPoint;
        targetVelocity = monsterSpeed * Time.fixedDeltaTime * (targetPoint - (Vector2)transform.position).normalized;
    }

    private void InterestedFixedUpdate()
    {
       
    }
    private void ChasingFixedUpdate()
    {
       
    }
    private void LostFixedUpdate()
    {


    }
    #endregion
  

    #region UpdateMethods

    private void IdleUpdate()
    {
      


    }

    private void ChasingUpdate()
    {
        throw new NotImplementedException();
    }

    private void InterestedUpdate()
    {
        throw new NotImplementedException();
    }

    #endregion

    private void OnDrawGizmos()
    {

     
        Vector3 mouseScreen = Input.mousePosition;
        Camera cam = Camera.main;
        if (cam == null) return;

        // Dünya düzlemi z = 0 üzerinde göstermek için kamera ile dünya arasındaki z farkını kullanıyoruz
        Vector3 mouseWorld = cam.ScreenToWorldPoint(new Vector3(mouseScreen.x, mouseScreen.y, -cam.transform.position.z));

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(mouseWorld, 0.2f);
        
       
        /*
        Gizmos.color = Color.darkGoldenRod;

        Gizmos.DrawSphere(targetPoint,0.4f);

        Gizmos.color = Color.darkKhaki;

        Gizmos.DrawSphere((Vector2) transform.position + targetVelocity, 0.4f);
        */
    }

}

