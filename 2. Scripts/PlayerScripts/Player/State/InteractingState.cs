using UnityEngine;

public class InteractingState : IPlayerState
{
    public void OnEnterState(PlayerMove playerMove)
    {
        Debug.Log("상호작용 상태 진입");
        // 플레이어 움직임 및 애니메이션을 멈춥니다.
        playerMove.anim.SetBool("run", false);
        playerMove.rb.bodyType = RigidbodyType2D.Static;
    }

    // 이 상태에서는 아무것도 하지 않습니다.
    public void UpdateState(PlayerMove playerMove) { }

    // 이 상태에서는 아무것도 하지 않습니다.
    public void FixedUpdateState(PlayerMove playerMove) { }

    // 상태 종료 시 특별한 정리 작업은 없습니다.
    public void OnExitState(PlayerMove playerMove)
    {
        playerMove.anim.SetBool("run", false);
    }
}