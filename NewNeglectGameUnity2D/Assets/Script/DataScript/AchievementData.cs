using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AchievementData
{
    public string achievementName;      // �ӹ� �̸�
    public string description;          // �ӹ� ����
    public int currentCount;            // �ӹ� ī��Ʈ
    public int successCount;            // �ӹ����� ī��Ʈ
    public int reward;                  // �ӹ����� ����
    public bool isLoop;                 // �ݺ� ����Ʈ�ΰ�
    public bool isSuccess;              // ���� ������ ���� �� �ִ� �����ΰ�
    public bool isReceiveReward;        // ������ ���� �����ΰ�.(�ݺ� ����Ʈ�� �ƴ� ��)
}


[System.Serializable]
public class AchievementDataArray
{
    public AchievementData[] achievementDataArray;
}