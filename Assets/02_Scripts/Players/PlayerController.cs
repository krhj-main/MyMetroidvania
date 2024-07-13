using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class PlayerController : MonoBehaviour
{
    Vector2 moveAxis;
    Rigidbody2D rig;
    Animator anim;
    public PlayerStateList pState;
    Casting cast;
    float gravity;
    bool canDash = true;
    bool dashed;
    bool restoreTime;
    float restoreTimeSpeed;

    SpriteRenderer sr;




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

    [Space(5)]
    [Header("�÷��̾� ����")]
    [SerializeField] int playerDamage;
    [SerializeField] float playerAttackSpeed;
    [SerializeField] float playerAttackCooltime;
    [SerializeField] private float attackActiveTime;
    bool isAttack;
    [SerializeField] GameObject slashEffect;
    [SerializeField] float timeBetweenAttack;
    float timeSinceAttack;

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


    [Space(5)]
    [Header("���� ����")]
    [SerializeField] float spell_Fireball_Damage;
    [SerializeField] float spell_Fireball_Speed;
    [SerializeField] float spell_Fireball_LifeTime;

    [SerializeField] int[] spell_Damage;
    [SerializeField] float[] spell_Speed;
    [SerializeField] float[] spell_LifeTime;

    enum Spell
    {
        FIREBALL,
        FIREBOMB,
        FIRESPRAY,
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
                manaStorage.fillAmount = Mana;
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
        RestoreTimeScale();
        UpdateJumpAction();
        Heal();
        ActiveCasting();
        
    }
    private void FixedUpdate()
    {
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
        Gizmos.DrawWireCube(UpAttack.position,UpAttackArea);
        // ��
        Gizmos.DrawWireCube(DownAttack.position,DownAttackArea);
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
        // Health ������Ƽ�� ���� ü�°��� ������Ʈ
        Health -= Mathf.RoundToInt(_damage);
        // �������� �浹 �����Ӹ��� ��� ��������ʵ��� �ڷ�ƾ ����
        StartCoroutine(StopTakingDamage());
        anim.SetTrigger("isHit");
        if (Health <= 0)
        {
            ActiveDeath();
        }
        Debug.Log(health);
    }

    // �÷��̾� ��� �� �޼���
    void ActiveDeath()
    {
        Debug.Log("�÷��̾� ���");
        anim.SetBool("isDeath",Health <= 0);
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
    // ������
    void FlashWhileInvincible()
    {
        // �÷��̾� ��������Ʈ�� ������ �����ð����ȿ��� �����̰Բ� �Ͽ� ǥ�ý�Ŵ
        sr.material.color = pState.invincible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * hitFlashSpeed, 1.0f)) : Color.white;
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
    // ������
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

    // �ǰݽ� ó���� ���� �޼���
    // ������
    IEnumerator StartTimeAgain(float _delay)
    {
        restoreTime = true;
        yield return new WaitForSeconds(_delay);
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
                // X�� ����
                Hit(XAttack, XAttackArea, ref pState.recoilingX, recoilXSpeed);
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
    void Hit(Transform _attackTransform, Vector2 _attackArea, ref bool _recoilDir, float _recoilStrength)
    {
        // ���� ���� �ڽ��ȿ� ���ݰ����� ���̾� ������Ʈ�� toHit�迭�� ����
        Collider2D[] toHit = Physics2D.OverlapBoxAll(_attackTransform.position,_attackArea,0,attackablekLayer);
        // ����Ʈ �ǰ� �� ��ü�� ������ ��
        List<Enemy> hitEnemy = new List<Enemy>();

        // �����ȿ� ���� ���ݰ��� ���̾ 0���� ���ٸ�
        if (toHit.Length > 0)
        {
            // �ݵ��� ����� �� �ְ� �ϰ�
            _recoilDir = true;
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
                e.EnemyHit(playerDamage, (transform.position - toHit[i].transform.position).normalized, _recoilStrength);
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


    ///<summary>
    /// ����, ��� �޼���
    ///</summary>
    /// ������ ����Ʈ �����ٰ� �߰��ϱ�.
    /// 


    void Heal()
    {
        if (Input.GetKey(KeyCode.V) && !pState.dashing && !pState.jumping && health < maxHealth)
        {
            pState.healing = true;
            anim.SetBool("isHeal", true);


            healTimer += Time.deltaTime;
            if (healTimer >= timeToHeal)
            {
                Health++;
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
            pState.casting = true;
            cast.TimeToCasting();
            
        }
        else if (Input.GetKeyDown(KeyCode.A) && pState.casting)
        {
            // ĳ���� ����
            pState.casting = false;
            cast.CancelCasting();
            
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
                    useSpell = Fireball();
                    break;

                case Spell.FIREBOMB:
                    Debug.Log("2 ���� ���");
                    break;

                case Spell.FIRESPRAY:
                    Debug.Log("3 ���� ���");
                    break;
            }
            StartCoroutine(ShootProjectile(useSpell, _dir, spell_LifeTime[(int)spell_Idx]));
        }
    }
    void ChooseSpell()
    {
        currentSpell++;
        currentSpell %= maxSpell;
    }
    GameObject Fireball()
    {
        GameObject fireball = Instantiate(Resources.Load<GameObject>("Prefabs/Spell/Fireball"));
        fireball.transform.position = transform.position;
        return fireball;
    }
    IEnumerator ShootProjectile(GameObject _go,int _dir,float _lifeTime)
    {
        while (_lifeTime <= 0)
        {
            _lifeTime -= Time.deltaTime;
            _go.transform.position += new Vector3(_dir * spell_Fireball_Speed, 0, 0);
        }
        yield return null;
    }

}
