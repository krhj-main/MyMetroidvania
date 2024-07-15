using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBomb : Spell
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override void OnTriggerEnter2D(Collider2D _col)
    {
        base.OnTriggerEnter2D(_col);
    }

    protected override void Hit(Vector2 _attackArea, float _radius, float _recoilStrength)
    {
        base.Hit(_attackArea, _radius, _recoilStrength);
    }
}
