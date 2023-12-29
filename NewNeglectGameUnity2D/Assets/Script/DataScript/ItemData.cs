using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName ="ItemData")]
public class ItemData
{
    public string itemName;
    public string itemShowName;

    public int  itemLevel;
    public int  itemMaxLevel;

    public int quantity;
    public int mixCount;

    public Sprite sprite;

    public bool isGainItem;
    public enum ItemType
    {
        GOLD,
        WEAPON
    }
}
