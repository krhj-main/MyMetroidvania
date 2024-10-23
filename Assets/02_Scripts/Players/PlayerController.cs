using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


[System.Serializable]
public class SpellSetting
{
    public int spell_Damage;
    public float spell_Speed;
    public float spell_LifeTime;
    public float spell_recoilForce;
    public Vector2 spell_Range;
    public float spell_radius;
    public LayerMask spell_AttackablekLayer;
    public int spell_HitLimit;

    public Transform spell_XTransform;
}

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public Vector2 moveAxis;
    Rigidbody2D rig;
    Animator anim;
    [HideInInspector]
    public PlayerStateList pState;
    Casting cast;
    float gravity;
    bool canDash = true;
    bool dashed;
    bool restoreTime;
    float restoreTimeSpeed;
    bool canFlash = true;
    bool openMap = false;

    SpriteRenderer sr;
    BoxCollider2D col;




    [Header("플레이어 이동속도")]
    [SerializeField] float moveSpeed;

    [Space(5)]
    [Header("플레이어 점프")]
    [SerializeField] float jumpForce;
    [SerializeField] int jumpExtra;
    int jumpCurrent;

    [Space(5)]
    [Header("플레이어 회피")]
    [SerializeField] float dodgeSpeed;
    [SerializeField] float dodgeCoolTime;
    [SerializeField] float dodgeTime;

    [Space(5)]
    [Header("그라운드 체크")]
    [SerializeField] bool Debugging;
    [SerializeField] Transform groundChecker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundOffset;
    public bool onGround;

    [Space(5)]
    [Header("플레이어 공격")]
    [SerializeField] int playerDamage;
    [SerializeField] float playerAttackSpeed;
    [SerializeField] float playerAttackCooltime;
    [SerializeField] private float attackActiveTime;
    bool isAttack;
    [SerializeField] GameObject slashEffect;
    [SerializeField] float timeBetweenAttack;

    [Space(5)]
    [Header("피격 반동")]
    [SerializeField] int recoilingXSteps = 5;
    [SerializeField] int recoilingYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;

    [Space(5)]
    [Header("공격 영역")]
    [SerializeField] Transform XAttack;
    [SerializeField] Transform UpAttack;
    [SerializeField] Transform DownAttack;
    [SerializeField] Vector2 XAttackArea;
    [SerializeField] Vector2 UpAttackArea;
    [SerializeField] Vector2 DownAttackArea;
    [SerializeField] LayerMask attackablekLayer;

    [Space(5)]
    [Header("플레이어 체력")]
    [SerializeField] public int maxHealth;
    public int health;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;

    [Space(5)]
    [Header("힐 관련 변수")]
    float healTimer;
    [SerializeField] float timeToHeal;


    // 플레이어 체력 변경 처리용 델리게이트
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

    [Space(5)]
    [Header("플레이어 마나")]
    [SerializeField] Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;


    [Space(5)]
    [Header("플레이어 반동")]
    [SerializeField] float recoilForce;
    [SerializeField] float recoilTime;


    

    

    

    enum Spell
    {
        FIREBALL,
        FIREBOMB,
        FIRESPRAY,
        FIREMETEOR,
    }
    Spell spell_Idx;
    GameObject useSpell;
    int maxSpell;
    int currentSpell = 0;
    


    public static PlayerController Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        health = maxHealth;
    }

    public int Health
    {
        get
        {
            return health;
        }
        set
        {
            if (health != value)
            {
                health = Mathf.Clamp(value,0,maxHealth);

                if (onHealthChangedCallback != null)
                {
                    onHealthChangedCallback.Invoke();
                }
            }
        }
    }
    public float Mana
    {
        get
        {
            return mana;
        }
        set
        {
            if (mana != value)
            {
                mana = Mathf.Clamp(value,0,1);
                manaStorage.fillAmount = mana;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        pState = GetComponent<PlayerStateList>();
        cast = GetComponent<Casting>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        maxSpell = Enum.GetValues(typeof(Spell)).Length;
        col = GetComponent<BoxCollider2D>();
        manaStorage = GameObject.Find("Mana").GetComponent<Image>();

        gravity = rig.gravityScale;

        Health = maxHealth;
        Mana = mana;
    }

    // Update is called once per frame
    void Update()
    {
        onGround = Grounded();
        if (pState.cutscene) return;

        if (pState.alive)
        {
            GetInput();
        }
        
        if (pState.dashing) return;
        if (pState.alive)
        {
            ActiveMove();
            ActiveFlip();
            ActiveAttack();
            ActiveDodge();
            RestoreTimeScale();
            UpdateJumpAction();
            Heal();
            ActiveCasting();
            ToggleMap();
        }
        
        
    }
    private void FixedUpdate()
    {
        if (pState.cutscene) return;
        if (pState.dashing) return;
        FlashWhileInvincible();
        Recoil();        
    }
    // 플레이어 입력 인식과 관련 처리
    void GetInput()
    {
        // 플레이어 상하좌우 인식을 벡터2 값에 저장
        moveAxis = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));

        // 땅에 있는게 아니라면 점프 애니메이션을 실행
        anim.SetBool("isJump", !Grounded());

        // C 키를 입력받고, 현재 점프횟수가 0보다 크거나 플레이어가 점프 중이라면
        if (Input.GetKeyDown(KeyCode.C) && (jumpCurrent > 0 || !pState.jumping))
        {
            jumpCurrent--;
            ActiveJump();
            // 점프 실행 1회 줄이고, 점프 메서드 실행
        }

        // 공격 X키를 입력받아 공격실행
        isAttack = Input.GetKeyDown(KeyCode.X);

        // 미니맵열기
        openMap = Input.GetKey(KeyCode.M);

        
        
    }
    void ToggleMap()
    {
        if (openMap)
        {
            UIManager.Instance.mapHandler.SetActive(true);
        }
        else
        {
            UIManager.Instance.mapHandler.SetActive(false);
        }
    }

    // 플레이어 이동 메서드
    void ActiveMove()
    {

        // 입력받은 벡터2의 x값과 이동속도를 곱해 이동
        rig.velocity = new Vector2(moveAxis.x * moveSpeed, rig.velocity.y);

        // 이동값이 0 이 아니라면 걷기 애니메이션 실행
        anim.SetBool("isWalk",moveAxis.x!=0);
    }

    
    // 이동 입력값에 따라 플레이어 좌우 반전
    void ActiveFlip()
    {
        // 이동값이 양수면 
        if (moveAxis.x > 0)
        {
            // 플레이어의 스케일을 1로 해서 오른쪽을 바라보게 설정
            transform.localScale = new Vector2(1, transform.localScale.y);
            // 플레이어 상태가 오른쪽을 보고있다고 설정
            pState.lookRight = true;
        }
        // 이동값이 음수라면
        if (moveAxis.x < 0)
        {
            // 플레이어 스케일을 -1로 해 왼쪽을 바라보게 설정
            transform.localScale = new Vector2(-1, transform.localScale.y);
            // 플레이어 상태가 왼쪽을 보고있다고 설정
            pState.lookRight = false;
        }
    }

    // 플레이어 점프 메서드
    void ActiveJump()
    {
        // 플레이어 점프 상태로
        pState.jumping = true;
        // x 이동값은 그대로 유지한 채 점프력 만큼 y방향으로 이동
        rig.velocity = new Vector2(rig.velocity.x, jumpForce);        
    }
    void UpdateJumpAction()
    {
        // 땅에 닿아 있다면 점프 가능수 = 최대 점프 가능 횟수 만큼
        if (Grounded())
        {
            pState.jumping = false;
            jumpCurrent = jumpExtra;
        }
    }

    // 플레이어 슬라이딩 회피와 관련 메서드
    void ActiveDodge()
    {
        // Z키를 눌렀으며, 대시가 가능한 상태고, 대시중이 아니면서 땅에 닿아있다면
        if (Input.GetKeyDown(KeyCode.Z) && canDash && !dashed && Grounded())
        {
            dashed = true;
            StartCoroutine(Dash());
            // 대시중인 상태로 바꾸고 대시 코루틴을 실행
        }
        // 그냥 땅에만 닿아있다면
        if(Grounded())
        {
            // 대시중이지 않다고 설정
            dashed = false;
            
        }
    }

    // 대시 코루틴
    IEnumerator Dash()
    {
        // 대시중일땐 대시가 불가능해야하므로, false
        canDash = false;
        // 플레이어 상태 대시 상태로 설정
        pState.dashing = true;
        // 대시 중에 중력의 영향을 받지 않게 일시적으로 중력값 0으로 설정

        gameObject.layer = 7;
        gameObject.tag = "InvinciblePlayer";
        // 대시중에는 공격받지 않게 레이어를 트리거로 바꾸고
        rig.gravityScale = 0;
        
        // 땅에 닿아있고 이동 y값이 0과 같거나 크면 구르기 실행
        if (Grounded() && moveAxis.y >= 0)
        {
            anim.SetTrigger("isRoll");
        }
        // 땅에 닿아있는데 y값이 아래로 향하고있다면 슬라이딩 실행
        else if (Grounded() && moveAxis.y < 0)
        {
            anim.SetTrigger("isSlide");
        }
        // 플레이어 좌우 상태에 따라 방향 설정
        int _dir = pState.lookRight ? 1 : -1;
        // 방향 값 * 속도값으로 대시 플레이어 속도를 설정
        rig.velocity = new Vector2(_dir * dodgeSpeed, 0);
        // 회피 지속시간을 기다린 후
        yield return new WaitForSeconds(dodgeTime);
        // 플레이어가 대시상태가 아니라고 바꿈
        pState.dashing = false;
        // 중력도 원래대로 초기화
        rig.gravityScale = gravity;
        // 다음 대시 쿨타임을 기다림
        yield return new WaitForSeconds(dodgeCoolTime);
        // 대시를 다시 가능하게끔 true로 설정
        canDash = true;
        gameObject.layer = 6;
        gameObject.tag = "Player";
        // 다시 레이어에 충돌되게 트리거를 해제
    }

    // 플레이어가 땅에있는지 아닌지 판별하는 메서드
    bool Grounded()
    {
        // 플레이어가 다운된 그라운드체커 컴포넌트를 활용해 groundLayer를 체크
        bool isGround = Physics2D.OverlapCircle(groundChecker.position, groundOffset, groundLayer);
        return isGround;
    }

    // 디버깅 관련한 메서드
    private void OnDrawGizmos()
    {
        // 디버깅 설정이 false 면 기즈모를 그리지 않음
        if (!Debugging) return;

        // 기즈모의 컬러 설정
        Gizmos.color = Color.red;
        // 그라운드체커의 위치를 확인하기위한 구체 기즈모를 그림
        Gizmos.DrawSphere(groundChecker.position,groundOffset);

        // 플레이어의 공격범위를 보여주기 위한 와이어큐브를 그림
        // 좌우
        Gizmos.DrawWireCube(XAttack.position,XAttackArea);
        // 상
        //Gizmos.DrawWireCube(UpAttack.position,UpAttackArea);
        // 하
        //Gizmos.DrawWireCube(DownAttack.position,DownAttackArea);
    }


    // 플레이어 공격, 피격시 반동으로 밀려나는 것과 관련된 메서드
    // 반동값
    void Recoil()
    {
        if (pState.recoilingX)
        {
            if (pState.lookRight)
            {
                rig.velocity = new Vector2(-recoilXSpeed, 0);
            }
            else
            {
                rig.velocity = new Vector2(recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            rig.gravityScale = 0;
            if (moveAxis.y < 0)
            {
                rig.velocity = new Vector2(rig.velocity.x, recoilYSpeed);
            }
            else
            {
                rig.velocity = new Vector2(rig.velocity.x, -recoilYSpeed);
            }
            //airJumpCounter = 0;
        }
        else
        {
            rig.gravityScale = gravity;
        }

        // 피격 효과 멈추기
        if (pState.recoilingX && stepsXRecoiled < recoilingXSteps)
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }

        if (pState.recoilingY && stepsYRecoiled < recoilingYSteps)
        {
            stepsYRecoiled++;
        }
        else
        {
            StopRecoilY();
        }

        if (Grounded())
        {
            StopRecoilY();
        }
    }
    // X방향의 반동을 멈추게 해 주는주기 위한 메서드
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    // Y방향의 반동 멈춤 해 주는주기 위한 메서드
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }


    //플레이어 피격 시 체력 감소 처리
    public void TakeDamage(float _damage)
    {
        if (pState.alive)
        {
            // Health 프로퍼티로 현재 체력값을 업데이트
            Health -= Mathf.RoundToInt(_damage);
            
            if (Health <= 0)
            {
                anim.SetTrigger("isDeath");
                StartCoroutine(Death());
            }
            else
            {
                anim.SetTrigger("isHit");
                // 연속으로 충돌 데미지를 받지 않도록하는 코루틴 실행
                StartCoroutine(StopTakingDamage());
            }
        }
    }

    // 피격 처리를 위한 시간을 주기위한 코루틴
    IEnumerator StopTakingDamage()
    {
        // 플레이어가 잠시 무적이 되게끔 invincible을 true로 설정
        pState.invincible = true;
        // 피격 튀겨나는 파티클 효과를 생성시킴
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        // 1.5초 후에 파티클 효과 삭제
        Destroy(_bloodSpurtParticles, 1.5f);
        //anim.SetTrigger("isHitted");
        // 1초 후
        yield return new WaitForSeconds(1f);
        // 플레이어가 다시 피격될 수 있게 invincible을 false로 설정
        pState.invincible = false;
    }

    // 무적시간동안 플레이어가 깜빡거리게 확인 할 수 있도록하는 메서드
    IEnumerator Flash()
    {
        Debug.Log("Flash");
        sr.enabled = !sr.enabled;
        canFlash = false;
        yield return new WaitForSeconds(0.15f);
        canFlash = true;
    }

    void FlashWhileInvincible()
    {
        // 플레이어 스프라이트를 무적시간동안에 깜빡이게끔 하여 표시시킴
        //sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;

        if (pState.invincible && !pState.cutscene)
        {
            if (Time.timeScale > 0.2 && canFlash)
            {
                StartCoroutine(Flash());
            }
        }
        else
        {
            sr.enabled = true;
        }
    }

    // 플레이어가 피격 시 느려진 시간을 다시 되돌리는 메서드 구현중
    void RestoreTimeScale()
    {
        if (restoreTime)
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.deltaTime * restoreTimeSpeed;
            }
            else
            {
                Time.timeScale = 1;
                restoreTime = false;
            }
        }
    }
    // 플레이어 피격시 시간을 느리게하는 메서드    
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if (_delay > 0)
        {
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            StopCoroutine(StartTimeAgain(_delay));
            restoreTime = true;
        }
    }

    // 피격시 처리를 위한 메서드    
    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }
    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        // 피격 튀겨나는 파티클 효과를 생성시킴
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        // 1.5초 후에 파티클 효과 삭제
        Destroy(_bloodSpurtParticles, 1.5f);
        

        yield return new WaitForSeconds(0.9f);

        StartCoroutine(UIManager.Instance.ActiveDeathScreen());
    }
    // 플레이어 공격 관련 메서드
    void ActiveAttack()
    {
        // 플레이어 입력을 받아서 isAttack이 true가 된다면
        if (isAttack)
        {
            // 공격 애니메이션을 실행하고
            anim.SetTrigger("isAttack");
            
            // 땅에 닿아있고 y축 이동값이 0이하라면 
            if (Grounded() && moveAxis.y <= 0)
            {
                int _recoilLR = pState.lookRight ? 1 : -1;
                // X축 공격
                Hit(XAttack, XAttackArea, ref pState.recoilingX, Vector2.right * _recoilLR, recoilXSpeed);
            }
            // y축 이동값이 양수 값이라면
            else if (moveAxis.y > 0)
            {
                // 위쪽 공격
            }
            // 땅에 닿지않고 y축 이동값이 아래쪽
            else if (!Grounded() && moveAxis.y < 0)
            {
                // 공중 시 아래 공격
            }
        }
    }

    // 플레이어 공격 관련 처리 메서드
    // 공격 위치 트랜스폼, 공격 영역, 반동값 처리변수, 반동값 방향
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool,Vector2 _recoilDir, float _recoilStrength)
    {
        // 공격 영역 박스안에 공격가능한 레이어 오브젝트를 toHit배열에 저장
        Collider2D[] toHit = Physics2D.OverlapBoxAll(_attackTransform.position,_attackArea,0,attackablekLayer);
        // 리스트 피격 된 적의 갯수를 담음
        List<Enemy> hitEnemy = new List<Enemy>();

        // 공격안에 있는 공격가능 레이어가 0보다 크다면
        if (toHit.Length > 0)
        {
            // 반동을 줄수가 있게 하고
            _recoilBool = true;
        }

        // 공격범위안에 있는 적만큼 반복문 실행
        for (int i = 0; i < toHit.Length; i++)
        {
            // Enemy 스크립트가 붙어있는 e 객체를
            // 공격범위안에 있는 오브젝트에 Enemy 스크립트가 붙어있는 객체를 생성하고
            Enemy e = toHit[i].GetComponent<Enemy>();

            // e 객체가 정상적으로 Enemy를 붙여온 것이 맞고, 피격 리스트에 포함되지 않았다면
            if (e && !hitEnemy.Contains(e))
            {
                // 해당 Enemy 스크립트에서 피격처리 메서드를 실행
                e.EnemyHit(playerDamage, _recoilDir, _recoilStrength);
                // 리스트에 e 객체 추가
                hitEnemy.Add(e);
            }
            // 공격 영역 안에 있는 객체의 태그가 Enemy 라면
            if (toHit[i].CompareTag("Enemy"))
            {
                // 플레이어가 마나를 얻음, 마나를 managain 만큼 회복
                Mana += manaGain;
            }
        }
    }

    public IEnumerator WalkIntoNewScene(Vector2 _exitDir, float _delay)
    {
        pState.invincible = true;
        if (_exitDir.y > 0)
        {
            rig.velocity = jumpForce * _exitDir;
        }

        if (_exitDir.x != 0)
        {
            moveAxis.x = _exitDir.x > 0 ? 1 : -1;

            ActiveMove();
        }

        ActiveFlip();
        yield return new WaitForSeconds(_delay);
        pState.invincible = false;
        pState.cutscene = false;
    }


    ///<summary>
    /// 마법, 힐링 메서드
    ///</summary>
    /// 마법은 리스트로 만들어다가 추가하기.
    /// 


    void Heal()
    {
        if (Input.GetKey(KeyCode.V) && !pState.dashing && !pState.jumping && health < maxHealth && Mana >= 0.5f)
        {
            pState.healing = true;
            anim.SetBool("isHeal", true);


            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
                Mana -= 0.5f;
                healTimer = 0;
            }
        }
        else
        {
            pState.healing = false;
            anim.SetBool("isHeal", false);
            healTimer = 0;
        }   
    }
    void ActiveCasting()
    {
        if (Input.GetKeyDown(KeyCode.A) && !pState.dashing && !pState.casting)
        {
            Debug.Log("cast");
            pState.casting = true;
            cast.TimeToCasting();
            
        }
        else if (Input.GetKeyDown(KeyCode.A) && pState.casting)
        {
            // 캐스팅 취소
            Debug.Log("cast cancle");
            pState.casting = false;
            cast.CancelCasting();
            currentSpell = 0;
            
        }

        if (pState.casting && Input.GetKeyDown(KeyCode.D))
        {
            ChooseSpell();
        }

        if (pState.casting && Input.GetKeyDown(KeyCode.S))
        {
            int _dir = pState.lookRight ? 1 : -1;
            spell_Idx = (Spell)currentSpell;
            // 마법 사용
            switch (spell_Idx)
            {
                case Spell.FIREBALL:
                    Debug.Log("1 번째 마법");
                    useSpell = Fireball(_dir);
                    break;

                case Spell.FIREBOMB:
                    Debug.Log("2 번째 마법");
                    useSpell = FireBomb(_dir);
                    break;

                case Spell.FIRESPRAY:
                    Debug.Log("3 번째 마법");
                    useSpell = FireIncendiary(_dir);
                    break;
                case Spell.FIREMETEOR:
                    Debug.Log("4 번째 마법");
                    useSpell = FireMeteor(_dir);
                    break;

            }
        }
    }
    void ChooseSpell()
    {
        currentSpell++;
        currentSpell %= maxSpell;
        Debug.Log($"{currentSpell+1} 번 마법 선택");
    }
    GameObject Fireball(int _dir)
    {
        if (Mana < 0.1f) 
            return null;
        Mana -= 0.1f;
        GameObject fireball = Instantiate(Resources.Load<GameObject>("Prefabs/Spell/Fireball"));
        fireball.transform.position = transform.position;
        fireball.transform.localScale = new Vector3(_dir * fireball.transform.localScale.x, fireball.transform.localScale.y, fireball.transform.localScale.z);
        return fireball;
    }
    GameObject FireBomb(int _dir)
    {
        if (Mana < 0.3f) return null;
        Mana -= 0.3f;
        GameObject bomb = Instantiate(Resources.Load<GameObject>("Prefabs/Spell/FireBomb"));
        bomb.transform.position = transform.position;
        bomb.transform.localScale = new Vector3(_dir * bomb.transform.localScale.x, bomb.transform.localScale.y, bomb.transform.localScale.z);
        return bomb;
    }
    GameObject FireIncendiary(int _dir)
    {
        if (Mana < 0.3f) return null;
        Mana -= 0.3f;
        GameObject incendiary = Instantiate(Resources.Load<GameObject>("Prefabs/Spell/FireIncendiary"));
        incendiary.transform.position = transform.position;
        return incendiary;
    }
    GameObject FireMeteor(int _dir)
    {
        if (Mana < 0.7f) return null;
        Mana -= 0.7f;
        GameObject meteor = Instantiate(Resources.Load<GameObject>("Prefabs/Spell/FireMeteor"));
        meteor.transform.position = transform.position;
        return meteor;
    }

}
