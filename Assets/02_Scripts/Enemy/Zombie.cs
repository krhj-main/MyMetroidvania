using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zombie : Enemy
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    protected override void Awake()
    {
        base.Awake();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!isRecoiling)
        {
            transform.position = Vector2.MoveTowards(transform.position, new Vector2(PlayerController.Instance.transform.position.x, transform.position.y),speed * Time.deltaTime);
        }
    }

    public  override void EnemyHit(float _damageDone, Vector2 _hitDirection, float _hitForce)
    {
        base.EnemyHit(_damageDone, _hitDirection, _hitForce);


    }
}
