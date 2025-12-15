using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldItem : MonoBehaviour
{
    public ItemBase rewardItem;
    public string uniqueId;

    Vector3 MoveDir = Vector3.zero;
    float MagnetRange = 10.0f;
    float MagnetSpeed= 5.0f;
    float targetHeight = 4f; // 목표 높이

    bool canbeget;

    Player player;

    void Start()
    {
        player = FindAnyObjectByType<Player>();

        // 오브젝트를 목표 높이까지 위로 이동
        if (!rewardItem.QuestItem) StartCoroutine(MoveUpAndEnableMagnet());
    }

    void Update()
    {
        Magnet();
    }


    IEnumerator MoveUpAndEnableMagnet()
    {
        float startY = transform.position.y;
        float targetY = startY + targetHeight;

        // 목표 위치까지 이동
        while (transform.position.y < targetY)
        {
            transform.position += new Vector3(0, Time.deltaTime * MagnetSpeed, 0);
            yield return null; // 다음 프레임까지 대기
        }

        canbeget = true; // 목표 위치에 도달했으므로 마그넷 작동 가능
    }

    void Magnet()
    {
        if (rewardItem.QuestItem) return;

        MoveDir = player.transform.position - transform.position;
        MoveDir.z = 0.0f;
        if (MoveDir.magnitude <= MagnetRange && canbeget)
        {
            MoveDir.Normalize();
            transform.position += MoveDir * Time.deltaTime * MagnetSpeed;
        }
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && rewardItem != null)
        {
            //퀘스트 아이템일 경우 유니크ID를 통해 의도되지 않은 현상 예외처리
            if (rewardItem.QuestItem) GameMgr.inst.userData.uniqueItem.Add(uniqueId);

            // 인벤토리에 반영
            ItemManager.inst.GetItem(rewardItem);

            UIManager.inst.GetToast().Init("아이템 습득! ESC키를 눌러 인벤토리를 확인해보세요!", Color.white);

            Destroy(gameObject);
        }
    }
}
