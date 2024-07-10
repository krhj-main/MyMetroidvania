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





    [Header ("플레이어 이동속도")]
    [SerializeField] float moveSpeed;

    [Space (5)]
    [Header("플레이어 점프")]
    [SerializeField] float jumpForce;
    [SerializeField] int jumpExtra;
    int jumpCurrent;

    [Space(5)]
    [Header("그라운드 체크")]
    [SerializeField] bool Debugging;
    [SerializeField] Transform groundChecker;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundOffset;

    [Space(5)]
    [Header("플레이어 공격")]
    [SerializeField] float playerAttackSpeed;
    [SerializeField] float playerAttackCooltime;

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
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ActiveMove();
        ActiveFlip();
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
    }
}
