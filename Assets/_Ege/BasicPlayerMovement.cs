using UnityEngine;

public class BasicPlayerMovement : MonoBehaviour, IHaveHealth
{
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

    [Header("Zemin Kontrolü")]
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask groundLayer;

    [Header("Can")]
    public int WantedMaxHealth;
    public int CurrentHealth { get; set; }
    public int MaxHealth { get; set; }

    private Rigidbody2D rb;
    private bool isGrounded;
    private float moveInput;
    

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        // Havada dönmeyi engellemek için rotasyonu kilitler
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        MoveSpeed = walkSpeed;
        Stamina = MaxStamina;
        MaxHealth = WantedMaxHealth;
        CurrentHealth = MaxHealth;
    }

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

        if(Input.GetKeyDown(KeyCode.Z)) TakeDamage(10);
        if(Input.GetKeyDown(KeyCode.X)) Heal(10);
    }

    void FixedUpdate()
    {
        // Hareket uygulama
        rb.linearVelocity = new Vector2(moveInput * MoveSpeed, rb.linearVelocity.y);

        // Karakterin yönünü çevirme (Flip)
        if (moveInput > 0) transform.localScale = new Vector3(1, 1, 1);
        else if (moveInput < 0) transform.localScale = new Vector3(-1, 1, 1);
    }

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
}
