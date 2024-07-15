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
        // ���� ���� �ڽ��ȿ� ���ݰ����� ���̾� ������Ʈ�� toHit�迭�� ����
        //Collider2D[] toHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, fireball.spell_AttackablekLayer);
        int toHit = Physics2D.OverlapCircleNonAlloc(_attackArea, _radius,hitSize,spellOption.spell_AttackablekLayer);
        // ����Ʈ �ǰ� �� ��ü�� ������ ��
        List<Enemy> hitEnemy = new List<Enemy>();

        // ���ݹ����ȿ� ���� ����ŭ �ݺ��� ����
        for (int i = 0; i < toHit; i++)
        {
            if (hitSize[i].CompareTag("Enemy"))
            {
                Debug.Log("hitEnemy");
                Enemy e = hitSize[i].GetComponent<Enemy>();
                if (e && !hitEnemy.Contains(e))
                {
                    // ���� Enemy ��ũ��Ʈ���� �ǰ�ó�� �޼��带 ����
                    e.EnemyHit(spellOption.spell_Damage, (transform.position - hitSize[i].transform.position).normalized, _recoilStrength);
                    // ����Ʈ�� e ��ü �߰�
                    hitEnemy.Add(e);
                }
            }
        }
    }
}
