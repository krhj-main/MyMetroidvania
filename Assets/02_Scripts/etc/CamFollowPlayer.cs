using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

// �ó׸ӽ� ī�޶� ������ ��ũ��Ʈ
// �÷��̾� ���� ����
public class CamFollowPlayer : MonoBehaviour
{
    CinemachineVirtualCamera cam;               // �ó׸ӽŹ����ī�޶� ���� �޾ƿ��� ���� ��ü
    CinemachineVirtualCameraBase camBase;       // �ó׸ӽŹ����ī�޶��� ���� �����ϱ� ���� ��ü
    CinemachineConfiner2D confiner;             // �ó׸ӽ������̳�(����) ������ ��ü
    // Start is called before the first frame update
    void Start()
    {

        // �ó׸ӽſ� ������ ��ũ��Ʈ�̱� ������
        // ������ ã���ʰ� GetComponent�� Ȱ���� ��ü ���� ����
        camBase = GetComponent<CinemachineVirtualCameraBase>();
        cam = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();

        // ī�޶� ������ Ʈ������ ���� -> �÷��̾�
        camBase.Follow = GameObject.Find("Player").transform;
        // ī�޶��� ���� ���� �ݶ��̴� ���� -> ���� �� �信�� �̸����� ����, �� ��ȯ�ÿ��� ���� �̸����� ���� ����
        confiner.m_BoundingShape2D = GameObject.Find("CameraConfiner").GetComponent<PolygonCollider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
