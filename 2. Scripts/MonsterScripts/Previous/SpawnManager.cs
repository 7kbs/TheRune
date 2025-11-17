using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public GameObject[] Monster;
    public Transform[] SpawnPoint; 
    public float spawnInterval = 6.0f; // 몬스터 스폰 간격

    void Start()
    {
        InvokeRepeating("MonsterSpawn", 0, spawnInterval); // 주기적으로 몬스터 스폰
    }

    
    void Update()
    {
        MonsterSpawn();
    }

    void MonsterSpawn()
    {
        // 현재 활성화된 몬스터 수 체크
        if (GameObject.FindGameObjectsWithTag("Monster").Length >= SpawnPoint.Length)
            return;

        for (int i = 0; i < SpawnPoint.Length; i++)
        {
            // SpawnPoint 배열의 각 인덱스에 자식이 없는 경우에만 몬스터를 생성

            //지하 몬스터
            if (i == 2 || i == 3 || i == 4 || i == 10 || i == 15)
            {
                if (SpawnPoint[i].childCount == 0)
                {
                    int RandomMonsterIndex = Random.Range(3, Monster.Length);
                    GameObject mon = Instantiate(Monster[RandomMonsterIndex], SpawnPoint[i]);
                    mon.tag = "Monster"; // 몬스터 태그 설정
                }
            }
            //지상 몬스터
            else
            {
                if (SpawnPoint[i].childCount == 0)
                {
                    int RandomMonsterIndex = Random.Range(0, 3); //수정예정
                    GameObject mon = Instantiate(Monster[RandomMonsterIndex], SpawnPoint[i]);
                    mon.tag = "Monster"; // 몬스터 태그 설정
                }
            }
        }
    }
}
