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

    // ���� �޼��� Ȯ���ϰ� ���� ���¸� ������Ʈ�ϴ� �޼���

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
        // ���/��ų�� Ŭ������ ��(3���� ������Ʈ ���̱�)
        newWidthSize = screenWidth - rSize.width;
        //newHeigthSize = screenHeight - lSize.height - rSize.height;

        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidthSize, backButton.GetComponent<RectTransform>().sizeDelta.y);
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
        var getAchievementData = DataManager.GetDataManager().GetAchievementData();
        foreach (AchievementData achievement in getAchievementData)
        {
            string tempName = achievement.achievementName + "Button";

            if (achievement.achievementName.Equals("Login"))
            {
                if (tempName.Equals(button.name))
                {
                    // �α���
                    // �̼� ����
                    if (achievement.isSuccess)
                    {
                        if (!achievement.isReceiveReward)
                        {
                            // ������ ���� �ȹ��� , �����ɷ� üũ���ֱ�
                            SoundManager.GetInstance().PlayAchievementRewardSound();
                            DataManager.GetDataManager().SetAchievementDataIsReceiveReward(achievement.achievementName, true);
                            DataManager.GetDataManager().UpdateAchievementData(achievement.achievementName, true, 1);
                            DataManager.GetDataManager().GainGold(achievement.reward);

                        }
                        else
                        {
                            // ������ �̹� ����
                            SoundManager.GetInstance().PlayFailSound();
                            //button.GetComponentInChildren<Text>().text = "�Ϸ�";
                        }
                    }
                    else
                    {
                        // ����
                        SoundManager.GetInstance().PlayFailSound();
                        //button.GetComponentInChildren<Text>().text = "�ޱ�";
                        Debug.Log("Login Reward already received");
                    }
                }
            }
            else
            {
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
            // ����
            if (achievement.isSuccess)
            {
                // �α��� ���
                if (achievement.achievementName.Equals("Login"))
                {
                    // 1ȸ���� ȹ��
                    if (achievement.isReceiveReward)
                    {
                        achievementButtons[num].GetComponent<Image>().color = Color.gray;
                        achievementButtons[num].GetComponentInChildren<Text>().text = "�Ϸ�";
                    }
                    // ���� ��ȹ��
                    else
                    {
                        achievementButtons[num].GetComponent<Image>().color = Color.yellow;
                        achievementButtons[num].GetComponentInChildren<Text>().text = "�ޱ�";
                    }
                }
                // ���ӽð� / ���� ���
                else
                {
                    achievementButtons[num].GetComponent<Image>().color = Color.yellow;
                    achievementButtons[num].GetComponentInChildren<Text>().text = "�ޱ�";

                    //if (achievement.achievementName.Equals("EnemyKill"))
                    //{
                    //    // ���ӽð� 
                        
                    //}
                    //else if (achievement.achievementName.Equals("AccessingTime"))
                    //{
                    //    // ���� ���
                    //}
                    //else
                    //{
                    //    // ������ ����. ����
                    //    Debug.Log("No Achievement Name");
                    //}
                }
            }
            // ����
            else
            {
                achievementButtons[num].GetComponent<Image>().color = Color.gray;
                achievementButtons[num].GetComponentInChildren<Text>().text = "�ޱ�";

            }

            // ����ī���� , ���� 
            achievementCountTexts[num].GetComponent<Text>().text = achievement.currentCount +" / " + achievement.successCount;
            achievementRewardTexts[num].GetComponent<Text>().text = ((achievement.currentCount / achievement.successCount) * achievement.reward).ToString();
            num++;
        }
    }

}
