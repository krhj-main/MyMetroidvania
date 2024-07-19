using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// 캔버스에 부착된 플레이어 체력을 표시해주는 UI 스크립트
public class HeartController : MonoBehaviour
{
    PlayerController player;                    // 플레이어 객체

    GameObject[] heartContainers;               // 체력의 빈칸 -> 최대 체력
    Image[] heartFills;                         // 채워진 체력 -> 현재 체력
    public Transform heartParent;               // UI를 표시시킬 위치
    public GameObject heartContainerPrefab;     // 체력 아이콘프리팹
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;

        // 최대 체력은 플레이어의 최대체력만큼 게임오브젝트 배열 생성
        heartContainers = new GameObject[PlayerController.Instance.maxHealth];
        // 초기 값이므로 현재 체력도 최대 체력만큼 생성
        heartFills = new Image[PlayerController.Instance.maxHealth];

        // 처음 플레이어 체력 업데이트
        PlayerController.Instance.onHealthChangedCallback += UpdateHearsHUD;
        InstantiateHeartContainers();
        UpdateHearsHUD();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // 플레이어의 최대체력 설정 메서드
    void SetHeartContainers()
    {
        // 최대 체력으로 설정한 값의 배열길이만큼 반복문 실행
        for (int i = 0; i < heartContainers.Length; i++)
        {
            // 반복문 i값이 최대체력량 보다 낮은 상태라면
            if (i < PlayerController.Instance.maxHealth)
            {
                // 활성화
                heartContainers[i].SetActive(true);
            }
            // 최대체력량 이상이라면
            else
            {
                // 해당 객체는 비활성화
                heartContainers[i].SetActive(false);
            }
        }
    }

    // 플레이어의 현재체력 설정 메서드
    void SetFilledHearts()
    {
        // 플레이어의 최대체력량 만큼 반복문 실행
        for (int i = 0; i < heartFills.Length; i++)
        {
            // 플레이어의 현재 체력 아래라면
            if (i < PlayerController.Instance.health)
            {
                // 채워진 하트를 활성화
                heartFills[i].gameObject.SetActive(true);
            }
            // 현재 체력보다 많은 수라면
            else
            {
                // 채워진 하트 비활성화
                heartFills[i].gameObject.SetActive(false);
            }
        }
    }

    // 최대체력량 만큼 체력 컨테이너 오브젝트 생성 메서드
    void InstantiateHeartContainers()
    {
        // 최대체력량 만큼 반복문 실행
        for (int i = 0; i < PlayerController.Instance.maxHealth; i++)
        {
            // 하트이미지 프리팹을 생성하고 게임오브젝트 temp 객체에 저장
            GameObject temp = Instantiate(heartContainerPrefab);
            // 하트를 위치시킬 오브젝트의 상속
            temp.transform.SetParent(heartParent, false);
            // 최대체력 배열에 temp 오브젝트 넣기
            heartContainers[i] = temp;
            // 현재 체력 배열에 temp 오브젝트의 자식중 채워진 하트 이미지도 추가
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    // 플레이어 체력량을 업데이트시키는 메서드
    void UpdateHearsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}
