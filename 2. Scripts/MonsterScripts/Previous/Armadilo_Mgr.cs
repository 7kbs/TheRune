using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Armadilo_Mgr : MonoBehaviour
{
    GameObject playerTr;

    float Att_Speed = 10.0f;
    Vector2 Att_Dir = Vector3.up;
    public GameObject[] Objects = null;
    public Rigidbody2D[] rigid;    

    bool isAttack = false;
    // Start is called before the first frame update
    void Start()
    {
        isAttack = true;
    }

    private void Update()
    {
        if (isAttack)
        {
            for (int i = 0; i < Objects.Length; i++)
            {
                Att_Dir = Objects[i].transform.up;
                rigid[i].AddForce(Att_Dir * Att_Speed, ForceMode2D.Impulse);
            }
            isAttack = false;
        }

        Destroy(gameObject, 2.0f);  
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {
            Debug.Log("플레이어 때리기");
            collision.gameObject.GetComponent<Player>().TakeDamage(10);
        }
    }
}
