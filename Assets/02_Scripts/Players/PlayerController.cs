using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 moveAxis;
    Rigidbody2D rig;
    Animator anim;
    Singleton gm;
    PlayerStateList pState;
    float gravity;
    bool canDash = true;
    bool dashed;





    [Header ("플레이어 이동속도")]
    [SerializeField] float moveSpeed;

    [Space (5)]
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
    [Header("공격 범위")]
    [SerializeField] Transform XAttack;
    [SerializeField] Transform UpAttack;
    [SerializeField] Transform DownAttack;
    [SerializeField] Vector2 XAttackArea;
    [SerializeField] Vector2 UpAttackArea;
    [SerializeField] Vector2 DownAttackArea;



    [Space(5)]
    [Header("플레이어 반동")]
    [SerializeField] float recoilForce;
    [SerializeField] float recoilTime;
    






    // Start is called before the first frame update
    void Start()
    {
        gm = GetComponent<Singleton>();
        rig = GetComponent<Rigidbody2D>();
        pState = GetComponent<PlayerStateList>();
        anim = GetComponent<Animator>();

        gravity = rig.gravityScale;
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


    void ActiveAttack()
    {
        if (Input.GetKeyDown(KeyCode.X) && !pState.attacking)
        {
            anim.SetTrigger("isAttack");
            //StartCoroutine(Attacking());
        }
    }
    IEnumerator Attacking()
    {
        pState.attacking = true;
        float currentActiveTime = attackActiveTime;
        currentActiveTime-= Time.deltaTime;
        
        if (currentActiveTime > 0 && Input.GetKeyDown(KeyCode.X))
        {
            anim.SetTrigger("isAttack2");
        }
        yield return new WaitForSeconds(playerAttackCooltime);
        pState.attacking = false;
    }
}
