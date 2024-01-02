using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class UIManager : MonoBehaviour
{
    private static UIManager instance = null;

    public GameObject neglectRewardObject;
    public GameObject neglectRewardEquipment;
    public GameObject neglectRewardGold;
    public GameObject neglectRewardExp;
    public GameObject neglectRewardText;



    public GameObject menuPanel;
    public GameObject BarObject;


    public GameObject achievementObject;
    public GameObject playerInfoObject;
    public GameObject playerEquipmentObject;
    public GameObject playerSkillObject;
    public GameObject GachaShopObject;

    public Text playerNameText;
    public Text playerPowerText;
    public Text playerLevelText;
    public Text playerGoldText;

    GameObject player;
    Player.PlayerInfoData playerInfo;

    bool isResetClick;
    //List<Dictionary<string, object>> getStatCsv;

    bool isMenuCilck;

    float widthSize;
    float heightSize;

    public static UIManager GetUIManager()
    {
        return instance;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        isMenuCilck = false;
        isResetClick = false;
        player = GameObject.Find("Player");
        playerInfo = player.GetComponent<Player>().GetPlayerInfo();
        SetNameUI();
        SetLevelUI();
        BarFill();
        SetPowerUI();
        SetGoldUI();
        ShowNeglectRewardPanel();

        widthSize = gameObject.GetComponent<RectTransform>().rect.width;
        heightSize = gameObject.GetComponent<RectTransform>().rect.height;
    }

    public float GetWidthSize()
    {
        return widthSize;
    }

    public float GetHeightSize()
    {
        return heightSize;
    }

    // Update is called once per frame
    void ShowNeglectRewardPanel()
    {
        if (NeglectGameManager.GetNeglectGameManager().GetIsExistOfflineRewards() == false)
        {
            neglectRewardObject.SetActive(false);
            return;
        }
        else
        {
            neglectRewardObject.SetActive(true);

            var getReward = NeglectGameManager.GetNeglectGameManager().GetOfflineReward();

            int getOfflineTime = getReward[3];
            if (getOfflineTime > 0)
            {
                int offlineHours = getOfflineTime / 60;
                int offlineMinutes = getOfflineTime % 60;
                neglectRewardText.GetComponent<Text>().text = "오프라인 보상 : ";
                if (offlineHours > 0)
                    neglectRewardText.GetComponent<Text>().text += offlineHours + "시간";
                if (offlineMinutes > 0)
                    neglectRewardText.GetComponent<Text>().text += offlineMinutes + "분";
            }

            // 0 장비 / 1 골드 / 2 경험치
            if (getReward[0] == 0)
            {
                neglectRewardEquipment.SetActive(false);
            }
            else
            {
                neglectRewardEquipment.transform.Find("EquipmentText").gameObject.GetComponent<Text>().text = getReward[0].ToString();
            }

            if (getReward[1] == 0)
            {
                neglectRewardGold.SetActive(false);
            }
            else
            {
                neglectRewardGold.transform.Find("GoldText").gameObject.GetComponent<Text>().text = getReward[1].ToString();
            }

            if (getReward[2] == 0)
            {
                neglectRewardExp.SetActive(false);
            }
            else
            {
                neglectRewardExp.transform.Find("ExpText").gameObject.GetComponent<Text>().text = getReward[2].ToString();
            }
        }
    }

    public void CloseNeglectRewardPanel()
    {
        SoundManager.GetInstance().PlayClickSound();
        neglectRewardObject.SetActive(false);
    }

    public static void UIManagerUpdateEvent()
    {
        if (instance != null)
        {
            instance.DataUpdateAndTextUpdate();
            //Debug.Log("UpdateEvnetTest UIManager");
        }
    }

    void DataUpdateAndTextUpdate()
    {
        UpDataPlayerData();
        SetPowerUI();
        SetLevelUI();
        SetGoldUI();
        BarFill();
    }

    public void MenuButtonCilck()
    {
        SoundManager.GetInstance().PlayMenuClickSound();
        if (!isMenuCilck)
        {
            isMenuCilck = true;
            menuPanel.SetActive(true);
        }
        else
        {
            isMenuCilck = false;
            menuPanel.SetActive(false);
        }
    }

    public void UpDataPlayerData()
    {
        playerInfo = player.GetComponent<Player>().GetPlayerInfo();
    }

    public void SetNameUI()
    {
        playerNameText.text = playerInfo.name;
    }

    public void SetLevelUI()
    {
        playerLevelText.text = "Lv." + playerInfo.level;
    }

    public void SetPowerUI()
    {
        playerPowerText.text = "전투력 : "+ playerInfo.power;

    }

    public void SetGoldUI()
    {
        playerGoldText.text = string.Format("{0:#,0}", playerInfo.currentGold);
    }

    public void BarFill()
    {
        if (player == null)
            return;
        GameObject hpFill = BarObject.transform.Find("HpBar").gameObject;
        GameObject mpFill = BarObject.transform.Find("MpBar").gameObject;
        GameObject expFill = BarObject.transform.Find("ExpBar").gameObject;

        hpFill.GetComponent<Slider>().value = (float)playerInfo.currentHp / (float)playerInfo.maxHp;
        mpFill.GetComponent<Slider>().value = (float)playerInfo.currentMp / (float)playerInfo.maxMp;
        expFill.GetComponent<Slider>().value = (float)playerInfo.currentExp / (float)playerInfo.maxExp;

        hpFill.transform.Find("HpText").gameObject.GetComponent<Text>().text = playerInfo.currentHp +" / "+ playerInfo.maxHp;
        mpFill.transform.Find("MpText").gameObject.GetComponent<Text>().text = playerInfo.currentMp + " / "+ playerInfo.maxMp;
        expFill.transform.Find("ExpText").gameObject.GetComponent<Text>().text = playerInfo.currentExp + " / "+ playerInfo.maxExp;


    }

    public void AchievementButtonCilck()
    {
        SoundManager.GetInstance().PlayMenuClickSound();
        achievementObject.SetActive(true);
    }

    public void PlayerInfoButtonCilck()
    {
        SoundManager.GetInstance().PlayMenuClickSound();
        playerInfoObject.SetActive(true);
    }

    public void PlayerEquipmentCilck()
    {
        SoundManager.GetInstance().PlayMenuClickSound();
        playerEquipmentObject.SetActive(true);
    }

    public void PlayerSkillCilck()
    {
        SoundManager.GetInstance().PlayMenuClickSound();
        playerSkillObject.SetActive(true);
    }

    public void GachaShopCilck()
    {
        SoundManager.GetInstance().PlayMenuClickSound();
        GachaShopObject.SetActive(true);
    }

    public void ResetCilck()
    {
        SoundManager.GetInstance().PlayMenuClickSound();
        DataManager.GetDataManager().RemoveAllData();
        isResetClick = true;
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
#endif
    }

    public void StartSceneCilck()
    {
        DataManager.GetDataManager().SaveAllData();
    }


    public bool GetIsResetClick()
    {
        return isResetClick;
    }

    public void ClosePlayerInfo()
    {
        SoundManager.GetInstance().PlayMenuClickSound();
        playerInfoObject.SetActive(false);
    }
}
