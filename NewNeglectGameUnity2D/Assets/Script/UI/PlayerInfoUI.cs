using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerInfoUI : MonoBehaviour
{
    private static PlayerInfoUI instance = null;

    public Text playerStatPointText;
    public Text playerStatAtkText;
    public Text playerStatDefText;
    public Text playerStatMoveSpeedText;
    public Text playerStatMaxHpText;
    public Text playerStatMaxMpText;


    public Text playerInfoLevelText;
    public Text playerInfoPowerText;
    public Text playerInfoAtkText;
    public Text playerInfoDefText;
    public Text playerInfoMoveSpeedText;
    public Text playerInfoMaxHpText;
    public Text playerInfoMaxMpText;

    GameObject player;
    Player.PlayerInfoData playerInfoData;
    List<Dictionary<string, object>> getStatCsv;

    

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        player = GameObject.Find("Player");
        playerInfoData = player.GetComponent<Player>().GetPlayerInfo();

        getStatCsv = DataManager.GetDataManager().GetStatCSV();
        UpdateData();
    }

    public static void PlayerInfoUIUpdateEvent()
    {
        if (instance != null)
        {
            instance.UpdateData();
            //Debug.Log("UpdateEvnetTest PlayerInfoUI");
        }
    }


    // Update is called once per frame
    //void Update()
    //{
    //    TextUpdate();
    //    UpDataPlayerData();
    //    UpdatePlayerInfoMenu();
    //}

    void UpdateData()
    {
        UpDataPlayerData();
        TextUpdate();
        UpdatePlayerInfoMenu();
    }

    public void UpDataPlayerData()
    {
        playerInfoData = player.GetComponent<Player>().GetPlayerInfo();
    }

    void DataUpdate()
    {
        //SetText();
        //EquipmentInfoText();
    }


    public void TextUpdate()
    {
        if (playerInfoPowerText.IsActive())
            playerInfoPowerText.text = "전투력 : " + playerInfoData.power;
    }

    public void SetStatPointUI()
    {
        playerStatPointText.text = "스킬포인트 : " + playerInfoData.statPoint;
    }

    public void UpdatePlayerInfoMenu()
    {
        SetStatPointUI();
        SetStatDetail();
        SetStatText(playerStatAtkText, "atk");
        SetStatText(playerStatDefText, "def");
        SetStatText(playerStatMoveSpeedText, "moveSpeed");
        SetStatText(playerStatMaxHpText, "maxHp");
        SetStatText(playerStatMaxMpText, "maxMp");
    }

    public void SetStatDetail()
    {
        playerInfoLevelText.text = "Lv." + playerInfoData.level + "(" + ((float)playerInfoData.currentExp / (float)playerInfoData.maxExp * 100.0f).ToString("F2") + "%)";
        playerInfoAtkText.text = "공격력 : " + playerInfoData.atk;
        playerInfoDefText.text = "방어력 : " + playerInfoData.def;
        playerInfoMoveSpeedText.text = "아동속도 : " + playerInfoData.moveSpeed;
        playerInfoMaxHpText.text = "최대체력 : " + playerInfoData.maxHp;
        playerInfoMaxMpText.text = "최대마나 : " + playerInfoData.maxMp;
    }

    public void SetStatText(Text textObject, string stat)
    {
        string statName = " ";
        string level;
        var getPlayerData = DataManager.GetDataManager().GetPlayerData();
        if (stat.Equals("atk"))
        {
            statName = "공격력";
            level = getPlayerData.atkLevel.ToString();
        }
        else if (stat.Equals("def"))
        {
            statName = "방어력";
            level = getPlayerData.defLevel.ToString();
        }
        else if (stat.Equals("moveSpeed"))
        {
            statName = "이동속도";
            level = getPlayerData.moveSpeedLevel.ToString();
        }
        else if (stat.Equals("maxHp"))
        {
            statName = "최대체력";
            level = getPlayerData.maxHpLevel.ToString();
        }
        else if (stat.Equals("maxMp"))
        {
            statName = "최대마나";
            level = getPlayerData.maxMpLevel.ToString();
        }
        else
        {
            Debug.Log("player stat ui data error(1)!");
            return;
        }

        int statCsvCount = getStatCsv.Count;
        string statCsvNext = "";
        if (int.Parse(level) < statCsvCount)    
        {
            if (getStatCsv[int.Parse(level) + 1][stat].ToString() == "")
            {
                level = "MAX";
            }
            else
                statCsvNext = getStatCsv[int.Parse(level) + 1][stat].ToString();
        }
        else
        {
            Debug.Log("player stat ui data error(2)!");
            return;
        }

        if (level.Equals("MAX"))
            textObject.text = statName + " Lv." + level;
        else
            textObject.text = statName + " Lv." + level + "\n"
                + getStatCsv[int.Parse(level)][stat] + " -> " + statCsvNext;
    }

    public void AtkButtonCilck()
    {
        if (playerInfoData.statPoint > 0)
        {
            int level = DataManager.GetDataManager().GetPlayerData().atkLevel;
            if ((DataManager.GetDataManager().GetStatCSV()[level + 1]["atk"].ToString()).Equals(""))
            {
                SoundManager.GetInstance().PlayFailSound();
                return;
            }

            DataManager.GetDataManager().StatPointUse();
            DataManager.GetDataManager().AtkLevelUp();
            SoundManager.GetInstance().PlayClickSound();
        }
    }

    public void DefButtonCilck()
    {
        if (playerInfoData.statPoint > 0)
        {
            int level = DataManager.GetDataManager().GetPlayerData().defLevel;
            if ((getStatCsv[level + 1]["def"].ToString()).Equals(""))
            {
                SoundManager.GetInstance().PlayFailSound();
                return;
            }
            DataManager.GetDataManager().StatPointUse();
            DataManager.GetDataManager().DefLevelUp();
            SoundManager.GetInstance().PlayClickSound();
        }
    }

    public void MoveSpeedButtonCilck()
    {
        if (playerInfoData.statPoint > 0)
        {
            int level = DataManager.GetDataManager().GetPlayerData().moveSpeedLevel;
            if ((getStatCsv[level + 1]["moveSpeed"].ToString()).Equals(""))
            {
                SoundManager.GetInstance().PlayFailSound();
                return;
            }
            DataManager.GetDataManager().StatPointUse();
            DataManager.GetDataManager().MoveSpeedLevelUp();
            SoundManager.GetInstance().PlayClickSound();
        }
    }

    public void MaxHpButtonCilck()
    {
        if (playerInfoData.statPoint > 0)
        {
            int level = DataManager.GetDataManager().GetPlayerData().maxHpLevel;
            if ((getStatCsv[level + 1]["maxHp"].ToString()).Equals(""))
            {
                SoundManager.GetInstance().PlayFailSound();
                return; 
            }
            DataManager.GetDataManager().StatPointUse();
            DataManager.GetDataManager().MaxHpLevelUp();
            SoundManager.GetInstance().PlayClickSound();
        }
    }



    public void MaxMpButtonCilck()
    {
        if (playerInfoData.statPoint > 0)
        {
            int level = DataManager.GetDataManager().GetPlayerData().maxMpLevel;
            if ((getStatCsv[level + 1]["maxMp"].ToString()).Equals(""))
            {
                SoundManager.GetInstance().PlayFailSound();
                return;
            }
            DataManager.GetDataManager().StatPointUse();
            DataManager.GetDataManager().MaxMpLevelUp();
            SoundManager.GetInstance().PlayClickSound();
        }
    }
}
