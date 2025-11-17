using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossPatternChainCtrl : MonoBehaviour
{
    private BoxCollider2D boxCollider;

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        boxCollider.enabled = false;  // 처음에는 박스 콜라이더 비활성화
        StartCoroutine(ActivateCollider());
    }

    IEnumerator ActivateCollider()
    {
        yield return new WaitForSeconds(0.4f);  // 0.4초 대기
        boxCollider.enabled = true;  // 박스 콜라이더 활성화

        yield return new WaitForSeconds(0.55f);  // 애니메이션 재생 시간 대기
        Destroy(gameObject);  // 체인 삭제
    }


    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.name.Contains("Player"))
        {
            Player player = collision.gameObject.GetComponent<Player>();
            if(player != null)
                player.TakeDamage(20);
            Debug.Log("플레이어가 사슬에 피격됨");
        }
    }
}