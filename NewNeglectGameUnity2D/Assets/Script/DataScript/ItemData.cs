using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName ="ItemData")]
public class ItemData
{
    public string itemName;                             // 아이템 이름
    public string itemShowName;                         // 보여지는 아이템 이름(한글)
        
    public int  itemLevel;                              // 레벨
    public int  itemMaxLevel;                           // 최대 레벨

    public int quantity;                                // 보유량
    public int mixCount;                                // 합성 갯수
            
    public Sprite sprite;                               // 이미지

    public bool isGainItem;                             // 아이템을 습득여부
    public enum ItemType
    {
        GOLD,
        WEAPON
    }
}
