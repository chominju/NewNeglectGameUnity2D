using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName ="ItemData")]
public class ItemData
{
    public string itemName;                             // ������ �̸�
    public string itemShowName;                         // �������� ������ �̸�(�ѱ�)
        
    public int  itemLevel;                              // ����
    public int  itemMaxLevel;                           // �ִ� ����

    public int quantity;                                // ������
    public int mixCount;                                // �ռ� ����
            
    public Sprite sprite;                               // �̹���

    public bool isGainItem;                             // �������� ���濩��
    public enum ItemType
    {
        GOLD,
        WEAPON
    }
}
