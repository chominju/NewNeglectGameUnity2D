using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName = "SkillData")]
public class SkillData// : ScriptableObject
{
    public string skillName;                    // ��ų �̸�
    public string skillShowName;                // // �������� ��ų �̸�(�ѱ�)

    public int skillLevel;                      // ��ų ����
    public int skillMaxLevel;                   // ��ų �ʿ� �ִ� ����
    public int skillCurrentMaxLevel;            // ��ų ���� �ִ� ����        

    public int transcendenceLevel;              // �ʿ� Ƚ��
    public int transcendenceMaxLevel;           // �ʿ� �ִ� Ƚ��

    public int quantity;                        // ������    
    public int mixCount;                        // �ռ� ����

    public int skillCoolTime;                   // ��ų ��Ÿ��
    public float currentSkillCoolTime;          // ���� ��ų ��Ÿ��

    public Sprite sprite;                       // �̹���

    public float holdingTime;                   // ���� ��������

    public bool isGainSkill;                    // ��ų ���濩��
}
