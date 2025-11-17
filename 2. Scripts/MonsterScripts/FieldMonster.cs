using UnityEngine;

public class FieldMonster : MonsterBase
{
    [Space(5f)]
    [Header("Spawn")]
    [HideInInspector] public Vector3 spawnPoint;
    public float maxDistanceFromSpawn = 30f;
    public float respawnDelay = 5f;

    [Space(5f)]
    [Header("Patrol")]
    public float moveSpeed = 2f;
    public float traceRange = 5f;
    public float patrolDuration = 3f;
    public float thinkDuration = 2f;

    [Space(5f)]
    [Header("Attack")]
    public float attackRange = 1.5f;
    public float attackCooldown = 2f;
    public float lastAttackTime;

    [HideInInspector] public Vector3 patrolDirection;

    protected float stateTimer;
    protected IMonsterState currentState;

    [SerializeField] protected DropTable dropTable;

    void Awake()
    {
        spawnPoint = transform.position;
        anim = GetComponent<Animator>();
    }

    void Start()
    {
        ChangeState(new PatrolState());
        Initialize();
    }

    void Update()
    {
        if (isDead) return;

        if (Vector3.Distance(transform.position, spawnPoint) > maxDistanceFromSpawn)
        {
            Despawn();
            return;
        }

        currentState?.Update(this);

        float targetFill = curHP / maxHP;
        HpBar.fillAmount = targetFill;

        if (DelayHpbar.fillAmount > HpBar.fillAmount)
            DelayHpbar.fillAmount = Mathf.Lerp(DelayHpbar.fillAmount, HpBar.fillAmount, Time.deltaTime * 1.5f);
        else
            DelayHpbar.fillAmount = HpBar.fillAmount;
    }

    public void ChangeState(IMonsterState newState)
    {
        if (currentState != null) currentState.Exit(this);
        currentState = newState;
        currentState?.Enter(this);
    }

    protected override void Die()
    {
        isDead = true;
        ItemDrop();
        Invoke(nameof(Respawn), respawnDelay);
        gameObject.SetActive(false);
    }

    void Respawn()
    {
        transform.position = spawnPoint;
        gameObject.SetActive(true);
        Initialize();
    }

    protected virtual void Despawn()
    {
        gameObject.SetActive(false);
        Invoke(nameof(Respawn), respawnDelay);
    }

    public virtual void Move(Vector3 dir)
    {
        transform.Translate(dir * moveSpeed * Time.deltaTime);
        Flip(dir);
    }

    /// <summary>
    /// 현재 모든 스프라이트가 ‘왼쪽’을 기본 방향으로 보고 있을 때의 Flip 로직
    /// </summary>
    public void Flip(Vector3 dir)
    {
        if (dir.x == 0) return;

        float baseScale = Mathf.Abs(transform.localScale.x);
        bool isRight = dir.x > 0; // 오른쪽으로 이동 중인지 여부

        // 왼쪽 바라보는 게 기본이라서, 오른쪽으로 이동 시 반전
        transform.localScale = new Vector3(isRight ? -baseScale : baseScale, transform.localScale.y, transform.localScale.z);

        float fixedScale = isRight ? -1 : 1;
        if (HpBar != null) HpBar.transform.localScale = new Vector3(fixedScale, 1, 1);
        if (DelayHpbar != null) DelayHpbar.transform.localScale = new Vector3(fixedScale, 1, 1);
    }

    protected void ItemDrop()
    {
        ItemBase droppedItem = dropTable.GetDrop();
        if (droppedItem == null) return;

        GameObject reward = droppedItem.reward;
        if (reward != null)
        {
            Instantiate(reward, transform.position, Quaternion.identity);
        }
    }

    public override void Attack(Player player)
    {
        // 공격 시 플레이어를 바라보게
        Vector3 dir = (player.transform.position - transform.position).normalized;
        Flip(dir);

        // 공격 로직 구현 (예: 데미지, 애니메이션 이벤트 등)
    }
}