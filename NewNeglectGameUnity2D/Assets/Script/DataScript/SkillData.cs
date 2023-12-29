using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//[CreateAssetMenu(menuName = "SkillData")]
public class SkillData// : ScriptableObject
{
    public string skillName;
    public string skillShowName;

    public int skillLevel;
    public int skillMaxLevel;
    public int skillCurrentMaxLevel;            

    public int transcendenceLevel;              // 초월 횟수
    public int transcendenceMaxLevel;           // 초월 최대 횟수

    public int quantity;
    public int mixCount;

    public int skillCoolTime;

    public Sprite sprite;

    public float holdingTime;

    public float currentSkillCoolTime;

    public bool isGainSkill;
}
