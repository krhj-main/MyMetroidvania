using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CamFollowPlayer : MonoBehaviour
{
    CinemachineVirtualCamera cam;
    CinemachineVirtualCameraBase camBase;
    CinemachineConfiner2D confiner;
    // Start is called before the first frame update
    void Start()
    {
        camBase = GetComponent<CinemachineVirtualCameraBase>();
        cam = GetComponent<CinemachineVirtualCamera>();
        confiner = GetComponent<CinemachineConfiner2D>();

        camBase.Follow = GameObject.Find("Player").transform;
        confiner.m_BoundingShape2D = GameObject.Find("CameraConfiner").GetComponent<PolygonCollider2D>();
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
