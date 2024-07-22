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




    [Header("�÷��̾� �̵��ӵ�")]
    [SerializeField] float moveSpeed;

    [Space(5)]
    [Header("�÷��̾� ����")]
    [SerializeField] float jumpForce;
    [SerializeField] int jumpExtra;
    int jumpCurrent;

    [Space(5)]
    [Header("�÷��̾� ����")]
    [SerializeField] float dodgeSpeed;
    [SerializeField] float dodgeCoolTime;
    [SerializeField] float dodgeTime;

    [Space(5)]
    [Header("�׶��� üũ")]
    [SerializeField] bool Debugging;
    [SerializeField] Transform groundChecker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundOffset;
    public bool onGround;

    [Space(5)]
    [Header("�÷��̾� ����")]
    [SerializeField] int playerDamage;
    [SerializeField] float playerAttackSpeed;
    [SerializeField] float playerAttackCooltime;
    [SerializeField] private float attackActiveTime;
    bool isAttack;
    [SerializeField] GameObject slashEffect;
    [SerializeField] float timeBetweenAttack;

    [Space(5)]
    [Header("�ǰ� �˹�")]
    [SerializeField] int recoilingXSteps = 5;
    [SerializeField] int recoilingYSteps = 5;
    [SerializeField] float recoilXSpeed = 100;
    [SerializeField] float recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;

    [Space(5)]
    [Header("���� ����")]
    [SerializeField] Transform XAttack;
    [SerializeField] Transform UpAttack;
    [SerializeField] Transform DownAttack;
    [SerializeField] Vector2 XAttackArea;
    [SerializeField] Vector2 UpAttackArea;
    [SerializeField] Vector2 DownAttackArea;
    [SerializeField] LayerMask attackablekLayer;

    [Space(5)]
    [Header("�÷��̾� ü��")]
    [SerializeField] public int maxHealth;
    public int health;
    [SerializeField] GameObject bloodSpurt;
    [SerializeField] float hitFlashSpeed;

    [Space(5)]
    [Header("�� ���� ����")]
    float healTimer;
    [SerializeField] float timeToHeal;


    // �÷��̾� ü�� ���� ó���� ��������Ʈ
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangedCallback;

    [Space(5)]
    [Header("�÷��̾� ����")]
    [SerializeField] Image manaStorage;
    [SerializeField] float mana;
    [SerializeField] float manaDrainSpeed;
    [SerializeField] float manaGain;


    [Space(5)]
    [Header("�÷��̾� �ݵ�")]
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
    // �÷��̾� �Է� �νĿ� ���� ó��
    void GetInput()
    {
        // �÷��̾� �����¿� �ν��� ����2 ���� ����
        moveAxis = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));

        // ���� �ƴѰ��� �ִٸ� ���� �ִϸ��̼��� ����
        anim.SetBool("isJump", !Grounded());

        // C Ű�� �Է¹ް�, �ִ� �������� 0���� ũ�ų� �÷��̾ ���� ����ִٸ�
        if (Input.GetKeyDown(KeyCode.C) && (jumpCurrent > 0 || !pState.jumping))
        {
            jumpCurrent--;
            ActiveJump();
            // ���� ���� 1�� ���̰�, ���� �޼��� ����
        }

        // ���� XŰ�� �Է¹޾� �Ҹ����� ����
        isAttack = Input.GetKeyDown(KeyCode.X);

        // �̴ϸʿ���
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

    // �÷��̾� �̵� �޼���
    void ActiveMove()
    {

        // �Է¹��� ����2�� x���� �̵��ӵ��� ���� �̵�
        rig.velocity = new Vector2(moveAxis.x * moveSpeed, rig.velocity.y);

        // �̵����� 0 �� �ƴ϶�� �ȱ� �ִϸ��̼� ����
        anim.SetBool("isWalk",moveAxis.x!=0);
    }

    
    // �̵� �Է°��� ���� �÷��̾� �¿� ����
    void ActiveFlip()
    {
        // �̵����� ������ 
        if (moveAxis.x > 0)
        {
            // �÷��̾��� �������� 1�� �ؼ� �������� �ٶ󺸰� ����
            transform.localScale = new Vector2(1, transform.localScale.y);
            // �÷��̾� ���°� �������� �����ִٰ� ����
            pState.lookRight = true;
        }
        // �̵����� �������
        if (moveAxis.x < 0)
        {
            // �÷��̾� �������� -1�� �� ������ �ٶ󺸰� ����
            transform.localScale = new Vector2(-1, transform.localScale.y);
            // �÷��̾� ���°� ������ �����ִٰ� ����
            pState.lookRight = false;
        }
    }

    // �÷��̾� ���� �޼���
    void ActiveJump()
    {
        // �÷��̾� ���� ������
        pState.jumping = true;
        // x �̵����� ������ä ������ ������ ����ŭ y������ �̵�
        rig.velocity = new Vector2(rig.velocity.x, jumpForce);        
    }
    void UpdateJumpAction()
    {
        // ���� ���� ����ִٸ� ���� ���ɼ� = �ִ� ���� ���� ���� ����
        if (Grounded())
        {
            pState.jumping = false;
            jumpCurrent = jumpExtra;
        }
    }

    // ������� �����̵� ��ǿ� ���� �޼���
    void ActiveDodge()
    {
        // ZŰ�� ��������, �뽬�� ������ ���°�, �뽬���� �ƴϸ鼭 ���� ����ִٸ�
        if (Input.GetKeyDown(KeyCode.Z) && canDash && !dashed && Grounded())
        {
            dashed = true;
            StartCoroutine(Dash());
            // �뽬���� ���·� �ٲٰ� �뽬 �ڷ�ƾ�� ����
        }
        // �׳� ������ ����ִٸ�
        if(Grounded())
        {
            // �뽬������ �ʴٰ� ����
            dashed = false;
            
        }
    }

    // �뽬 �ڷ�ƾ
    IEnumerator Dash()
    {
        // �뽬���϶��� �뽬�� �Ұ����ؾ��ϹǷ�, false
        canDash = false;
        // �÷��̾� ���� �뽬 ������ ����
        pState.dashing = true;
        // �뽬 �߿� �߷��� ������ ���� �ʰ� �Ͻ������� �߷°� 0���� ����

        gameObject.layer = 7;
        gameObject.tag = "InvinciblePlayer";
        // �뽬�߿��� ���ݹ��� �ʰ� ���͸� ����ϰԲ� Ʈ���ŷ� �ٲٰ�
        rig.gravityScale = 0;
        
        // ���� ����ְ� �̵� y�ప�� 0�� ���ų� ũ�� ������ ����
        if (Grounded() && moveAxis.y >= 0)
        {
            anim.SetTrigger("isRoll");
        }
        // ���� ����ִµ� y���� �Ʒ��� ������������ �����̵� ����
        else if (Grounded() && moveAxis.y < 0)
        {
            anim.SetTrigger("isSlide");
        }
        // �÷��̾� �¿� ���¿� ���� ���� ����
        int _dir = pState.lookRight ? 1 : -1;
        // ���� �� * �ӵ����� ���� �÷��̾� �ӵ��� ����
        rig.velocity = new Vector2(_dir * dodgeSpeed, 0);
        // ȸ�� ����ð��� ��ٸ� ��
        yield return new WaitForSeconds(dodgeTime);
        // �÷��̾ �뽬���°� �ƴ϶�� �ٲ�
        pState.dashing = false;
        // �߷µ� ���������� �ʱ�ȭ
        rig.gravityScale = gravity;
        // ���� �뽬 ��Ÿ���� ������
        yield return new WaitForSeconds(dodgeCoolTime);
        // �뽬�� �ٽ� �����ϰԲ� true�� ����
        canDash = true;
        gameObject.layer = 6;
        gameObject.tag = "Player";
        // �ٽ� ���Ϳ� �浹�ǰ� Ʈ���Ÿ� ����
    }

    // �÷��̾ �����ִ��� �ƴ��� �Ǻ��ϴ� �޼���
    bool Grounded()
    {
        // �÷��̾ �ٿ��� �׶���üĿ �������Ʈ�� Ȱ���� groundLayer�� ����
        bool isGround = Physics2D.OverlapCircle(groundChecker.position, groundOffset, groundLayer);
        return isGround;
    }

    // ������ ����� �޼���
    private void OnDrawGizmos()
    {
        // ����� �Ҹ����� false �� ����� �׸��� ����
        if (!Debugging) return;

        // ������� �÷� ����
        Gizmos.color = Color.red;
        // �׶���üĿ�� ������ Ȯ���ϱ����� ��ü ����� �׸���
        Gizmos.DrawSphere(groundChecker.position,groundOffset);

        // �÷��̾��� ���ݹ����� �����ϱ� ���� ���̾�ť�� ����� �׸���
        // �¿�
        Gizmos.DrawWireCube(XAttack.position,XAttackArea);
        // ��
        //Gizmos.DrawWireCube(UpAttack.position,UpAttackArea);
        // ��
        //Gizmos.DrawWireCube(DownAttack.position,DownAttackArea);
    }


    // �÷��̾� ����, �ǰݽ� �ݵ����� �з����� ���� �� �޼���
    // ������
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

        // �ǰ� ȿ�� ���߱�
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
    // X������ �ݵ��� ����� �� �����ֱ� ���� �޼���
    void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }
    // Y������ �ݵ� ���� �� �����ֱ� ���� �޼���
    void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }


    //�÷��̾� �ǰ� �� ü�� ���� ó��
    public void TakeDamage(float _damage)
    {
        if (pState.alive)
        {
            // Health ������Ƽ�� ���� ü�°��� ������Ʈ
            Health -= Mathf.RoundToInt(_damage);
            
            if (Health <= 0)
            {
                anim.SetTrigger("isDeath");
                StartCoroutine(Death());
            }
            else
            {
                anim.SetTrigger("isHit");
                // �������� �浹 �����Ӹ��� ��� ��������ʵ��� �ڷ�ƾ ����
                StartCoroutine(StopTakingDamage());
            }
        }
    }

    // �ǰ� ó���� ���� �ð��� �ֱ����� �ڷ�ƾ
    IEnumerator StopTakingDamage()
    {
        // �÷��̾ ��� ������ �ǰԲ� invincible�� true�� ����
        pState.invincible = true;
        // �ǰ� �𳯸��� ��ƼŬ ȿ���� ������Ű��
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        // 1.5�� �ڿ� ��ƼŬ ȿ�� ����
        Destroy(_bloodSpurtParticles, 1.5f);
        //anim.SetTrigger("isHitted");
        // 1�� ��
        yield return new WaitForSeconds(1f);
        // �÷��̾ �ٽ� �ǰݵ� �� �ְ� invincible�� false�� ����
        pState.invincible = false;
    }

    // �����ð����� �÷��̾ ���������� Ȯ�� �� �� �ֵ����ϴ� �޼���
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
        // �÷��̾� ��������Ʈ�� ������ �����ð����ȿ��� �����̰Բ� �Ͽ� ǥ�ý�Ŵ
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

    // �÷��̾ �ǰ� �� ������ �ð��� �ٽ� �ǵ����� �޼��� ������
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
    // �÷��̾� �ǰݽ� �ð��� �������Բ� �ϴ� �޼���    
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

    // �ǰݽ� ó���� ���� �޼���    
    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
    }
    IEnumerator Death()
    {
        pState.alive = false;
        Time.timeScale = 1f;
        // �ǰ� �𳯸��� ��ƼŬ ȿ���� ������Ű��
        GameObject _bloodSpurtParticles = Instantiate(bloodSpurt, transform.position, Quaternion.identity);
        // 1.5�� �ڿ� ��ƼŬ ȿ�� ����
        Destroy(_bloodSpurtParticles, 1.5f);
        

        yield return new WaitForSeconds(0.9f);

        StartCoroutine(UIManager.Instance.ActiveDeathScreen());
    }
    // �÷��̾� ���� ���� �޼���
    void ActiveAttack()
    {
        // �÷��̾� �Է��� �޾Ƽ� isAttack�� true�� �ȴٸ�
        if (isAttack)
        {
            // ���� �ִϸ��̼��� �����ϰ�
            anim.SetTrigger("isAttack");
            
            // ���� ����ְ� y�� �̵����� 0���϶�� 
            if (Grounded() && moveAxis.y <= 0)
            {
                int _recoilLR = pState.lookRight ? 1 : -1;
                // X�� ����
                Hit(XAttack, XAttackArea, ref pState.recoilingX, Vector2.right * _recoilLR, recoilXSpeed);
            }
            // y�� �̵����� ���� ���Ѵٸ�
            else if (moveAxis.y > 0)
            {
                // ���� ����
            }
            // ���� ������� ������ y�� �̵����� �Ʒ����
            else if (!Grounded() && moveAxis.y < 0)
            {
                // ���� �� �Ʒ� ����
            }
        }
    }

    // �÷��̾� ���� ���� ó�� �޼���
    // ���� ��ġ Ʈ������, ���� ����, �ݵ��� ó������, �ݵ��� ����
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilBool,Vector2 _recoilDir, float _recoilStrength)
    {
        // ���� ���� �ڽ��ȿ� ���ݰ����� ���̾� ������Ʈ�� toHit�迭�� ����
        Collider2D[] toHit = Physics2D.OverlapBoxAll(_attackTransform.position,_attackArea,0,attackablekLayer);
        // ����Ʈ �ǰ� �� ��ü�� ������ ��
        List<Enemy> hitEnemy = new List<Enemy>();

        // �����ȿ� ���� ���ݰ��� ���̾ 0���� ���ٸ�
        if (toHit.Length > 0)
        {
            // �ݵ��� ����� �� �ְ� �ϰ�
            _recoilBool = true;
        }

        // ���ݹ����ȿ� ���� ����ŭ �ݺ��� ����
        for (int i = 0; i < toHit.Length; i++)
        {
            // Enemy ��ũ��Ʈ�� �پ��� e ��ü��
            // ���ݹ����ȿ� ���� ������Ʈ�� Enemy ��ũ��Ʈ�� �پ��� ��ü�� �����ϰ�
            Enemy e = toHit[i].GetComponent<Enemy>();

            // e ��ü�� ���������� Enemy�� �پ��� ���� �����ư�, �ǰ� ����Ʈ�� ���Ե��� �ʾҴٸ�
            if (e && !hitEnemy.Contains(e))
            {
                // ���� Enemy ��ũ��Ʈ���� �ǰ�ó�� �޼��带 ����
                e.EnemyHit(playerDamage, _recoilDir, _recoilStrength);
                // ����Ʈ�� e ��ü �߰�
                hitEnemy.Add(e);
            }
            // ���� ���� �ȿ� ���� ��ü�� �±װ� Enemy ���
            if (toHit[i].CompareTag("Enemy"))
            {
                // �÷��̾ �������� ��, ������ managain ��ŭ ȸ��
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
    /// ����, ��� �޼���
    ///</summary>
    /// ������ ����Ʈ �����ٰ� �߰��ϱ�.
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
            // ĳ���� ����
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
            // ���� ���
            switch (spell_Idx)
            {
                case Spell.FIREBALL:
                    Debug.Log("1 ���� ���");
                    useSpell = Fireball(_dir);
                    break;

                case Spell.FIREBOMB:
                    Debug.Log("2 ���� ���");
                    useSpell = FireBomb(_dir);
                    break;

                case Spell.FIRESPRAY:
                    Debug.Log("3 ���� ���");
                    useSpell = FireIncendiary(_dir);
                    break;
                case Spell.FIREMETEOR:
                    Debug.Log("4 ���� ���");
                    useSpell = FireMeteor(_dir);
                    break;

            }
        }
    }
    void ChooseSpell()
    {
        currentSpell++;
        currentSpell %= maxSpell;
        Debug.Log($"{currentSpell+1} �� ���� ����");
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
