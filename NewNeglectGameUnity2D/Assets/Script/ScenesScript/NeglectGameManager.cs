using System.Collections;
using UnityEngine;
using System.IO;
using System.Collections.Generic;
//using UnityEditor;
using Cinemachine;
using System;
//System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");


public class NeglectGameManager : MonoBehaviour
{
    private static NeglectGameManager instance;
    public GameObject playerPrefab;
    public GameObject virtualCamera;

    public GameObject playerCanvas;

    private int[] offlineRewards;
    private bool isExistOfflineRewards;

    private DateTime loginTime;
    private DateTime loginRewardDate;

    private bool isAlreadyLoginToday;

    float saveTimer = 10.0f;

    void Start()
    {
        if (instance == null)
            instance = this;
        GameObject dataManager = GameObject.Find("DataManager");
        isAlreadyLoginToday = false;
        if (dataManager != null)
        {
            Debug.Log("FindManager");
            dataManager.SetActive(true);
        }

        CreatePlayer();


        offlineRewards = new int[4];
        isExistOfflineRewards = true;
        
        
        OfflineReward();


        SaveLoginOutLog("Login");


        AddComponentEnemyManager();

        Invoke("CreatePlayerCanvas", 0.0001f);

        StartCoroutine(AutoSaveCoroutine());
    }

    private void Update()
    {
        DateTime currentDay = DateTime.Now;
        if (loginRewardDate.Day != currentDay.Day)
        {
            loginRewardDate = currentDay;
            DataManager.GetDataManager().SetAchievementDataIsReceiveReward("Login", false);
        }


    }

    private IEnumerator AutoSaveCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(saveTimer); // 10�� ���� ���

