using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

delegate void UpdateData();

public class DataManager : MonoBehaviour
{
    private static DataManager instance = null;

    private PlayerData playerData;
    //private static PlayerData playerDataAsset;
    private ItemData[] equipmentData;
    private SkillData[] skillData;
    private AchievementData[] achievementData;
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


    //private const string loginLogPath = @"Assets\Resources\TextFile\LoginLog.txt";
    //private const string lastLogoutLogPath = @"Assets\Resources\TextFile\LastLoginLog.txt";
    //private const string playerStatFileName = "playerStatData";
    //private const string equipmentFileName = "EquipmentInfo";
    //private const string skillFileName = "SkillInfo";
    //private const string achievementFileName = "AchievementInfo";
    //private const string equipmentDataFileName = "EquipmenData";
    //private const string enemyDataFileName = "enemyData";
    //private const string fieldDataFileName = "fieldData";

    private const string playerStatFileName = "playerStatData";
    private const string equipmentFileName = "EquipmentInfo";
    private const string skillFileName = "SkillInfo";
    private const string achievementFileName = "AchievementInfo";
    private const string equipmentDataFileName = "EquipmenData";
    private const string enemyDataFileName = "enemyData";
    private const string fieldDataFileName = "fieldData";


    private string playerDataSavePath;
    private string equipmentDataSavePath;
    private string achievementDataSavePath;// = @"Assets\Resources\SaveData\Achievement.json";
    private string skillDataSavePath;
    private string loginLogPath; 
    private string lastLogoutLogPath;


    private event UpdateData PlayerUpdateEvnet;
    private event UpdateData EquipmentUpdateEvnet;
    private event UpdateData SkillUpdateEvnet;
    private event UpdateData AchievementUpdateEvnet;

    private void Start()
    {

        Debug.Log("DataManagerStart");
        if (instance == null)
            instance = this;

        playerDataSavePath = Application.persistentDataPath + "PlayerData.json"; //Application.streamingAssetsPath /*+ "\\SaveData\\"*/+"PlayerData.json";
        equipmentDataSavePath = Application.persistentDataPath;//Application.streamingAssetsPath;/* + "\\SaveData\\";*/// + "PlayerData.json";// + "/EquipmentData.dat";
        skillDataSavePath = Application.persistentDataPath;//Application.streamingAssetsPath; /*+"\\SaveData\\";*/// + "PlayerData.json";// + "/SkillData.dat";
        achievementDataSavePath = Application.persistentDataPath + "Achievementdata.json";
        loginLogPath =  Application.persistentDataPath + "LoginLog.txt";
        lastLogoutLogPath = Application.persistentDataPath + "LastLoginLog.txt";
        //playerData = new PlayerData();

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

        //playerStatCsv = FileReader.StreamReaderRead(playerStatFileName);
        //enemyDataCsv = FileReader.StreamReaderRead(enemyDataFileName);                                          // 몬스터의 데이터 가져옴
        //fieldDataCsv = FileReader.StreamReaderRead(fieldDataFileName);
        //equipmentCsv = FileReader.StreamReaderRead(equipmentFileName);
        //skillCsv = FileReader.StreamReaderRead(skillFileName);
        //achievementCsv = FileReader.StreamReaderRead(achievementFileName);




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
        Debug.Log("CreateEquipmentData");
        CreateEquipmentData();
        Debug.Log("CreateSkillData");
        CreateSkillData();
        Debug.Log("CreateAchievementData");
        CreateAchievementData();
    }


    public void SaveAllData()
    {
        SavePlayerData();
        SaveEquipmentData();
        SaveSkillData();
        SaveAchievementData();
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
        //UpdateAllData();
    }

    public void SaveEquipmentData()
    {
        int size = equipmentData.Length;
        for (int i = 0; i < size; i++)
        {
            if (equipmentData[i] == null)
                continue;
            string newPath = equipmentDataSavePath + ("/" + equipmentCsv[i]["EquipmentName"].ToString() + ".json");
            string json = JsonUtility.ToJson(equipmentData[i]);
            File.WriteAllText(newPath, json);
        }
        EquipmentUpdateEvnet();
        //UpdateAllData();
    }

