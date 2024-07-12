using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// 시네머신 카메라에 부착된 스크립트
// 플레이어 추적 역할
public class CamFollowPlayer : MonoBehaviour
{
    CinemachineVirtualCamera cam;               // 시네머신버츄얼카메라 값을 받아오기 위한 객체
    CinemachineVirtualCameraBase camBase;       // 시네머신버츄얼카메라의 값을 설정하기 위한 객체
    CinemachineConfiner2D confiner;             // 시네머신컨파이너(범위) 설정할 객체
    // Start is called before the first frame update
    void Start()
    {

        // 시네머신에 부착된 스크립트이기 때문에
        // 별도로 찾지않고 GetComponent를 활용해 객체 값을 설정
        camBase = GetComponent<CinemachineVirtualCameraBase>();
        cam = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();

        // 카메라가 추적할 트랜스폼 설정 -> 플레이어
        camBase.Follow = GameObject.Find("Player").transform;
        // 카메라의 제한 범위 콜라이더 설정 -> 게임 씬 뷰에서 이름으로 설정, 씬 전환시에도 같은 이름으로 새로 생성
        confiner.m_BoundingShape2D = GameObject.Find("CameraConfiner").GetComponent<PolygonCollider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
