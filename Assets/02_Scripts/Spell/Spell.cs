using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Spell : MonoBehaviour
{
    [SerializeField] protected bool debugging;
    [SerializeField] protected SpellSetting spellOption;

    protected CircleCollider2D col;
    protected Collider2D[] hitSize;
    protected Animator anim;
    // Start is called before the first frame update
    protected virtual void Start()
    {
        col = GetComponent<CircleCollider2D>();
        anim = GetComponent<Animator>();
        hitSize = new Collider2D[spellOption.spell_HitLimit];
        Destroy(gameObject, spellOption.spell_LifeTime);
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        transform.position += new Vector3(transform.localScale.x * spellOption.spell_Speed * Time.deltaTime, 0, 0);
    }
    protected virtual void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.CompareTag("Enemy"))
        {
            spellOption.spell_Speed *= 0.1f;
            anim.SetTrigger("isHit");
            Hit(transform.position, spellOption.spell_radius, spellOption.spell_recoilForce);
            col.enabled = false;
        }
    }
    protected virtual void OnDrawGizmos()
    {
        if (!debugging) return;
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position,spellOption.spell_radius);
    }

    protected virtual void Hit(Vector2 _attackArea, float _radius, float _recoilStrength)
    {
        // ���� ���� �ڽ��ȿ� ���ݰ����� ���̾� ������Ʈ�� toHit�迭�� ����
        
        int toHit = Physics2D.OverlapCircleNonAlloc(_attackArea, _radius,
                             hitSize, spellOption.spell_AttackablekLayer);
        // ����Ʈ �ǰ� �� ��ü�� ������ ��
        List<Enemy> hitEnemy = new List<Enemy>();

        // ���ݹ����ȿ� ���� ����ŭ �ݺ��� ����
        for (int i = 0; i < toHit; i++)
        {
            if (hitSize[i].CompareTag("Enemy"))
            {
                Debug.Log("hitEnemy");
                Enemy e = hitSize[i].GetComponent<Enemy>();
                e.EnemyHit(spellOption.spell_Damage, (transform.position - hitSize[i].transform.position).normalized, _recoilStrength);
            }
        }
    }
}