    public void SaveSkillData()
    {
        int size = skillData.Length;
        for (int i = 0; i < size; i++)
        {
            if (skillData[i] == null)
                continue;
            string newPath = skillDataSavePath + ("/" + skillCsv[i]["SkillName"].ToString() + ".json");
            string json = JsonUtility.ToJson(skillData[i]);
            File.WriteAllText(newPath, json);
        }
        SkillUpdateEvnet();
        //UpdateAllData();
    }

    public void RemoveAllData()
    {
        Debug.Log("Delete All Data");
        RemovePlayerData();
        RemoveEquipmentData();
        RemoveSkillData();
        RemoveAchievementData();
        RemoveAllLoginoutLog();
    }

    public void RemovePlayerData()
    {
        Debug.Log("Delete Player Data");
        if (File.Exists(playerDataSavePath))
        {
            File.Delete(playerDataSavePath);
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
            playerData.playerObject = data.playerObject;
            playerData.currentGold = data.currentGold;

            return true;
        }
        else
            return false;


        //if (File.Exists(playerDataSavePath))
        //{
        //    BinaryFormatter bf = new BinaryFormatter();
        //    FileStream file = File.Open(playerDataSavePath, FileMode.Open);
        //    PlayerData data = (PlayerData)bf.Deserialize(file);
        //    file.Close();

        //    playerData.playerName = data.playerName;
        //    playerData.playerLevel = data.playerLevel;
        //    playerData.atkLevel = data.atkLevel;
        //    playerData.defLevel = data.defLevel;
        //    playerData.moveSpeedLevel = data.moveSpeedLevel;
        //    playerData.maxHpLevel = data.maxHpLevel;
        //    playerData.maxMpLevel = data.maxMpLevel;
        //    playerData.currentExp = data.currentExp;
        //    playerData.statPoint = data.statPoint;
        //    playerData.animationSpeed = data.animationSpeed;
        //    playerData.takeSkills = data.takeSkills;
        //    playerData.skillPreset = data.skillPreset;
        //    playerData.equipItemName = data.equipItemName;
        //    playerData.playerObject = data.playerObject;
        //    playerData.currentGold = data.currentGold;

        //    return true;
        //}
        //else
        //    return false;
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
                string newPath = equipmentDataSavePath + ("/" + equipmentCsv[i]["EquipmentName"].ToString() + ".json");
                if (File.Exists(newPath))
                {
                    string json = File.ReadAllText(newPath);
                    ItemData data = JsonUtility.FromJson<ItemData>(json);

                    equipmentData[i] = new ItemData();
                    //equipmentData[i] = ScriptableObject.CreateInstance<ItemData>();
                    //equipmentData[i].name = equipmentCsv[i]["EquipmentName"].ToString() + "Data";
                    equipmentData[i].itemName = data.itemName;
                    equipmentData[i].itemShowName = data.itemShowName;
                    equipmentData[i].itemLevel = data.itemLevel;
                    equipmentData[i].itemMaxLevel = data.itemMaxLevel;
                    equipmentData[i].quantity = data.quantity;
                    equipmentData[i].mixCount = data.mixCount;
                    equipmentData[i].isGainItem = data.isGainItem;
                    //equipmentData[i].sprite = data.sprite;
                    equipmentData[i].sprite = Resources.Load<Sprite>(equipmentCsv[i]["EquipmentName"].ToString());
                }
                else
                {
                    Debug.Log("EquipmentData No or Error");
                    return false;
                }
            }
            return true;


            //for (int i = 0; i < equipmentCsvSize; i++)
            //{
            //    string newPath =  equipmentDataSavePath + ("/" + equipmentCsv[i]["EquipmentName"].ToString() + ".json");
            //    if (File.Exists(newPath))
            //    {
            //        BinaryFormatter bf = new BinaryFormatter();
            //        FileStream file = File.Open(newPath, FileMode.Open);
            //        ItemData data = (ItemData)bf.Deserialize(file);
            //        file.Close();

