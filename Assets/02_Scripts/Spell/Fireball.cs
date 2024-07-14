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
        // ���� ���� �ڽ��ȿ� ���ݰ����� ���̾� ������Ʈ�� toHit�迭�� ����
        //Collider2D[] toHit = Physics2D.OverlapBoxAll(_attackTransform.position, _attackArea, 0, fireball.spell_AttackablekLayer);
        int toHit = Physics2D.OverlapCircleNonAlloc(_attackArea,2f,hitSize,fireball.spell_AttackablekLayer);
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
                    e.EnemyHit(fireball.spell_Damage, (transform.position - hitSize[i].transform.position).normalized, _recoilStrength);
                    // ����Ʈ�� e ��ü �߰�
                    hitEnemy.Add(e);
                }
            }

            // Enemy ��ũ��Ʈ�� �پ��� e ��ü��
            // ���ݹ����ȿ� ���� ������Ʈ�� Enemy ��ũ��Ʈ�� �پ��� ��ü�� �����ϰ�
            

            // e ��ü�� ���������� Enemy�� �پ��� ���� �����ư�, �ǰ� ����Ʈ�� ���Ե��� �ʾҴٸ�
            
        }
    }
}
