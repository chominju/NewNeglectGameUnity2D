using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;

delegate void UpdateData();

public class DataManager : MonoBehaviour
{
    private static DataManager instance = null;

    private PlayerData playerData;
    //private static PlayerData playerDataAsset;
    private ItemData[] equipmentData;
    private SkillData[] skillData;
    private AchievementData[] achievementData;
    private DateTimeData lastLogoutData;


    //private Dictionary<string,int> achievementData;
    private int skillDataQuantity;
    private List<Dictionary<string, object>> playerStatCsv;
    private List<Dictionary<string, object>> equipmentCsv;
    private List<Dictionary<string, object>> skillCsv;
    private List<Dictionary<string, object>> achievementCsv;
    private List<Dictionary<string, object>>[] equipmentDetailCsv;
    private List<Dictionary<string, object>>[] skillDetailCsv;
    private List<Dictionary<string, object>> enemyDataCsv;                                                         // 적들 데이터 받아오기
    private List<Dictionary<string, object>> fieldDataCsv;
    private string loginLogText;
    private string achievementDataText;
    private string lastLoginLogText;


    private const string playerStatFileName = "playerStatData";
    private const string equipmentFileName = "EquipmentInfo";
    private const string skillFileName = "SkillInfo";
    private const string achievementFileName = "AchievementInfo";
    private const string equipmentDataFileName = "EquipmenData";
    private const string enemyDataFileName = "enemyData";
    private const string fieldDataFileName = "fieldData";


    private string playerDataSavePath;
    private string equipmentDataSavePath;
    private string achievementDataSavePath;
    private string skillDataSavePath;
    private string loginLogPath; 
    private string lastLogoutLogPath;

    private string lastLogoutDataPath;


    private event UpdateData PlayerUpdateEvnet;
    private event UpdateData EquipmentUpdateEvnet;
    private event UpdateData SkillUpdateEvnet;
    private event UpdateData AchievementUpdateEvnet;

    private void Start()
    {
        if (instance == null)
            instance = this;

        playerDataSavePath = Application.persistentDataPath + "PlayerData.json"; 
        equipmentDataSavePath = Application.persistentDataPath;
        skillDataSavePath = Application.persistentDataPath;
        achievementDataSavePath = Application.persistentDataPath + "Achievementdata.json";
        loginLogPath =  Application.persistentDataPath + "LoginLog.txt";
        lastLogoutLogPath = Application.persistentDataPath + "LastLoginLog.txt";
        lastLogoutDataPath = Application.persistentDataPath + "LastLogoutData.json";

        if (instance !=null)
        {
            instance.CsvDataLoad();
        }

    }

    public static DataManager GetDataManager()
    {
        return instance;
    }


    public  void CsvDataLoad()
    {
        DelegateInit();

        playerStatCsv = FileReader.ReadCSVFile(playerStatFileName);
        enemyDataCsv = FileReader.ReadCSVFile(enemyDataFileName);                                          // 몬스터의 데이터 가져옴
        fieldDataCsv = FileReader.ReadCSVFile(fieldDataFileName);
        equipmentCsv = FileReader.ReadCSVFile(equipmentFileName);
        skillCsv = FileReader.ReadCSVFile(skillFileName);
        achievementCsv = FileReader.ReadCSVFile(achievementFileName);

        loginLogText = FileReader.ReadTXTFile(loginLogPath);
        achievementDataText = FileReader.ReadTXTFile(achievementDataSavePath);
        lastLoginLogText = FileReader.ReadTXTFile(lastLogoutLogPath);

        CreateEquipmentDetailData();
        CreateSkillDetailData();
    }

    public void DelegateInit()
    {
        PlayerUpdateEvnet += Player.PlayerUpdateEvent;
        PlayerUpdateEvnet += UIManager.UIManagerUpdateEvent;
        PlayerUpdateEvnet += PlayerInfoUI.PlayerInfoUIUpdateEvent;
        PlayerUpdateEvnet += GachaUI.GachaUIUpdateEvent;
        PlayerUpdateEvnet += EquipmentUI.EquipmentUIUpdateEvent;
        PlayerUpdateEvnet += SkillUI.SKillUIUpdateEvent;
        EquipmentUpdateEvnet += EquipmentUI.EquipmentUIUpdateEvent;
        SkillUpdateEvnet += SkillUI.SKillUIUpdateEvent;
        SkillUpdateEvnet += SkillPresetManager.SkillPresetUIUpdateEvent;
        AchievementUpdateEvnet += AchievementManager.AchievementUpdateEvent;
    }

