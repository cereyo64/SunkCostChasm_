using System.Collections;
using UnityEngine;


public class BasicPlayerMovement : MonoBehaviour, IHaveHealth
{

    #region Variables

    [Header("Hareket Ayarlarý")]
    float MoveSpeed;
    public float walkSpeed;
    public float jumpForce;

    
    [Header("Hareket Ayarlarý")]
    public bool IsRunning;
    public bool StaminaIsFinished;
    public float runSpeed;
    public float MaxStamina;
    public float Stamina;
    public float StaminaIncraseRate;
    public float StaminaDecraseRate;

    [Header("Hareket Durumu ")]
    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;

    public MovementType currentMoveType = MovementType.Walking;
    public Ladder grabbedLadder;
    public float climbingSpeed;

    [Header("Görünüþ Ayarlarý")]

    public Transform[] flippableVisuals;

    [Header("Iþýk Ayarlarý")]
    public Transform lightTransform;
    [SerializeField] private float lightSwitchTime = 2f;
    [SerializeField] private float lightElapsedTime = 0;
    [SerializeField] private bool inTransition = false;
    [SerializeField] private bool isRightSide;
    private  Coroutine lightCoroutine;

    [Header("Zemin Kontrolü")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Can Durumu")]
    public int WantedMaxHealth;
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

  

    #endregion
    public enum MovementType
    {
        Walking,
        Sprinting,
        Ladder,
        Stunned

    }


    #region Baþlangýç / Kapanýþ 
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Havada dönmeyi engellemek için rotasyonu kilitler
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        MoveSpeed = walkSpeed;
        Stamina = MaxStamina;
        MaxHealth = WantedMaxHealth;
        CurrentHealth = MaxHealth;
        isRightSide = true;
        
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

    #region Hareket Eventleri Aboneleri
    private void MovementEvents_OnGrabLadder(object sender, Ladder grabbedLadder)
    {
        if(grabbedLadder != null)
        {
            this.grabbedLadder = grabbedLadder;
            SwitchMovementTypes(MovementType.Ladder);

        }
    }

    #endregion

    #region Update ve FixedUpdate
    void Update()
    {
        // 1. Zemin kontrolü ve Input alýmý
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Zýplama Kontrolü
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        // 3. KOÞMA MANTIÐI (Update içinde karar veriyoruz)
        // Shift'e basýlýyorsa, stamina bitmemiþse ve hareket ediyorsa koþ
        if (Input.GetKey(KeyCode.LeftShift) && !StaminaIsFinished && Mathf.Abs(moveInput) > 0.1f)
        {
            IsRunning = true;
            MoveSpeed = runSpeed;
        }
        else
        {
            IsRunning = false;
            MoveSpeed = walkSpeed;
        }

        // 4. STAMINA YÖNETÝMÝ
        if (IsRunning)
        {
            // Stamina tüketimi
            Stamina -= StaminaDecraseRate * Time.deltaTime;
            if (Stamina <= 0) // 0'ýn altýna düþmesine izin verme
            {
                Stamina = 0;
                StaminaIsFinished = true;
            }
        }
        else
        {
            // Stamina dolumu (Koþmadýðý her an dolmalý)
            if (Stamina < MaxStamina)
            {
                Stamina += StaminaIncraseRate * Time.deltaTime;
            }

            // Stamina belli bir eþiði geçince tekrar koþmaya izin ver (Örn: %20 dolunca)
            if (Stamina >= (MaxStamina * 0.2f))
            {
                StaminaIsFinished = false;
            }
        }

        // Deðeri her zaman 0 ile MaxStamina arasýnda tut (Clamp)
        Stamina = Mathf.Clamp(Stamina, 0, MaxStamina);
        CurrentHealth = Mathf.Clamp(CurrentHealth, -1, MaxHealth);

        if (Input.GetKeyDown(KeyCode.Z)) TakeDamage(10);
        if (Input.GetKeyDown(KeyCode.X)) Heal(10);

        HandleLightDirection();

    }
    void FixedUpdate()
    {
        // Hareket uygulama
        rb.linearVelocity = new Vector2(moveInput * MoveSpeed, rb.linearVelocity.y);

        // Karakterin yönünü çevirme (Flip)
        if(moveInput > 0)
        {
            foreach(Transform t in flippableVisuals)
            {
                t.rotation = Quaternion.Euler(t.rotation.x, 0, t.rotation.z);
            }
        }
        else if (moveInput < 0)
        {
            foreach (Transform t in flippableVisuals)
            {
                t.rotation = Quaternion.Euler(t.rotation.x, -180, t.rotation.z);
                lightTransform.Rotate(Vector3.forward, -lightSwitchTime * Mathf.Deg2Rad);

                
            }
        }

        //if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        //else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);

    }

    #endregion


    #region Iþýk Fonksiyonlarý
    //Iþýðýn saða sola geçmesi için input algýlar
    private void HandleLightDirection()
    {
        int lightSwitchDir = 0;

        if (Input.GetKeyDown(KeyCode.E))
        {
            lightSwitchDir = 1;

        }
        if (Input.GetKeyDown(KeyCode.Q))
        {
            lightSwitchDir = -1;
        }

        if (lightSwitchDir != 0 && lightCoroutine == null && !inTransition)
        {
            lightCoroutine = StartCoroutine(SwitchLightDirection(lightSwitchDir));
        }
    }

    #endregion

    #region Hareket Durumu Update Fonksiyonlarý

    public void WalkingUpdate()
    {
        // 1. Zemin kontrolü ve Input alýmý
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Zýplama Kontrolü
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }

        if (Input.GetKey(KeyCode.LeftShift) && !StaminaIsFinished && Mathf.Abs(moveInput) > 0.1f)
        {
            SwitchMovementTypes(MovementType.Sprinting);
        }

    }
    public void SprintingUpdate()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
        moveInput = Input.GetAxisRaw("Horizontal");

