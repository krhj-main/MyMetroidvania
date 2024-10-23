using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 플레이어 상태를 스크립트
// 플레이어 상태 리스트
public class PlayerStateList : MonoBehaviour
{
    public bool lookRight;              // 오른쪽을 바라보고 있는지 여부
    public bool attacking;              // 공격중인지 여부
    public bool dashing;                // 대시중인지 여부
    public bool invincible;             // 무적 상태인지 여부
    public bool recoilingX;             // X축 반동 상태 여부
    public bool recoilingY;             // Y축 반동 상태 여부
    public bool jumping;                // 점프중인지 여부
    public bool healing;                // 힐 사용 중인지 여부
    public bool casting;                // 마법 스킬을 캐스팅 중인지 여부
    public bool cutscene = false;               // 컷 씬 진행중인지 여부
    public bool alive = true;                  // 플레이어가 살아있는지 여부
}
