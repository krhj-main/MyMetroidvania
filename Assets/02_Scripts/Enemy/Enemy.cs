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
    [SerializeField] protected GameObject blood;


    protected float recoilTimer;
    protected Rigidbody2D rig;
    protected SpriteRenderer sr;
    protected Animator anim;


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

        // 차저
        Charger_Idle,
        Charger_Detect,
        Charger_Charge,



    }
    [SerializeField]
    protected EnemyStates currnetEnemyState;

    protected virtual EnemyStates GetCurrentEnemyState
    {
        get { return currnetEnemyState; }

        set {
            if (currnetEnemyState != value)
            {
                currnetEnemyState = value;

                ChangeCurrentAnaimation();
            }

        }

    }
    protected virtual void Start()
    {
        
    }
    protected virtual void Awake()
    {
        rig = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
        player = PlayerController.Instance;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    protected virtual void Update()
    {
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
        else
        {
            UpdateEnemyStates();
        }
    }

    protected virtual void UpdateEnemyStates()
    {
        
    }
    protected virtual void ChangeCurrentAnaimation()
    {

    }
    protected void ChangeState(EnemyStates _newState)
    {
        GetCurrentEnemyState = _newState;
    }

    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        //Debug.Log(health);

        if (!isRecoiling)
        {
            GameObject _blood = Instantiate(blood, transform.position + (Vector3.up*0.3f), Quaternion.identity);
            Destroy(_blood, 4f);
            rig.velocity = _hitForce * _hitDirection * recoilFactor;
        }
    }


    protected virtual void Death(float _destroyTime)
    {
        Destroy(gameObject,_destroyTime);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && health > 0)
        {
            Attack();

            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0f, 5, 0.5f);
            }
            else
            {


            }

            }
        }
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}
