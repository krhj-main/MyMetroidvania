using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bat : Enemy
{
    [SerializeField] float chaseDistance;

    float timer;
    
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        ChangeState(EnemyStates.Bat_Idle);
    }
    protected override void Update()
    {
        base.Update();
        if (!PlayerController.Instance.pState.alive)
        {
            ChangeState(EnemyStates.Bat_Idle);
        }
    }
    protected override void UpdateEnemyStates()
    {
        float _dist = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        switch (GetCurrentEnemyState)
        {
            case EnemyStates.Bat_Idle:
                rig.velocity = Vector2.zero;
                if (_dist < chaseDistance)
                {
                    ChangeState(EnemyStates.Bat_Chase);
                }
                break;
            case EnemyStates.Bat_Chase:
                rig.MovePosition(Vector2.MoveTowards(transform.position, PlayerController.Instance.transform.position, Time.deltaTime * speed));
                FlipBat();
                break;
            case EnemyStates.Bat_Stunned:
                timer += Time.deltaTime;

                if (timer > 0.5f)//stunDuration)
                {
                    ChangeState(EnemyStates.Bat_Idle);
                    timer = 0;

                }
                break;
            case EnemyStates.Bat_Death:
                Death(Random.Range(5,10));
                break;
        }
    }
    protected override void Death(float _destroyTime)
    {
        rig.gravityScale = 12f;
        base.Death(_destroyTime);
    }
    public override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);

        if (health > 0)
        {
            ChangeState(EnemyStates.Bat_Stunned);

        }
        else
        {
            ChangeState(EnemyStates.Bat_Death);
        }
    }
    protected override void ChangeCurrentAnaimation()
    {
        anim.SetBool("isIdle", GetCurrentEnemyState == EnemyStates.Bat_Idle);

        anim.SetBool("isChase", GetCurrentEnemyState == EnemyStates.Bat_Chase);

        anim.SetBool("isHitted", GetCurrentEnemyState == EnemyStates.Bat_Stunned);

        if (GetCurrentEnemyState == EnemyStates.Bat_Death)
        {
            anim.SetTrigger("isDeath");
        }

    }
    void FlipBat()
    {
        sr.flipX = PlayerController.Instance.transform.position.x < transform.position.x;
    }
}