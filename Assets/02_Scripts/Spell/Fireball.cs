using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Fireball : MonoBehaviour
{
    [SerializeField] SpellSetting fireball;

    Collider2D[] hitSize;
    // Start is called before the first frame update
    void Start()
    {
        hitSize = new Collider2D[fireball.spell_HitLimit];
        Destroy(gameObject,fireball.spell_LifeTime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += new Vector3(transform.localScale.x * 3f * Time.deltaTime, 0,0);
    }
    private void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.CompareTag("Enemy"))
        {
            Hit(transform.position, fireball.spell_recoilForce);
        }
    }

    void Hit(Vector2 _attackArea, float _recoilStrength)
    {
        // 공격 범위 박스안에 공격가능한 레이어 오브젝트를 toHit배열에 저장
        //Collider2D[] toHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, fireball.spell_AttackablekLayer);
        int toHit = Physics2D.OverlapCircleNonAlloc(_attackArea,2f,hitSize,fireball.spell_AttackablekLayer);
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
                    e.EnemyHit(fireball.spell_Damage, (transform.position - hitSize[i].transform.position).normalized, _recoilStrength);
                    // 리스트에 e 객체 추가
                    hitEnemy.Add(e);
                }
            }

            // Enemy 스크립트가 붙어진 e 객체에
            // 공격범위안에 들어온 오브젝트중 Enemy 스크립트가 붙어진 객체를 저장하고
            

            // e 객체에 정상적으로 Enemy가 붙어진 적이 감지됐고, 피격 리스트에 포함되지 않았다면
            
        }
    }
}
