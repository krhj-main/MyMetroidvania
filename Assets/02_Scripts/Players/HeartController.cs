using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


// ĵ������ ������ �÷��̾� ü���� ǥ�����ִ� UI ��ũ��Ʈ
public class HeartController : MonoBehaviour
{
    PlayerController player;                    // �÷��̾� ��ü

    GameObject[] heartContainers;               // ü���� ��ĭ -> �ִ� ü��
    Image[] heartFills;                         // ä���� ü�� -> ���� ü��
    public Transform heartParent;               // UI�� ǥ�ý�ų ��ġ
    public GameObject heartContainerPrefab;     // ü�� ������������
    // Start is called before the first frame update
    void Start()
    {
        player = PlayerController.Instance;

        // �ִ� ü���� �÷��̾��� �ִ�ü�¸�ŭ ���ӿ�����Ʈ �迭 ����
        heartContainers = new GameObject[PlayerController.Instance.maxHealth];
        // �ʱ� ���̹Ƿ� ���� ü�µ� �ִ� ü�¸�ŭ ����
        heartFills = new Image[PlayerController.Instance.maxHealth];

        // ó�� �÷��̾� ü�� ������Ʈ
        PlayerController.Instance.onHealthChangedCallback += UpdateHearsHUD;
        InstantiateHeartContainers();
        UpdateHearsHUD();
    }

    // Update is called once per frame
    void Update()
    {

    }

    // �÷��̾��� �ִ�ü�� ���� �޼���
    void SetHeartContainers()
    {
        // �ִ� ü������ ������ ���� �迭���̸�ŭ �ݺ��� ����
        for (int i = 0; i < heartContainers.Length; i++)
        {
            // �ݺ��� i���� �ִ�ü�·� ���� ���� ���¶��
            if (i < PlayerController.Instance.maxHealth)
            {
                // Ȱ��ȭ
                heartContainers[i].SetActive(true);
            }
            // �ִ�ü�·� �̻��̶��
            else
            {
                // �ش� ��ü�� ��Ȱ��ȭ
                heartContainers[i].SetActive(false);
            }
        }
    }

    // �÷��̾��� ����ü�� ���� �޼���
    void SetFilledHearts()
    {
        // �÷��̾��� �ִ�ü�·� ��ŭ �ݺ��� ����
        for (int i = 0; i < heartFills.Length; i++)
        {
            // �÷��̾��� ���� ü�� �Ʒ����
            if (i < PlayerController.Instance.health)
            {
                // ä���� ��Ʈ�� Ȱ��ȭ
                heartFills[i].gameObject.SetActive(true);
            }
            // ���� ü�º��� ���� �����
            else
            {
                // ä���� ��Ʈ ��Ȱ��ȭ
                heartFills[i].gameObject.SetActive(false);
            }
        }
    }

    // �ִ�ü�·� ��ŭ ü�� �����̳� ������Ʈ ���� �޼���
    void InstantiateHeartContainers()
    {
        // �ִ�ü�·� ��ŭ �ݺ��� ����
        for (int i = 0; i < PlayerController.Instance.maxHealth; i++)
        {
            // ��Ʈ�̹��� �������� �����ϰ� ���ӿ�����Ʈ temp ��ü�� ����
            GameObject temp = Instantiate(heartContainerPrefab);
            // ��Ʈ�� ��ġ��ų ������Ʈ�� ���
            temp.transform.SetParent(heartParent, false);
            // �ִ�ü�� �迭�� temp ������Ʈ �ֱ�
            heartContainers[i] = temp;
            // ���� ü�� �迭�� temp ������Ʈ�� �ڽ��� ä���� ��Ʈ �̹����� �߰�
            heartFills[i] = temp.transform.Find("HeartFill").GetComponent<Image>();
        }
    }

    // �÷��̾� ü�·��� ������Ʈ��Ű�� �޼���
    void UpdateHearsHUD()
    {
        SetHeartContainers();
        SetFilledHearts();
    }
}
