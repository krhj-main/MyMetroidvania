using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// ������ũ ������Ʈ�� ���� ��ũ��Ʈ
// �ǰ� �� �÷��̾�� �������� ��
public class Spikes : MonoBehaviour
{
    [SerializeField] int spikesDamage;      // ������ũ�� ������ ����
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // �ݶ��̴� �浹��
    private void OnCollisionEnter2D(Collision2D _col)
    {
        // �÷��̾ �浹 �Ǿ��� ��
        if (_col.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible)
        {
            // �÷��̾��� �ǰ� �� �޼��� ȣ��
            PlayerController.Instance.TakeDamage(spikesDamage);
            PlayerController.Instance.HitStopTime(0, 5, 0.5f);
        }
    }
}
