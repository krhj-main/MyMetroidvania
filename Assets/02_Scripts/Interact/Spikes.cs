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
        if (_col.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && PlayerController.Instance.Health > 0)
        {
            // �÷��̾��� �ǰ� �� �޼��� ȣ��
            PlayerController.Instance.TakeDamage(spikesDamage);
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0f, 5, 0.5f);
            }
        }
    }
}
