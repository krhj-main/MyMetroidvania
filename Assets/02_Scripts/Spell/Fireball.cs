using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fireball : Spell
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
    protected override void OnTriggerEnter2D(Collider2D _col)
    {
        base.OnTriggerEnter2D(_col);
    }
    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();
    }

    protected override void Hit(Vector2 _attackArea, float _radius, float _recoilStrength)
    {
        // 공격 범위 박스안에 공격가능한 레이어 오브젝트를 toHit배열에 저장
        //Collider2D[] toHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, fireball.spell_AttackablekLayer);
        int toHit = Physics2D.OverlapCircleNonAlloc(_attackArea, _radius,hitSize,spellOption.spell_AttackablekLayer);
        // 리스트 피격 적 객체를 생성한 뒤
        List<Enemy> hitEnemy = new List<Enemy>();

        // 공격범위안에 들어온 수만큼 반복문 실행
        for (int i = 0; i < toHit; i++)
        {
            if (hitSize[i].CompareTag("Enemy"))
            {
                Debug.Log("hitEnemy");
                Enemy e = hitSize[i].GetComponent<Enemy>();
                if (e && !hitEnemy.Contains(e))
                {
                    // 적의 Enemy 스크립트에서 피격처리 메서드를 실행
                    e.EnemyHit(spellOption.spell_Damage, (transform.position - hitSize[i].transform.position).normalized, _recoilStrength);
                    // 리스트에 e 객체 추가
                    hitEnemy.Add(e);
                }
            }
        }
    }
}
