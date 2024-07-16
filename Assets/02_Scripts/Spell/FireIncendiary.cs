using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireIncendiary : Spell
{
    [SerializeField] Vector2 area;
    PlayerController player;
    protected Collider2D[] hitSize;
    // Start is called before the first frame update
    protected override void Start()
    {
        player = PlayerController.Instance;
        spellOption.spell_XTransform = GameObject.Find("XSpell").transform;
        hitSize = new Collider2D[spellOption.spell_HitLimit];
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }
    private void FixedUpdate()
    {
        transform.position = spellOption.spell_XTransform.position;
        if (PlayerController.Instance.pState.lookRight)
        {
            transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
        else //if(!PlayerController.Instance.pState.lookRight)
        {
            transform.localScale = new Vector3(transform.localScale.x * -1, transform.localScale.y, transform.localScale.z);
        }
    }
    protected override void OnDrawGizmos()
    {
        if (!debugging) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position,area);
        //Gizmos.DrawCube(transform.position + offSet,area);
    }
    protected void Hit(Vector2 _attackArea, Vector2 _area, float _recoilStrength)
    {
        //int toHit = Physics2D.OverlapCircleNonAlloc(_attackArea, _radius, hitSize, spellOption.spell_AttackablekLayer);
        int toHit = Physics2D.OverlapBoxNonAlloc(_attackArea,_area,0,hitSize,spellOption.spell_AttackablekLayer);
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
                    e.EnemyHit(spellOption.spell_Damage, (player.transform.position - hitSize[i].transform.position).normalized, _recoilStrength);
                    // ����Ʈ�� e ��ü �߰�
                    hitEnemy.Add(e);
                }
            }
        }
    }
    protected override void OnTriggerEnter2D(Collider2D _col)
    {
        return;
    }
    private void OnTriggerStay2D(Collider2D _col)
    {
        if (_col.CompareTag("Enemy"))
        {
            Debug.Log("hit 3");
            Hit(transform.position, area, spellOption.spell_recoilForce);
        }
    }
}
