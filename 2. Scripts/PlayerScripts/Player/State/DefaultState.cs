using UnityEngine;

public class DefaultState : IPlayerState
{
    private float dashBufferTime = 0.12f;
    private float dashBufferCounter = 0f;

    public void OnEnterState(PlayerMove playerMove)
    {
        Debug.Log("DefaultState Enter");
        playerMove.anim.SetBool("die", false);
        playerMove.rb.bodyType = RigidbodyType2D.Dynamic;
        dashBufferCounter = 0f;
    }

    public void UpdateState(PlayerMove playerMove)
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        playerMove.movement = new Vector3(inputX, 0f, 0f);
        playerMove.isWalk = Mathf.Abs(inputX) > 0.01f;

        if (inputX < -0.01f) playerMove.transform.localScale = new Vector3(1f, 1f, 1f);
        else if (inputX > 0.01f) playerMove.transform.localScale = new Vector3(-1f, 1f, 1f);


        if (Input.GetKeyDown(KeyCode.LeftShift))
            dashBufferCounter = dashBufferTime;
        else
            dashBufferCounter -= Time.deltaTime;

        if (dashBufferCounter > 0f &&
            !playerMove.isDashing &&                                
            Mathf.Abs(inputX) > 0.01f &&                            
            Time.time >= playerMove.lastDashTime + playerMove.dashCooldown) 
        {
            playerMove.StartDash();
            dashBufferCounter = 0f;
        }

        bool grounded = playerMove.IsGrounded();

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
            playerMove.anim.SetFloat("speed", 0f);
        }
        else
        {
            playerMove.anim.SetBool("jump", false);
            playerMove.anim.SetBool("descent", false);
            playerMove.anim.SetFloat("speed", Mathf.Abs(inputX));
        }
    }

    public void FixedUpdateState(PlayerMove playerMove)
    {

    }

    public void OnExitState(PlayerMove playerMove)
    {
        playerMove.anim.SetBool("jump", false);
        playerMove.anim.SetBool("descent", false);
        playerMove.anim.SetFloat("speed", 0f);
        dashBufferCounter = 0f;
    }
}