    public void InitAllData()
    {
        CreateEquipmentData();
        CreateSkillData();
        CreateAchievementData();
    }


    public void SaveAllData()
    {
        SavePlayerData();
        SaveEquipmentData();
        SaveSkillData();
        SaveAchievementData();
        SaveLastLogoutData();
    }

    public void UpdateAllData()
    {
        PlayerUpdateEvnet();
        EquipmentUpdateEvnet();
        SkillUpdateEvnet();
    }

    public void SavePlayerData()
    {
        string json = JsonUtility.ToJson(playerData);
        File.WriteAllText(playerDataSavePath, json);
        PlayerUpdateEvnet();
    }

    public void SaveEquipmentData()
    {
        int size = equipmentData.Length;
        for (int i = 0; i < size; i++)
        {
            if (equipmentData[i] == null)
                continue;
            string newPath = equipmentDataSavePath + (equipmentCsv[i]["EquipmentName"].ToString() + ".json");
            string json = JsonUtility.ToJson(equipmentData[i]);
            File.WriteAllText(newPath, json);
        }
        EquipmentUpdateEvnet();
    }

    public void SaveSkillData()
    {
        int size = skillData.Length;
        for (int i = 0; i < size; i++)
        {
            if (skillData[i] == null)
                continue;
            string newPath = skillDataSavePath + (skillCsv[i]["SkillName"].ToString() + ".json");
            string json = JsonUtility.ToJson(skillData[i]);
            File.WriteAllText(newPath, json);
        }
        SkillUpdateEvnet();
    }

    public void RemoveAllData()
    {
        Debug.Log("Delete All Data");
        RemovePlayerData();
        RemoveEquipmentData();
        RemoveSkillData();
        RemoveAchievementData();
        RemoveAllLoginoutLog();
        RemoveAllLoginoutLog2();
    }

    public void RemovePlayerData()
    {
        Debug.Log("Delete Player Data");
        if (File.Exists(playerDataSavePath))
        {
            File.Delete(playerDataSavePath);
        }
    }

    public void RemoveAllLoginoutLog2()
    {
        Debug.Log("RemoveAllLoginoutLog2");
        if (File.Exists(lastLogoutDataPath))
        {
            File.Delete(lastLogoutDataPath);
        }
    }

    public void RemoveEquipmentData()
    {
        Debug.Log("Delete Equipment Data");
        if (File.Exists(equipmentDataSavePath))
        {
            File.Delete(equipmentDataSavePath);
        }
    }

    public void RemoveSkillData()
    {
        Debug.Log("Delete Skill Data");
        if (File.Exists(skillDataSavePath))
        {
            File.Delete(skillDataSavePath);
        }
    }



    #region 저장된 데이터 불러오기

    public void AllDataLoad()
    {
        LoadPlayerData();
        LoadEquipmentData();
        LoadSkillData();
        LoadAchievementData();
    }

    public bool LoadPlayerData()
    {
        //RemovePlayerData();

        if (File.Exists(playerDataSavePath))
        {
            Debug.Log("PlayerData Exist");

            playerData = new PlayerData();
            string json = File.ReadAllText(playerDataSavePath);
            PlayerData data = JsonUtility.FromJson<PlayerData>(json);

            playerData.playerName = data.playerName;
            playerData.playerLevel = data.playerLevel;
            playerData.atkLevel = data.atkLevel;
            playerData.defLevel = data.defLevel;
            playerData.moveSpeedLevel = data.moveSpeedLevel;
            playerData.maxHpLevel = data.maxHpLevel;
            playerData.maxMpLevel = data.maxMpLevel;
            playerData.currentExp = data.currentExp;
            playerData.currentHp = data.currentHp;
            playerData.statPoint = data.statPoint;
            playerData.animationSpeed = data.animationSpeed;
            playerData.takeSkills = data.takeSkills;
            playerData.skillPreset = data.skillPreset;
            playerData.equipItemName = data.equipItemName;
            playerData.currentGold = data.currentGold;

            return true;
        }
        else
            return false;
    }

