using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum MonsterType
{
    Frog,
    Crab,
    Onion,
    Armadillo,
    Bug
}

public class MonsterCtrl : MonoBehaviour
{
    public MonsterType monType;

    Player player;  //플레이어 추적용 변수
    Rigidbody2D rigid;
    Animator anim;

    int nextMoveDirection;
    float moveSpeed = 3.0f;
    float visionRange = 14.0f;
    float attackRange = 5.5f;
    float traceRange = 20.0f;
    float PatternDelay = 2.5f;
    bool isRun;
    bool isAttacking = false;  // 공격 중인지 체크하는 변수
    bool isDeactivating = false;  // 비활성화 중인지 체크하는 변수
    public float patrolRange = 5.0f; // 패트롤 범위
    private float patrolTime; // 패트롤 시간
    private Vector3 initialPosition; // 초기 스폰 위치
    bool isInvincible;
    bool isTrace;

    //--- 패트롤에 필요한 변수
    Vector3 basePosition = Vector3.zero;   // 몬스터의 초기 스폰 위치(기준점이 된다.)
    private bool isPatrolling = false; // 패트롤 상태
    public LayerMask groundLayer; // 땅(플랫폼) 레이어 설정

    float waitTime = 0.0f;                  // Patrol 시 목표점에 도착하면 잠시 대기시키기 위한 랜덤 시간 변수
    Vector3 patrolTarget = Vector3.zero;    // Patrol 시 움직여야 될 다음 목표 좌표
    Vector3 patrolDirection = Vector3.zero;  // Patrol 시 움직여야 될 방향 벡터
    double patrolTimeCounter = 0.0f;         // 이동 총 누적시간 카운트용 변수
    //--- 패트롤에 필요한 변수

    public float maxHP;
    public float curHp;
    public float MonAttackPower;
    public Image HpBar;
    public Image DelayHpbar;


    public GameObject BugSkillObj;  //버그몬스터 스킬오브젝트
    public GameObject ArmadilloObj; //아르마딜로몬스터 스킬 오브젝트
    public Transform Att_Pos;

    public GameObject HitParticle;

    //아이템 드롭
    public GameObject[] Rewards;
    float[] dropProbabilities = { 0.4f, 0.2f, 0.2f, 0.1f, 0.1f }; // 각 아이템의 드롭 확률

    
    void OnEnable()
    {
        curHp = maxHP;
        HpBar.fillAmount = curHp / maxHP;
        isInvincible = false;
        isDeactivating = false;  // 활성화될 때 비활성화 중이 아님을 표시

        if (GlobalValue.sceneType != GlobalValue.SceneType.Boss)
            initialPosition = transform.parent.position;
    }

    void OnDisable()
    {
        if (GlobalValue.sceneType != GlobalValue.SceneType.Boss)
        {
            transform.position = transform.parent.position;
            Invoke("ReSpawn", 5.0f);
        }
    }

    void Start()
    {
        player = FindAnyObjectByType<Player>();
        rigid = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();

        switch (monType)
        {
            case MonsterType.Onion:
                MonAttackPower = 15;
                break;

            case MonsterType.Crab:
                MonAttackPower = 20;
                break;

            case MonsterType.Frog:
                MonAttackPower = 25;
                break;

            case MonsterType.Armadillo:
                MonAttackPower = 25;
                break;

            case MonsterType.Bug:
                MonAttackPower = 30;
                break;
        }

        HpBar.fillAmount = curHp / maxHP;
        DelayHpbar.fillAmount = curHp / maxHP;
    }

    void Update()
    {
        if (GlobalValue.sceneType != GlobalValue.SceneType.Boss)
        {
            if (!isDeactivating && Vector3.Distance(transform.parent.position, transform.position) > traceRange
                && !isTrace)
            {
                DeSpawn();  // 너무 멀리 떨어지면 비활성화
            }
        }

        float targetFill = curHp / maxHP;
        HpBar.fillAmount = targetFill;

        if (DelayHpbar.fillAmount > HpBar.fillAmount)
        {
            DelayHpbar.fillAmount = Mathf.Lerp(DelayHpbar.fillAmount, HpBar.fillAmount, Time.deltaTime * 1.5f);
        }
        else
        {
            DelayHpbar.fillAmount = HpBar.fillAmount; // 회복 시에는 메인 체력바에 즉시 맞춰줌
        }
    }

