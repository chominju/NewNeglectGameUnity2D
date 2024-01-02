using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class AchievementManager : MonoBehaviour
{
    private static AchievementManager instance = null;

    public GameObject achievementUi;
    public Button backButton;

    private int receiveReward;
    private DateTime rewardStartTime;


    public GameObject []achievementButtons;
    public GameObject []achievementCountTexts;
    public GameObject []achievementRewardTexts;
    public GameObject []achievementBackground;

    // 업적 달성을 확인하고 업적 상태를 업데이트하는 메서드

    private void Start()
    {
        if (instance == null)
            instance = this;

        receiveReward = 0;

        SetBackButtonSize();
        GetLoginTime();
        DataUpdateAndTextUpdate();
    }

    void SetBackButtonSize()
    {
        float screenWidth = UIManager.GetUIManager().GetWidthSize();
        float screenHeight = UIManager.GetUIManager().GetHeightSize();
        Rect rSize = achievementUi.GetComponent<RectTransform>().rect;
        float newWidthSize = 0;
        //float newHeigthSize = 0;
        // 장비/스킬를 클릭했을 때(3개의 오브젝트 보이기)
        newWidthSize = screenWidth - rSize.width;
        //newHeigthSize = screenHeight - lSize.height - rSize.height;

        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidthSize, backButton.GetComponent<RectTransform>().sizeDelta.y);
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
        var getAchievementData = DataManager.GetDataManager().GetAchievementData();
        foreach (AchievementData achievement in getAchievementData)
        {
            string tempName = achievement.achievementName + "Button";

            if (achievement.achievementName.Equals("Login"))
            {
                if (tempName.Equals(button.name))
                {
                    // 로그인
                    // 미션 성공
                    if (achievement.isSuccess)
                    {
                        if (!achievement.isReceiveReward)
                        {
                            // 보상을 아직 안받음 , 받은걸로 체크해주기
                            SoundManager.GetInstance().PlayAchievementRewardSound();
                            DataManager.GetDataManager().SetAchievementDataIsReceiveReward(achievement.achievementName, true);
                            DataManager.GetDataManager().UpdateAchievementData(achievement.achievementName, true, 1);
                            DataManager.GetDataManager().GainGold(achievement.reward);

                        }
                        else
                        {
                            // 보상을 이미 받음
                            SoundManager.GetInstance().PlayFailSound();
                            //button.GetComponentInChildren<Text>().text = "완료";
                        }
                    }
                    else
                    {
                        // 실패
                        SoundManager.GetInstance().PlayFailSound();
                        //button.GetComponentInChildren<Text>().text = "받기";
                        Debug.Log("Login Reward already received");
                    }
                }
            }
            else
            {
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
                    }
                }
            }
        }
        AchievementUpdateEvent();
    }

    void SetAchievementButtonAndText()
    {
        var getAchievementData = DataManager.GetDataManager().GetAchievementData();

        int num = 0;
        foreach (AchievementData achievement in getAchievementData)
        {
            // 성공
            if (achievement.isSuccess)
            {
                // 로그인 경우
                if (achievement.achievementName.Equals("Login"))
                {
                    // 1회보상 획득
                    if (achievement.isReceiveReward)
                    {
                        achievementButtons[num].GetComponent<Image>().color = Color.gray;
                        achievementButtons[num].GetComponentInChildren<Text>().text = "완료";
                    }
                    // 아직 미획득
                    else
                    {
                        achievementButtons[num].GetComponent<Image>().color = Color.yellow;
                        achievementButtons[num].GetComponentInChildren<Text>().text = "받기";
                    }
                }
                // 접속시간 / 몬스터 사냥
                else
                {
                    achievementButtons[num].GetComponent<Image>().color = Color.yellow;
                    achievementButtons[num].GetComponentInChildren<Text>().text = "받기";

                    //if (achievement.achievementName.Equals("EnemyKill"))
                    //{
                    //    // 접속시간 
                        
                    //}
                    //else if (achievement.achievementName.Equals("AccessingTime"))
                    //{
                    //    // 몬스터 사냥
                    //}
                    //else
                    //{
                    //    // 업적에 없음. 버그
                    //    Debug.Log("No Achievement Name");
                    //}
                }
            }
            // 실패
            else
            {
                achievementButtons[num].GetComponent<Image>().color = Color.gray;
                achievementButtons[num].GetComponentInChildren<Text>().text = "받기";

            }

            // 업적카운터 , 보상 
            achievementCountTexts[num].GetComponent<Text>().text = achievement.currentCount +" / " + achievement.successCount;
            achievementRewardTexts[num].GetComponent<Text>().text = ((achievement.currentCount / achievement.successCount) * achievement.reward).ToString();
            num++;
        }
    }

}
