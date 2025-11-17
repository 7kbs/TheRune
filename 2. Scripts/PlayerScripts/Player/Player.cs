using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class Player : MonoBehaviour
{
    PlayerMove playerMove;
    [HideInInspector] public AudioSource[] audioSource;
    [HideInInspector] public Animator anim;

    [Header("UI")]
    public Image HpBar;
    public Image MpBar;
    [HideInInspector] public SpriteRenderer sprite;
    public GameObject[] AttSlash;

    bool isInvincible = false;
    bool isGetRune = false;
    [HideInInspector] public bool isStealth; //은신

    Interactable currentInteractable;


    void Awake()
    {
        playerMove = GetComponent<PlayerMove>();
        anim = GetComponent<Animator>();
        sprite = GetComponent<SpriteRenderer>();
        audioSource = GetComponents<AudioSource>();
    }

    void Start()
    {
        playerMove.OnLand += OnLanding;
        var interactables = FindObjectsOfType<Interactable>();
        foreach (var interactable in interactables)
        {
            if (interactable != null) // 안전장치
            {
                interactable.OnPlayerEnter += HandleEnter;
                interactable.OnPlayerExit += HandleExit;
            }
        }
    }

    void Update()
    {
        UpdateUI();
        InteractionInput();
    }

    void UpdateUI()
    {
        var hp = GameMgr.inst.userData.PlayerHp;
        var maxHp = GameMgr.inst.userData.PlayerMaxHp;
        var mp = GameMgr.inst.userData.PlayerMp;
        var maxMp = GameMgr.inst.userData.PlayerMaxMp;

        HpBar.fillAmount = hp / maxHp;
        MpBar.fillAmount = mp / maxMp;
    }

    void InteractionInput()
    {
        if (currentInteractable == null) return;

        if (!(currentInteractable.interactType == InteractableType.WarpPoint) &&
            Input.GetKeyDown(KeyCode.F))
        {
            currentInteractable.Interact(this);
        }

        if (currentInteractable.interactType == InteractableType.WarpPoint &&
            Input.GetKeyDown(KeyCode.UpArrow))
        {
            currentInteractable.Interact(this);
        }
    }

    public IEnumerator RestoreAlpha(float delay)
    {
        yield return new WaitForSeconds(delay);
        Color c = sprite.color;
        c.a = 1f;
        sprite.color = c;
    }

    public void TakeDamage(float value)
    {
        // 무적 상태가 아니라면 데미지를 받음
        if (!isInvincible && !playerMove.IsInteractionState)
        {
            GameMgr.inst.userData.PlayerHp -= value;

            if (GameMgr.inst.userData.PlayerHp <= 0)
            {
                GameMgr.inst.userData.PlayerHp = 0;
                GameMgr.inst.PlayerDie();
                anim.SetBool("die", true);
                transform.localScale = new Vector3(1, 1, 1);
            }

            // 무적 상태 시작
            StartCoroutine(InvincibilityCoroutine());
        }
        DataMgr.inst.SaveData();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "StoneThorn") TakeDamage(10);

        if (collision.gameObject.tag == "Monster" || collision.gameObject.tag == "MonsterAttack")
        {
            if (collision.gameObject.name.Contains("Armadillo_Skill") || isInvincible) return;

            Vector2 PushDir = collision.transform.position - transform.position;
            PushDir.Normalize();
            GetComponent<Rigidbody2D>().AddForce(-PushDir * 100.0f, ForceMode2D.Impulse);

            MonsterBase mon = collision.gameObject.GetComponentInParent<MonsterBase>();
            TakeDamage(mon.AttackPower);  // 몬스터의 공격에 데미지를 받음
        }
    }

    void OnLanding()
    {
        // 착지 사운드 재생
        if (playerMove.isJump)  // 방금 점프했던 상태였으면
        {
            audioSource[1].mute = false;
            audioSource[1].volume = SoundMgr.SFX_Value;
            audioSource[1].Play();
        }
        else audioSource[1].mute = true;
    }


    void OnTriggerEnter2D(Collider2D col)
    {
        var inter = col.GetComponent<Interactable>();
        if (inter != null)
        {
            inter.OnPlayerEnter += HandleEnter;
            inter.OnPlayerExit += HandleExit;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        var inter = col.GetComponent<Interactable>();
        if (inter != null)
        {
            inter.OnPlayerEnter -= HandleEnter;
            inter.OnPlayerExit -= HandleExit;
        }
    }

    private void HandleEnter(Interactable inter)
    {
        currentInteractable = inter;
        Debug.Log("Enter: " + inter.name);

        if (inter.interactType == InteractableType.WarpPoint) GameMgr.inst.InfoPanelOn("↑키를 눌러 이동하세요!");
        else GameMgr.inst.InfoPanelOn("F키를 눌러 상호작용 하세요!");
    }

    private void HandleExit(Interactable inter)
    {
        if (currentInteractable == inter) currentInteractable = null;
        // 안내 UI 제거
    }


    void OnParticleCollision(GameObject other)
    {
        // 몬스터의 파티클 공격이 플레이어와 충돌한 경우
        if (other.gameObject.tag.Contains("MonsterAttack"))
        {
            TakeDamage(20);  // 데미지 판정 처리
        }
    }

    // 무적 상태를 위한 Coroutine
    IEnumerator InvincibilityCoroutine()
    {
        isInvincible = true;  // 무적 상태 설정

        float blinkInterval = 0.1f;  // 깜빡임 간격 설정 (0.1초)
        float invincibilityDuration = 1.5f;  // 무적 지속 시간
        float elapsed = 0f;

        while (elapsed < invincibilityDuration)
        {
            sprite.enabled = !sprite.enabled;  // 스프라이트 활성화/비활성화
            yield return new WaitForSeconds(blinkInterval);  // 깜빡임 간격만큼 대기
            elapsed += blinkInterval;
        }

        sprite.enabled = true;  // 깜빡임 종료 후 스프라이트를 다시 활성화
        isInvincible = false;  // 무적 상태 설정
    }

    public void SetDefaultState() => PlayerMove.inst.ChangeState(new DefaultState());
}