            // 10�ʰ� ����ϸ� �� �κп��� ������ ������ ����
            DataManager.GetDataManager().SaveAllData();
            Debug.Log("Auto Save All Data");
        }
    }


    public DateTime GetLoginTime()
    {
        return loginTime;
    }
  public static NeglectGameManager GetNeglectGameManager()
    {
        return instance;
    }

    public int[] GetOfflineReward()
    {
        return offlineRewards;
    }

    public bool GetIsExistOfflineRewards()
    {
        return isExistOfflineRewards;
    }

    void CreatePlayer()
    {
        var player = Instantiate(playerPrefab);
        virtualCamera.GetComponent<CinemachineVirtualCamera>().Follow = player.transform;
    }

    void AddComponentEnemyManager()
    {
        gameObject.AddComponent<EnemyManager>();
    }
    
    void CreatePlayerCanvas()
    {
        Instantiate(playerCanvas);
    }

    void SaveLoginOutLog(string LogType)
    {   
        DataManager.GetDataManager().SaveLoginOutLog(LogType);
    }


    private void OnApplicationQuit()
    {

        if (!UIManager.GetUIManager().GetIsResetClick())
        {
            SaveLoginOutLog("Logout");
            DataManager.GetDataManager().SaveAllData();
        }
    }


    void OfflineReward()
    {
        // ������ ���
        int offlineTime = OfflineTime();
        if (offlineTime == -1)
        {
            isExistOfflineRewards = false;
            loginTime = GetCurrentData();
            Debug.Log("No Save Data New Player");
        }
        else if (offlineTime == 0)
        {
            // 1���� �ȵ�ä�� ������� ��.
            DataManager.GetDataManager().GainGold(1);
            offlineRewards[0] = 0;
            offlineRewards[1] = 1;
            offlineRewards[2] = 0;
            offlineRewards[3] = 0;
        }
        else if (offlineTime == 10000)
        {
            // �ִ� ��ġ�� ����WoodenSword
            DataManager.GetDataManager().GainEquipmentGacha("WoodenSword", 144);
            DataManager.GetDataManager().GainGold(1440);
            DataManager.GetDataManager().GainExp(1440);

            offlineRewards[0] = 144;                // ���
            offlineRewards[1] = 1440;               // ���
            offlineRewards[2] = 1440;               // ����ġ
            offlineRewards[3] = 1440;               // �������� �ð�
        }
        else
        {
            int timeTemp = offlineTime;
            int equipmentTemp = (int)(timeTemp * 0.1f);
            int goldTemp = timeTemp;
            int expTemp = timeTemp * 5;

            DataManager.GetDataManager().GainEquipmentGacha("WoodenSword", equipmentTemp);
            DataManager.GetDataManager().GainGold(goldTemp);
            DataManager.GetDataManager().GainExp(expTemp);


            offlineRewards[0] = equipmentTemp;
            offlineRewards[1] = goldTemp;
            offlineRewards[2] = expTemp;
            offlineRewards[3] = offlineTime;
        }

        if (!isAlreadyLoginToday)
        {
            // ������ ù �α���
            DataManager.GetDataManager().SetAchievementDataIsSuccess("Login", true);
            DataManager.GetDataManager().SetAchievementDataIsReceiveReward("Login", false);
        }
        loginRewardDate = loginTime;
    }

    DateTime GetCurrentData()
    {
        // ����ð��� ������.
        int currentYear = int.Parse(System.DateTime.Now.ToString("yyyy"));
        int currentMonth = int.Parse(System.DateTime.Now.ToString("MM"));
        int currentDay = int.Parse(System.DateTime.Now.ToString("dd"));
        int currentHour = int.Parse(System.DateTime.Now.ToString("HH"));
        int currentMinute = int.Parse(System.DateTime.Now.ToString("mm"));
        int currentSecond = int.Parse(System.DateTime.Now.ToString("mm"));

        DateTime currentDate = new DateTime(currentYear, currentMonth, currentDay, currentHour, currentMinute, currentSecond); // ���� ��¥
        return currentDate;
    }

    public int OfflineTime()
    {
        int offlineTime = -1;
        DateTimeData getLast = DataManager.GetDataManager().LoadLastLogoutData();

        if (getLast.year == 0)
        {
            // ������ �α��� ����� ���� ��.
            Debug.Log("No lastLoginLog");
            return offlineTime;
        }
        else
        {
            // ���������� ����� �α��� ����� ������.
            int loadYear = getLast.year;
            int loadMonth = getLast.month;
            int loadDay = getLast.day;
            int loadHour = getLast.hour;
            int loadMinute = getLast.minute;
            int loadSecond = getLast.second;



            // ����ð��� ������.
            int currentYear = int.Parse(System.DateTime.Now.ToString("yyyy"));
            int currentMonth = int.Parse(System.DateTime.Now.ToString("MM"));
            int currentDay = int.Parse(System.DateTime.Now.ToString("dd"));
            int currentHour = int.Parse(System.DateTime.Now.ToString("HH"));
            int currentMinute = int.Parse(System.DateTime.Now.ToString("mm"));
            int currentSecond = int.Parse(System.DateTime.Now.ToString("mm"));

            DateTime loadDate = new DateTime(loadYear, loadMonth, loadDay, loadHour, loadMinute, loadSecond); // ���� ��¥

            DateTime currentDate = new DateTime(currentYear, currentMonth, currentDay, currentHour, currentMinute, currentSecond); // ���� ��¥
            loginTime = currentDate;

            TimeSpan duration = currentDate - loadDate; // �� ��¥ ������ �ð� ���� ���

            // �������� �� , �ð� , �� , �� ��������.
            int offlineDay = duration.Days;
            int offlineHours = duration.Hours;
            int offlineMinutes = duration.Minutes;
            int offlineSeconds = duration.Seconds;

            // �ٸ��� �ϳ��� ������ ���� �α����� �������� ��
            if ((loadYear != currentYear) || (loadMonth != currentMonth) || (loadDay != currentDay))
                isAlreadyLoginToday = false;
            else
                isAlreadyLoginToday = true;

            // �����Ⱓ�� 1���� �Ѿ��ٸ�, �ִ뺸��(24�ð�) ����
            if (offlineDay >= 1)
                return 10000;
            else
            {
                // �����Ⱓ�� 1�� �̸��̸� �п� �´� ������ ����.
                return (offlineHours * 60 + offlineMinutes);
            }
        }
    }

}
