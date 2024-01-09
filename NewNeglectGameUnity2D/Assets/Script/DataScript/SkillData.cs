using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName = "SkillData")]
public class SkillData// : ScriptableObject
{
    public string skillName;                    // 스킬 이름
    public string skillShowName;                // // 보여지는 스킬 이름(한글)

    public int skillLevel;                      // 스킬 레벨
    public int skillMaxLevel;                   // 스킬 초월 최대 레벨
    public int skillCurrentMaxLevel;            // 스킬 현재 최대 레벨        

    public int transcendenceLevel;              // 초월 횟수
    public int transcendenceMaxLevel;           // 초월 최대 횟수

    public int quantity;                        // 보유량    
    public int mixCount;                        // 합성 갯수

    public int skillCoolTime;                   // 스킬 쿨타임
    public float currentSkillCoolTime;          // 현재 스킬 쿨타임

    public Sprite sprite;                       // 이미지

    public float holdingTime;                   // 몇초 지속인지

    public bool isGainSkill;                    // 스킬 습득여부
}
