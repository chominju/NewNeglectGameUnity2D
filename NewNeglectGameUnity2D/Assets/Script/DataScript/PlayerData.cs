using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//: ScriptableObject
    //[CreateAssetMenu(menuName = "PlayerData")]
    [System.Serializable]
public class PlayerData
{
    public string playerName;                        // �̸�
    public int playerLevel;                      // �÷��̾� ����
    public int atkLevel;                        // ���ݷ� ���� ����
    public int defLevel;                        // ���� ���� ����
    public int moveSpeedLevel;                 // �̵��ӵ� ���� ����
    public int maxHpLevel;                     // �ִ� ü�� ���� ����
    public int maxMpLevel;                     // �ִ� ���� ���� ����

    public int currentExp;                         // ���� ����ġ
    public int currentHp;                         // ���� ����ġ
    public int statPoint;

    public float animationSpeed;                 // �ִϸ��̼� �ӵ�(���ݼӵ�)

    public List<GameObject> takeSkills;              // ������ �ִ� ��ų
    public string[] skillPreset;                    // ��ų ������

    public string equipItemName;                   // �������� ������

    public int currentGold;                         // ���� ���
                
    enum EQUIP_TYPE { WEAPON, ARMOUR,END };
}
