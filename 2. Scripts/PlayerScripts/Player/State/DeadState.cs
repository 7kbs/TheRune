using UnityEngine;

public class DeadState : IPlayerState
{
    public void OnEnterState(PlayerMove playerMove)
    {
        Debug.Log("사망 상태 진입");
        // 플레이어의 물리 속도를 0으로 만듭니다.
        playerMove.rb.linearVelocity = Vector2.zero;
        playerMove.anim.SetBool("run", false);
        playerMove.anim.SetBool("jump", false);
        playerMove.anim.SetTrigger("die"); // "die" 트리거를 재생합니다.
    }

    // 사망 상태에서는 어떤 입력도 받지 않습니다.
    public void UpdateState(PlayerMove playerMove) { }

    // 사망 상태에서는 물리적 움직임을 막습니다.
    public void FixedUpdateState(PlayerMove playerMove) { }

    public void OnExitState(PlayerMove playerMove)
    {
        // 사망 상태에서 벗어날 때의 로직 (예: 부활, 재시작)
    }
}
