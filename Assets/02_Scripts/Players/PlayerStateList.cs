using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어에 부착된 스크립트
// 플레이어 상태 나타냄
public class PlayerStateList : MonoBehaviour
{
    public bool lookRight;              // 오른쪽을 보고있는지 여부
    public bool attacking;              // 공격중인 상태인지 여부
    public bool dashing;                // 대쉬중인 상태의 여부
    public bool invincible;             // 피격 시 무적상태 여부
    public bool recoilingX;             // X축 반동 실행 여부
    public bool recoilingY;             // Y축 반동 실행 여부
    public bool jumping;                // 점프중인 상태 여부
    public bool healing;                // 힐 스펠 상태 여부
    public bool casting;                // 스펠 사용위해 캐스팅 중 여부
    public bool cutscene;               // 컷 화면전환 중인지 여부
    public bool alive;                  // 플레이어가 살아있는지 여부
}
