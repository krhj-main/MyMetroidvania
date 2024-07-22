using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 스파이크 오브젝트에 부착 스크립트
// 피격 시 플레이어에게 데미지를 줌
public class Spikes : MonoBehaviour
{
    [SerializeField] int spikesDamage;      // 스파이크의 데미지 설정
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 콜라이더 충돌시
    private void OnCollisionEnter2D(Collision2D _col)
    {
        // 플레이어가 충돌 되었을 때
        if (_col.gameObject.CompareTag("Player") && !PlayerController.Instance.pState.invincible && PlayerController.Instance.Health > 0)
        {
            // 플레이어의 피격 시 메서드 호출
            PlayerController.Instance.TakeDamage(spikesDamage);
            if (PlayerController.Instance.pState.alive)
            {
                PlayerController.Instance.HitStopTime(0f, 5, 0.5f);
            }
        }
    }
}
