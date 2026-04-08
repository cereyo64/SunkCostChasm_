using UnityEngine;

public class LadderMovement : MovementTypeBase
{
    public Vector2 ladderBPoint;
    public Vector2 ladderAPoint;

    public float currentProgress;
    public float ladderClimbSpeed = 5f;
    public float ladderLenght;

    [SerializeField] private float _currentDir;
    public override void EnterMovement(Rigidbody2D rigidBody2D, Collider2D collider2D)
    {
        base.EnterMovement(rigidBody2D, collider2D);

        ladderLenght = Vector2.Distance(ladderAPoint, ladderBPoint);
        currentProgress = 0f;

        Vector3 ladderProjected = Vector3.Project(rb2D.position, (ladderBPoint - ladderAPoint));

        Vector2 cached = (Vector2)ladderProjected;

        currentProgress = Vector2.Distance(cached,ladderAPoint);

        //Find along which progress does this make it live on 

        ladderAPoint = transform.InverseTransformPoint(ladderAPoint);
        ladderBPoint = transform.InverseTransformPoint(ladderBPoint);
    }
    public override void UpdateCache()
    {
        _currentDir = Input.GetAxisRaw("Vertical");


    }
    public override void Movement()
    {
        if (ladderLenght <= 0f) return;

        currentProgress = Mathf.Clamp(currentProgress + (_currentDir * ladderClimbSpeed * Time.fixedDeltaTime) , 0f, ladderLenght);

        float t = currentProgress / ladderLenght;
        Vector2 moveDir = Vector2.Lerp(ladderAPoint, ladderBPoint, t);

        rb2D.MovePosition(moveDir);
    }
    







}