    public bool LoadEquipmentData()
    {
        int equipmentCsvSize = equipmentCsv.Count;
        if (equipmentCsvSize <= 0)
        {
            Debug.Log("equipmentCsvSize < 0");
            return false;
        }
        else
        {         
            equipmentData = new ItemData[equipmentCsvSize];


            for (int i = 0; i < equipmentCsvSize; i++)
            {
                string newPath = equipmentDataSavePath + (equipmentCsv[i]["EquipmentName"].ToString() + ".json");
                if (File.Exists(newPath))
                {
                    string json = File.ReadAllText(newPath);
                    ItemData data = JsonUtility.FromJson<ItemData>(json);

                    equipmentData[i] = new ItemData();
                    equipmentData[i].itemName = data.itemName;
                    equipmentData[i].itemShowName = data.itemShowName;
                    equipmentData[i].itemLevel = data.itemLevel;
                    equipmentData[i].itemMaxLevel = data.itemMaxLevel;
                    equipmentData[i].quantity = data.quantity;
                    equipmentData[i].mixCount = data.mixCount;
                    equipmentData[i].isGainItem = data.isGainItem;
                    equipmentData[i].sprite = Resources.Load<Sprite>(equipmentCsv[i]["Sprite"].ToString());

                }
                else
                {
                    Debug.Log("EquipmentData No or Error");
                    return false;
                }
            }
            return true;
        }
    }

    public bool LoadSkillData()
    {
        int skillCsvSize = skillCsv.Count;
        if (skillCsvSize <= 0)
        {
            Debug.Log("skillCsvSize < 0");
            return false;
        }
        else
        {
            skillData = new SkillData[skillCsvSize];
            for (int i = 0; i < skillCsvSize; i++)
            {
                string newPath = skillDataSavePath + (skillCsv[i]["SkillName"].ToString() + ".json");

                if (File.Exists(newPath))
                {
                    string json = File.ReadAllText(newPath);
                    SkillData data = JsonUtility.FromJson<SkillData>(json);

                    skillData[i] = new SkillData();
                    skillData[i].skillName = data.skillName;
                    skillData[i].skillShowName = data.skillShowName;
                    skillData[i].skillLevel = data.skillLevel;
                    skillData[i].skillMaxLevel = data.skillMaxLevel;
                    skillData[i].transcendenceLevel = data.transcendenceLevel;
                    skillData[i].transcendenceMaxLevel = data.transcendenceMaxLevel;
                    skillData[i].skillCurrentMaxLevel = data.transcendenceLevel * 20;
                    skillData[i].quantity = data.quantity;
                    skillData[i].mixCount = data.mixCount;
                    skillData[i].skillCoolTime = data.skillCoolTime;
                    skillData[i].holdingTime = data.holdingTime;
                    skillData[i].currentSkillCoolTime = data.currentSkillCoolTime;
                    skillData[i].isGainSkill = data.isGainSkill;

                    skillData[i].sprite = Resources.Load<Sprite>(skillCsv[i]["Icon_Sprite"].ToString());
                }

                else
                {
                    Debug.Log("SkillData No or Error");
                    return false;
                }
            }
            return true;
        }
    }

    #endregion

    #region 데이터 생성하기(플레이어,장비,스킬)
    public void CreatePlayerData(string playerName)
    {
        playerData = new PlayerData();
        playerData.playerName = playerName;        
        playerData.playerLevel = 1;        
        playerData.atkLevel = 1;           
        playerData.defLevel = 1;           
        playerData.moveSpeedLevel = 1;     
        playerData.maxHpLevel = 1;         
        playerData.maxMpLevel = 1;

        playerData.currentHp = int.Parse(playerStatCsv[playerData.playerLevel]["MaxHp"].ToString());
        playerData.currentExp = 0;
        playerData.statPoint = 0;
        playerData.animationSpeed = 2;   
        playerData.skillPreset = new string[6];
        playerData.equipItemName = "";
    }

