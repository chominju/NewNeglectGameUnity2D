using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillUI : MonoBehaviour
{
    private static SkillUI instance = null;

    public GameObject skillInfoUi;
    public GameObject skillUi_skillPreset;
    public GameObject playerSkillUi;
    public Button backButton;
    public Image skillImage;
    public Text skillNameText;
    public Text skillLevelText;
    public Text skillInfoText;

    public GameObject levelUpButton;
    public GameObject transcendenceButton;
    public GameObject skillEquipButton;

    public Text levelUpButtonText;
    public Text transcendenceButtonText;
    public Text skillRequiredGoldText;
    public Text playerGoldText;
    public GameObject eqRequiredGoldImage;

    public GameObject[] skillObject;


    Button currentSkillButton;
    SkillData currentSkillData;
    List<Dictionary<string, object>> currentSkillDetailCsv;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        currentSkillButton = null;
        skillInfoUi.SetActive(false);   
        skillUi_skillPreset.SetActive(false);

        SetBackButtonSize(false);
        //backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(1280, 1080);
        SetSkillUI();
    }

    void SetBackButtonSize(bool clickInfo)
    {
        float screenWidth = UIManager.GetUIManager().GetWidthSize();
        float screenHeight = UIManager.GetUIManager().GetHeightSize();
        Rect lSize = skillInfoUi.GetComponent<RectTransform>().rect;
        Rect rSize = playerSkillUi.GetComponent<RectTransform>().rect;
        float newWidthSize = 0;
        //float newHeigthSize = 0;
        if (clickInfo)
        {
            // ���/��ų�� Ŭ������ ��(3���� ������Ʈ ���̱�)
            newWidthSize = screenWidth - lSize.width - rSize.width;
            //newHeigthSize = screenHeight - lSize.height - rSize.height;
        }
        else
        {
            // ���/��ų�� Ŭ�� ������ ��(2���� ������Ʈ�� ���̱�)
            newWidthSize = screenWidth - rSize.width;
            //newHeigthSize = screenHeight - rSize.height;
        }

        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(newWidthSize, backButton.GetComponent<RectTransform>().sizeDelta.y);
    }

    public static void SKillUIUpdateEvent()
    {
        if (instance != null)
        {
            instance.DataUpdateAndTextUpdate();
            //Debug.Log("UpdateEvnetTest SkillUI");
        }
    }

    void DataUpdateAndTextUpdate()
    {
        if (gameObject.activeSelf)
        {
            SetSkillUI();
            if (skillInfoUi.activeSelf)
            {
                SkillInfoButton();
               // SkillInfoText();
            }
        }
    }

    public Button GetCurrentSkillButton()
    {
        return currentSkillButton;
    }

    void SetSkillUI()
    {
        var getData = DataManager.GetDataManager().GetSkillData();

        playerGoldText.text = string.Format("{0:#,0}", DataManager.GetDataManager().GetPlayerData().currentGold);

        for (int i = 0; i < skillObject.Length; i++)
        {
            if (skillObject[i].name.Equals(getData[i].skillName))
            {
                GameObject levelTextObj = skillObject[i].transform.Find("LevelText").gameObject;
                GameObject sliderObj = skillObject[i].transform.Find("CountSlider").gameObject;
                GameObject countlTextObj = sliderObj.transform.Find("CountText").gameObject;

                if (getData[i].quantity <= 0 && !getData[i].isGainSkill)
                {
                    skillObject[i].GetComponent<Image>().color = Color.gray;
                    levelTextObj.SetActive(false);
                    sliderObj.SetActive(false);
                    countlTextObj.SetActive(false);
                }
                else
                {
                    levelTextObj.SetActive(true);
                    sliderObj.SetActive(true);
                    countlTextObj.SetActive(true);

                    skillObject[i].GetComponent<Image>().color = Color.white;

                    levelTextObj.GetComponent<Text>().text = "Lv. " + getData[i].skillLevel;

                    sliderObj.GetComponent<Slider>().value = (float)getData[i].quantity / (float)getData[i].mixCount;

                    countlTextObj.GetComponent<Text>().text = getData[i].quantity + " / " + getData[i].mixCount;
                }
            }
        }

    }

    public void SkillLevelUpButtonClick(Button levelUpButton)
    {
        int requiredGold = int.Parse(currentSkillDetailCsv[currentSkillData.skillLevel]["RequiredGold"].ToString());
        if (currentSkillData.skillLevel >= currentSkillData.skillCurrentMaxLevel)
        {
            SoundManager.GetInstance().PlayFailSound();
            return;
        }
        if (DataManager.GetDataManager().GetPlayerData().currentGold >= requiredGold)
        {
            SoundManager.GetInstance().PlayClickSound();
            DataManager.GetDataManager().SkillLevelUp(currentSkillButton.name);
            DataManager.GetDataManager().GainGold(-requiredGold);
            DataUpdateAndTextUpdate();
        }
        else
        {
            SoundManager.GetInstance().PlayFailSound();
            levelUpButton.GetComponent<Image>().color = Color.gray;
        }
    }


    // Update is called once per frame
    void Update()
    {

    }

    public void SkillTranscendenceButtonClick()
    {
        //skillAwakenButton
        if(currentSkillData.quantity>= currentSkillData.mixCount)
        {
            SoundManager.GetInstance().PlayClickSound();
            DataManager.GetDataManager().SkillTranscendenceLevelUp(currentSkillData.skillName);
        }
        else
        {
            SoundManager.GetInstance().PlayFailSound();
        }
    }

    void SkillInfoOn()
    {
        skillInfoUi.SetActive(true);
    }

    void SkillInfoOff()
    {
        skillInfoUi.SetActive(false);
    }

    public void CloseSkillUI()
    {
        SetBackButtonSize(false);
        //backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(1280, 1080);
        skillInfoUi.SetActive(false);
        skillUi_skillPreset.SetActive(false);
        this.gameObject.SetActive(false);
    }

    // ��ų ��Ͽ� �ִ� ��ų�� Ŭ������ �� (��ų ����â�̶� ���ư�� ������)
    public void SkillClick(Button eqButton)
    {
        skillUi_skillPreset.SetActive(false);
        currentSkillButton = eqButton;

        currentSkillData = DataManager.GetDataManager().GetFindSkillData(currentSkillButton.name);
        currentSkillDetailCsv = DataManager.GetDataManager().GetSkillDetailCsv(currentSkillButton.name);

        SkillInfoButton();
        //SkillInfoText();
    }

    void SkillInfoButton()
    {
        SetBackButtonSize(true);
        //backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(730, 1080);
        skillInfoUi.SetActive(true);


        skillImage.GetComponent<Image>().sprite = currentSkillData.sprite;
        skillNameText.text = currentSkillData.skillShowName;
        //eqInfoText;


        skillInfoText.text = "���� ȿ��\n���ݷ�\t\t" + currentSkillDetailCsv[currentSkillData.skillLevel]["HaveAtk"] + " -> " + currentSkillDetailCsv[currentSkillData.skillLevel + 1]["HaveAtk"] +
            "\n\n���� ȿ��\n���ݷ�\t\t" + currentSkillDetailCsv[currentSkillData.skillLevel]["SkillAtk"] + " -> " + currentSkillDetailCsv[currentSkillData.skillLevel]["SkillAtk"];






        if (currentSkillData.quantity <= 0 && currentSkillData.isGainSkill==false)
        {
            skillLevelText.text = "";
            skillRequiredGoldText.text = "";
            levelUpButton.SetActive(false);
            transcendenceButton.SetActive(false);
            skillEquipButton.SetActive(false);
            eqRequiredGoldImage.SetActive(false);
        }
        else
        {
            levelUpButton.SetActive(true);
            transcendenceButton.SetActive(true);
            skillEquipButton.SetActive(true);
            eqRequiredGoldImage.SetActive(true);

            skillLevelText.text = "Lv. " + currentSkillData.skillLevel + " / " + currentSkillData.skillCurrentMaxLevel;
            skillRequiredGoldText.text = string.Format("{0:#,0}", currentSkillDetailCsv[currentSkillData.skillLevel]["RequiredGold"]);

            int requiredGold = int.Parse(currentSkillDetailCsv[currentSkillData.skillLevel]["RequiredGold"].ToString());
            if ((DataManager.GetDataManager().GetPlayerData().currentGold >= requiredGold) && (currentSkillData.skillLevel < currentSkillData.skillCurrentMaxLevel))
            {
                levelUpButton.GetComponent<Image>().color = Color.yellow; // ��ư Ȱ��ȭ
            }
            else
                levelUpButton.GetComponent<Image>().color = Color.gray; // ��ư ��Ȱ��ȭ



            if ((currentSkillData.quantity >= currentSkillData.mixCount) && (currentSkillData.transcendenceLevel < currentSkillData.transcendenceMaxLevel))
                transcendenceButton.GetComponent<Image>().color = Color.magenta; // ��ư Ȱ��ȭ
            else
                transcendenceButton.GetComponent<Image>().color = Color.gray; // ��ư ��Ȱ��ȭ






            if (currentSkillData.transcendenceLevel < currentSkillData.transcendenceMaxLevel)
                transcendenceButtonText.text = "�ʿ��ϱ�";
            else
                transcendenceButtonText.text = "�ִ��ʿ�";

            if (currentSkillData.skillLevel >= currentSkillData.skillCurrentMaxLevel)
                levelUpButtonText.text = "�ִ밭ȭ";
            else
                levelUpButtonText.text = "��ȭ�ϱ�";
        }

        

    }
    //void SkillInfoText()
    //{
    //    //var getData = DataManager.GetDataManager().GetSkillData();
    //    //backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(730, 1080);
    //    //skillInfoUi.SetActive(true);

    //    //foreach (var tempData in getData)
    //    //{
    //    //    if (currentSkillButton.name.Equals(tempData.skillName))
    //    //    {
    //    //        skillImage.GetComponent<Image>().sprite = tempData.sprite;
    //    //        skillNameText.text = tempData.skillShowName;
    //    //        skillLevelText.text = "Lv. " + tempData.skillLevel + " / " + tempData.skillMaxLevel;
    //    //        //eqInfoText;

    //    //        var getCsv = DataManager.GetDataManager().GetSkillDetailCsv(tempData.skillName);
    //    //        if (getCsv == null)
    //    //            Debug.Log("SkillDetail Data Load Error");

    //    //        skillInfoText.text = currentSkillButton.name + "���� ȿ��\n���ݷ�\t\t" + getCsv[tempData.skillLevel]["HaveAtk"] + " -> " + getCsv[tempData.skillLevel + 1]["HaveAtk"] +
    //    //            "\n\n���� ȿ��\n���ݷ�\t\t" + getCsv[tempData.skillLevel]["EquipAtk"] + " -> " + getCsv[tempData.skillLevel]["EquipAtk"];



    //    //        skillRequiredGoldText.text = string.Format("{0:#,0}", getCsv[currentSkillData.skillLevel]["RequiredGold"]);


    //    //        int requiredGold = int.Parse(currentSkillDetailCsv[currentSkillData.skillLevel]["RequiredGold"].ToString());
    //    //        if (DataManager.GetDataManager().GetPlayerData().currentGold >= requiredGold)
    //    //        {
    //    //            levelUpButton.GetComponent<Image>().color = new Color(255, 219, 0, 255);
    //    //        }
    //    //        else
    //    //            levelUpButton.GetComponent<Image>().color = new Color(0, 0, 0, 150);

    //    //    }
    //    //}




    //    //backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(730, 1080);
    //    //skillInfoUi.SetActive(true);

    //    //if (currentSkillData.transcendenceLevel < currentSkillData.transcendenceMaxLevel)
    //    //    transcendenceButtonText.text = "�ʿ��ϱ�";
    //    //else
    //    //    transcendenceButtonText.text = "�ִ��ʿ�";

    //    //if (currentSkillData.skillLevel >= currentSkillData.skillCurrentMaxLevel)
    //    //    levelUpButtonText.text = "�ִ밭ȭ";
    //    //else
    //    //    levelUpButtonText.text = "��ȭ�ϱ�";


    //    //skillImage.GetComponent<Image>().sprite = currentSkillData.sprite;
    //    //skillNameText.text = currentSkillData.skillShowName;
    //    //skillLevelText.text = "Lv. " + currentSkillData.skillLevel + " / " + currentSkillData.skillCurrentMaxLevel;
    //    ////eqInfoText;


    //    //skillInfoText.text = "���� ȿ��\n���ݷ�\t\t" + currentSkillDetailCsv[currentSkillData.skillLevel]["HaveAtk"] + " -> " + currentSkillDetailCsv[currentSkillData.skillLevel + 1]["HaveAtk"] +
    //    //    "\n\n���� ȿ��\n���ݷ�\t\t" + currentSkillDetailCsv[currentSkillData.skillLevel]["SkillAtk"] + " -> " + currentSkillDetailCsv[currentSkillData.skillLevel]["SkillAtk"];



    //    //skillRequiredGoldText.text = string.Format("{0:#,0}", currentSkillDetailCsv[currentSkillData.skillLevel]["RequiredGold"]);


    //    //int requiredGold = int.Parse(currentSkillDetailCsv[currentSkillData.skillLevel]["RequiredGold"].ToString());
    //    //if (DataManager.GetDataManager().GetPlayerData().currentGold >= requiredGold)
    //    //{
    //    //    levelUpButton.GetComponent<Image>().color = new Color(255, 219, 0, 255);
    //    //}
    //    //else
    //    //    levelUpButton.GetComponent<Image>().color = new Color(0, 0, 0, 150);

    //}

    
    // ��ų�� ����
    public void SkillEquipButtonClick()
    {
        SoundManager.GetInstance().PlayClickSound();
        skillUi_skillPreset.SetActive(true);

        var getSkillPreset = DataManager.GetDataManager().GetSkillPreset();

        for (int i = 0; i < 6; i++)
        {
            int buttonNum = i + 1;
            GameObject presetButton = skillUi_skillPreset.transform.Find("SkillUi_SkillPresetButton" + buttonNum).gameObject;
            if (getSkillPreset[i] == "")
            {
                presetButton.transform.Find("SkillImage" + buttonNum).GetComponent<Image>().sprite = null;
                continue;
            }
            else
            {
                if (presetButton == null)
                    Debug.Log("skillpreset Image Error");
                presetButton.transform.Find("SkillImage"+ buttonNum).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Skill/Icon/" + "Icon_"+getSkillPreset[i]);
            }
        }
        //DataManager.EquipmentLevelUp(CurrentSkillButton.name);
    }

    public void PreSetSkillSetUp(Button presetSkillButton)
    {

        if (currentSkillButton == null)
            Debug.Log("ERROR");
        else
        {
            var getSkillPreset = DataManager.GetDataManager().GetSkillPreset();
            for(int i=0; i<6; i++)
            {
                if (getSkillPreset[i] == "")
                    continue;
                else
                {
                    if (currentSkillButton.name == getSkillPreset[i])
                    {
                        DataManager.GetDataManager().SetSkillPreset("", i + 1);
                        break;
                    }

                }
            }


            if (presetSkillButton.name == "SkillUi_SkillPresetButton1")
            {
                DataManager.GetDataManager().SetSkillPreset(currentSkillButton.name, 1);
                //skillPreset[0] = Resources.Load<GameObject>("Prefab/Skill/Prefab/" + CurrentSkillButton.name);
            }
            else if (presetSkillButton.name == "SkillUi_SkillPresetButton2")
            {
                DataManager.GetDataManager().SetSkillPreset(currentSkillButton.name, 2);
               // skillPreset[1] = Resources.Load<GameObject>("Prefab/Skill/Prefab/" + CurrentSkillButton.name);
            }
            else if (presetSkillButton.name == "SkillUi_SkillPresetButton3")
            {
                DataManager.GetDataManager().SetSkillPreset(currentSkillButton.name, 3);
               // skillPreset[2] = Resources.Load<GameObject>("Prefab/Skill/Prefab/" + CurrentSkillButton.name);
            }
            else if (presetSkillButton.name == "SkillUi_SkillPresetButton4")
            {
                DataManager.GetDataManager().SetSkillPreset(currentSkillButton.name, 4);
                //skillPreset[3] = Resources.Load<GameObject>("Prefab/Skill/Prefab/" + CurrentSkillButton.name);
            }
            else if (presetSkillButton.name == "SkillUi_SkillPresetButton5")
            {
                DataManager.GetDataManager().SetSkillPreset(currentSkillButton.name, 5);
                //skillPreset[4] = Resources.Load<GameObject>("Prefab/Skill/Prefab/" + CurrentSkillButton.name);
            }
            else if (presetSkillButton.name == "SkillUi_SkillPresetButton6")
            {
                DataManager.GetDataManager().SetSkillPreset(currentSkillButton.name, 6);
                //skillPreset[5] = Resources.Load<GameObject>("Prefab/Skill/Prefab/" + CurrentSkillButton.name);
            }
            //presetSkillButton.gameObject.GetComponentInChildren<Image>().sprite = Resources.Load<Sprite>("Prefab/Skill/Sprite/" + CurrentSkillButton.name);
        }

        skillUi_skillPreset.SetActive(false);

        //if (PresetSkillButtonClick == null)
        //    PresetSkillButtonClick = presetSkillButton;

        //skillUI
    }
}