            //        equipmentData[i] = ScriptableObject.CreateInstance<ItemData>();
            //        equipmentData[i].name = equipmentCsv[i]["EquipmentName"].ToString() + "Data";
            //        equipmentData[i].itemName = data.itemName;
            //        equipmentData[i].itemShowName = data.itemShowName;
            //        equipmentData[i].itemLevel = data.itemLevel;
            //        equipmentData[i].itemMaxLevel = data.itemMaxLevel;
            //        equipmentData[i].quantity = data.quantity;
            //        equipmentData[i].mixCount = data.mixCount;
            //        equipmentData[i].sprite = data.sprite;
            //    }
            //    else
            //    {
            //        Debug.Log("EquipmentData No or Error");
            //        return false;
            //    }    
            //}
            //return true;
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
                string newPath = skillDataSavePath + ("/" + skillCsv[i]["SkillName"].ToString() + ".json");

                if (File.Exists(newPath))
                {
                    string json = File.ReadAllText(newPath);
                    SkillData data = JsonUtility.FromJson<SkillData>(json);

                    skillData[i] = new SkillData();
                    //skillData[i] = ScriptableObject.CreateInstance<SkillData>();
                    //skillData[i].name = skillCsv[i]["SkillName"].ToString() + "Data";
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
                    //skillData[i].sprite = data.sprite;
                    skillData[i].holdingTime = data.holdingTime;
                    skillData[i].currentSkillCoolTime = data.currentSkillCoolTime;
                    skillData[i].isGainSkill = data.isGainSkill;

                    skillData[i].sprite = Resources.Load<Sprite>(skillCsv[i]["Icon_Sprite"].ToString());
                }


                //    if (File.Exists(newPath))
                //{
                //    BinaryFormatter bf = new BinaryFormatter();
                //    FileStream file = File.Open(newPath, FileMode.Open);
                //    SkillData data = (SkillData)bf.Deserialize(file);
                //    file.Close();

                    //    skillData[i] = ScriptableObject.CreateInstance<SkillData>();
                    //    skillData[i].name = skillCsv[i]["SkillName"].ToString() + "Data";
                    //    skillData[i].skillName = data.skillName;
                    //    skillData[i].skillShowName = data.skillShowName;
                    //    skillData[i].skillLevel = data.skillLevel;
                    //    skillData[i].skillMaxLevel = data.skillMaxLevel;
                    //    skillData[i].transcendenceLevel = data.transcendenceLevel;
                    //    skillData[i].transcendenceMaxLevel = data.transcendenceMaxLevel;
                    //    skillData[i].quantity = data.quantity;
                    //    skillData[i].mixCount = data.mixCount;
                    //    skillData[i].skillCoolTime = data.skillCoolTime;
                    //    skillData[i].sprite = data.sprite;
                    //    skillData[i].holdingTime = data.holdingTime;
                    //    skillData[i].currentSkillCoolTime = data.currentSkillCoolTime;
                    //}
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
        int size = 0;
        foreach (var temp in playerStatCsv)
        {
            Debug.Log(temp["MaxHp"].ToString());
            Debug.Log(temp["MaxMp"].ToString());
            Debug.Log(temp["MaxExp"].ToString());
        }



        Debug.Log("CreatePlayerData Datamaneger");
        playerData = new PlayerData();
        //playerData = ScriptableObject.CreateInstance<PlayerData>();
        playerData.playerName = playerName;        
        playerData.playerLevel = 1;        
        playerData.atkLevel = 1;           
        playerData.defLevel = 1;           
        playerData.moveSpeedLevel = 1;     
        playerData.maxHpLevel = 1;         
        playerData.maxMpLevel = 1;

        Debug.Log("playerStatCsv[playerData.playerLevel][MaxHp].ToString() : " + playerStatCsv[playerData.playerLevel]["MaxHp"].ToString());
        playerData.currentHp = int.Parse(playerStatCsv[playerData.playerLevel]["MaxHp"].ToString());
        Debug.Log("CreatePlayerData Datamaneger MaxHp after");
        playerData.currentExp = 0;
        playerData.statPoint = 0;
        playerData.animationSpeed = 2;   
        playerData.skillPreset = new string[6];
        playerData.equipItemName = null;