    public void CreateEquipmentData()
    {
        int size = equipmentCsv.Count;
        if (equipmentData == null)
            equipmentData = new ItemData[size];
        for (int i = 0; i < size; i++)
        {
            equipmentData[i] = new ItemData();
            equipmentData[i].itemName = equipmentCsv[i]["EquipmentName"].ToString();
            equipmentData[i].itemShowName = equipmentCsv[i]["EquipmentShowName"].ToString();
            equipmentData[i].itemLevel = 1;
            equipmentData[i].itemMaxLevel = int.Parse(equipmentCsv[i]["MaxLevel"].ToString());
            equipmentData[i].quantity = 0;
            equipmentData[i].isGainItem = false;
            equipmentData[i].mixCount = int.Parse(equipmentCsv[i]["MixCount"].ToString());
            equipmentData[i].sprite = Resources.Load<Sprite>(equipmentCsv[i]["Sprite"].ToString());
            string newPath = equipmentDataSavePath + (equipmentCsv[i]["EquipmentName"].ToString() + ".json");
            string json = JsonUtility.ToJson(equipmentData[i]);
            File.WriteAllText(newPath, json);
        }
    }

    public void CreateSkillData()
    {
        int size = skillCsv.Count;
        if (skillData == null)
            skillData = new SkillData[size];
        for (int i = 0; i < size; i++)
        {
            skillData[i] = new SkillData();
            skillData[i].skillName = skillCsv[i]["SkillName"].ToString();
            skillData[i].skillShowName = skillCsv[i]["SkillShowName"].ToString();
            skillData[i].skillLevel = 1;
            skillData[i].skillMaxLevel = int.Parse(skillCsv[i]["MaxLevel"].ToString());
            skillData[i].quantity = 0;
            skillData[i].mixCount = int.Parse(skillCsv[i]["MixCount"].ToString());
            skillData[i].sprite = Resources.Load<Sprite>(skillCsv[i]["Icon_Sprite"].ToString());
            skillData[i].transcendenceLevel = 1;
            skillData[i].skillCurrentMaxLevel = skillData[i].transcendenceLevel * 20;
            skillData[i].transcendenceMaxLevel = int.Parse(skillCsv[i]["TranscendenceMaxLevel"].ToString());
            skillData[i].skillCoolTime = int.Parse(skillCsv[i]["SkillCoolTime"].ToString());
            skillData[i].currentSkillCoolTime = (float)skillData[i].skillCoolTime;
            skillData[i].holdingTime = float.Parse(skillCsv[i]["HoldingTime"].ToString());
            skillData[i].isGainSkill = false;



            string newPath = skillDataSavePath + (skillCsv[i]["SkillName"].ToString() + ".json");
            string json = JsonUtility.ToJson(skillData[i]);
            File.WriteAllText(newPath, json);
        }
    }

    #endregion

    #region 데이터 디테일 생성하기(장비,스킬)
    public void CreateEquipmentDetailData()
    {
        int size = equipmentCsv.Count;
        equipmentDetailCsv = new List<Dictionary<string, object>>[size];

        for (int i = 0; i < size; i++)
        {
            if (equipmentDetailCsv[i] == null)
            {
                var tempPath = equipmentCsv[i]["EquipmentName"].ToString() + "Detail";
                equipmentDetailCsv[i] = FileReader.ReadCSVFile(tempPath);
                if (equipmentDetailCsv[i] == null)
                {
                    Debug.Log(tempPath + "NO Detail File");
                }

            }
        }
    }

    public void CreateSkillDetailData()
    {
        int size = skillCsv.Count;
        skillDetailCsv = new List<Dictionary<string, object>>[size];

        for (int i = 0; i < size; i++)
        {
            if (skillDetailCsv[i] == null)
            {
                var tempPath = skillCsv[i]["SkillName"].ToString() + "Detail";
                skillDetailCsv[i] = FileReader.ReadCSVFile(tempPath);
                if (skillDetailCsv[i] == null)
                {
                    Debug.Log(tempPath + "NO Detail File");
                }

            }
        }
    }

    #endregion

