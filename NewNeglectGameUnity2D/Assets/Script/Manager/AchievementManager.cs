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

    // ���� �޼��� Ȯ���ϰ� ���� ���¸� ������Ʈ�ϴ� �޼���

    private void Start()
    {
        if (instance == null)
            instance = this;

        successEnemyKill = 20;              // 20���� ���� �� ����
        successAccseeingTime = 3;           // 3�� �����Ҷ� ����
        
        accseeingTimeReward = 100;          //  ���ӽð� ���� : 100��� 
        loginReward = 1000;                 // �α��� ���� :   1000��� 
        enmeyKillReward = 300;              // óġ �� : 300���
        receiveReward = 0;
        GetLoginTime();
        DataUpdateAndTextUpdate();
    }

    private void Update()
    {
        // ����ð� ��������
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
        // ����ð��� ������.
        int currentYear = int.Parse(System.DateTime.Now.ToString("yyyy"));
        int currentMonth = int.Parse(System.DateTime.Now.ToString("MM"));
        int currentDay = int.Parse(System.DateTime.Now.ToString("dd"));
        int currentHour = int.Parse(System.DateTime.Now.ToString("HH"));
        int currentMinute = int.Parse(System.DateTime.Now.ToString("mm"));
        int currentSecond = int.Parse(System.DateTime.Now.ToString("mm"));

        DateTime currentDate = new DateTime(currentYear, currentMonth, currentDay, currentHour, currentMinute, currentSecond);

        return currentDate;
    }

    // ����ޱ� ��ư�� ������ ��
    public void ReceiveAchievementRewardClick(Button button)
    {
        var getAchievementData =  DataManager.GetDataManager().GetAchievementData();
        foreach (AchievementData achievement in getAchievementData)
        {
            string tempName = achievement.achievementName + "Button";
            // ����ޱ� ��ư�� ������ ��
            if (tempName.Equals(button.name))
            {
                // �ӹ� �����϶�
                if (achievement.isSuccess)
                {
                    // �װ��� �ݺ��ӹ� �϶�
                    if (achievement.isLoop)
                    {
                        // ������ ����ϱ�.
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
                        // ������ 1ȸ �����ε� �̹� �޾���.(�α��κ���)
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
                            // ������ 1ȸ �����ε� ���� ���� �ȹ޾���.(�α��κ���)
                            //ReceiveAchievementReward(achievement.achievementName);
                            // ������, �̹̹���, ������ ������Ʈ
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



        
    // ��ư ���󺯰� (�Ϸ�� �����, �̿Ϸ�� ȸ��)
    void SetAchievementButtonAndText()
    {
        var getAchievementData = DataManager.GetDataManager().GetAchievementData();
        int num = 0;
        foreach (AchievementData achievement in getAchievementData)
        {
            // ��ư ����
            // 1. �ݺ��ӹ��ε�, �ӹ��޼��� �߰ų�
            // 2. �ݺ��ӹ��� �ƴѵ�, ���������� �����ʾҰ�, �ӹ��޼��� �� ���
            //IsSuccessAchievement(achievement);
            if ((achievement.isLoop && achievement.isSuccess) || (!achievement.isLoop && achievement.isSuccess && !achievement.isReceiveReward))
            {
                // ����
                //if (achievement.isLoop == false)
                //    achievement.isReceiveReward = true;
                achievementButtons[num].GetComponent<Image>().color = Color.yellow;
                //achievementBackground[num].GetComponent<Image>().color = new Color(214, 214, 214, 255);
            }
            else
            {
                // ����
                achievementButtons[num].GetComponent<Image>().color = Color.gray;
            }

            // �۾� ����
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




    // ������ �޴� ��. 
    // ������ � ���� üũ�ؾߵ�
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

    // Ư�� ������ �޼� ������ Ȯ���ϴ� �޼���
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