        //string json = JsonUtility.ToJson(playerData);
        //File.WriteAllText(playerDataSavePath, json);
    }

    public void CreateEquipmentData()
    {
        int size = equipmentCsv.Count;
        Debug.Log("CreateEquipmentData size :"+size);
        Debug.Log("CreateEquipmentData Conut before");
        if (equipmentData == null)
            equipmentData = new ItemData[size];
        Debug.Log("CreateEquipmentData Conut after");
        for (int i = 0; i < size; i++)
        {
        Debug.Log("CreateEquipmentData new B");
            equipmentData[i] = new ItemData();
        Debug.Log("CreateEquipmentData new A");
            //string temp = equipmentCsv[i]["SaveDataPath"].ToString();
            //equipmentData[i] = ScriptableObject.CreateInstance<ItemData>();
            //equipmentData[i].name = equipmentCsv[i]["EquipmentName"].ToString() + "Data";
        Debug.Log("CreateEquipmentData Name A");
            equipmentData[i].itemName = equipmentCsv[i]["EquipmentName"].ToString();
        Debug.Log("CreateEquipmentData Show A");
            equipmentData[i].itemShowName = equipmentCsv[i]["EquipmentShowName"].ToString();
        Debug.Log("CreateEquipmentData level A");
            equipmentData[i].itemLevel = 1;
        Debug.Log("CreateEquipmentData maxL A");
            equipmentData[i].itemMaxLevel = int.Parse(equipmentCsv[i]["MaxLevel"].ToString());
        Debug.Log("CreateEquipmentData qun A");
            equipmentData[i].quantity = 0;
            equipmentData[i].isGainItem = false;
        Debug.Log("CreateEquipmentData mix A");
            equipmentData[i].mixCount = int.Parse(equipmentCsv[i]["MixCount"].ToString());
        Debug.Log("CreateEquipmentData spr A");
            equipmentData[i].sprite = Resources.Load<Sprite>(equipmentCsv[i]["Sprite"].ToString());
        Debug.Log("CreateEquipmentData spr2 A");

            string newPath = equipmentDataSavePath + ("\\" + equipmentCsv[i]["EquipmentName"].ToString() + ".json");
        Debug.Log("CreateEquipmentData json A");
            string json = JsonUtility.ToJson(equipmentData[i]);
        Debug.Log("CreateEquipmentData newPath:"+newPath);
            File.WriteAllText(newPath, json);
        Debug.Log("CreateEquipmentData json3 A");



            //BinaryFormatter bf = new BinaryFormatter();

            //string newPath = equipmentDataSavePath + ("/" + equipmentCsv[i]["EquipmentName"].ToString() + ".json");
            //FileStream file = File.Create(newPath);
            //bf.Serialize(file, equipmentData[i]);
            //file.Close();
        }


        //int size = equipmentCsv.Count;
        //equipmentData = new ItemData[size];
        //for (int i = 0; i < size; i++)
        //{
        //    string temp = equipmentCsv[i]["SaveDataPath"].ToString();
        //    equipmentData[i] = AssetDatabase.LoadAssetAtPath(equipmentCsv[i]["SaveDataPath"].ToString(), typeof(ItemData)) as ItemData;

        //    if (equipmentData[i] == null)
        //    {
        //        equipmentData[i] = ScriptableObject.CreateInstance<ItemData>();
        //        equipmentData[i].name = equipmentCsv[i]["EquipmentName"].ToString() + "Data";
        //        equipmentData[i].itemName = equipmentCsv[i]["EquipmentName"].ToString();
        //        equipmentData[i].itemShowName = equipmentCsv[i]["EquipmentShowName"].ToString();
        //        equipmentData[i].itemLevel = 1;
        //        equipmentData[i].itemMaxLevel = int.Parse(equipmentCsv[i]["MaxLevel"].ToString());
        //        equipmentData[i].quantity = 0;
        //        equipmentData[i].mixCount = int.Parse(equipmentCsv[i]["MixCount"].ToString());
        //        equipmentData[i].sprite = Resources.Load<Sprite>(equipmentCsv[i]["EquipmentName"].ToString());


        //        //AssetDatabase.CreateAsset(equipmentDataAsset[i], equipmentCsv[i]["SaveDataPath"].ToString());
        //        //EditorUtility.SetDirty(equipmentDataAsset[i]);
        //        //AssetDatabase.SaveAssets();
        //    }
        //}
    }

    public void CreateSkillData()
    {
        int size = skillCsv.Count;
        if (skillData == null)
            skillData = new SkillData[size];
        for (int i = 0; i < size; i++)
        {
            //string temp = skillCsv[i]["SaveDataPath"].ToString();
            //skillData[i] = AssetDatabase.LoadAssetAtPath(skillCsv[i]["SaveDataPath"].ToString(), typeof(SkillData)) as SkillData;

            //if (skillData[i] == null)
            //{
            //skillData[i] = ScriptableObject.CreateInstance<SkillData>();
            //skillData[i].name = skillCsv[i]["SkillName"].ToString() + "Data";
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



                string newPath = skillDataSavePath + ("/" + skillCsv[i]["SkillName"].ToString() + ".json");
            string json = JsonUtility.ToJson(skillData[i]);
            File.WriteAllText(newPath, json);



            //BinaryFormatter bf = new BinaryFormatter();
            //    string newPath = skillDataSavePath + ("/" + skillCsv[i]["SkillName"].ToString() + ".json");
            //    FileStream file = File.Create(newPath);
            //    bf.Serialize(file, skillData[i]);
            //    file.Close();
            //}
        }

        //int size = skillCsv.Count;
        //skillDataQuantity = size;
        //skillData = new SkillData[size];
        //for (int i = 0; i < size; i++)
        //{
        //    string temp = skillCsv[i]["SaveDataPath"].ToString();
        //    skillData[i] = AssetDatabase.LoadAssetAtPath(skillCsv[i]["SaveDataPath"].ToString(), typeof(SkillData)) as SkillData;

        //    if (skillData[i] == null)
        //    {
        //        skillData[i] = ScriptableObject.CreateInstance<SkillData>();
        //        skillData[i].name = skillCsv[i]["SkillName"].ToString() + "Data";
        //        skillData[i].skillName = skillCsv[i]["SkillName"].ToString();
        //        skillData[i].skillShowName = skillCsv[i]["SkillShowName"].ToString();
        //        skillData[i].skillLevel = 1;
        //        skillData[i].skillMaxLevel = int.Parse(skillCsv[i]["MaxLevel"].ToString());
        //        skillData[i].quantity = 0;
        //        skillData[i].mixCount = int.Parse(skillCsv[i]["MixCount"].ToString());
        //        skillData[i].sprite = Resources.Load<Sprite>(skillCsv[i]["Icon_Sprite"].ToString());
        //        skillData[i].transcendenceLevel = 0;
        //        skillData[i].transcendenceMaxLevel = int.Parse(skillCsv[i]["TranscendenceMaxLevel"].ToString());
        //        skillData[i].skillCoolTime = int.Parse(skillCsv[i]["SkillCoolTime"].ToString());
        //        skillData[i].currentSkillCoolTime = (float)skillData[i].skillCoolTime;
        //        skillData[i].holdingTime = float.Parse(skillCsv[i]["HoldingTime"].ToString());
        //        //AssetDatabase.CreateAsset(skillDataAsset[i], skillCsv[i]["SaveDataPath"].ToString());
        //        //EditorUtility.SetDirty(skillDataAsset[i]);
        //        //AssetDatabase.SaveAssets();
        //    }
        //}
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
                //equipmentDetailCsv[i] = FileReader.StreamReaderRead(tempPath);
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
                //skillDetailCsv[i] = FileReader.StreamReaderRead(tempPath);
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
    public string GetLoginLog()
    {
        return loginLogText;
    }
    //public bool IsLoginExist()
    //{
    //    return playerData != null;
    //}
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



    public string[] LoadlastLoginLog()
    {
        string[] temp = new string[6];
        int index = 0;
        if (File.Exists(lastLogoutLogPath))
        {   
            using (StreamReader reader = new StreamReader(lastLogoutLogPath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    temp[index] = line;
                    index++;
                }
            }
        }
        else
        {
            Debug.Log("파일이 존재하지 않습니다.");
            return null;
        }

        if (index != 6)
            return null;
        else
            return temp;
            

        //Debug.Log("마지막 로그인 기록 찾는중...");
        //if (lastLoginLogText == "")
        //{
        //    Debug.Log("로그인 기록 Text이 없습니다.");
        //    return null;
        //}
        //FileReader.WriteTXTFile(lastLoginLogPath, inOut + ":" + System.DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss"));
        //Debug.Log(System.DateTime.Now.ToString("yyyy-MM-dd:HH-mm-ss"));

        //return lastLoginLogText;
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

    //public void PlayerLevelUp()
    //{
    //    playerData.playerLevel++;
    //    GainStatPoint();
    //}
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

    //public void GainStatPoint()
    //{
    //    playerData.statPoint++;
    //    SavePlayerData();
    //}

    public void StatPointUse()
    {
        playerData.statPoint--;
        SavePlayerData();
    }

    //public void SetExp(int exp)
    //{
    //    playerData.currentExp = exp;
    //}

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
                //EditorUtility.SetDirty(equipment);
                //AssetDatabase.SaveAssets();
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
                //EditorUtility.SetDirty(equipment);
                //AssetDatabase.SaveAssets();
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
                //EditorUtility.SetDirty(equipment);
                //AssetDatabase.SaveAssets();
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
                    //EditorUtility.SetDirty(equipment);
                    //AssetDatabase.SaveAssets();
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



    //public void SetGold(int gold)
    //{
    //    playerData.currentGold = gold;
    //}


    public void SetSkillPreset(string skillName , int presetNum)
    {
        playerData.skillPreset[presetNum - 1] = skillName;

        SkillUpdateEvnet();

        //GameObject findObject = GameObject.FindGameObjectWithTag("SkillPresetUI");

        //if (findObject == null)
        //    return;
        //else
        //    findObject.SendMessage("UpdateSkillPreset");

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
        string jsonData = File.ReadAllText(achievementDataSavePath/*Application.persistentDataPath + "/Achievementdata.json"*/); // 파일에서 JSON 문자열 불러오기
        achievementData = JsonUtility.FromJson<AchievementDataArray>(jsonData).achievementDataArray; // JSON 문자열을 객체 배열로 역직렬화

        //if (File.Exists(saveAchievementDataPath))
        //{



        //    Debug.Log("PlayerData Exist");

        //    string json = File.ReadAllText(saveAchievementDataPath);
        //    achievementData = JsonUtility.FromJson<Dictionary<string, int>>(json);
            
        //    return true;
        //}
        //else
        //    return false;
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


            //string newPath = skillDataSavePath + ("/" + skillCsv[i]["SkillName"].ToString() + ".json");
            //string json = JsonUtility.ToJson(skillData[i]);
            //File.WriteAllText(newPath, json);

        }

        string jsonData = JsonUtility.ToJson(new AchievementDataArray { achievementDataArray = achievementData }); // 객체 배열을 JSON 문자열로 변환
        File.WriteAllText(achievementDataSavePath/*Application.persistentDataPath + "/Achievementdata.json"*/, jsonData); // JSON 문자열을 파일로 저장
    }


    public void SaveAchievementData()
    {
        string jsonData = JsonUtility.ToJson(new AchievementDataArray { achievementDataArray = achievementData }); // 객체 배열을 JSON 문자열로 변환
        File.WriteAllText(achievementDataSavePath/*Application.persistentDataPath + "/Achievementdata.json"*/, jsonData); // JSON 문자열을 파일로 저장
    }

    public void RemoveAchievementData()
    {
        Debug.Log("Delete Achievement Data");
        if (File.Exists(achievementDataSavePath/*Application.persistentDataPath + "/Achievementdata.json"*/))
        {
            File.Delete(achievementDataSavePath/*Application.persistentDataPath + "/Achievementdata.json"*/);
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
