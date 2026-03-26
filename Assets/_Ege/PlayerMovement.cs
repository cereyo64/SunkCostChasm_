using System.Collections;
using UnityEditor.Rendering.Universal;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class PlayerMovement : MonoBehaviour
{

    [Header("Refs")]
    public Rigidbody rb;
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
    public LayerMask groundMask;          // "Ground" katmanýný seç
    public float GroundCheckDistance;
    public bool CanJump;
    public bool isGrounded;// Inspector'da gözlemlemek için
    public bool CanDoubleJump; // Double Jump kontrol et

    private Collider _col;

    [Header("Crouch")]
    public KeyCode crouchKey;
    public LayerMask environmentMask;     // Tavan/duvar/zemin (Player hariç!)
    public CapsuleCollider NormalSize;    // Ayakta collider
    public CapsuleCollider CrouchSize;    // Eðik collider
    CapsuleCollider _currentCol;          // Aktif olan
    public bool isCrouching;

    [Header("Air Fast-Fall (Double-Tap)")]
    public KeyCode FastFallKey = KeyCode.S;
    public float DoubleTapWindow = 0.25f; // saniye
    public float FastFallForce;

    [Header("Ceiling Check (Rays)")]
    public bool CeilingUseTripleRays = true; // kapatýrsan sadece merkez ray kullanýlýr
    public float CeilingRayStartOffset = 0.3f; // merkezden yukarý baþlat
    public float CeilingRayHorizontalInset = 0.05f; // sol/sað ray'ler kenardan içeri ofset
    public float CeilingRaySkin = 0.01f; // yan/üst güvenlik payý

    [Header("Ceiling / Stand-up Tuning")]
    public float StandClearanceMultiplier = 1f;   // 0.5–1.5 tipik
    public float StandExtraClearance = 0.0f;    // metre cinsinden ek boþluk
    public float CeilingSkin = 0.01f;   // yanlardan güvenlik payý

    [Header("Dodge (Minimal)")]
    public KeyCode DodgeKey = KeyCode.LeftShift;
    public float DodgeDistance = 3f;      // sabit menzil
    public float DodgeImpulse = 18f;     // AddForce(Impulse)
    public float DodgeCooldown = 0.35f;
    public bool AllowAirDodge = true;

    [Header("Dodge Stop (Ray)")]
    public LayerMask dodgeStopMask;       // yalnýz bu katmanlar dursun
    public float DodgeStopRayLength = 0.55f; // merkezden ileri

    [Header("Climb (Low-Obstacle Vault)")]
    public bool EnableClimb = true;
    public float ClimbRayUpOffset = 0.45f;   // üst ray için +Y
    public float ClimbDuration = 0.9f;   // 1 sn civarý
    public float ClimbUp = 1.0f;   // yukarý mesafe
    public float ClimbForward = 0.8f;   // ileri mesafe


    bool isDodging;
    bool isClimbing;
    bool isWallJumping;
    bool IsFastFalling;
    public bool IsCanWallJump;
    bool CantTakeDamage;
    float _nextDodgeTime = 0f;
    float _facing = 1f; // hareket kodunda güncelleniyor
    bool canMoveLeft = true, canMoveRight = true;

    //[Header("Wall Probe")]
    float wallCheckInset = 0.02f;      // kenardan içeri baþlat
    float wallCheckDistance = 0.06f;   // kýsa mesafe kontrol
    float wallCheckHeight = 0.6f;      // gövde ortasý civarý

    bool _blockLeft, _blockRight;

    void Start()
    {
        TryGetComponent(out _col);

        // Baþlangýç: ayakta
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
        ProbeWalls();        // önce duvar var mý, bak
        MovementFixed();     // fiziksel hareket

        //Zýplarken fazla uçmasýn diye
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
        HandleCrouch();  // Eðilme Kodlarý
        HandleDodge();   // Dodge kodlarý
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

        Vector3 leftOrigin = new Vector3(b.min.x + wallCheckInset, yMid, b.center.z);
        Vector3 rightOrigin = new Vector3(b.max.x - wallCheckInset, yMid, b.center.z);

        if (Physics.Raycast(leftOrigin, Vector3.left, wallCheckDistance, environmentMask, QueryTriggerInteraction.Ignore))
            _blockLeft = true;
        if (Physics.Raycast(rightOrigin, Vector3.right, wallCheckDistance, environmentMask, QueryTriggerInteraction.Ignore))
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

        Vector3 delta = new Vector3(inputX * MoveSpeed * Time.fixedDeltaTime, 0f, 0f);
        rb.MovePosition(rb.position + delta);
    }



    void GroundCheck()
    {
        Vector3 LeftSide = new Vector3(transform.position.x - 0.3f, transform.position.y, transform.position.z);
        Vector3 RightSide = new Vector3(transform.position.x + 0.3f, transform.position.y, transform.position.z);

        if (Physics.Raycast(transform.position, Vector3.down, GroundCheckDistance, groundMask, QueryTriggerInteraction.Ignore))
            isGrounded = true;
        else if (Physics.Raycast(LeftSide, Vector3.down, GroundCheckDistance, groundMask, QueryTriggerInteraction.Ignore))
            isGrounded = true;
        else if (Physics.Raycast(RightSide, Vector3.down, GroundCheckDistance, groundMask, QueryTriggerInteraction.Ignore))
            isGrounded = true;
        else { isGrounded = false; }
    }

    public void jump()
    {
        JumpFixed();

        if (ConsumeAirFastFallRequest())
        {
            rb.AddForce(Vector3.down * (FastFallForce), ForceMode.Impulse);
            canMoveLeft = false;
            canMoveRight = false;
            IsFastFalling = true;
        }
    }

    public void JumpFixed()
    {
        if (isClimbing) return;

        // 1. Zemin ve Duvar Kontrollerini Güncelle
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

        // 2. TEK BÝR TUÞ BASIMINDA KARAR VER (Input.GetKeyDown burada olmalý)
        if (Input.GetKeyDown(JumpKey))
        {
            // ÖNCELÝK SIRALAMASI ÇOK ÖNEMLÝ:

            // A. WALL JUMP (Eðer duvardaysan ve bekleme süresi bittiyse)
            // Not: Burada 'IsCanWallJump' senin cooldown deðiþkenin olmalý
            if (!isGrounded && (_blockLeft || _blockRight) && IsCanWallJump)
            {
                StartCoroutine(WallJumping());
                return; // Wall jump yaptýysan diðer zýplamalarý kontrol etme
            }

            // B. NORMAL JUMP
            if (CanJump)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Temiz zýplama
                rb.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
                CanJump = false; // Havaya kalktýðý an normal zýplama biter
                return;
            }

            // C. DOUBLE JUMP
            if (CanDoubleJump && !isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
                rb.AddForce(Vector3.up * (normalJumpForce * 0.8f), ForceMode.Impulse);
                CanDoubleJump = false;
                return;
            }
        }

        DetectAirFastFallDoubleTap();
    }

    float _sLastTap = -1f;
    bool _airFastFallRequested = false;

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

    // FixedUpdate (veya hareket fiziðinin olduðu yerde) kullan:
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
        if (isDodging) return; // Dodge sürerken crouch'u biz kontrol ediyoruz

        bool hold = Input.GetKey(crouchKey);

        if (hold)
        {
            if (!isCrouching) SetCrouch(true); // eðil
            return;
        }

        // Ayaða kalkmak istiyor -> baþüstü boþsa kalk
        if (isCrouching && !IsStandBlocked())
            SetCrouch(false);
    }

    void SetCrouch(bool crouch)
    {
        isCrouching = crouch;

        // Hýz
        MoveSpeed = crouch ? CrouchSpeed : WalkSpeed;
        CanJump = crouch ? false : true;

        // Collider aç/kapat
        NormalSize.enabled = !crouch;
        CrouchSize.enabled = crouch;
        _currentCol = crouch ? CrouchSize : NormalSize;
    }

    // Tek bir yukarý ray: “Normal” boya dönmek için yeterli boþluk var mý?
    bool IsStandBlocked()
    {
        // Þu anki (çoðunlukla crouch) collider AABB
        Bounds cb = _currentCol.bounds;

        // Ayakta yarým yükseklik (world)
        float sy = transform.lossyScale.y;
        float normalHalf = (NormalSize.height * sy) * 0.5f;

        // Ray baþlangýç yüksekliði: merkezden +0.3 m
        Vector3 baseCenter = cb.center;
        float startY = baseCenter.y + CeilingRayStartOffset;

        // Ray uzunluðu: ayakta tepeye kadar olan alan - küçük skin
        float rayLen = Mathf.Max(0.01f, normalHalf - CeilingRayStartOffset - CeilingRaySkin);

        // Merkez ray
        Vector3 originCenter = new Vector3(baseCenter.x, startY, baseCenter.z);

        bool HitRay(Vector3 o)
        {
            return Physics.Raycast(
                o, Vector3.up, rayLen,
                environmentMask, QueryTriggerInteraction.Ignore
            );
        }

        if (!CeilingUseTripleRays)
        {
#if UNITY_EDITOR
            Debug.DrawRay(originCenter, Vector3.up * rayLen, Color.yellow);
#endif
            return HitRay(originCenter);
        }

        // Sol/sað ray'ler: kenardan "inset" kadar içeriden
        float xEdge = Mathf.Max(0.01f, cb.extents.x - CeilingRaySkin);
        float xOff = Mathf.Max(0f, xEdge - CeilingRayHorizontalInset);
        Vector3 left = originCenter + Vector3.left * xOff;
        Vector3 right = originCenter + Vector3.right * xOff;

#if UNITY_EDITOR
        Debug.DrawRay(originCenter, Vector3.up * rayLen, Color.yellow);
        Debug.DrawRay(left, Vector3.up * rayLen, Color.yellow);
        Debug.DrawRay(right, Vector3.up * rayLen, Color.yellow);
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

        // Yön: A/D varsa onu, yoksa bakýþ yönün
        int xDir = 0;
        if (Input.GetKey(KeyCode.A)) xDir = -1;
        else if (Input.GetKey(KeyCode.D)) xDir = 1;
        else xDir = (_facing >= 0f) ? 1 : -1;

        Vector3 dir = new Vector3(xDir, 0f, 0f);
        float startX = rb.position.x;
        float targetX = startX + xDir * DodgeDistance;

        // Y ve rotasyonu kilitle, doðal hýzlanma ver
        RigidbodyConstraints saved = rb.constraints;
        rb.constraints = saved | RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
        rb.AddForce(dir * DodgeImpulse, ForceMode.Impulse);

        const float tol = 0.005f;
        float deadline = Time.time + 1f; // güvenlik

        while (true)
        {
            yield return new WaitForFixedUpdate();

            // 1) Ýleri ray çarparsa BU dodge anýnda biter
            Bounds cb = _currentCol ? _currentCol.bounds : _col.bounds;
            Vector3 origin = cb.center;

            if (Physics.Raycast(origin, dir, DodgeStopRayLength, dodgeStopMask, QueryTriggerInteraction.Ignore))
            { targetX = rb.position.x; break; }

            // 2) Sabit menzile ulaþýldý mý?
            float traveled = Mathf.Abs(rb.position.x - startX);
            if (traveled + tol >= DodgeDistance ||
                (xDir > 0 && rb.position.x >= targetX - tol) ||
                (xDir < 0 && rb.position.x <= targetX + tol)) break;

            // 3) Güvenlik: olaðan dýþý durum
            if (Time.time > deadline) { targetX = rb.position.x; break; }
        }

        // Snap + 1 fizik karesi X dondur -> momentum sýfýr
        rb.MovePosition(new Vector3(targetX, rb.position.y, rb.position.z));
        rb.constraints = saved | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        yield return new WaitForFixedUpdate();
        rb.constraints = saved;

        isDodging = false;
        _nextDodgeTime = Time.time + DodgeCooldown;
    }

    void TryStartClimb()
    {
        if (!EnableClimb || isClimbing || isDodging) return;

        int dir;
        // ESKÝ: if (CanClimb(out dir) && Input.GetKeyDown(JumpKey))
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

        // RAY BAÞLANGICI: ÖN YÜZ (minX/maxX) + küçük iç ofset
        float frontX = (dir > 0) ? b.max.x : b.min.x;
        float inset = Mathf.Max(0.001f, wallCheckInset);
        Vector3 front = new Vector3(frontX - dir * inset, b.center.y, b.center.z);

        Vector3 lowOrigin = front + Vector3.down * ClimbRayUpOffset;// alt ray
        Vector3 highOrigin = front + Vector3.up * ClimbRayUpOffset; // üst ray
        Vector3 fwd = new Vector3(dir, 0f, 0f);

        bool lowHit = Physics.Raycast(lowOrigin, fwd, wallCheckDistance * 5, environmentMask, QueryTriggerInteraction.Ignore);
        bool highHit = Physics.Raycast(highOrigin, fwd, wallCheckDistance * 5, environmentMask, QueryTriggerInteraction.Ignore);

        return lowHit && !highHit; // alt görüyor, üst görmüyorsa: týrmanýlabilir alçak engel
    }


    IEnumerator ClimbRoutine(int dir)
    {
        isClimbing = true;

        // hedef pozisyon (yukarý+ileri)
        Vector3 start = rb.position;
        Vector3 target = start + Vector3.up * ClimbUp + new Vector3(dir * ClimbForward, 0f, 0f);

        // güvenli kýsýtlar
        var saved = rb.constraints;
        rb.constraints = saved | RigidbodyConstraints.FreezeRotation;

        float t = 0f, d = Mathf.Max(0.1f, ClimbDuration);
        while (t < d)
        {
            yield return new WaitForFixedUpdate();

            float u = t / d;                  // 0.1
            float s = u * u * (3f - 2f * u); // SmoothStep
            Vector3 pos = Vector3.Lerp(start, target, s);

            rb.MovePosition(new Vector3(pos.x, pos.y, rb.position.z));
            t += Time.fixedDeltaTime;
        }

        // küçük "snap" ve momentum öldürme
        rb.MovePosition(new Vector3(target.x, target.y, rb.position.z));
        rb.constraints = saved | RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezeRotation;
        yield return new WaitForFixedUpdate();
        rb.constraints = saved;

        isClimbing = false;
    }


    public void CameraFollow()
    {
        Cm.transform.position = new Vector3(transform.position.x, transform.position.y, Cm.transform.position.z);
    }

    IEnumerator WallJumping()
    {
        IsCanWallJump = false; // Kapýyý kilitle

        rb.linearVelocity = Vector2.zero;

        rb.AddForce(Vector2.up * wallJumpForce, ForceMode.Impulse);

        if (_blockLeft) 
        {
            rb.AddForce(Vector2.left * -(wallJumpVerticalForce), ForceMode.Impulse);
            canMoveLeft = false;
            yield return new WaitForSeconds(0.25f);
            if (!canMoveLeft) canMoveLeft = true;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        }

        if (_blockRight) 
        {
            rb.AddForce(Vector2.left * wallJumpVerticalForce, ForceMode.Impulse); 
            canMoveRight = false;
            yield return new WaitForSeconds(0.25f);
            if (!canMoveRight) canMoveRight = true;
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        } 

        yield return new WaitForSeconds(0.25f);

        IsCanWallJump = true; // Yarým saniye sonra kapýyý tekrar aç
    }
}
