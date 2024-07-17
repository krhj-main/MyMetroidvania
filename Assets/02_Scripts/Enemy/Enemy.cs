using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float health;

    [SerializeField] protected float recoilLength;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected PlayerController player;
    [SerializeField] protected float speed;

    [SerializeField] protected float damage;

    protected float recoilTimer;
    protected Rigidbody2D rig;

    protected enum EnemyStates
    {
        // 크라울러
        Crawler_Idle,
        Crawler_Flip,


        // 박쥐
        Bat_Idle,
        Bat_Chase,
        Bat_Stunned,
        Bat_Death,


    }
    protected EnemyStates currnetEnemyState;
    protected virtual void Start()
    {
        
    }
    protected virtual void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateEnemyStates();

        if (health <= 0)
        {
            Destroy(gameObject);
        }

        if (isRecoiling)
        {
            if (recoilTimer < recoilFactor)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    protected virtual void UpdateEnemyStates()
    {
        
    }
    protected void ChangeState(EnemyStates _newState)
    {
        currnetEnemyState = _newState;
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        //Debug.Log(health);

        if (!isRecoiling)
        {
            rig.AddForce(-_hitForce * _hitDirection * recoilFactor);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
            PlayerController.Instance.HitStopTime(0f, 5, 0.5f);
        }
    }
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}
