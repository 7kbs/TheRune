using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AcornBomb : MonoBehaviour
{
    Player player;

    public SkillData data;
    public GameObject ExplotionEffect;

    public float blinkDuration; // 깜빡임 전체 시간
    public float blinkInterval; // 처음 깜빡이는 간격
    public Color explosionColor = Color.red; // 폭발 색상
    private SpriteRenderer spriteRenderer;
    private Color originalColor;

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalColor = spriteRenderer.color;

        StartCoroutine(BlinkAndExplode());
        Invoke("Explode", 2.0f);
    }

    IEnumerator BlinkAndExplode()
    {
        float timer = 0f;
        float currentInterval = blinkInterval;

        while (timer < blinkDuration)
        {
            // 알파값 조정
            float alpha = Mathf.PingPong(timer * 4, 1); // 깜빡이는 효과 (4는 주기 조정)
            spriteRenderer.color = new Color(explosionColor.r, explosionColor.g, explosionColor.b, alpha);

            timer += Time.deltaTime;

            // 간격 조정 (시간이 지남에 따라 주기를 짧게)
            if (timer > currentInterval)
            {
                currentInterval *= 0.9f; // 10% 줄임
                timer = 0f;
            }
            yield return null; // 다음 프레임까지 대기
        }

        // 폭발 효과 후 알파값 완전히 0으로
        spriteRenderer.color = new Color(originalColor.r, originalColor.g, originalColor.b, 0);

        yield break; // 코루틴 종료
    }

    void Explode()
    {
        GameObject explosion = Instantiate(ExplotionEffect, transform.position, Quaternion.identity);
        explosion.GetComponent<Explosion>().Init(player, data.damage);
        Destroy(gameObject);
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.name == "Vine")
        {
            DestroyVine(collision.gameObject);
        }
    }

    void DestroyVine(GameObject Obj)
    {
        GameMgr.inst.userData.VineDestroy = true;
        Obj.GetComponentInChildren<ParticleSystem>().Play();
        Destroy(Obj, 1.5f);
    }
}