    #region 데이터 파일 변수 통째로 가져오기(플레이어,장비,스킬)
    public PlayerData GetPlayerData()
    {
        return playerData;
    }

    public ItemData[] GetEquipmentData()
    {
        return equipmentData;
    }

    public SkillData[] GetSkillData()
    {
        return skillData;
    }
    #endregion

    #region 데이터 파일 변수 이름으로 그거만 찾아오기
    public ItemData GetFindEquipmentData(string equipmentName)
    {
        ItemData temp = new ItemData();
        foreach (var findEquipmentData in equipmentData)
        {
            if (findEquipmentData.itemName.Equals(equipmentName))
                return findEquipmentData;
        }
        return temp;
    }
    public SkillData GetFindSkillData(string skillName)
    {
        SkillData temp = new SkillData();
        foreach (var findSkillData in skillData)
        {
            if (findSkillData.skillName.Equals(skillName))
                return findSkillData;
        }
        return temp;
    }

    #endregion

    #region CSV파일 변수 가져오기
    public List<Dictionary<string, object>> GetEquipmentDetailCsv(string equipmentName)
    {
        foreach (var detailCSV in equipmentDetailCsv)
        {
            // [0]에 EquipmentName이 적혀있다. 여기서 0은 1레벨을 말한다.
            if (detailCSV[0]["EquipmentName"].ToString().Equals(equipmentName))
                return detailCSV;
        }
        return null;
    }
    public List<Dictionary<string, object>> GetSkillDetailCsv(string skillName)
    {
        foreach (var detailCSV in skillDetailCsv)
        {
            // [0]에 SkillName 적혀있다. 여기서 0은 1레벨을 말한다.
            if (detailCSV[0]["SkillName"].ToString().Equals(skillName))
                return detailCSV;
        }
        return null;
    }
    public List<Dictionary<string, object>> GetStatCSV()
    {
        return playerStatCsv;
    }

    public List<Dictionary<string, object>> GetEquipmentCSV()
    {
        return equipmentCsv;
    }

    public List<Dictionary<string, object>> GetEnemyCSV()
    {
        return enemyDataCsv;
    }

    public List<Dictionary<string, object>> GetFieldCSV()
    {
        return fieldDataCsv;
    }

    #endregion

