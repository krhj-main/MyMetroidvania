using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crawler: Enemy
{
    [SerializeField] float flipWaitTime;
    [SerializeField] float ledgeCheckX;
    [SerializeField] float ledgeCheckY;

    [SerializeField] LayerMask groundLayer;

    float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Crawler_Idle);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        ChangeState(EnemyStates.Crawler_Idle);
    }

    protected override void Death(float _destroyTime)
    {
        base.Death(_destroyTime);
    }

    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Death(0.05f);
        }

        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Crawler_Idle:
                Vector3 _ledgeCheckStartPoint = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
                Debug.DrawRay(transform.position+_ledgeCheckStartPoint,Vector2.down,Color.red);
                Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
                Debug.DrawRay(transform.position, _wallCheckDir, Color.cyan);

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStartPoint, Vector2.down, ledgeCheckY, groundLayer) ||
                    Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, groundLayer))
                {
                    ChangeState(EnemyStates.Crawler_Flip);
                }

                if (transform.localScale.x > 0)
                {
                    rig.velocity = new Vector2(speed, rig.velocity.y);
                }
                else
                {
                    rig.velocity = new Vector2(-speed, rig.velocity.y);
                }
                break;

            case EnemyStates.Crawler_Flip:
                timer += Time.deltaTime;

                if (timer > flipWaitTime)
                {
                    timer = 0;
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                    ChangeState(EnemyStates.Crawler_Idle);
                }
                break;


        }
    }
}