    void FixedUpdate()
    {
        MonsterAI();
    }

    void MonsterAI()
    {
        if (PlayerMove.inst.IsInteractionState) return;

        if (curHp <= 0)
        {
            if (GlobalValue.sceneType == GlobalValue.SceneType.Boss)
            {
                Destroy(gameObject);
            }
            else
            {
                RewardSpawn();
                DeSpawn();
            }

        }

        // 공격 중일 때는 방향 전환을 막기 위해 early return
        if (isAttacking)
            return;

        // 타겟과 보스 사이의 거리 확인
        float distance = Vector3.Distance(player.transform.position, transform.position);

        // 시야 범위 안에 들어올 때
        if (distance <= visionRange && !player.isStealth)
        {
            //Debug.Log("나는 타겟을 바라보는 상태");

            if (monType == MonsterType.Onion)
            {
                Patrol();
            }

            // 공격 사거리 안에 들어올 시 공격
            if (distance <= attackRange)
            {
                if (monType == MonsterType.Onion)
                    return;

                //Debug.Log("나는 공격 상태");
                isRun = false;
                anim.SetBool("Walk", false);
                PatternDelay -= Time.deltaTime;
                Attack();  // 공격 실행
            }
            else
            {
                if (monType == MonsterType.Onion)
                    return;

                // 공격 애니메이션 실행 중이 아닐 때 추적
                //Debug.Log("나는 추적 상태");
                Trace();
            }
        }
        else
        {
            //사망시, n초 뒤 스폰포인트에 부활.
            //사망하지 않았을 시, 액티브 켜기
            Patrol();
        }
    }
    void RewardSpawn()
    {
        float randomValue = Random.Range(0f, 1f); // 0.0에서 1.0 사이의 랜덤 값 생성
        float cumulativeProbability = 0f; // 누적 확률 초기화

        for (int i = 0; i < Rewards.Length; i++)
        {
            cumulativeProbability += dropProbabilities[i]; // 현재 확률 추가

            if (randomValue < cumulativeProbability) // 랜덤 값이 누적 확률보다 작으면
            {

                GameObject reward = Instantiate(Rewards[i]); // 해당 아이템 생성
                reward.transform.position = new Vector3(
                    Random.Range(transform.position.x + 1.0f, transform.position.x - 1.0f)
                    , transform.position.y, transform.position.z);
                //transform.position; // 아이템 위치 설정
                reward = Instantiate(Rewards[Random.Range(0, 3)]); // 추가 아이템 생성
                reward.transform.position = new Vector3(
                    Random.Range(transform.position.x + 1.0f, transform.position.x - 1.0f)
                    , transform.position.y, transform.position.z);
               
                // 선택된 아이템과 그에 해당하는 확률 로그 출력
                //Debug.Log($"아이템: {Rewards[i].name}"); //확률: {dropProbabilities[i] * 100}%");
                break; // 아이템 생성 후 반복문 종료
            }
        }
    }

