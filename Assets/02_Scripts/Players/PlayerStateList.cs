using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// �÷��̾ ������ ��ũ��Ʈ
// �÷��̾� ���� ��Ÿ��
public class PlayerStateList : MonoBehaviour
{
    public bool lookRight;              // �������� �����ִ��� ����
    public bool attacking;              // �������� �������� ����
    public bool dashing;                // �뽬���� ������ ����
    public bool invincible;             // �ǰ� �� �������� ����
    public bool recoilingX;             // X�� �ݵ� ���� ����
    public bool recoilingY;             // Y�� �ݵ� ���� ����
    public bool jumping;                // �������� ���� ����
    public bool healing;                // �� ���� ���� ����
    public bool casting;                // ���� ������� ĳ���� �� ����
    public bool cutscene;               // �� ȭ����ȯ ������ ����
    public bool alive;                  // �÷��̾ ����ִ��� ����
}
