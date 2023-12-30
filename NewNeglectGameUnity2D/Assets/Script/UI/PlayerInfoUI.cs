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

    public Button atkButton;
    public Button defButton;
    public Button moveSpeedButton;
    public Button maxHpButton;
    public Button maxMpButton;

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

    public void SetStatButton()
    {
       if(playerInfoData.statPoint>0)
        {
            atkButton.GetComponent<Image>().color = Color.yellow;
            defButton.GetComponent<Image>().color = Color.yellow;
            moveSpeedButton.GetComponent<Image>().color = Color.yellow;
            maxHpButton.GetComponent<Image>().color = Color.yellow;
            maxMpButton.GetComponent<Image>().color = Color.yellow;
        }
       else
        {
            atkButton.GetComponent<Image>().color = Color.gray;
            defButton.GetComponent<Image>().color = Color.gray;
            moveSpeedButton.GetComponent<Image>().color = Color.gray;
            maxHpButton.GetComponent<Image>().color = Color.gray;
            maxMpButton.GetComponent<Image>().color = Color.gray;
        }
    }

    public void UpdatePlayerInfoMenu()
    {
        SetStatPointUI();
        SetStatButton();
        SetStatDetail();
        SetStatText(playerStatAtkText, "Atk");
        SetStatText(playerStatDefText, "Def");
        SetStatText(playerStatMoveSpeedText, "MoveSpeed");
        SetStatText(playerStatMaxHpText, "MaxHp");
        SetStatText(playerStatMaxMpText, "MaxMp");
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
        Button curButton;
        string statName = " ";
        string level;
        var getPlayerData = DataManager.GetDataManager().GetPlayerData();
        if (stat.Equals("Atk"))
        {
            statName = "공격력";
            level = getPlayerData.atkLevel.ToString();
            curButton = atkButton;
        }
        else if (stat.Equals("Def"))
        {
            statName = "방어력";
            level = getPlayerData.defLevel.ToString();
            curButton = defButton;
        }
        else if (stat.Equals("MoveSpeed"))
        {
            statName = "이동속도";
            level = getPlayerData.moveSpeedLevel.ToString();
            curButton = moveSpeedButton;
        }
        else if (stat.Equals("MaxHp"))
        {
            statName = "최대체력";
            level = getPlayerData.maxHpLevel.ToString();
            curButton = maxHpButton;
        }
        else if (stat.Equals("MaxMp"))
        {
            statName = "최대마나";
            level = getPlayerData.maxMpLevel.ToString();
            curButton = maxMpButton;
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
                curButton.GetComponent<Image>().color = Color.gray;
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
            if ((DataManager.GetDataManager().GetStatCSV()[level + 1]["Atk"].ToString()).Equals(""))
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
            if ((getStatCsv[level + 1]["Def"].ToString()).Equals(""))
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
            if ((getStatCsv[level + 1]["MoveSpeed"].ToString()).Equals(""))
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
            if ((getStatCsv[level + 1]["MaxHp"].ToString()).Equals(""))
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
            if ((getStatCsv[level + 1]["MaxMp"].ToString()).Equals(""))
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
