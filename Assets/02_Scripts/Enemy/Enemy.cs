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
    protected Rigidbody2D rigid;

    protected virtual void Start()
    {
        rigid = GetComponent<Rigidbody2D>();
        player = PlayerController.Instance;
    }
    // Start is called before the first frame update

    // Update is called once per frame
    protected virtual void Update()
    {
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
    public virtual void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        health -= _damageDone;
        Debug.Log(health);

        if (!isRecoiling)
        {
            rigid.AddForce(-_hitForce * _hitDirection * recoilFactor);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            Attack();
            PlayerController.Instance.HitStopTime(0, 5, 0.5f);
        }
    }
    protected virtual void Attack()
    {
        PlayerController.Instance.TakeDamage(damage);
    }
}
