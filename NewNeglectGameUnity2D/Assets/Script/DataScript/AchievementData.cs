using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class AchievementData
{
    public string achievementName;      // 임무 이름
    public string description;          // 임무 설명
    public int currentCount;            // 임무 카운트
    public int successCount;            // 임무성공 카운트
    public int reward;                  // 임무성공 보상
    public bool isLoop;                 // 반복 퀘스트인가
    public bool isSuccess;              // 지금 보상을 받을 수 있는 상태인가
    public bool isReceiveReward;        // 보상을 받은 상태인가.(반복 퀘스트가 아닐 때)
}


[System.Serializable]
public class AchievementDataArray
{
    public AchievementData[] achievementDataArray;
}