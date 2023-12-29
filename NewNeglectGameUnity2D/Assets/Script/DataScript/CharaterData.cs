using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="CharaterData")]
public class CharaterData : ScriptableObject
{
    public string CharaterName;
    public int level;
    public int hp;
    public int maxHp;
    public int mp;
    public int maxMp;
    public int exp;
    public int maxExp;
    public int atk;
    public int def;
    public int power;
    public float speed;
    public float animationSpeed;
}
