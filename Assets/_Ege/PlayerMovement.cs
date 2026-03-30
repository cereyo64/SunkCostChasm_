using System.Collections;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovement : MonoBehaviour
{

    [Header("Refs")]
    public Rigidbody2D rb;
    public ReferanceManager RF;
    public Camera Cm;

    [Header("Move / Jump")]
    public KeyCode JumpKey;
    public float MoveSpeed;
    public float CrouchSpeed;
    public float WalkSpeed;
    public float JumpForce, wallJumpForce, normalJumpForce, wallJumpVerticalForce;
    public float maxVerticalSpeed;

    [Header("Ground Check (Raycast)")]
    public LayerMask groundMask;          // "Ground" katman�n� se�
    public float GroundCheckDistance;
    public bool CanJump;
    public bool isGrounded;// Inspector'da g�zlemlemek i�in
    public bool CanDoubleJump; // Double Jump kontrol et

    private Collider2D _col;

    [Header("Crouch")]
    public KeyCode crouchKey;
    public LayerMask environmentMask;     // Tavan/duvar/zemin (Player hari�!)
    public CapsuleCollider2D NormalSize;    // Ayakta collider
    public CapsuleCollider2D CrouchSize;    // E�ik collider
    private CapsuleCollider2D _currentCol;          // Aktif olan
    public bool isCrouching;

    [Header("Air Fast-Fall (Double-Tap)")]
    public KeyCode FastFallKey = KeyCode.S;
    public float DoubleTapWindow = 0.25f; // saniye
    public float FastFallForce;

    [Header("Ceiling Check (Rays)")]
    public bool CeilingUseTripleRays = true; // kapat�rsan sadece merkez ray kullan�l�r
    public float CeilingRayStartOffset = 0.3f; // merkezden yukar� ba�lat
    public float CeilingRayHorizontalInset = 0.05f; // sol/sa� ray'ler kenardan i�eri ofset
    public float CeilingRaySkin = 0.01f; // yan/�st g�venlik pay�

    [Header("Ceiling / Stand-up Tuning")]
    public float StandClearanceMultiplier = 1f;   // 0.5�1.5 tipik
    public float StandExtraClearance = 0.0f;    // metre cinsinden ek bo�luk
    public float CeilingSkin = 0.01f;   // yanlardan g�venlik pay�

    [Header("Dodge (Minimal)")]
    public KeyCode DodgeKey = KeyCode.LeftShift;
    public float DodgeDistance = 3f;      // sabit menzil
    public float DodgeImpulse = 18f;     // AddForce(Impulse)
    public float DodgeCooldown = 0.35f;
    public bool AllowAirDodge = true;

    [Header("Dodge Stop (Ray)")]
    public LayerMask dodgeStopMask;       // yaln�z bu katmanlar dursun
    public float DodgeStopRayLength = 0.55f; // merkezden ileri

    [Header("Climb (Low-Obstacle Vault)")]
    public bool EnableClimb = true;
    public float ClimbRayUpOffset = 0.45f;   // �st ray i�in +Y
    public float ClimbDuration = 0.9f;   // 1 sn civar�
    public float ClimbUp = 1.0f;   // yukar� mesafe
    public float ClimbForward = 0.8f;   // ileri mesafe


    bool isDodging;
    bool isClimbing;
    bool isWallJumping;
    bool IsFastFalling;
    public bool IsCanWallJump;
    bool CantTakeDamage;
    float _nextDodgeTime = 0f;
    float _facing = 1f; // hareket kodunda g�ncelleniyor
    bool canMoveLeft = true, canMoveRight = true;

    //[Header("Wall Probe")]
    float wallCheckInset = 0.02f;      // kenardan i�eri ba�lat
    float wallCheckDistance = 0.06f;   // k�sa mesafe kontrol
    float wallCheckHeight = 0.6f;      // g�vde ortas� civar�

    bool _blockLeft, _blockRight;

    void Start()
    {
        TryGetComponent<Collider2D>(out _col);

        // Ba�lang��: ayakta
        _currentCol = NormalSize;
        NormalSize.enabled = true;
        CrouchSize.enabled = false;
        isCrouching = false;
        MoveSpeed = WalkSpeed;
        EnableClimb = true;
    }

    void FixedUpdate()
    {
        GroundCheck();
        ProbeWalls();        // �nce duvar var m�, bak
        MovementFixed();     // fiziksel hareket

        //Z�plarken fazla u�mas�n diye
        if (rb.linearVelocity.y > maxVerticalSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, maxVerticalSpeed);
        }
    }

    void Update()
    {
        if (EnableClimb && (Input.GetKeyDown(JumpKey) || Input.GetKeyDown(KeyCode.W)))
            TryStartClimb();

        jump();
        HandleCrouch();  // E�ilme Kodlar�
        HandleDodge();   // Dodge kodlar�
    }

    void LateUpdate()
    {
        CameraFollow();
    }

    void ProbeWalls()
    {
        _blockLeft = _blockRight = false;

        var b = _currentCol.bounds; // aktif collider
        float yMid = Mathf.Clamp(b.center.y, b.min.y + wallCheckHeight * 0.25f, b.max.y - wallCheckHeight * 0.25f);

        Vector2 leftOrigin = new Vector2(b.min.x + wallCheckInset, yMid);
        Vector2 rightOrigin = new Vector2(b.max.x - wallCheckInset, yMid);

        if (Physics2D.Raycast(leftOrigin, Vector2.left, wallCheckDistance, environmentMask))
            _blockLeft = true;
        if (Physics2D.Raycast(rightOrigin, Vector2.right, wallCheckDistance, environmentMask))
            _blockRight = true;
    }

    void MovementFixed()
    {
        if (isDodging || isClimbing) return;

        float inputX = 0f;
        if (Input.GetKey(KeyCode.A) && canMoveLeft) { inputX -= 1f; _facing = -1f; }
        if (Input.GetKey(KeyCode.D) && canMoveRight) { inputX += 1f; _facing = 1f; }


        if (inputX < 0f && _blockLeft) inputX = 0f;
        if (inputX > 0f && _blockRight) inputX = 0f;

        Vector2 delta = new Vector2(inputX * MoveSpeed * Time.fixedDeltaTime, 0f);
        rb.MovePosition((Vector2)rb.position + (Vector2)delta);
    }



    void GroundCheck()
    {
        Vector2 LeftSide = new Vector2(transform.position.x - groundSideOffset, transform.position.y);
        Vector2 RightSide = new Vector2(transform.position.x + groundSideOffset, transform.position.y);

        if (Physics2D.Raycast(transform.position, Vector2.down, GroundCheckDistance, groundMask))
            isGrounded = true;
        else if (Physics2D.Raycast(LeftSide, Vector2.down, GroundCheckDistance, groundMask))
            isGrounded = true;
        else if (Physics2D.Raycast(RightSide, Vector2.down, GroundCheckDistance, groundMask))
            isGrounded = true;
        else { isGrounded = false; }
    }

    public void jump()
    {
        JumpFixed();

        if (ConsumeAirFastFallRequest())
        {
            rb.AddForce(Vector2.down * (FastFallForce), ForceMode2D.Impulse);
            canMoveLeft = false;
            canMoveRight = false;
            IsFastFalling = true;
        }
    }

    public void JumpFixed()
    {
        if (isClimbing) return;

        // 1. Zemin ve Duvar Kontrollerini G�ncelle
        if (isGrounded)
        {
            CanJump = true;
            CanDoubleJump = true;
            JumpForce = normalJumpForce;
            if (IsFastFalling)
            {
                canMoveLeft = true;
                canMoveRight = true;
                IsFastFalling = false;
            }
        }
        else
        {
            CanJump = false;
        }

        // 2. TEK B�R TU� BASIMINDA KARAR VER (Input.GetKeyDown burada olmal�)
        if (Input.GetKeyDown(JumpKey))
        {
            // �NCEL�K SIRALAMASI �OK �NEML�:

            // A. WALL JUMP (E�er duvardaysan ve bekleme s�resi bittiyse)
            // Not: Burada 'IsCanWallJump' senin cooldown de�i�kenin olmal�
            if (!isGrounded && (_blockLeft || _blockRight) && IsCanWallJump)
            {
                StartCoroutine(WallJumping());
                return; // Wall jump yapt�ysan di�er z�plamalar� kontrol etme
            }

            // B. NORMAL JUMP
            if (CanJump)
            {
                //rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Temiz z�plama
                
                rb.AddForce(Vector2.up * JumpForce, ForceMode2D.Impulse);
                CanJump = false; // Havaya kalkt��� an normal z�plama biter
                return;
            }

            // C. DOUBLE JUMP
            if (CanDoubleJump && !isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector2.up * (normalJumpForce * 0.8f), ForceMode2D.Impulse);
                CanDoubleJump = false;
                return;
            }
        }

        DetectAirFastFallDoubleTap();
    }

    float _sLastTap = -1f;
    bool _airFastFallRequested = false;
    private float groundSideOffset;

    void DetectAirFastFallDoubleTap()
    {
        if (Input.GetKeyDown(FastFallKey))
        {
            float t = Time.time;
            if (!isGrounded && (t - _sLastTap) <= DoubleTapWindow)
            {
                _airFastFallRequested = true;   // bir kere tetiklenir
                _sLastTap = -999f;              // reset
            }
            else
            {
                _sLastTap = t;
            }
        }
    }

    // FixedUpdate (veya hareket fizi�inin oldu�u yerde) kullan:
    public bool ConsumeAirFastFallRequest()
    {
        if (_airFastFallRequested)
        {
            _airFastFallRequested = false;
            return true; // tetik var
        }
        return false;
    }

    // -------- Crouch (Sade) --------
    void HandleCrouch()
    {
        if (isDodging) return; // Dodge s�rerken crouch'u biz kontrol ediyoruz

        bool hold = Input.GetKey(crouchKey);

        if (hold)
        {
            if (!isCrouching) SetCrouch(true); // e�il
            return;
        }

        // Aya�a kalkmak istiyor -> ba��st� bo�sa kalk
        if (isCrouching && !IsStandBlocked())
            SetCrouch(false);
    }

    void SetCrouch(bool crouch)
    {
        isCrouching = crouch;

        // H�z
        MoveSpeed = crouch ? CrouchSpeed : WalkSpeed;
        CanJump = crouch ? false : true;

        // Collider a�/kapat
        NormalSize.enabled = !crouch;
        CrouchSize.enabled = crouch;
        _currentCol = crouch ? CrouchSize : NormalSize;
    }

    // Tek bir yukar� ray: �Normal� boya d�nmek i�in yeterli bo�luk var m�?
    bool IsStandBlocked()
    {
        // �u anki (�o�unlukla crouch) collider AABB
        Bounds cb = _currentCol.bounds;

        // Ayakta yar�m y�kseklik (world)
        float sy = transform.lossyScale.y;
        float normalHalf = (NormalSize.size.y * sy) * 0.5f;

        // Ray ba�lang�� y�ksekli�i: merkezden +0.3 m
        Vector2 baseCenter = cb.center;
        float startY = baseCenter.y + CeilingRayStartOffset;

        // Ray uzunlu�u: ayakta tepeye kadar olan alan - k���k skin
        float rayLen = Mathf.Max(0.01f, normalHalf - CeilingRayStartOffset - CeilingRaySkin);

        // Merkez ray
        Vector2 originCenter = new Vector2(baseCenter.x, startY);

        bool HitRay(Vector2 o)
        {
            return Physics2D.Raycast(
                o, Vector2.up, rayLen,
                environmentMask
            );
        }

        if (!CeilingUseTripleRays)
        {
#if UNITY_EDITOR
            Debug.DrawRay(originCenter, Vector2.up * rayLen, Color.yellow);
#endif
            return HitRay(originCenter);
        }

        // Sol/sa� ray'ler: kenardan "inset" kadar i�eriden
        float xEdge = Mathf.Max(0.01f, cb.extents.x - CeilingRaySkin);
        float xOff = Mathf.Max(0f, xEdge - CeilingRayHorizontalInset);
        Vector2 left = originCenter + Vector2.left * xOff;
        Vector2 right = originCenter + Vector2.right * xOff;

#if UNITY_EDITOR
        Debug.DrawRay(originCenter, Vector2.up * rayLen, Color.yellow);
        Debug.DrawRay(left, Vector2.up * rayLen, Color.yellow);
        Debug.DrawRay(right, Vector2.up * rayLen, Color.yellow);
#endif

        return HitRay(originCenter) || HitRay(left) || HitRay(right);
    }

    void HandleDodge()
    {
        if (isDodging) return;
        if (Time.time < _nextDodgeTime) return;
        if (!AllowAirDodge && !isGrounded) return;
        if (Input.GetKeyDown(DodgeKey)) StartCoroutine(DodgeRoutine());
    }

    IEnumerator DodgeRoutine()
    {
        isDodging = true;

        // Y�n: A/D varsa onu, yoksa bak�� y�n�n
        int xDir = 0;
        if (Input.GetKey(KeyCode.A)) xDir = -1;
        else if (Input.GetKey(KeyCode.D)) xDir = 1;
        else xDir = (_facing >= 0f) ? 1 : -1;

        Vector2 dir = new Vector2(xDir, 0f);
        float startX = rb.position.x;
        float targetX = startX + xDir * DodgeDistance;

        // Y ve rotasyonu kilitle, do�al h�zlanma ver
        RigidbodyConstraints2D saved = rb.constraints;
        rb.constraints = saved | RigidbodyConstraints2D.FreezePositionY | RigidbodyConstraints2D.FreezeRotation;
        rb.AddForce(dir * DodgeImpulse, ForceMode2D.Impulse);

        const float tol = 0.005f;
        float deadline = Time.time + 1f; // g�venlik

        while (true)
        {
            yield return new WaitForFixedUpdate();

            // 1) �leri ray �arparsa BU dodge an�nda biter
            Bounds cb = _currentCol ? _currentCol.bounds : _col.bounds;
            Vector2 origin = cb.center;

            if (Physics2D.Raycast(origin, dir, DodgeStopRayLength, dodgeStopMask))
            { targetX = rb.position.x; break; }

            // 2) Sabit menzile ula��ld� m�?
            float traveled = Mathf.Abs(rb.position.x - startX);
            if (traveled + tol >= DodgeDistance ||
                (xDir > 0 && rb.position.x >= targetX - tol) ||
                (xDir < 0 && rb.position.x <= targetX + tol)) break;

            // 3) G�venlik: ola�an d��� durum
            if (Time.time > deadline) { targetX = rb.position.x; break; }
        }

        // Snap + 1 fizik karesi X dondur -> momentum s�f�r
        rb.MovePosition(new Vector2(targetX, rb.position.y));
        rb.constraints = saved | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForFixedUpdate();
        rb.constraints = saved;

        isDodging = false;
        _nextDodgeTime = Time.time + DodgeCooldown;
    }

    void TryStartClimb()
    {
        if (!EnableClimb || isClimbing || isDodging) return;

        int dir;
        // ESK�: if (CanClimb(out dir) && Input.GetKeyDown(JumpKey))
        if (CanClimb(out dir))
            StartCoroutine(ClimbRoutine(dir));
    }

    bool CanClimb(out int dir)
    {
        dir = 0;
        if (Input.GetKey(KeyCode.A)) dir = -1;
        else if (Input.GetKey(KeyCode.D)) dir = 1;
        else dir = (_facing >= 0f) ? 1 : -1;
        if (dir == 0) return false;

        var b = _currentCol ? _currentCol.bounds : _col.bounds;

        // RAY BA�LANGICI: �N Y�Z (minX/maxX) + k���k i� ofset
        float frontX = (dir > 0) ? b.max.x : b.min.x;
        float inset = Mathf.Max(0.001f, wallCheckInset);
        Vector2 front = new Vector2(frontX - dir * inset, b.center.y);

        Vector2 lowOrigin = front + Vector2.down * ClimbRayUpOffset;// alt ray
        Vector2 highOrigin = front + Vector2.up * ClimbRayUpOffset; // �st ray
        Vector2 fwd = new Vector2(dir, 0f);

        bool lowHit = Physics2D.Raycast(lowOrigin, fwd, wallCheckDistance * 5, environmentMask);
        bool highHit = Physics2D.Raycast(highOrigin, fwd, wallCheckDistance * 5, environmentMask);

        return lowHit && !highHit; // alt g�r�yor, �st g�rm�yorsa: t�rman�labilir al�ak engel
    }


    IEnumerator ClimbRoutine(int dir)
    {
        isClimbing = true;

        // hedef pozisyon (yukar�+ileri)
        Vector2 start = rb.position;
        Vector2 target = start + Vector2.up * ClimbUp + new Vector2(dir * ClimbForward, 0f);

        // g�venli k�s�tlar
        var saved = rb.constraints;
        rb.constraints = saved | RigidbodyConstraints2D.FreezeRotation;

        float t = 0f, d = Mathf.Max(0.1f, ClimbDuration);
        while (t < d)
        {
            yield return new WaitForFixedUpdate();

            float u = t / d;                  // 0.1
            float s = u * u * (3f - 2f * u); // SmoothStep
            Vector2 pos = Vector2.Lerp(start, target, t);

            rb.MovePosition(new Vector2(pos.x, pos.y));
            t += Time.fixedDeltaTime;
        }

        // k���k "snap" ve momentum �ld�rme
        rb.MovePosition(new Vector2(target.x, target.y));
        rb.constraints = saved | RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
        yield return new WaitForFixedUpdate();
        rb.constraints = saved;

        isClimbing = false;
    }


    public void CameraFollow()
    {
        Cm.transform.position = new Vector3(transform.position.x, transform.position.y, -10);
    }

    IEnumerator WallJumping()
    {
        IsCanWallJump = false; // Kap�y� kilitle

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(Vector2.up * wallJumpForce, ForceMode2D.Force);

            if (_blockLeft) 
        {
            rb.AddForce(Vector2.left * -(wallJumpVerticalForce), ForceMode2D.Impulse);
            canMoveLeft = false;
            yield return new WaitForSeconds(0.25f);
            if (!canMoveLeft) canMoveLeft = true;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (_blockRight) 
        {
            rb.AddForce(Vector2.left * wallJumpVerticalForce, ForceMode2D.Impulse); 
            canMoveRight = false;
            yield return new WaitForSeconds(0.25f);
            if (!canMoveRight) canMoveRight = true;
                rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        } 

        yield return new WaitForSeconds(0.25f);

        IsCanWallJump = true; // Yar�m saniye sonra kap�y� tekrar a�
    }





    private void OnDrawGizmos()
    {
        Vector2 LeftSide = new Vector2(transform.position.x - 0.3f, transform.position.y);
        Vector2 RightSide = new Vector2(transform.position.x + 0.3f, transform.position.y);

        Vector2 downPoint = (Vector2)transform.position + Vector2.down * GroundCheckDistance;

        Vector2 L_downPoint = LeftSide + Vector2.down * GroundCheckDistance;
        Vector2 R_downPoint = RightSide + Vector2.down * GroundCheckDistance;

        if (isGrounded)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }

        Gizmos.DrawLine(LeftSide,L_downPoint);
        Gizmos.DrawLine(RightSide, R_downPoint);
        Gizmos.DrawLine( (Vector2)transform.position, downPoint);
        
      
    }
}
