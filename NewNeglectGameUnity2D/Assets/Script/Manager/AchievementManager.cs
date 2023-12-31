using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager instance = null;

    public GameObject achievementUi;
    //private List<AchievementData> achievements;
    //private AchievementData[] achievements;

    //private bool isLogin;
    private int enemyKillCount;
    private int accseeingTimeCount;

    private int successEnemyKill;
    private int successAccseeingTime;

    private int accseeingTimeReward;
    private int loginReward;
    private int enmeyKillReward;
    private int receiveReward;
    private DateTime rewardStartTime;


    public GameObject []achievementButtons;
    public GameObject []achievementTexts;
    public GameObject []achievementRewardTexts;
    public GameObject []achievementBackground;

    // 업적 달성을 확인하고 업적 상태를 업데이트하는 메서드

    private void Start()
    {
        if (instance == null)
            instance = this;

        successEnemyKill = 20;              // 20마리 잡을 때 마다
        successAccseeingTime = 3;           // 3분 접속할때 마다
        
        accseeingTimeReward = 100;          //  접속시간 보상 : 100골드 
        loginReward = 1000;                 // 로그인 보상 :   1000골드 
        enmeyKillReward = 300;              // 처치 수 : 300골드
        receiveReward = 0;
        GetLoginTime();
        DataUpdateAndTextUpdate();
    }

    private void Update()
    {
        // 현재시간 가져오기
        DateTime realTime =  getRealTime();

        TimeSpan durationTime = realTime - rewardStartTime;

        int durationTimeDay = durationTime.Days;
        int durationTimeHours = durationTime.Hours;
        int durationTimeMinutes = durationTime.Minutes;

        int totalMinutes = durationTimeDay * 24 * 60 + durationTimeHours * 60 + durationTimeMinutes;

        if(totalMinutes >=1)
        {
            Debug.Log("AccessingTime : " + totalMinutes);
            DataManager.GetDataManager().UpdateAchievementData("AccessingTime", true, totalMinutes);
        }


    }

    public void CloseAchievementUI()
    {
        this.gameObject.SetActive(false);
    }

    void DataUpdateAndTextUpdate()
    {
        if (achievementUi.activeSelf)
        {
            SetAchievementButtonAndText();
        }
    }

    public static void AchievementUpdateEvent()
    {
        if (instance != null)
        {
            instance.DataUpdateAndTextUpdate();
        }
    }

    public void GetLoginTime()
    {
        rewardStartTime = NeglectGameManager.GetNeglectGameManager().GetLoginTime();
    }

    public DateTime getRealTime()
    {
        // 현재시간을 가져옴.
        int currentYear = int.Parse(System.DateTime.Now.ToString("yyyy"));
        int currentMonth = int.Parse(System.DateTime.Now.ToString("MM"));
        int currentDay = int.Parse(System.DateTime.Now.ToString("dd"));
        int currentHour = int.Parse(System.DateTime.Now.ToString("HH"));
        int currentMinute = int.Parse(System.DateTime.Now.ToString("mm"));
        int currentSecond = int.Parse(System.DateTime.Now.ToString("mm"));

        DateTime currentDate = new DateTime(currentYear, currentMonth, currentDay, currentHour, currentMinute, currentSecond);

        return currentDate;
    }

    // 보상받기 버튼을 눌렀을 때
    public void ReceiveAchievementRewardClick(Button button)
    {
        var getAchievementData =  DataManager.GetDataManager().GetAchievementData();
        foreach (AchievementData achievement in getAchievementData)
        {
            string tempName = achievement.achievementName + "Button";
            // 보상받기 버튼을 눌렀을 떄
            if (tempName.Equals(button.name))
            {
                // 임무 성공일때
                if (achievement.isSuccess)
                {
                    // 그것이 반복임무 일때
                    if (achievement.isLoop)
                    {
                        // 리워드 계산하기.
                        int rewardCount = achievement.currentCount / achievement.successCount;
                        int rest = achievement.currentCount % achievement.successCount;

                        if (achievement.achievementName.Equals("AccessingTime"))
                            rewardStartTime = getRealTime();

                        DataManager.GetDataManager().UpdateAchievementData(achievement.achievementName, true, rest);

                        receiveReward = rewardCount * achievement.reward;
                        DataManager.GetDataManager().GainGold(receiveReward);

                        SoundManager.GetInstance().PlayAchievementRewardSound();
                    }
                    else
                    {
                        // 보상이 1회 한정인데 이미 받았음.(로그인보상)
                        if (achievement.isReceiveReward)
                        {
                            SoundManager.GetInstance().PlayFailSound();
                            var tempPar = button.transform.parent;
                            //button.transform.parent.GetComponent<Image>().color = new Color(100, 100, 100, 150);
                            Debug.Log("Login Reward already received");
                            return;
                        }
                        else
                        {
                            // 보상이 1회 한정인데 아직 보상 안받았음.(로그인보상)
                            //ReceiveAchievementReward(achievement.achievementName);
                            // 데이터, 이미받음, 골드받음 업데이트
                            SoundManager.GetInstance().PlayAchievementRewardSound();
                            DataManager.GetDataManager().UpdateAchievementData(achievement.achievementName, true, 1);
                            DataManager.GetDataManager().SetAchievementDataIsReceiveReward(achievement.achievementName, true);
                            DataManager.GetDataManager().GainGold(achievement.reward);

                        }
                    }

                    achievement.isSuccess = false;
                }
                else
                {
                    SoundManager.GetInstance().PlayFailSound();
                    return;
                }

            }
                
        }
    }



        
    // 버튼 색상변경 (완료시 노란색, 미완료시 회색)
    void SetAchievementButtonAndText()
    {
        var getAchievementData = DataManager.GetDataManager().GetAchievementData();
        int num = 0;
        foreach (AchievementData achievement in getAchievementData)
        {
            // 버튼 관련
            // 1. 반복임무인데, 임무달성을 했거나
            // 2. 반복임무가 아닌데, 아직보상을 받지않았고, 임무달성을 한 경우
            //IsSuccessAchievement(achievement);
            if ((achievement.isLoop && achievement.isSuccess) || (!achievement.isLoop && achievement.isSuccess && !achievement.isReceiveReward))
            {
                // 성공
                //if (achievement.isLoop == false)
                //    achievement.isReceiveReward = true;
                achievementButtons[num].GetComponent<Image>().color = Color.yellow;
                //achievementBackground[num].GetComponent<Image>().color = new Color(214, 214, 214, 255);
            }
            else
            {
                // 실패
                achievementButtons[num].GetComponent<Image>().color = Color.gray;
            }

            // 글씨 관련
            if (achievement.achievementName.Equals("Login"))
            {
                if (achievement.isSuccess)
                {
                        achievementTexts[num].GetComponent<Text>().text = "1 / 1";
                        achievementRewardTexts[num].GetComponent<Text>().text = achievement.reward.ToString();// (achievement.currentCount / achievement.successCount);
                    //if (achievement.isReceiveReward)
                    //    achievementBackground[num].GetComponent<Image>().color = new Color(0, 0, 0, 255);
                }
                else
                {
                    achievementTexts[num].GetComponent<Text>().text = "0 / 1";
                    //achievementRewardTexts[num].GetComponent<Text>().text = "0";

                }
            }
            else if (achievement.achievementName.Equals("EnemyKill"))
            {
                achievementTexts[num].GetComponent<Text>().text = achievement.currentCount + " / " + achievement.successCount;
                //achievementRewardTexts[num].GetComponent<Text>().text = ((achievement.currentCount / achievement.successCount) * achievement.reward).ToString();
            }
            else if (achievement.achievementName.Equals("AccessingTime"))
            {
                achievementTexts[num].GetComponent<Text>().text = achievement.currentCount + " / " + achievement.successCount;
            }
            achievementRewardTexts[num].GetComponent<Text>().text = ((achievement.currentCount / achievement.successCount) * achievement.reward).ToString();
            num++;
        }
    }




    // 보상을 받는 곳. 
    // 보상이 몇개 인지 체크해야됨
    public void ReceiveAchievementReward(string achievementName)
    {
        if (achievementName.Equals("Login"))
        {
            receiveReward = loginReward;
        }
        else if (achievementName.Equals("EnmeyKill"))
        {
            int rewardCount = enemyKillCount / successEnemyKill;
            int rest = enemyKillCount % successEnemyKill;

            enemyKillCount = rest;
            receiveReward = rewardCount * enmeyKillReward;

        }
        else if (achievementName.Equals("AccessingTime"))
        {
            int rewardCount = accseeingTimeCount / successAccseeingTime;
            int rest = accseeingTimeCount % successAccseeingTime;

            accseeingTimeCount = rest;
            receiveReward = rewardCount * accseeingTimeReward;
        }
        else
            receiveReward = 0;
    }

    // 특정 업적의 달성 조건을 확인하는 메서드
    //private void IsSuccessAchievement(AchievementData achievementName)
    //{
    //    if (achievementName.achievementName.Equals("Login"))
    //    {
    //        if(achievementName.isSuccess && achievementName.isReceiveReward)
    //        if (isLogin == true)
    //            achievementName.isSuccess = true;
    //        else
    //            achievementName.isSuccess = false;
    //    }
    //    else if (achievementName.achievementName.Equals("EnmeyKill"))
    //    {
    //        if (enemyKillCount >= successEnemyKill)
    //            achievementName.isSuccess = true;
    //        else
    //            achievementName.isSuccess = false;
    //    }
    //    else if (achievementName.achievementName.Equals("AccessingTime"))
    //    {
    //        if (accseeingTimeCount >= successAccseeingTime)
    //            achievementName.isSuccess = true;
    //        else
    //            achievementName.isSuccess = false;
    //    }
    //    else
    //        Debug.Log("IsSuccessAchievement Func : achievementName is no exist");
    //}
}
