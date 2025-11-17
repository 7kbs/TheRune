using UnityEngine;

// 플레이어 상태의 기본 인터페이스
public interface IPlayerState
{
    // 상태에 진입할 때 한 번 호출됩니다.
    void OnEnterState(PlayerMove playerMove);

    // Update()에서 매 프레임 호출됩니다. 입력 처리에 사용됩니다.
    void UpdateState(PlayerMove playerMove);

    // FixedUpdate()에서 고정된 프레임마다 호출됩니다. 물리 이동 처리에 사용됩니다.
    void FixedUpdateState(PlayerMove playerMove);

    // 상태에서 벗어날 때 호출됩니다.
    void OnExitState(PlayerMove playerMove);
}