    #region 로그인 관련
    public void SaveLoginOutLog(string inOut)
    {
        Debug.Log("로그인 기록 Text 파일 찾는중...");
        if (loginLogText == "")
        {
            Debug.Log("로그인 기록 Text이 없습니다.");
            return;
        }
        FileReader.WriteTXTFile(loginLogPath, inOut + ":" + System.DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss"));
        Debug.Log(System.DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss"));

        if (inOut.Equals("Logout"))
        {
            SaveLastLogoutLog();
            SaveAllData();
        }
    }

    public void SaveLastLogoutLog()
    {
        using (StreamWriter writer = new StreamWriter(lastLogoutLogPath, false))
        {
            writer.WriteLine(System.DateTime.Now.ToString("yyyy"));
            writer.WriteLine(System.DateTime.Now.ToString("MM"));
            writer.WriteLine(System.DateTime.Now.ToString("dd"));
            writer.WriteLine(System.DateTime.Now.ToString("HH"));
            writer.WriteLine(System.DateTime.Now.ToString("mm"));
            writer.WriteLine(System.DateTime.Now.ToString("ss"));
        }
    }

    public void SaveLastLogoutData()
    {
        // 현재시간을 가져옴.
        lastLogoutData = new DateTimeData();
        lastLogoutData.year = int.Parse(System.DateTime.Now.ToString("yyyy"));
        lastLogoutData.month = int.Parse(System.DateTime.Now.ToString("MM"));
        lastLogoutData.day = int.Parse(System.DateTime.Now.ToString("dd"));
        lastLogoutData.hour = int.Parse(System.DateTime.Now.ToString("HH"));
        lastLogoutData.minute = int.Parse(System.DateTime.Now.ToString("mm"));
        lastLogoutData.second = int.Parse(System.DateTime.Now.ToString("mm"));

        string json = JsonUtility.ToJson(lastLogoutData);
        File.WriteAllText(lastLogoutDataPath, json);
    }

    public DateTimeData LoadLastLogoutData()
    {
        lastLogoutData = new DateTimeData();
        if (File.Exists(lastLogoutDataPath))
        {
            string json = File.ReadAllText(lastLogoutDataPath);
            lastLogoutData = JsonUtility.FromJson<DateTimeData>(json);

            Debug.Log("lastLogoutData : " + lastLogoutData);
        }
        return lastLogoutData;
    }

     public void RemoveAllLoginoutLog()
    {
        // 마지막 로그아웃기록 삭제
        if (File.Exists(lastLogoutLogPath))
        {
            File.Delete(lastLogoutLogPath);
            Debug.Log("Remove LastLogoutLog");
        }
        else
        {
            Debug.LogWarning("LastLogoutLog does not exist");
        }

        if (File.Exists(loginLogPath))
        {
            File.Delete(loginLogPath);
            Debug.Log("Remove LoginLogPath");
        }
        else
        {
            Debug.LogWarning("loginLog does not exist");
        }
    }

    #endregion

    #region 플레이어 스탯관련

    public void AtkLevelUp()
    {
        playerData.atkLevel++;
        SavePlayerData();
    }
    public void DefLevelUp()
    {
        playerData.defLevel++;
        SavePlayerData();
    }

    public void MoveSpeedLevelUp()
    {
        playerData.moveSpeedLevel++;
        SavePlayerData();
    }
    public void MaxHpLevelUp()
    {
        playerData.maxHpLevel++;
        SavePlayerData();
    }
    public void MaxMpLevelUp()
    {
        playerData.maxMpLevel++;
        SavePlayerData();
    }

    public void StatPointUse()
    {
        playerData.statPoint--;
        SavePlayerData();
    }

    #endregion

    public int EquipAtkEquipment(string equipmentName)
    {
        // -1 : 장비이름이 잘못됐거나, 장비 데이터가 존재하지않는경우
        if (equipmentName == null)
            return -1;
        foreach (var equipment in equipmentData)
        {
            if (equipment.itemName.Equals(equipmentName))
            {
                var equipmentDataDetail = GetEquipmentDetailCsv(equipmentName);
                int atk = int.Parse(equipmentDataDetail[equipment.itemLevel]["EquipAtk"].ToString());
                return atk;
            }

        }

        return -1;
    }

    public void EquipmentLevelUp(string equipmentName)
    {
        foreach (var equipment in equipmentData)
        {
            if (equipment.itemName.Equals(equipmentName))
            {
                equipment.itemLevel++;
                PlayerUpdateEvnet();
            }

        }
    }


    public void SkillLevelUp(string skillName)
    {
        foreach (var skill in skillData)
        {
            if (skill.skillName.Equals(skillName))
            {
                skill.skillLevel++;
                PlayerUpdateEvnet();
            }

        }
    }

    public void SkillTranscendenceLevelUp(string skillName)
    {
        foreach (var skill in skillData)
        {
            if (skill.skillName.Equals(skillName))
            {
                if (skill.transcendenceLevel < skill.transcendenceMaxLevel)
                {
                    skill.transcendenceLevel++;
                    skill.skillCurrentMaxLevel = skill.transcendenceLevel * 20;
                    skill.quantity -= skill.mixCount;
                    SkillUpdateEvnet();
                }
            }

        }
    }




    public void GainEquipment(string itemName)
    {
        {
            foreach (var equipment in equipmentData)
            {
                if (equipment.itemName.Equals(itemName))
                {
                    equipment.quantity++;
                    if (equipment.isGainItem == false)
                        equipment.isGainItem = true;
                    EquipmentUpdateEvnet();

                    PlayerUpdateEvnet();
                    return;
                }

            }
        }
    }

    public void GainExp(int exp)
    {
        int maxExp =  int.Parse(playerStatCsv[playerData.playerLevel]["MaxExp"].ToString());
        playerData.currentExp += exp;
        while (true)
        {
            if (playerData.currentExp >= maxExp)
            {
                maxExp = int.Parse(playerStatCsv[playerData.playerLevel]["MaxExp"].ToString());
                playerData.currentExp -= maxExp;
                playerData.playerLevel++;
                playerData.statPoint++;
                playerData.currentHp = int.Parse(playerStatCsv[playerData.maxHpLevel]["MaxHp"].ToString());

            }
            else
                break;
        }
        SavePlayerData();
    }

    public void GainGold(int gold)
    {
        playerData.currentGold += gold;
        SavePlayerData();
    }

    public string GetPlayerEquipItemName()
    {
        return playerData.equipItemName;
    }

    public void SetPlayerEquipItemName(string equipItemName)
    {
        playerData.equipItemName = equipItemName;
    }

    public void GainSkillGacha(string skillName , int quantity)
    {
        foreach (var skill in skillData)
        {
            if (skill.skillName.Equals(skillName))
            {
                if (skill.isGainSkill == false)
                    skill.isGainSkill = true;

                skill.quantity += quantity;

                PlayerUpdateEvnet();
                return;
            }

        }
    }

    public void GainEquipmentGacha(string equipmentName, int quantity)
    {
        foreach (var equipment in equipmentData)
        {
            if (equipment.itemName.Equals(equipmentName))
            {
                if (equipment.isGainItem == false)
                    equipment.isGainItem = true;
                equipment.quantity+= quantity;

                EquipmentUpdateEvnet();

                PlayerUpdateEvnet();
                return;
            }

        }
    }

    public void SetEquipmentQuantity(string itemName , int quantity)
    {
        foreach (var equipment in equipmentData)
        {
            if (equipment.itemName.Equals(itemName))
            {
                if (equipment.isGainItem == false)
                    equipment.isGainItem = true;

                equipment.quantity = quantity;
                EquipmentUpdateEvnet();
                return;
            }

        }
    }

    public void SetSkillQuantity(string skillName, int quantity)
    {
        foreach (var skill in skillData)
        {
            if (skill.skillName.Equals(skillName))
            {
                if (skill.isGainSkill == false)
                    skill.isGainSkill = true;

                skill.quantity = quantity;
                PlayerUpdateEvnet();
                return;
            }

        }
    }

    public int GetEquipmentHaveAtk()
    {
        int equipmentHaveAtk = 0;

        foreach (var equipment in equipmentData)
        {
            if (equipment.isGainItem)
            {
               var equipmentDataDetail= GetEquipmentDetailCsv(equipment.itemName);
                equipmentHaveAtk+=int.Parse(equipmentDataDetail[equipment.itemLevel]["HaveAtk"].ToString());
            }
        }

        return equipmentHaveAtk;
    }

    public int GetEquipmentEquipAtk()
    {
        int equipmentEquipAtk = 0;

        foreach (var equipment in equipmentData)
        {
            if (playerData.equipItemName.Equals(equipment.itemName))
            {
                var equipmentDataDetail = GetEquipmentDetailCsv(equipment.itemName);
                equipmentEquipAtk += int.Parse(equipmentDataDetail[equipment.itemLevel]["EquipAtk"].ToString());
                return equipmentEquipAtk;
            }
        }

        return equipmentEquipAtk;
    }

    public int GetSkillHaveAtk()
    {
        int skillHaveAtk = 0;

        foreach (var skill in skillData)
        {
            if (skill.isGainSkill)
            {
                var skillDataDetail = GetSkillDetailCsv(skill.skillName);
                skillHaveAtk += int.Parse(skillDataDetail[skill.skillLevel]["HaveAtk"].ToString());
            }
        }

        return skillHaveAtk;
    }

    public void SetSkillPreset(string skillName , int presetNum)
    {
        playerData.skillPreset[presetNum - 1] = skillName;

        SkillUpdateEvnet();
    }

    public string[] GetSkillPreset()
    {
        return playerData.skillPreset;
    }

    public void PlayerAttacked(int damage)
    {
        // 몬스터가 플레이어한테 데미지를 입힘.
        playerData.currentHp -= damage;

        PlayerUpdateEvnet();
    }

    public int GetPlayerPower()
    {
        int playerPower = 0;

        int playerAtkStat = int.Parse(playerStatCsv[playerData.atkLevel]["Atk"].ToString());
        int equipmentHaveAtk = 0;
        int skillHaveAtk = 0;

        foreach (var equipment in equipmentData)
        {
            var getCsv = GetEquipmentDetailCsv(equipment.itemName);
            equipmentHaveAtk +=int.Parse(getCsv[equipment.itemLevel]["HaveAtk"].ToString());

        }

        foreach (var skill in skillData)
        {
            var getCsv = GetSkillDetailCsv(skill.skillName);
            skillHaveAtk += int.Parse(getCsv[skill.skillLevel]["HaveAtk"].ToString());
        }
        playerPower = playerAtkStat + equipmentHaveAtk + skillHaveAtk;
        return playerPower;
    }

    #region 임무관련
    public void LoadAchievementData()
    {
        string jsonData = File.ReadAllText(achievementDataSavePath); // 파일에서 JSON 문자열 불러오기
        achievementData = JsonUtility.FromJson<AchievementDataArray>(jsonData).achievementDataArray; // JSON 문자열을 객체 배열로 역직렬화
    }

    public void CreateAchievementData()
    {
        int size = achievementCsv.Count;
        if (achievementData == null)
            achievementData = new AchievementData[size];
        achievementData = new AchievementData[size];
        for (int i = 0; i < size; i++)
        {
            achievementData[i] = new AchievementData();
            achievementData[i].achievementName = achievementCsv[i]["AchievementName"].ToString();
            achievementData[i].description = achievementCsv[i]["Description"].ToString();
            achievementData[i].currentCount = 0;
            achievementData[i].successCount = int.Parse(achievementCsv[i]["SuccessCount"].ToString());
            achievementData[i].reward = int.Parse(achievementCsv[i]["Reward"].ToString());
            achievementData[i].isLoop = (int.Parse(achievementCsv[i]["IsLoop"].ToString()) !=0);
            achievementData[i].isSuccess = false;
            achievementData[i].isReceiveReward = false;
        }

        string jsonData = JsonUtility.ToJson(new AchievementDataArray { achievementDataArray = achievementData }); // 객체 배열을 JSON 문자열로 변환
        File.WriteAllText(achievementDataSavePath, jsonData); // JSON 문자열을 파일로 저장
    }


    public void SaveAchievementData()
    {
        string jsonData = JsonUtility.ToJson(new AchievementDataArray { achievementDataArray = achievementData }); // 객체 배열을 JSON 문자열로 변환
        File.WriteAllText(achievementDataSavePath, jsonData); // JSON 문자열을 파일로 저장
    }

    public void RemoveAchievementData()
    {
        Debug.Log("Delete Achievement Data");
        if (File.Exists(achievementDataSavePath))
        {
            File.Delete(achievementDataSavePath);
        }
    }

    public void UpdateAchievementData(string achievementName, bool isFix , int count)
    {
        foreach(var achievement in achievementData)
        {
            if(achievement.achievementName.Equals(achievementName))
            {
                if (isFix)
                {
                    achievement.currentCount = count;
                }
                else
                {
                    // 아니면 증가
                    int value = achievement.currentCount;
                    value++;
                    achievement.currentCount = value;
                }
                if (achievement.currentCount >= achievement.successCount)
                    achievement.isSuccess = true;
                else
                    achievement.isSuccess = false;
                AchievementUpdateEvnet();
                return;
            }
        }
    }

    public AchievementData[] GetAchievementData()
    {
        return achievementData;
    }


    public void SetAchievementDataIsSuccess(string achievementName , bool isSuccess)
    {
        foreach (var achievement in achievementData)
        {
            if (achievement.achievementName.Equals(achievementName))
            {
                achievement.isSuccess = isSuccess;
                if (achievementName.Equals("Login"))
                    achievement.currentCount = 1;

                AchievementUpdateEvnet();
                return;
            }
        }
    }
    public void SetAchievementDataIsReceiveReward(string achievementName, bool isReceiveReward)
    {
        foreach (var achievement in achievementData)
        {
            if (achievement.achievementName.Equals(achievementName))
            {
                achievement.isReceiveReward = isReceiveReward;
                AchievementUpdateEvnet();
                return;
            }
        }
    }



    #endregion
}
