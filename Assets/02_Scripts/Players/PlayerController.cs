using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    Vector2 moveAxis;
    Rigidbody2D rig;
    Animator anim;
    public PlayerStateList pState;
    float gravity;
    bool canDash = true;
    bool dashed;
    bool restoreTime;
    float restoreTimeSpeed;

    SpriteRenderer sr;




    [Header("플레이어 이동속도")]
    [SerializeField] float moveSpeed;

    [Space(5)]
    [Header("플레이어 점프")]
    [SerializeField] float jumpForce;
    [SerializeField] int jumpExtra;
    int jumpCurrent;

    [Space(5)]
    [Header("플레이어 닷지")]
    [SerializeField] float dodgeSpeed;
    [SerializeField] float dodgeCoolTime;
    [SerializeField] float dodgeTime;

    [Space(5)]
    [Header("그라운드 체크")]
    [SerializeField] bool Debugging;
    [SerializeField] Transform groundChecker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundOffset;

    [Space(5)]
    [Header("플레이어 공격")]
    [SerializeField] int playerDamage;
    [SerializeField] float playerAttackSpeed;
    [SerializeField] float playerAttackCooltime;
    [SerializeField] private float attackActiveTime;
    bool isAttack;
    [SerializeField] GameObject slashEffect;
    [SerializeField] float timeBetweenAttack;
    float timeSinceAttack;

    [Space(5)]
    [Header("피격 넉백")]
    [SerializeField] int recoilingXSteps = 5;
    [SerializeField] int recoilingYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;

    [Space(5)]
    [Header("공격 범위")]
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
                health = Mathf.Clamp(health,0,maxHealth);

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
            return Mana;
        }
        set
        {
            if (mana != value)
            {
                mana = Mathf.Clamp(mana,0,1);
                manaStorage.fillAmount = Mana;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        pState = GetComponent<PlayerStateList>();
        anim = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();

        gravity = rig.gravityScale;

        Health = maxHealth;
        Mana = mana;
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        if (pState.dashing) return;
        ActiveMove();
        ActiveFlip();
        ActiveAttack();
        ActiveDodge();
        FlashWhileInvincible();
    }
    void GetInput()
    {
        moveAxis = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        anim.SetBool("isJump", !Grounded());
        if (Input.GetKeyDown(KeyCode.C) && (jumpCurrent > 0 || Grounded()))
        {
            
            jumpCurrent--;
            ActiveJump();
        }
        isAttack = Input.GetKeyDown(KeyCode.X);
        
    }
    void ActiveMove()
    {
        rig.velocity = new Vector2(moveAxis.x * moveSpeed, rig.velocity.y);
        anim.SetBool("isWalk",moveAxis.x!=0);
    }
    void ActiveFlip()
    {
        if (moveAxis.x > 0)
        {
            transform.localScale = new Vector2(1, transform.localScale.y);
            pState.lookRight = true;
        }
        if (moveAxis.x < 0)
        {
            transform.localScale = new Vector2(-1, transform.localScale.y);
            pState.lookRight = false;
        }
    }
    void ActiveJump()
    {
        rig.velocity = new Vector2(rig.velocity.x, jumpForce);
        if (Grounded())
        {
            jumpCurrent = jumpExtra;
        }
    }

    void ActiveDodge()
    {
        if (Input.GetKeyDown(KeyCode.Z) && canDash && !dashed && Grounded())
        {
            dashed = true;
            StartCoroutine(Dash());
        }
        if(Grounded())
        {
            dashed = false;
        }
    }
    IEnumerator Dash()
    {
        canDash = false;
        pState.dashing = true;
        rig.gravityScale = 0;
        if (Grounded() && moveAxis.y >= 0)
        {
            anim.SetTrigger("isRoll");
        }
        else if (Grounded() && moveAxis.y < 0)
        {
            anim.SetTrigger("isSlide");
        }
        int _dir = pState.lookRight ? 1 : -1;
        rig.velocity = new Vector2(_dir * dodgeSpeed, 0);
        yield return new WaitForSeconds(dodgeTime);
        pState.dashing = false;
        rig.gravityScale = gravity;
        yield return new WaitForSeconds(dodgeCoolTime);
        canDash = true;
    }

    bool Grounded()
    {
        bool isGround = Physics2D.OverlapCircle(groundChecker.position, groundOffset, groundLayer);
        return isGround;
    }
    private void OnDrawGizmos()
    {
        if (!Debugging) return;

        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundChecker.position,groundOffset);

        Gizmos.DrawWireCube(XAttack.position,XAttackArea);
        Gizmos.DrawWireCube(UpAttack.position,UpAttackArea);
        Gizmos.DrawWireCube(DownAttack.position,DownAttackArea);
    }


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
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

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
    public void TakeDamage(float _damage)
    {
        Health -= Mathf.RoundToInt(_damage);
        StartCoroutine(StopTakingDamage());
        Debug.Log(health);
    }
    IEnumerator StopTakingDamage()
    {
        pState.invincible = true;
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        Destroy(_bloodSpurtParticles, 1.5f);
        //anim.SetTrigger("isHitted");
        yield return new WaitForSeconds(1f);
        pState.invincible = false;
    }
    void FlashWhileInvincible()
    {
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
    }
    public void HitStopTime(float _newTimeScale, int _restoreSpeed, float _delay)
    {
        restoreTimeSpeed = _restoreSpeed;
        Time.timeScale = _newTimeScale;

        if (_delay > 0)
        {
            StopCoroutine(StartTimeAgain(_delay));
            StartCoroutine(StartTimeAgain(_delay));
        }
        else
        {
            restoreTime = true;
        }
    }
    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }
    void ActiveAttack()
    {
        if (Input.GetKeyDown(KeyCode.X) && isAttack)
        {
            anim.SetTrigger("isAttack");

            if (Grounded() && moveAxis.y < 0 || moveAxis.y == 0)
            {
                // X축 공격
                Hit(XAttack, XAttackArea, ref pState.recoilingX, recoilXSpeed);
            }
            else if (moveAxis.y > 0)
            {
                // 위쪽 공격
            }
            else if (!Grounded() && moveAxis.y < 0)
            {
                // 점프 후 아래 공격
            }
        }
    }
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        Collider2D[] toHit = Physics2D.OverlapBoxAll(_attackTransform.position,_attackArea,0,attackablekLayer);
        List<Enemy> hitEnemy = new List<Enemy>();

        if (toHit.Length > 0)
        {
            _recoilDir = true;
        }

        for (int i = 0; i < toHit.Length; i++)
        {
            Enemy e = toHit[i].GetComponent<Enemy>();

            if (e && !hitEnemy.Contains(e))
            {
                e.EnemyHit(playerDamage, (transform.position - toHit[i].transform.position).normalized, _recoilStrength);
                hitEnemy.Add(e);
            }
            if (toHit[i].CompareTag("Enemy"))
            {
                Mana += manaGain;
            }
        }
    }

}
