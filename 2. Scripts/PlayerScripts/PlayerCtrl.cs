using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerCtrl : MonoBehaviour
{
    public Image Hpbar;
    public Image Mpbar;
    public Image DelayHpbar;
    //플레이어 스킬 관련 코드
    public GameObject[] AttSlash; //기본공격 효과
     
    public GameObject Leaf;         //그림자 잎날
    public GameObject Fairy;        //정령소환
    public GameObject ShootPos;     //그림자 잎날 ShootPos
    [HideInInspector] public float direction;

    [HideInInspector] public bool isStealth; //은신
    float stealthTime;
    SpriteRenderer sprite;

    float FairyTimer = 0.0f; //정령소환 타이머


    public GameObject Acorn;  //도토리 폭탄
    float throwForce = 30.0f;   // 던지는 힘
    float throwAngle = 45.0f;   // 던지는 각도 (도 단위)

    float moveSpeed = 8f;
    float dashSpeed = 35f;
    float dashDuration = 0.2f;
    float dashCooldown = 0.5f;
    float jumpforce = 150.0f;

    AudioSource[] audioSource;
    SpriteRenderer SpRend;

    Rigidbody2D rb;
    private Vector3 movement;
    private bool isDashing;
    private float dashTimeLeft;
    private float lastDashTime;

    bool isInvincible = false;
    int JumpCount = 0;
    float Offset = 0.0f;
    bool isAttack = false;
    bool isJump = false;
    bool isWalk = false;
    Animator anim;

    [HideInInspector] public int[] zxcSkillType = new int[3];
    float[] skillCoolTime = new float[5] { 0.3f, 0.7f, 40.0f, 50.0f, 10.0f};
    float[] skillTimer = new float[5];
    bool isGetRune = false;

    // Start is called before the first frame update
    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();

        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponents<AudioSource>();

        // SkillMgr 리팩토링 으로 인한 주석처리
        //for (int i = 0; i < zxcSkillType.Length; i++)
        //{
        //    zxcSkillType[i] = GameMgr.inst.userData.SkillNumber[i];
        //    if (GameMgr.inst.userData.SkillNumber[i] != -1)
        //    {
        //        GameMgr.inst.SkillIconImg[i].enabled = true;
        //        GameMgr.inst.SkillIconImg[i].sprite = 
        //            GameMgr.inst.SkillIcons[GameMgr.inst.userData.SkillNumber[i]];
        //    }
        //}
        // SkillMgr 리팩토링 으로 인한 주석처리

        if (GlobalValue.sceneType == GlobalValue.SceneType.Game ||
            GlobalValue.sceneType == GlobalValue.SceneType.Lobby)
        transform.position = GameMgr.inst.userData.playerSavePos;

        //if (GameMgr.inst.userData.QuestClear[8])
            GameMgr.inst.SylvaronStoenObj.SetActive(true);

        Hpbar.fillAmount = GameMgr.inst.userData.PlayerHp / GameMgr.inst.userData.PlayerMaxHp;
        Mpbar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;
        DelayHpbar.fillAmount = GameMgr.inst.userData.PlayerHp / GameMgr.inst.userData.PlayerMaxHp;
    }

    // Update is called once per frame
    void Update()
    {
        // 입력 처리
        movement = new Vector3(Input.GetAxisRaw("Horizontal"),0,0);
        movement.z = 0.0f;

        //if (GlobalValue.isPlayerStop)
        //{
        //    audioSource[0].mute = true;
        //    audioSource[1].mute = true;
        //}

        //if (GlobalValue.isPlayerStop == false)
        //{
        //    if (Input.GetKeyDown(KeyCode.Z) && zxcSkillType[0] != -1)
        //    {    
        //        isAttack = true;
        //        Attack(zxcSkillType[0], 0);               
        //    }
        //    if (Input.GetKeyDown(KeyCode.X) && zxcSkillType[1] != -1)
        //    {
        //        isAttack = true;
        //        Attack(zxcSkillType[1], 1);                
        //    }
        //    if (Input.GetKeyDown(KeyCode.C) && zxcSkillType[2] != -1)
        //    {
        //        isAttack = true;
        //        Attack(zxcSkillType[2] , 2);
        //    }
        //}

        // 대시 입력 처리
        if (Input.GetKeyDown(KeyCode.LeftShift) && Time.time >= lastDashTime + dashCooldown)
        {
            //if (GlobalValue.isPlayerStop || isWalk == false) return;

            if (GlobalValue.sceneType == GlobalValue.SceneType.Boss && !BossMapMgr.Inst.CutSceneOver)
                return;

            StartDash();
        }
        if (Input.GetKeyDown(KeyCode.Space) && JumpCount < 2)
        {
            //if (GlobalValue.isPlayerStop) return;

            if (GlobalValue.sceneType == GlobalValue.SceneType.Boss && !BossMapMgr.Inst.CutSceneOver) return;

            rb.AddForce(Vector2.up * jumpforce, ForceMode2D.Impulse);
            isJump = true;
            JumpCount++;
            anim.SetBool("jump", true);
            anim.SetBool("run", false);            
        }

        if (FairyTimer > 0.0f)
        {
            FairyTimer -= Time.deltaTime;            
            if (FairyTimer <= 0.0f) 
            {
                FairyTimer = 0.0f;
                Fairy.gameObject.SetActive(false);
            }
        }
        SkillCoolTimeCheck();

        //if (Input.GetKeyDown(KeyCode.Alpha1))
        //    UsePotionItem(1);
        //if (Input.GetKeyDown(KeyCode.Alpha2))
        //    UsePotionItem(2);

        float targetFill = GameMgr.inst.userData.PlayerHp / GameMgr.inst.userData.PlayerMaxHp;
        Hpbar.fillAmount = targetFill;

        if(DelayHpbar.fillAmount > Hpbar.fillAmount)
        {
            DelayHpbar.fillAmount = Mathf.Lerp(DelayHpbar.fillAmount, Hpbar.fillAmount, Time.deltaTime * 1.5f);
        }
        else
        {
            DelayHpbar.fillAmount = Hpbar.fillAmount; // 회복 시에는 메인 체력바에 즉시 맞춰줌
        }
    }

    void FixedUpdate()  //프레임과 무관하게 일정한 이동보장 위해 Rigidbody기반 물리이동 이므로 FixedUpdate 사용
    {
        if (isDashing)
        {
            ContinueDash();
        }
        else
        {
            Move();
        }

        //if (GlobalValue.isPlayerStop)
        //    anim.SetBool("run", false);
    }


    void Move()
    {
        //if (GlobalValue.isPlayerStop) return;

        transform.position = transform.position + movement.normalized * moveSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftArrow))
        {
            isWalk = true;
            anim.SetBool("run", true);
            transform.localScale = new Vector3(1.0f, 1.0f, 0.0f);

            if (!isJump)
            {
                audioSource[0].mute = false;
                //audioSource[0].volume = GlobalValue.SFX_Value;
            }
            else
                audioSource[0].mute = true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            isWalk = true;
            anim.SetBool("run", true);
            transform.localScale = new Vector3(-1.0f, 1.0f, 0.0f);

            if (!isJump)
            {
                audioSource[0].mute = false;
                //audioSource[0].volume = GlobalValue.SFX_Value;
            }
            else
                audioSource[0].mute = true;
        }
        else
        {
            isWalk = false;
            anim.SetBool("run", false);
            audioSource[0].mute = true;
        }
    }

    void StartDash()
    {
        anim.SetTrigger("dash");
        isDashing = true;
        dashTimeLeft = dashDuration;
        lastDashTime = Time.time;        
    }

    void ContinueDash()
    {
        if (dashTimeLeft > 0)
        {
            Vector2 moveDir = new Vector2(movement.x, movement.y);
            rb.MovePosition(rb.position + moveDir.normalized * dashSpeed * Time.deltaTime);            
            dashTimeLeft -= Time.deltaTime;
            audioSource[0].mute = true;
            SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Dash);
        }
        else
        {
            isDashing = false;
        }
    }

    int FXIndex = -1; //공격 효과 나타내기위한 인덱스 변수
    void Attack(int skillIndex, int zxcType)
    {
        if (GlobalValue.sceneType == GlobalValue.SceneType.Boss && !BossMapMgr.Inst.CutSceneOver)
        {
            return;
        }
        if (GlobalValue.sceneType == GlobalValue.SceneType.Lobby && skillIndex != 0)
            return;

        switch (skillIndex)
        {
            case 0: //기본공격
                if (skillTimer[0] > 0.0f)
                    return;

                skillTimer[0] = skillCoolTime[0];
                if (Input.GetKey(KeyCode.DownArrow))
                {
                    rb.gravityScale = 1.0f;
                    anim.SetTrigger("downAttack");
                    FXIndex = 1;
                    Invoke(nameof(RigidGravity), 1.0f);
                    Invoke(nameof(AttackFXStart), 0.2f);
                }
                else
                {
                    FXIndex = 0;
                    anim.SetTrigger("attack");
                    Invoke(nameof(AttackFXStart), 0.2f);
                }                
                SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Sword);
                break;

            case 1: //그림자 잎날
                if (skillTimer[1] > 0.0f)
                    return;

                if (GameMgr.inst.userData.PlayerMp < 5)
                {
                    GameMgr.inst.InfoPanelOn("마나가 부족합니다");
                    return;
                }

                skillTimer[1] = skillCoolTime[1];
                anim.SetTrigger("attack");
                // 캐릭터의 방향에 따라 발사 방향 설정
                direction = transform.localScale.x < 0 ? 1f : -1f;

                GameObject leaf = Instantiate(Leaf);
                leaf.transform.position = ShootPos.transform.position;

                //마나소모
                GameMgr.inst.userData.PlayerMp -= 5;
                Mpbar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;

                //스킬 사운드
                SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Leaf);
                break;

            case 2: //은신
                if (skillTimer[2] > 0.0f)
                    return;

                if (GameMgr.inst.userData.PlayerMp < 40)
                {
                    GameMgr.inst.InfoPanelOn("마나가 부족합니다");
                    return;
                }

                skillTimer[2] = skillCoolTime[2];
                Stealth();

                //마나소모
                GameMgr.inst.userData.PlayerMp -= 40;
                Mpbar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;

                //스킬 사운드
                SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Stealth);
                break;

            case 3: //정령소환
                if (skillTimer[3] > 0.0f)
                    return;

                if (GameMgr.inst.userData.PlayerMp < 30)
                {
                    GameMgr.inst.InfoPanelOn("마나가 부족합니다");
                    return;
                }
                skillTimer[3] = skillCoolTime[3];
                FairyTimer = 20.0f;                
                Fairy.gameObject.SetActive(true);
                
                //마나소모
                GameMgr.inst.userData.PlayerMp -= 30;
                Mpbar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;

                //스킬 사운드
                SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Fairy);
                break;

            case 4: //도토리 폭탄
                if (skillTimer[4] > 0.0f)
                    return;

                if (GameMgr.inst.userData.PlayerMp < 15)
                {
                    GameMgr.inst.InfoPanelOn("마나가 부족합니다");
                    return;
                }

                skillTimer[4] = skillCoolTime[4];
                direction = transform.localScale.x < 0 ? 1f : -1f;


                //마나소모
                GameMgr.inst.userData.PlayerMp -= 15;
                Mpbar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;

                //스킬 사운드
                SoundMgr.inst.SFX_Play((int)SoundMgr.SFX_Sound.Bomb);
                ThrowGrenade();
                break;
        }

        switch (zxcType)
        {
            case 0:
                if (GameMgr.inst.userData.PlayerMp <= 0 && zxcSkillType[0] > 0)
                {
                    return;
                }        // SkillMgr 리팩토링 으로 인한 주석처리

                //GameMgr.inst.CoolTimeCtrl[0].SkillTimeCtrl(skillCoolTime[zxcSkillType[0]]);
                break;

            case 1:
                if (GameMgr.inst.userData.PlayerMp <= 0 && zxcSkillType[1] > 0)
                {
                    return;
                }        // SkillMgr 리팩토링 으로 인한 주석처리

                //GameMgr.inst.CoolTimeCtrl[1].SkillTimeCtrl(skillCoolTime[zxcSkillType[1]]);
                break;

            case 2:
                if (GameMgr.inst.userData.PlayerMp <= 0 && zxcSkillType[2] > 0)
                {
                    return;
                }        // SkillMgr 리팩토링 으로 인한 주석처리

                //GameMgr.inst.CoolTimeCtrl[2].SkillTimeCtrl(skillCoolTime[zxcSkillType[2]]);
                break;
        }

        if (GameMgr.inst.userData.PlayerMp < 0)
            GameMgr.inst.userData.PlayerMp = 0;
    }
    void ThrowGrenade()
    {
        // 위치 편차 설정 (수류탄이 흩어지도록 약간 다른 위치에 생성)
        float positionOffsetRange = 0.2f;

        // 수류탄을 약간 다른 위치에서 생성
        Vector3 position1 = ShootPos.transform.position + new Vector3(Random.Range(-positionOffsetRange, positionOffsetRange), 0, 0);
        Vector3 position2 = ShootPos.transform.position + new Vector3(Random.Range(-positionOffsetRange, positionOffsetRange), 0, 0);
        Vector3 position3 = ShootPos.transform.position + new Vector3(Random.Range(-positionOffsetRange, positionOffsetRange), 0, 0);

        GameObject grenade1 = Instantiate(Acorn, position1, ShootPos.transform.rotation);
        GameObject grenade2 = Instantiate(Acorn, position2, ShootPos.transform.rotation);
        GameObject grenade3 = Instantiate(Acorn, position3, ShootPos.transform.rotation);

        // 각 수류탄의 Rigidbody2D 가져오기
        Rigidbody2D rb1 = grenade1.GetComponent<Rigidbody2D>();
        Rigidbody2D rb2 = grenade2.GetComponent<Rigidbody2D>();
        Rigidbody2D rb3 = grenade3.GetComponent<Rigidbody2D>();

        // 위쪽으로 살짝 던지면서 퍼지는 방향으로 힘 적용
        Vector2 throwDirection1 = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;
        Vector2 throwDirection2 = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;
        Vector2 throwDirection3 = new Vector2(Random.Range(-0.5f, 0.5f), 1).normalized;

        // 각 수류탄에 힘 가하기 (살짝 위쪽으로 던지면서 흩뿌리는 효과)
        rb1.AddForce(throwDirection1 * throwForce * 0.8f, ForceMode2D.Impulse);
        rb2.AddForce(throwDirection2 * throwForce * 0.9f, ForceMode2D.Impulse);
        rb3.AddForce(throwDirection3 * throwForce * 1.0f, ForceMode2D.Impulse);
    }

    void Stealth()
    {
        if (GlobalValue.sceneType == GlobalValue.SceneType.Boss && !BossMapMgr.Inst.CutSceneOver)
            return;

        // 스텔스 상태가 아닐 때만 스텔스 활성화
        if (!isStealth)
        {
            // 스프라이트의 현재 색상을 가져옴
            Color color = sprite.color;

            // 알파값을 132/255로 설정 (약 0.52)
            color.a = 132f / 255f;

            // 변경된 색상을 다시 설정
            sprite.color = color;

            isStealth = true;
            stealthTime = 10.0f; // 스텔스 지속 시간 초기화
        }
    }

    void SkillCoolTimeCheck()
    {
        //스킬 쿨타임 감소 변수들
        if (0.0f < skillTimer[0])
        {
            skillTimer[0] -= Time.deltaTime;

            if (skillTimer[0] <= 0.0f)
            {
                skillTimer[0] = 0.0f;
            }
        }

        if (0.0f < skillTimer[1])
        {
            skillTimer[1] -= Time.deltaTime;

            if (skillTimer[1] <= 0.0f)
            {
                skillTimer[1] = 0.0f;
            }
        }

        if (0.0f < skillTimer[2])
        { 
            skillTimer[2] -= Time.deltaTime;

            if (skillTimer[2] <= 0.0f)
            {
                skillTimer[2] = 0.0f;
            }
        }

        if (0.0f < skillTimer[3])
        {
            skillTimer[3] -= Time.deltaTime;
            if (skillTimer[3] <= 0.0f)
            {
                skillTimer[3] = 0.0f;
            }
        }

        if (0.0f < skillTimer[4])
        {
            skillTimer[4] -= Time.deltaTime;

            if (skillTimer[4] <= 0.0f)
            {
                skillTimer[4] = 0.0f;
            }
        }

        if (isStealth)
        {
            // 스텔스 시간이 흐르는 동안 카운트 다운
            stealthTime -= Time.deltaTime;

            if (stealthTime <= 0f)
            {
                // 스텔스 상태 종료
                isStealth = false;

                // 알파값을 다시 1로 설정 (완전 불투명)
                Color color = sprite.color;
                color.a = 1f;
                sprite.color = color;
            }
        }
    }
    void AttackFXStart() //플레이어 검기 효과 On
    {        
        AttSlash[FXIndex].gameObject.SetActive(true);
        Invoke(nameof(AttackFXEnd), 0.1f);
    }

    void AttackFXEnd() //플레이어 검기 효과 Off
    {
        AttSlash[FXIndex].gameObject.SetActive(false);
        isAttack = false;
    }

    void RigidGravity()
    {
        rb.gravityScale = 15.0f;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            if(isJump)
            {
                audioSource[1].mute = false;
                //audioSource[1].volume = GlobalValue.SFX_Value;
                audioSource[1].Play();
            }
            else
            {
                audioSource[1].mute = true;
            }
            JumpCount = 0;            
            isJump = false;
            anim.SetBool("jump", false);
        }

        if (collision.gameObject.tag == "StoneThorn")
        {
            TakeDamage(10);
        }

        if (collision.gameObject.tag == "Monster" || collision.gameObject.tag == "MonsterAttack")
        {
            if (collision.gameObject.name.Contains("Armadillo_Skill"))
                return;

            Vector2 PushDir = collision.transform.position - transform.position;
            PushDir.Normalize();
            rb.AddForce(-PushDir * 100.0f, ForceMode2D.Impulse);

            MonsterCtrl monCtrl = collision.gameObject.GetComponentInParent<MonsterCtrl>();
            TakeDamage(monCtrl.MonAttackPower);  // 몬스터의 공격에 데미지를 받음
        }
    }

    //private void OnTriggerEnter2D(Collider2D collision)
    //{
    //    if (GlobalValue.sceneType == GlobalValue.SceneType.Lobby)
    //    {
    //        if(collision.gameObject.name == "NPC")
    //        {
    //            GameMgr.inst.TagName = "NPC"; //어떤오브젝트와 상호작용 했는지 전달
    //            GameMgr.inst.InfoPanelOn("F키를 눌러 상호작용 하세요!");
    //        }

    //        if(collision.gameObject.name == "WarpPoint_Lobby")
    //        {
    //            GameMgr.inst.TagName = "WarpPoint_Lobby";
    //            GameMgr.inst.InfoPanelOn("↑키를 눌러 이동하세요!");
    //        }

    //        if(collision.gameObject.name == "Flower")
    //        {
    //            //GameMgr.inst.itemData.GetItems[3] = true;
    //            //GameMgr.inst.itemData.ItemCount[3] = 1;
    //            GameMgr.inst.InfoPanelOn("'향기나는 꽃'습득! ESC키를 눌러 인벤토리를 확인하세요!", 4.0f);
    //            Destroy(collision.gameObject);
    //        }

    //        if (collision.gameObject.name == "NPC (1)")
    //        {
    //            GameMgr.inst.TagName = "GameNPC";
    //            GameMgr.inst.InfoPanelOn("F키를 눌러 상호작용 하세요!");
    //        }

    //        if(collision.gameObject.name == "RuneGroup")
    //        {
    //            GameMgr.inst.TagName = "RuneGroup";
    //            GameMgr.inst.InfoPanelOn("F키를 눌러 상호작용 하세요!");
    //        }

    //        if (collision.gameObject.name == "SylvaronStone")
    //        {
    //            //if (GameMgr.inst.userData.GetQuestReward[7] == false)
    //            {
    //                GameMgr.inst.InfoPanelOn("퀘스트를 완료해주세요!");
    //                return;
    //            }
    //            //GameMgr.inst.DialogueBoxOpen.SetActive(true);
    //            GameMgr.inst.SylvaronStoenObj.SetActive(true);
    //            //GameMgr.inst.SylvaronDialogue();
    //            Destroy(collision.gameObject);
    //        }
    //    }
    //    else if(GlobalValue.sceneType == GlobalValue.SceneType.Game)
    //    {
    //        if(collision.gameObject.name == "WarpPoint_Game")
    //        {
    //            GameMgr.inst.TagName = "WarpPoint_Game";
    //            GameMgr.inst.InfoPanelOn("↑키를 눌러 이동하세요!");
    //        }

    //        if (collision.gameObject.name.Contains("RuneStonePiece"))
    //        {
    //            //아이템 갯수 증가 
    //            //GameMgr.inst.itemData.GetItems[0] = true;
    //            //GameMgr.inst.itemData.ItemCount[0] += 1;

    //            //어떤 룬스톤을 먹었는지 체크
    //            if (collision.gameObject.name == "RuneStonePiece")
    //                GameMgr.inst.userData.GetStonePiece[0] = true;
    //            else if (collision.gameObject.name == "RuneStonePiece (1)")
    //                GameMgr.inst.userData.GetStonePiece[1] = true;
    //            else if (collision.gameObject.name == "RuneStonePiece (2)")
    //                GameMgr.inst.userData.GetStonePiece[2] = true;

    //            ////룬스톤을 세개 다 모았는지 아닌지 체크하는 조건문
    //            //if (GameMgr.inst.userData.QuestIndex == 3 && GameMgr.inst.itemData.ItemCount[0] >= 3)
    //            {
    //                //if (GameMgr.inst.userData.QuestIndex == 4)
    //                    return;

    //                //GameMgr.inst.userData.QuestClear[GameMgr.inst.userData.QuestIndex] = true;
    //                GameMgr.inst.InfoPanelOn("퀘스트를 완료하고 보상을 얻으세요!");
    //                //GameMgr.inst.QuestUIRefresh();
    //            }
    //            GameMgr.inst.InfoPanelOn("아이템 습득! ESC키를 눌러 인벤토리를 확인해보세요!");
    //            DataMgr.inst.SaveData();
    //            Destroy(collision.gameObject);
    //        }

    //        if (collision.gameObject.name == "StoneObject")
    //        {
    //            GameMgr.inst.TagName = "StoneObject";
    //            GameMgr.inst.InfoPanelOn("F키를 눌러 상호작용 하세요!");
    //        }

    //        if (collision.gameObject.name == "WarpPoint_inBoss")
    //        {
    //            GameMgr.inst.TagName = "WarpPoint_inBoss";
    //            GameMgr.inst.InfoPanelOn("↑키를 눌러 이동하세요!");
    //        }

    //        if (collision.gameObject.name == "NPC")
    //        {
    //            if (GameMgr.inst.userData.PuzzleClear)
    //                return;

    //            GameMgr.inst.TagName = "GameNPC";
    //            GameMgr.inst.InfoPanelOn("F키를 눌러 상호작용 하세요!");
    //        }

    //        if(collision.gameObject.name == "Trap1")
    //        {
    //            GameMgr.inst.userData.playerSavePos = transform.position; //포지션 저장
    //            SceneManager.LoadScene("BattleScene");
    //            SceneManager.LoadScene("PlayerScene", LoadSceneMode.Additive);              
    //        }


    //    }

    //    else if(GlobalValue.sceneType == GlobalValue.SceneType.Boss)
    //    {
    //        if(collision.gameObject.name == "RuneStone")
    //        {
    //            isGetRune = true;
    //            GameMgr.inst.userData.BossDie = true;
    //            //GameMgr.inst.userData.QuestClear[GameMgr.inst.userData.QuestIndex] = true;
    //            //GameMgr.inst.QuestUIRefresh();
    //            DataMgr.inst.SaveData();
    //            Destroy(collision.gameObject);
    //        }

    //        if(collision.gameObject.name == "WarpPoint_Boss")
    //        {
    //            if (isGetRune)
    //            {
    //                GameMgr.inst.TagName = "WarpPoint_Boss";
    //                GameMgr.inst.InfoPanelOn("↑키를 눌러 이동하세요!");
    //            }
    //        }
    //        if (collision.gameObject.tag == "Monster" || collision.gameObject.tag == "MonsterAttack")
    //        {
    //            TakeDamage(20);
    //        }
    //    }
    //    else if(GlobalValue.sceneType == GlobalValue.SceneType.Battle)
    //    {
    //        if(collision.gameObject.tag == "Monster")
    //        {
    //            if (isStealth || collision.gameObject.GetComponentInParent<UnderBossCtrl>().isgroggy)
    //            {
    //                return;
    //            }

    //            TakeDamage(20);
                
    //        }
    //        if(collision.gameObject.name == "WarpPoint_Battle")
    //        {
    //            GameMgr.inst.TagName = "WarpPoint_Battle";
    //            GameMgr.inst.InfoPanelOn("↑키를 눌러 이동하세요!");
    //        }
    //    }
        
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (GlobalValue.sceneType == GlobalValue.SceneType.Lobby)
    //    {
    //        if (collision.gameObject.name == "NPC")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }

    //        if (collision.gameObject.name == "WarpPoint_Lobby")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }

    //        if (collision.gameObject.name == "RuneGroup")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }

    //        if (collision.gameObject.name == "NPC (1)")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }
    //    }

    //    if (GlobalValue.sceneType == GlobalValue.SceneType.Game)
    //    {
    //        if (collision.gameObject.name == "WarpPoint_Game")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }

    //        if( collision.gameObject.name == "WarpPoint_inBoss")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }


    //        if (collision.gameObject.name == "NPC")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }
    //    }

    //    if(GlobalValue.sceneType == GlobalValue.SceneType.Boss)
    //    {
    //        if (collision.gameObject.name == "WarpPoint_Boss")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }
    //    }

    //    if (GlobalValue.sceneType == GlobalValue.SceneType.Battle)
    //    {
    //        if (collision.gameObject.name == "WarpPoint_Battle")
    //        {
    //            GameMgr.inst.TagName = "";
    //        }
    //    }

    //}

    void OnParticleCollision(GameObject other)
    {
        // 몬스터의 파티클 공격이 플레이어와 충돌한 경우
        if (other.gameObject.tag.Contains("MonsterAttack"))
        {            
            TakeDamage(20);  // 데미지 판정 처리
        }
    }

    public void TakeDamage(float value)
    {
        // 무적 상태가 아니라면 데미지를 받음
        if (!isInvincible)
        {
            GameMgr.inst.userData.PlayerHp -= value;

            if (GameMgr.inst.userData.PlayerHp <= 0)
            {
                GameMgr.inst.userData.PlayerHp = 0;
                GameMgr.inst.PlayerDie();
                anim.SetBool("die", true);
                transform.localScale = new Vector3(1, 1, 1);
            }

            //Hpbar.fillAmount = GameMgr.inst.userData.PlayerHp / GameMgr.inst.userData.PlayerMaxHp;
            // 무적 상태 시작
            StartCoroutine(InvincibilityCoroutine());
        }
        DataMgr.inst.SaveData();
    }

    //public void UsePotionItem(ItemType type)
    //{
    //    if (GameMgr.inst.isPlayerDie)
    //        return;

    //    //if (GameMgr.inst.userData.PotionAdd[index - 1] == false)
    //    {
    //        GameMgr.inst.InfoPanelOn("아이템이 등록되지 않았습니다!");
    //        return;
    //    }

    //    if(type == ItemType.Potion_HP)
    //    {
    //        //if (GameMgr.inst.itemData.ItemCount[1] <= 0)               
    //        {
    //            GameMgr.inst.InfoPanelOn("아이템 수량이 부족합니다!");
    //            return;
    //        }
    //        if (GameMgr.inst.userData.PlayerHp >= GameMgr.inst.userData.PlayerMaxHp)
    //        {
    //            GameMgr.inst.InfoPanelOn("체력이 충분히 채워졌습니다!");
    //            return;
    //        }

    //        SoundMgr.inst.UI_Play((int)SoundMgr.UI_Sound.ItemGet);
    //        //GameMgr.inst.itemData.ItemCount[1] -= 1;
    //        //invenCtrl.ItemCountText[1].text = $"{GameMgr.inst.userData.ItemCount[1]}";
    //        //HP증가
    //        GameMgr.inst.userData.PlayerHp += 50;
    //        if (GameMgr.inst.userData.PlayerHp > GameMgr.inst.userData.PlayerMaxHp)
    //        {
    //            GameMgr.inst.userData.PlayerHp = GameMgr.inst.userData.PlayerMaxHp;
    //        }
    //        Hpbar.fillAmount = GameMgr.inst.userData.PlayerHp / GameMgr.inst.userData.PlayerMaxHp;
    //    }

    //    if(type == ItemType.Potion_MP)
    //    {
    //        //if (GameMgr.inst.itemData.ItemCount[2] <= 0)
    //        {
    //            GameMgr.inst.InfoPanelOn("아이템 수량이 부족합니다!");
    //            return;
    //        }

    //        if (GameMgr.inst.userData.PlayerMp >= GameMgr.inst.userData.PlayerMaxMp)
    //        {
    //            GameMgr.inst.InfoPanelOn("마나가 충분히 채워졌습니다!");
    //            return;
    //        }

    //        SoundMgr.inst.UI_Play((int)SoundMgr.UI_Sound.ItemGet);
    //        //GameMgr.inst.itemData.ItemCount[2] -= 1;
    //        //invenCtrl.ItemCountText[2].text = $"{GameMgr.inst.userData.ItemCount[2]}";

    //        //MP증가
    //        GameMgr.inst.userData.PlayerMp += 30;
    //        if(GameMgr.inst.userData.PlayerMp > GameMgr.inst.userData.PlayerMaxMp)
    //        {
    //            GameMgr.inst.userData.PlayerMp = GameMgr.inst.userData.PlayerMaxMp;
    //        }
    //        Mpbar.fillAmount = GameMgr.inst.userData.PlayerMp / GameMgr.inst.userData.PlayerMaxMp;
    //    }

    //    GameMgr.inst.PotionAddQuickSlot(type); //UI갱신
    //    DataMgr.inst.SaveData();
    //}

    // 무적 상태를 위한 Coroutine
    private IEnumerator InvincibilityCoroutine()
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

    public void playerDieAnimOff()
    {
        anim.SetBool("die", false);
    }
}