    void Patrol()
    {
        isTrace = false;
        float dist = Vector3.Distance(transform.position, player.transform.position);

        if (dist > 35f)
        {
            anim.SetBool("Walk", false);
            return;
        }

        // 부모 기준 위치 설정
        float patrolLimitLeft = basePosition.x - 15f;
        float patrolLimitRight = basePosition.x + 15f;

        // Raycast를 사용하여 낭떠러지 감지
        Vector3 rayOrigin = transform.position + new Vector3(patrolDirection.x * 0.5f, 0, 0); // 몬스터의 앞쪽으로 Raycast를 쏨
        RaycastHit2D groundInfo = Physics2D.Raycast(rayOrigin, Vector2.down, 1f, groundLayer); // 발 아래로 레이저를 쏘아 땅이 있는지 확인

        if (isPatrolling)
        {
            // 몬스터 이동
            transform.Translate((patrolDirection * Time.deltaTime * moveSpeed), Space.World);

            // 애니메이션 상태 설정
            anim.SetBool("Walk", true); // 이동 중에는 걷기 애니메이션 켜기

            Vector3 currentScale = transform.localScale;

            // 이동 방향에 따라 로컬 스케일 변경
            if (patrolDirection.x < 0) // 왼쪽으로 이동
            {
                HpBar.transform.localScale = new Vector3(1, 1, 1);
                DelayHpbar.transform.localScale = new Vector3(1, 1, 1);
                transform.localScale = new Vector3(Mathf.Abs(currentScale.x), currentScale.y, currentScale.z); // x를 양수로 유지
            }
            else if (patrolDirection.x > 0) // 오른쪽으로 이동
            {
                HpBar.transform.localScale = new Vector3(-1, 1, 1);
                DelayHpbar.transform.localScale = new Vector3(-1, 1, 1);
                transform.localScale = new Vector3(-Mathf.Abs(currentScale.x), currentScale.y, currentScale.z); // x를 음수로 변경
            }

            
            // 패트롤 시간 카운터
            patrolTimeCounter += Time.deltaTime;

            // 2초 동안 이동하면 멈춤
            if (patrolTimeCounter >= 2.0f || groundInfo.collider == null) // 땅이 없으면 방향 전환
            {
                isPatrolling = false; // 이동 중지
                anim.SetBool("Walk", false); // 멈추면 걷기 애니메이션 끄기
                waitTime = 2.0f; // 2초 대기 시간 설정
            }
        }
        else
        {
            // 애니메이션 상태 설정
            anim.SetBool("Walk", false); // 멈추면 걷기 애니메이션 끄기

            // 대기 시간 처리
            if (waitTime > 0.0f)
            {
                waitTime -= Time.deltaTime; // 대기 시간 감소
                return; // 대기 중이면 아무것도 하지 않음
            }

            // 방향 결정 및 이동 방향 설정
            patrolDirection = new Vector3(Random.Range(-1f, 1f), 0, 0); // x축 방향 랜덤 설정
            patrolDirection.Normalize();

            // 현재 위치가 범위 내인지 확인하고 범위를 초과하면 반대 방향으로 전환
            if (transform.position.x < patrolLimitLeft || transform.position.x > patrolLimitRight)
            {
                patrolDirection.x = -patrolDirection.x; // 반대 방향으로 전환
            }

            patrolTimeCounter = 0.0f; // 이동 시간 초기화
            isPatrolling = true; // 이동 시작
        }
    }

    void Trace()
    {
        if (isInvincible)
            return;

        isTrace = true;
        isRun = true;
        anim.SetBool("Walk", true);

        // 플레이어와 보스의 X 위치 차이를 계산하여 방향 벡터 생성
        Vector3 direction = player.transform.position - transform.position;
        direction = new Vector3(direction.x, direction.y, 0).normalized;

        // 정규화된 방향 벡터에 이동 속도를 곱해서 일정 속도로 이동
        transform.Translate(direction * moveSpeed * Time.deltaTime);

        // 방향 전환 (공격 중이 아닐 때만 방향 전환 가능)
        Vector3 localScale = transform.localScale;
        if (direction.x > 0 && direction.y < 1.0f)
        {
            localScale.x = Mathf.Abs(localScale.x) * -1;  // 오른쪽을 바라보도록
            HpBar.transform.localScale = new Vector3(-1, 1, 1);
            DelayHpbar.transform.localScale = new Vector3(-1, 1, 1);
        }
        else
        {
            localScale.x = Mathf.Abs(localScale.x);       // 왼쪽을 바라보도록
            HpBar.transform.localScale = new Vector3(1, 1, 1);
            DelayHpbar.transform.localScale = new Vector3(1, 1, 1);
        }
        transform.localScale = localScale;  // 크기 유지하면서 방향 전환
    }

