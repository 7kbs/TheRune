using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fairy : MonoBehaviour
{
    Player player;

    public SkillData data;
    public GameObject FairyAttackObj;

    GameObject playerTr;
    Vector3 Pos = Vector3.zero;
    Queue<GameObject> AttackRange = new Queue<GameObject>();
    
    float AttackTimer = 0.0f;


    void Start()
    {
        player = FindAnyObjectByType<Player>();

        Destroy(gameObject, data.duration);
        playerTr = GameObject.FindGameObjectWithTag("Player");
    }


    void Update()
    {
        if(AttackRange.Count > 0)
        {
            AttackTimer -= Time.deltaTime;
            if(AttackTimer <= 0.0f)
            {
                GameObject obj = Instantiate(FairyAttackObj, transform);
                obj.transform.position = transform.position;
                FairyAttack fa = obj.GetComponent<FairyAttack>();
                fa.monsterTr = AttackRange.Peek();
                AttackTimer = 1.0f;
            }
        }

        Pos = new Vector3(playerTr.transform.position.x, playerTr.transform.position.y + 3.5f, 0.0f);

        transform.position = Pos;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Monster" || collision.gameObject.name == "Boss")
        {
            AttackRange.Enqueue(collision.gameObject);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Monster" || collision.gameObject.name == "Boss")
        {
            AttackRange.Dequeue();
        }
    }
}
