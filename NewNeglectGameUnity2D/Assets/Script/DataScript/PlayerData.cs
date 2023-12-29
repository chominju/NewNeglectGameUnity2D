using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//: ScriptableObject
    //[CreateAssetMenu(menuName = "PlayerData")]
    [System.Serializable]
public class PlayerData
{
    public string playerName;                        // 이름
    public int playerLevel;                      // 플레이어 레벨
    public int atkLevel;                        // 공격력 스탯 레벨
    public int defLevel;                        // 방어력 스탯 레벨
    public int moveSpeedLevel;                 // 이동속도 스탯 레벨
    public int maxHpLevel;                     // 최대 체력 스탯 레벨
    public int maxMpLevel;                     // 최대 마나 스탯 레벨

    public int currentExp;                         // 현재 경험치
    public int currentHp;                         // 현재 경험치
    public int statPoint;

    public float animationSpeed;                 // 애니메이션 속도(공격속도)
                                                     //public int playerLevel = 1;                      // 플레이어 레벨
                                                     //public int atkLevel = 1;                        // 공격력 스탯 레벨
                                                     //public int defLevel = 1;                        // 방어력 스탯 레벨
                                                     //public int moveSpeedLevel = 1;                 // 이동속도 스탯 레벨
                                                     //public int maxHpLevel = 1;                     // 최대 체력 스탯 레벨
                                                     //public int maxMpLevel = 1;                     // 최대 마나 스탯 레벨

    //public int currentExp = 0;                         // 현재 경험치
    //public int statPoint = 0;

    //public float animationSpeed = 2;                 // 애니메이션 속도(공격속도)
    //public int power = 0;                              // 전투력


    public List<GameObject> takeSkills;              // 가지고 있는 스킬
    public string[] skillPreset;                    // 스킬 프리셋

    public string equipItemName;                   // 장착중인 아이템

    public GameObject playerObject;

    public int currentGold;                         // 현재 골드
                
    enum EQUIP_TYPE { WEAPON, ARMOUR,END };
}
