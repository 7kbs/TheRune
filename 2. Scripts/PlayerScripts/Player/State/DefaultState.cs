using UnityEngine;

public class DefaultState : IPlayerState
{
    // ��� �Է� ����(�ɼ�, ���ο����� ���)
    private float dashBufferTime = 0.12f;
    private float dashBufferCounter = 0f;

    public void OnEnterState(PlayerMove playerMove)
    {
        Debug.Log("DefaultState Enter");
        // ���� ��ȯ �� ���� �ʱ�ȭ �ʿ��ϸ� ���⼭ (���� PlayerMove�� �����ϹǷ� �ּ�ȭ)
        dashBufferCounter = 0f;
    }

    public void UpdateState(PlayerMove playerMove)
    {
        // --- �Է� ���� (PlayerMove�� ����) ---
        float inputX = Input.GetAxisRaw("Horizontal");
        playerMove.movement = new Vector3(inputX, 0f, 0f);
        playerMove.isWalk = Mathf.Abs(inputX) > 0.01f;

        // --- ���� ���� (���) ---
        if (inputX < -0.01f) playerMove.transform.localScale = new Vector3(1f, 1f, 1f);
        else if (inputX > 0.01f) playerMove.transform.localScale = new Vector3(-1f, 1f, 1f);

        // --- ���� �Է��� PlayerMove.Update()���� ����/�ڿ��׷� ó���� ---
        // (���� ���⼭�� ���� ���� ȣ������ ����)

        // --- ��� �Է� (���۸�) ---
        if (Input.GetKeyDown(KeyCode.LeftShift))
            dashBufferCounter = dashBufferTime;
        else
            dashBufferCounter -= Time.deltaTime;

        if (dashBufferCounter > 0f &&
            !playerMove.isDashing &&                                // ���� ��� ���� �ƴϰ�
            Mathf.Abs(inputX) > 0.01f &&                            // �̵� �Է�(��ô� �̵� �߸� ����ϴ� ���� ����)
            Time.time >= playerMove.lastDashTime + playerMove.dashCooldown) // ��Ÿ�� üũ
        {
            playerMove.StartDash();
            dashBufferCounter = 0f;
        }

        // --- �ִϸ��̼� ó�� (PlayerMove�� IsGrounded()�� rb.velocity ���) ---
        bool grounded = playerMove.IsGrounded();

        // �����̸� jump / descent��, speed�� 0���� ����
        if (!grounded)
        {
            float vy = playerMove.rb.linearVelocity.y;

            if (vy > 0.05f)
            {
                playerMove.anim.SetBool("jump", true);
                playerMove.anim.SetBool("descent", false);
            }
            else if (vy < -0.05f)
            {
                playerMove.anim.SetBool("jump", false);
                playerMove.anim.SetBool("descent", true);
            }
            // ���߿����� speed �ִϸ��̼� ��Ȱ��ȭ
            playerMove.anim.SetFloat("speed", 0f);
        }
        else
        {
            // ���� ���� jump/descent �����ϰ� speed ����
            playerMove.anim.SetBool("jump", false);
            playerMove.anim.SetBool("descent", false);
            playerMove.anim.SetFloat("speed", Mathf.Abs(inputX));
        }
    }

    public void FixedUpdateState(PlayerMove playerMove)
    {
        // ���� ������Ʈ�� PlayerMove.FixedUpdate�� ����(HandleMove, gravity ��)
        // ������ FixedUpdate������ Ư���� ���� ����.
        // ��, ���� ���¿��� ���� ����(��: ���º� ������ ����)�� �ʿ��ϸ� ���⼭ ó��.
    }

    public void OnExitState(PlayerMove playerMove)
    {
        // ���� ���� �� �ִ� �ʱ�ȭ
        playerMove.anim.SetBool("jump", false);
        playerMove.anim.SetBool("descent", false);
        playerMove.anim.SetFloat("speed", 0f);
        dashBufferCounter = 0f;
    }
}