    void Attack()
    {
        // 공격 전 타겟을 바라보도록 처리
        Vector3 direction = player.transform.position - transform.position;
        direction = new Vector3(direction.x, direction.y, 0).normalized;

        // 공격 중일 때 방향을 고정하고, 타겟을 바라보는 방향 설정
        if (!isAttacking)
        {
            Vector3 localScale = transform.localScale;

            if (direction.x > 0 && direction.y < 1.0f)
            {
                localScale.x = Mathf.Abs(localScale.x) * -1;  // 오른쪽을 바라보도록
                HpBar.transform.localScale = new Vector3(-1, 1, 1);
            }
            else
            {
                localScale.x = Mathf.Abs(localScale.x);       // 왼쪽을 바라보도록
                HpBar.transform.localScale = new Vector3(1, 1, 1);
            }

            transform.localScale = localScale;  // 크기 유지하면서 방향 전환

        }

        if (PatternDelay <= 0)
        {
            // 공격 상태로 전환
            isAttacking = true;
            int RandomPattern = 1; //Random.Range(1, 4);

            switch (RandomPattern)
            {
                case 1:
                    //Debug.Log("1번 패턴 사용");
                    anim.SetTrigger("Attack");

                    if(monType == MonsterType.Bug)
                        Invoke(nameof(BugMonsterAttackSkill), 1.5f);

                    if (monType == MonsterType.Armadillo)
                    {
                        ArmadilloObj.transform.position = Att_Pos.position;                        
                        Instantiate(ArmadilloObj);
                    }
                    break;
                //case 2:
                //    //Debug.Log("2번 패턴 사용");
                //    anim.SetTrigger("Attack");

                //    if (monType == MonsterType.Bug)
                //        Invoke(nameof(BugMonsterAttackSkill), 1.5f);

                //    if (monType == MonsterType.Armadillo)
                //    {
                //        ArmadilloObj.transform.position = Att_Pos.position;                       
                //        Instantiate(ArmadilloObj);
                //    }

                //    break;
                //case 3:
                //    //Debug.Log("3번 패턴 사용");
                //    anim.SetTrigger("Attack");

                //    if (monType == MonsterType.Bug)
                //        Invoke(nameof(BugMonsterAttackSkill), 1.5f);

                //    if (monType == MonsterType.Armadillo)
                //    {
                //        ArmadilloObj.transform.position = Att_Pos.position;                        
                //        Instantiate(ArmadilloObj);
                //    }

                //    break;
            }

            PatternDelay = 2.5f;  // 공격 후 딜레이 초기화

            // 일정 시간 후에 공격 상태 해제
            Invoke("EndAttack", 1.0f);  // 공격 애니메이션 시간만큼 딜레이를 줌
        }
    }
    void BugMonsterAttackSkill()
    {
        BugSkillObj.SetActive(true);
        Invoke(nameof(BugMonsterSkillOff), 1.5f);
    }
    void BugMonsterSkillOff()
    {
        BugSkillObj.SetActive(false);
    }

    void EndAttack()
    {
        isAttacking = false;  // 공격 종료 후 방향 전환 가능하게 함
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag.Contains("StoneThorn"))
        {
            RewardSpawn();
            Destroy(gameObject, 1.0f);
        }
    }

    public void TakeDamage(GameObject obj, float Damage)
    {
        if (!isInvincible)
        {
            HitParticle.gameObject.SetActive(true);
            Invoke(nameof(HitParticleOff), 0.5f);

            GameMgr.inst.DamageTextSpawn(-Damage, transform.position, Color.red);
            curHp -= Damage;

            Vector2 PushDir = obj.transform.position - transform.position;
            PushDir.Normalize();
            rigid.AddForce(-PushDir * 50.0f, ForceMode2D.Impulse);

            // 무적 상태 시작
            StartCoroutine(InvincibilityCoroutine());            
        }
    }

    void HitParticleOff()
    {
        HitParticle.gameObject.SetActive(false);
    }

    void DeSpawn()
    {
        isDeactivating = true;  // 비활성화 중임을 표시
        gameObject.SetActive(false);  // 오브젝트 비활성화
    }

    void ReSpawn()
    {
        isDeactivating = false;  // 다시 활성화될 때 비활성화 상태 해제
        gameObject.SetActive(true);  // 오브젝트 활성화
    }

    // 무적 상태를 위한 Coroutine
    private IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;  // 무적 상태 설정
        yield return new WaitForSeconds(0.1f); 
        isInvincible = false;  // 무적 상태 해제
    }
}