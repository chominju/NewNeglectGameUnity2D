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
                                                     //public int playerLevel = 1;                      // �÷��̾� ����
                                                     //public int atkLevel = 1;                        // ���ݷ� ���� ����
                                                     //public int defLevel = 1;                        // ���� ���� ����
                                                     //public int moveSpeedLevel = 1;                 // �̵��ӵ� ���� ����
                                                     //public int maxHpLevel = 1;                     // �ִ� ü�� ���� ����
                                                     //public int maxMpLevel = 1;                     // �ִ� ���� ���� ����

    //public int currentExp = 0;                         // ���� ����ġ
    //public int statPoint = 0;

    //public float animationSpeed = 2;                 // �ִϸ��̼� �ӵ�(���ݼӵ�)
    //public int power = 0;                              // ������


    public List<GameObject> takeSkills;              // ������ �ִ� ��ų
    public string[] skillPreset;                    // ��ų ������

    public string equipItemName;                   // �������� ������

    public GameObject playerObject;

    public int currentGold;                         // ���� ���
                
    enum EQUIP_TYPE { WEAPON, ARMOUR,END };
}
