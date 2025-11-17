using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBulletCtrl : MonoBehaviour
{
    Vector3 AttackPos;
    GameObject Player;
    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player");
        AttackPos = Player.transform.position - transform.position;
        transform.up = AttackPos;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += AttackPos * Time.deltaTime * 3.0f;

        Destroy(gameObject, 3.0f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.name == "Player")
        {            
            Player p_Ctrl = collision.gameObject.GetComponent<Player>();
            p_Ctrl.TakeDamage(20);
            Destroy(this.gameObject);
        }
    }
}
