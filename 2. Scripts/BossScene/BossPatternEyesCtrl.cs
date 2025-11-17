using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternEyesCtrl : MonoBehaviour
{
    Vector3 dir;
    float eyeSpeed = 9.9f;   //날아 다니는 속도
    int DirX = 1;     //날아갈 X 방향 값
    int DirY = 1;     //날아갈 Y 방향 값
    // 맵 경계 설정
    float mapMinX = -29f;
    float mapMaxX = 29f;
    float mapMinY = -4.5f;
    float mapMaxY = 15.5f;
    // 맵 경계 설정

    float activeTime = 0f;  // 오브젝트가 활성화된 실제 시간
    float maxLifetime = 20f;  // 최대 생존 시간

    void Start()
    {
        dir = new Vector3(DirX, DirY, 0.0f);
    }

    void Update()
    {
        if (PlayerMove.inst.IsInteractionState)
        {
            eyeSpeed = 0;
        }
        else
        {
            eyeSpeed = 9.9f;
            activeTime += Time.deltaTime;  // 실제 활성 시간 증가
        }

        // 활성 시간이 최대 생존 시간을 초과하면 오브젝트 삭제
        if (activeTime >= maxLifetime)
        {
            Destroy(gameObject);
            return;
        }

        // 물체의 위치를 확인하고 방향을 반전
        if (transform.position.x < mapMinX + 0.5f || transform.position.x > mapMaxX - 0.5f)
        {
            DirX = -DirX;  // X 방향 반전
        }

        if (transform.position.y < mapMinY + 0.5f || transform.position.y > mapMaxY - 0.5f)
        {
            DirY = -DirY;  // Y 방향 반전
        }

        // 방향 벡터 업데이트
        dir = new Vector3(DirX, DirY, 0.0f);
        dir.Normalize();

        // 물체의 위치 업데이트 (이 부분은 다른 메서드로 분리하는 것이 좋습니다.)
        transform.position += (dir * Time.deltaTime * eyeSpeed);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>())
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(20);
            Debug.Log("플레이어가 파멸의 눈에 피격됨");
        }
    }
}