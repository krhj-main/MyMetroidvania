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
        // 공격 범위 박스안에 공격가능한 레이어 오브젝트를 toHit배열에 저장
        
        int toHit = Physics2D.OverlapCircleNonAlloc(_attackArea, _radius,
                             hitSize, spellOption.spell_AttackablekLayer);
        // 리스트 피격 적 객체를 생성한 뒤
        List<Enemy> hitEnemy = new List<Enemy>();

        // 공격범위안에 들어온 수만큼 반복문 실행
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
