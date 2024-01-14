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
            yield return new WaitForSeconds(saveTimer); // 10초 동안 대기

            // 10초가 경과하면 이 부분에서 저장할 동작을 수행
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
        // 분으로 계산
        int offlineTime = OfflineTime();
        if (offlineTime == -1)
        {
            isExistOfflineRewards = false;
            loginTime = GetCurrentData();
            Debug.Log("No Save Data New Player");
        }
        else if (offlineTime == 0)
        {
            // 1분이 안된채로 잠수탔을 때.
            DataManager.GetDataManager().GainGold(1);
            offlineRewards[0] = 0;
            offlineRewards[1] = 1;
            offlineRewards[2] = 0;
            offlineRewards[3] = 0;
        }
        else if (offlineTime == 10000)
        {
            // 최대 방치형 보상WoodenSword
            DataManager.GetDataManager().GainEquipmentGacha("WoodenSword", 144);
            DataManager.GetDataManager().GainGold(1440);
            DataManager.GetDataManager().GainExp(1440);

            offlineRewards[0] = 144;                // 장비
            offlineRewards[1] = 1440;               // 골드
            offlineRewards[2] = 1440;               // 경험치
            offlineRewards[3] = 1440;               // 오프라인 시간
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
            // 오늘의 첫 로그인
            DataManager.GetDataManager().SetAchievementDataIsSuccess("Login", true);
            DataManager.GetDataManager().SetAchievementDataIsReceiveReward("Login", false);
        }
        loginRewardDate = loginTime;
    }

    DateTime GetCurrentData()
    {
        // 현재시간을 가져옴.
        int currentYear = int.Parse(System.DateTime.Now.ToString("yyyy"));
        int currentMonth = int.Parse(System.DateTime.Now.ToString("MM"));
        int currentDay = int.Parse(System.DateTime.Now.ToString("dd"));
        int currentHour = int.Parse(System.DateTime.Now.ToString("HH"));
        int currentMinute = int.Parse(System.DateTime.Now.ToString("mm"));
        int currentSecond = int.Parse(System.DateTime.Now.ToString("mm"));

        DateTime currentDate = new DateTime(currentYear, currentMonth, currentDay, currentHour, currentMinute, currentSecond); // 시작 날짜
        return currentDate;
    }

    public int OfflineTime()
    {
        int offlineTime = -1;
        DateTimeData getLast = DataManager.GetDataManager().LoadLastLogoutData();

        if (getLast.year == 0)
        {
            // 마지막 로그인 기록이 없을 떄.
            Debug.Log("No lastLoginLog");
            return offlineTime;
        }
        else
        {
            // 마지막으로 저장된 로그인 기록을 가져옴.
            int loadYear = getLast.year;
            int loadMonth = getLast.month;
            int loadDay = getLast.day;
            int loadHour = getLast.hour;
            int loadMinute = getLast.minute;
            int loadSecond = getLast.second;



            // 현재시간을 가져옴.
            int currentYear = int.Parse(System.DateTime.Now.ToString("yyyy"));
            int currentMonth = int.Parse(System.DateTime.Now.ToString("MM"));
            int currentDay = int.Parse(System.DateTime.Now.ToString("dd"));
            int currentHour = int.Parse(System.DateTime.Now.ToString("HH"));
            int currentMinute = int.Parse(System.DateTime.Now.ToString("mm"));
            int currentSecond = int.Parse(System.DateTime.Now.ToString("mm"));

            DateTime loadDate = new DateTime(loadYear, loadMonth, loadDay, loadHour, loadMinute, loadSecond); // 시작 날짜

            DateTime currentDate = new DateTime(currentYear, currentMonth, currentDay, currentHour, currentMinute, currentSecond); // 시작 날짜
            loginTime = currentDate;

            TimeSpan duration = currentDate - loadDate; // 두 날짜 사이의 시간 간격 계산

            // 오프라인 일 , 시간 , 분 , 초 가져오기.
            int offlineDay = duration.Days;
            int offlineHours = duration.Hours;
            int offlineMinutes = duration.Minutes;
            int offlineSeconds = duration.Seconds;

            // 다른게 하나라도 있으면 오늘 로그인을 하지않은 것
            if ((loadYear != currentYear) || (loadMonth != currentMonth) || (loadDay != currentDay))
                isAlreadyLoginToday = false;
            else
                isAlreadyLoginToday = true;

            // 미접기간이 1일이 넘었다면, 최대보상(24시간) 제공
            if (offlineDay >= 1)
                return 10000;
            else
            {
                // 미접기간이 1일 미만이면 분에 맞는 보상을 제공.
                return (offlineHours * 60 + offlineMinutes);
            }
        }
    }

}
