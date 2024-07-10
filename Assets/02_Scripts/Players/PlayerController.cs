using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Vector2 moveAxis;
    Rigidbody2D rig;
    Animator anim;
    [SerializeField] float moveSpeed;

    [SerializeField] float jumpForce;
    [SerializeField] GameObject groundcheker;

    // Start is called before the first frame update
    void Start()
    {
        rig = GetComponent<Rigidbody2D>();
        //anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        GetInput();
        ActiveMove();
    }
    void GetInput()
    {
        moveAxis = new Vector2(Input.GetAxis("Horizontal"),Input.GetAxis("Vertical"));
        if (Grounded())
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                ActiveJump();
            }
        }
    }
    void ActiveMove()
    {
        rig.velocity = new Vector2(moveAxis.x * moveSpeed, rig.velocity.y);
    }
    void ActiveJump()
    {
        rig.velocity = new Vector2(rig.velocity.x, jumpForce);
    }
    bool Grounded()
    {
        return true;
    }
}