        // 2. Zýplama Kontrolü
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        }
        if (Input.GetKey(KeyCode.LeftShift) && !StaminaIsFinished && Mathf.Abs(moveInput) > 0.1f)
        {
            IsRunning = true;
            MoveSpeed = runSpeed;
        }
        else
        {
            SwitchMovementTypes(MovementType.Walking);
            IsRunning = false;
            MoveSpeed = walkSpeed;
        }

        // 4. STAMINA YÖNETÝMÝ
        if (IsRunning)
        {
            // Stamina tüketimi
            Stamina -= StaminaDecraseRate * Time.deltaTime;
            if (Stamina <= 0) // 0'ýn altýna düþmesine izin verme
            {
                Stamina = 0;
                StaminaIsFinished = true;
            }
        }
        else
        {
            // Stamina dolumu (Koþmadýðý her an dolmalý)
            if (Stamina < MaxStamina)
            {
                Stamina += StaminaIncraseRate * Time.deltaTime;
            }

            // Stamina belli bir eþiði geçince tekrar koþmaya izin ver (Örn: %20 dolunca)
            if (Stamina >= (MaxStamina * 0.2f))
            {
                StaminaIsFinished = false;
            }
        }

    }
    public void LadderUpdate()
    {
        //Üst ve Aþaðý hareketi algýlar
        moveInput = Input.GetAxisRaw("Vertical");
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, groundLayer);
    }


     //Hareket Durumunu Deðiþtirir , deðiþen duruma göre anlýk deðiþimler yapar.
    public void SwitchMovementTypes(MovementType newType)
    {
        if (newType == currentMoveType) return;

        switch (newType)
        {
            case MovementType.Sprinting:

                IsRunning = true;
                MoveSpeed = runSpeed;

            break;


            case MovementType.Walking:

            break;

            case MovementType.Ladder:



            break;

        }

        currentMoveType = newType;

    }

    #endregion

    public void MovementFixedUpdate()
    {

    }

    public void SprintingFixedUpdate()
    {

    }
    public void LadderFixedUpdate()
    {

    }

    //Zaman içerisinde ýþýðýn saðdaysa sola, soldaysa saða geçmesini saðlar
    public IEnumerator SwitchLightDirection(int direction)
    {
      

        Quaternion ARot = lightTransform.rotation;
        Quaternion Brot;

        if(direction > 0)
        {
            if (isRightSide)
            {
                lightCoroutine = null;
                inTransition = false;
                Debug.Log("Finished Rotation");
                yield break;

            }
             lightElapsedTime = 0;
             inTransition = true;
             Debug.Log(" SCAT MAN ");
     
            Brot = Quaternion.Euler(0, 0, -90);
            isRightSide = true;
        }

        else if (direction < 0)
        {
            if (!isRightSide)
            {
                lightCoroutine = null;
                inTransition = false;
                Debug.Log("Finished Rotation");
                yield break;
            }


            lightElapsedTime = 0;
            inTransition = true;
            Debug.Log(" SCAT MAN ");
            Brot = Quaternion.Euler(0, 0, -270);
            isRightSide = false;
        }
        else
        {
            lightCoroutine = null;
            inTransition = false;
            Debug.Log("Finished Rotation");
            yield break;
        }

        while (lightElapsedTime < lightSwitchTime)
        {
            lightElapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(lightElapsedTime / lightSwitchTime);
            Quaternion lerpedQuaternion = Quaternion.Lerp(ARot,Brot,t);

            lightTransform.rotation = lerpedQuaternion;

            yield return null;
        }

        lightTransform.rotation = Brot;
        lightCoroutine = null;
        inTransition = false;
        Debug.Log("Finished Rotation");
    }

    #region Saðlýk Fonksiyonlarý
    public void TakeDamage(int damageAmount)
    {
        CurrentHealth -= damageAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, -1, MaxHealth);
        print("Canýmý " + CurrentHealth);
        if (CurrentHealth <= 0) Dead();
    }

    public void Heal(int healAmount)
    {
        if (CurrentHealth >= MaxHealth) return;
        CurrentHealth += healAmount;
        CurrentHealth = Mathf.Clamp(CurrentHealth, -1, MaxHealth);
        print("Canýmý " + CurrentHealth);
    }

    public void Dead()
    {
        print("Player is Dead!");
        Destroy(gameObject);
    }


    #endregion
}
