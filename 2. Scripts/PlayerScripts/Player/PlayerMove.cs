using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 8f;
    public Vector2 movement;
    public bool isWalk;

    [Header("Jump Settings")]
    public float jumpForce = 13f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public float coyoteTime = 0.1f;
    public float jumpBufferTime = 0.1f;
    public int maxJumpCount = 2;

    [Header("Dash Settings")]
    public float dashForce = 18f;
    public float dashDuration = 0.2f;
    public float dashCooldown = 0.6f;
    [HideInInspector] public bool isDashing;
    [HideInInspector] public float lastDashTime;

    [Header("Check Ground")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundRadius = 0.2f;

    [Header("Components")]
    public Rigidbody2D rb;
    public Animator anim;

    // ���� ���� �����
    IPlayerState currentState;
    float coyoteCounter;
    float jumpBufferCounter;
    bool isFacingRight = true;

    public System.Action OnLand; // ���� �̺�Ʈ

    [HideInInspector] public bool isJump;
    [HideInInspector] public int jumpCount = 0;
    bool wasGrounded;

    [HideInInspector] public bool isPlayerControllable = true;
    public bool IsDefaultState => currentState is DefaultState;
    public bool IsInteractionState => currentState is InteractingState;
    public bool IsDeadState => currentState is DeadState;

    public static PlayerMove inst; 

    private void Awake()
    {
        inst = this;
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        ChangeState(new DefaultState());
    }

    private void Update()
    {
        if (isDashing) return; // ��� �߿� �Է� ����
        currentState?.UpdateState(this);

        HandleJumpLogic();
    }

    private void FixedUpdate()
    {
        bool grounded = IsGrounded();

        // ���� ����
        if (!wasGrounded && grounded)
        {
            OnLand?.Invoke(); // Player.cs���� ��� ��
            isJump = false;
            jumpCount = 0;
        }

        wasGrounded = grounded;

        if (isDashing) return;
        currentState?.FixedUpdateState(this);
        HandleMove();
        ApplyGravityCurve();
    }

    public void ChangeState(IPlayerState newState)
    {
        currentState?.OnExitState(this);
        currentState = newState;
        currentState.OnEnterState(this);
    }

    // === ���� ���� ===
    void HandleJumpLogic()
    {
        bool grounded = IsGrounded();

        if (grounded)
            coyoteCounter = coyoteTime;
        else
            coyoteCounter -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferCounter = jumpBufferTime;
        else
            jumpBufferCounter -= Time.deltaTime;

        // ���� ���� ����
        if (jumpBufferCounter > 0 && (coyoteCounter > 0 || jumpCount < maxJumpCount))
        {
            Jump();
            jumpBufferCounter = 0;
        }

        // ���� ���� Ű �� ��� ������
        if (Input.GetKeyUp(KeyCode.Space) && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f);
        }
    }

    void Jump()
    {
        rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
        isJump = true;
        jumpCount++;
    }

    // === �̵� ===
    void HandleMove()
    {
        rb.linearVelocity = new Vector2(movement.x * moveSpeed, rb.linearVelocity.y);
    }

    // === ���� ���� ===
    void ApplyGravityCurve()
    {
        if (rb.linearVelocity.y < 0)
        {
            // �ϰ� �� ������
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.linearVelocity.y > 0 && !Input.GetKey(KeyCode.Space))
        {
            // ��� �� �����̽� ���� ���� ����
            rb.linearVelocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    // === ��� ===
    public void StartDash()
    {
        if (Time.time < lastDashTime + dashCooldown || isDashing) return;

        lastDashTime = Time.time;
        StartCoroutine(DashRoutine());
    }

    private System.Collections.IEnumerator DashRoutine()
    {
        isDashing = true;
        float startTime = Time.time;

        float dashDir = transform.localScale.x > 0 ? -1 : 1;
        rb.linearVelocity = new Vector2(dashDir * dashForce, 0f);

        // ��� �� �߷� ����
        rb.gravityScale = 0f;
        anim.SetTrigger("dash");

        while (Time.time < startTime + dashDuration)
            yield return null;

        rb.gravityScale = 3f; // �⺻�� ���� (������Ʈ �⺻�� �°� ����)
        isDashing = false;
    }

    public bool IsGrounded()
    {
        return Physics2D.OverlapCircle(groundCheck.position, groundRadius, groundLayer);
    }

    private void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
}
