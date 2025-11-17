using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternDarkFallingCtrl : MonoBehaviour
{
    public float fallSpeed = 20f;
    float activeFallSpeed;
    float lifetime = 3f;    //실제 생존시간
    float activeTime = 0f;  //실제 활성시간

    void Update()
    {
        if (PlayerMove.inst.IsInteractionState)
        {
            activeFallSpeed = 0f;   //속도를 0으로 설정해 정지
            return; //Update문 Return
        }

        //활성 상태 낙하 및 타이머
        activeFallSpeed = fallSpeed;
        transform.position += Vector3.down * activeFallSpeed * Time.deltaTime;

        activeTime += Time.deltaTime; //활성시간 누적
        if (activeTime >= lifetime)     //실제활성 시간이 생존 시간 초과 시 삭제
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if (player != null)
                player.TakeDamage(20);
            Debug.Log("플레이어가 다크폴링에 피격됨");
        }
    }
}
