using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Charger : Enemy
{
    [SerializeField] float ledgeCheckX;
    [SerializeField] float ledgeCheckY;
    [SerializeField] float chargeSpeedMultiplier;
    [SerializeField] float chargeDuration;
    [SerializeField] float jumpForce;
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
            ChangeState(EnemyStates.Charger_Idle);
        }
    }
    protected override void Awake()
    {
        base.Awake();
        ChangeState(EnemyStates.Charger_Idle);
        rig.gravityScale = 12f;
    }


    protected override void UpdateEnemyStates()
    {
        if (health <= 0)
        {
            Destroy(gameObject, 0.05f);
        }
        Vector3 _ledgeCheckStartPoint = transform.localScale.x > 0 ? new Vector3(ledgeCheckX, 0) : new Vector3(-ledgeCheckX, 0);
        //Debug.DrawRay(transform.position + _ledgeCheckStartPoint, Vector2.down, Color.red);
        Vector2 _wallCheckDir = transform.localScale.x > 0 ? transform.right : -transform.right;
        //Debug.DrawRay(transform.position, _wallCheckDir, Color.cyan);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Charger_Idle:

                if (!Physics2D.Raycast(transform.position + _ledgeCheckStartPoint, Vector2.down, ledgeCheckY, groundLayer) ||
                    Physics2D.Raycast(transform.position, _wallCheckDir, ledgeCheckX, groundLayer))
                {
                    transform.localScale = new Vector2(transform.localScale.x * -1, transform.localScale.y);
                }
                
                RaycastHit2D _hit = Physics2D.Raycast(transform.position + _ledgeCheckStartPoint, _wallCheckDir, ledgeCheckX * 10f);
                Debug.DrawRay(transform.position + _ledgeCheckStartPoint, _wallCheckDir * 10f * ledgeCheckX, Color.red);
                if (_hit.collider != null && _hit.collider.gameObject.CompareTag("Player"))
                {
                    Debug.Log("Player");
                    ChangeState(EnemyStates.Charger_Detect);
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
            case EnemyStates.Charger_Detect:
                rig.velocity = new Vector2(0,jumpForce);

                ChangeState(EnemyStates.Charger_Charge);
                break;
            case EnemyStates.Charger_Charge:
                timer += Time.deltaTime;

                if (timer < chargeDuration)
                {
                    if (Physics2D.Raycast(transform.position, Vector2.down, ledgeCheckY, groundLayer))
                    {
                        if (transform.localScale.x > 0)
                        {
                            rig.velocity = new Vector2(speed * chargeSpeedMultiplier, rig.velocity.y);
                        }
                        else
                        {
                            rig.velocity = new Vector2(-speed * chargeSpeedMultiplier, rig.velocity.y);
                        }
                    }
                    else
                    {
                        rig.velocity = new Vector2(0, rig.velocity.y);
                    }
                }
                else
                {
                    timer = 0;
                    ChangeState(EnemyStates.Charger_Idle);
                }
                break;

        }
    }

    protected override void ChangeCurrentAnaimation()
    {
        if (GetCurrentEnemyState == EnemyStates.Charger_Idle)
        {
            anim.speed = 1;
        }
        if (GetCurrentEnemyState == EnemyStates.Charger_Charge)
        {
            anim.speed = chargeSpeedMultiplier;
        }
    }
}
