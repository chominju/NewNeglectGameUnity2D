using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class EquipmentUI : MonoBehaviour
{
    private static EquipmentUI instance = null;

    public GameObject eqInfoUi;
    public Button backButton;
    public Image eqImage;
    public Text eqNameText;
    public Text eqLevelText;
    public Text eqInfoText;

    public Text eqRequiredGoldText;
    public Text playerGoldText;

    public GameObject[] equipmentObject;

    Button currentEquipmentButton;
    ItemData currentEquipmentData;
    List<Dictionary<string, object>> currentEquipmentDetailCsv; 
    GameObject currentEquipmentMixButton;
    
    public GameObject eqRequiredGoldImage;
    public GameObject levelUpButton;

    bool isMixAble;

    //bool isEqClick;
    // RequiredGold

    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;
        currentEquipmentButton = null;
        eqInfoUi.SetActive(false);
        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(1280, 1080);
        SetEquipmentUI();
        isMixAble = false;
    }
    void DataUpdateAndTextUpdate()
    {
        SetEquipmentUI();
        if (eqInfoUi.activeSelf)
            EquipmentInfoText();
    }

    public static void EquipmentUIUpdateEvent()
    {
        if (instance != null)
        {
            instance.DataUpdateAndTextUpdate();
            Debug.Log("EquipmentUI UpdateEventTest");
        }
    }

    void SetEquipmentUI()
    {
        var getData = DataManager.GetDataManager().GetEquipmentData();

        playerGoldText.text = string.Format("{0:#,0}", DataManager.GetDataManager().GetPlayerData().currentGold);

        for (int i = 0; i < equipmentObject.Length; i++)
        {
            if (equipmentObject[i].name.Equals(getData[i].itemName))
            {
                GameObject levelTextObj = equipmentObject[i].transform.Find("LevelText").gameObject;
                GameObject sliderObj = equipmentObject[i].transform.Find("CountSlider").gameObject;
                GameObject countlTextObj = sliderObj.transform.Find("CountText").gameObject;

                if (getData[i].quantity <= 0 && getData[i].isGainItem==false)
                {
                    equipmentObject[i].GetComponent<Image>().color = Color.gray;
                    levelTextObj.SetActive(false);
                    sliderObj.SetActive(false);
                    countlTextObj.SetActive(false);
                }
                else
                {
                    levelTextObj.SetActive(true);
                    sliderObj.SetActive(true);
                    countlTextObj.SetActive(true);

                    equipmentObject[i].GetComponent<Image>().color = Color.white;
                    levelTextObj.GetComponent<Text>().text = "Lv. " + getData[i].itemLevel;

                    sliderObj.GetComponent<Slider>().value = (float)getData[i].quantity / (float)getData[i].mixCount;

                    countlTextObj.GetComponent<Text>().text = getData[i].quantity + " / " + getData[i].mixCount;
                }
            }
        }
    }

    public void EquipmentLevelUp(Button levelUpButton)
    {
        int requiredGold = int.Parse(currentEquipmentDetailCsv[currentEquipmentData.itemLevel]["RequiredGold"].ToString());
        if (DataManager.GetDataManager().GetPlayerData().currentGold >= requiredGold)
        {

            SoundManager.GetInstance().PlayClickSound();
            DataManager.GetDataManager().EquipmentLevelUp(currentEquipmentButton.name);
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

    void EquipmentInfoOn()
    {
        eqInfoUi.SetActive(true);
    }

    void EquipmentInfoOff()
    {
        eqInfoUi.SetActive(false);
    }

    public void CloseEquipmentUI()
    {
        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(1280, 1080);
        if (eqInfoUi != null)
            eqInfoUi.SetActive(false);
        if (currentEquipmentMixButton != null)
            currentEquipmentMixButton.SetActive(false);
        this.gameObject.SetActive(false);
    }

    public void EquipmentClick(Button eqButton)
    {
        SoundManager.GetInstance().PlayClickSound();

        if (currentEquipmentButton != null)                                                            // 장비를 누르지 않았을 때
        {
            currentEquipmentMixButton.SetActive(false);                                             // 합성버튼 비활성화
        }
        currentEquipmentButton = eqButton;
        currentEquipmentData = DataManager.GetDataManager().GetFindEquipmentData(currentEquipmentButton.name);
        currentEquipmentDetailCsv = DataManager.GetDataManager().GetEquipmentDetailCsv(currentEquipmentButton.name);
        currentEquipmentMixButton = currentEquipmentButton.transform.Find("MixButton").gameObject;

        isMixAble = IsEquipmentMixAble();
        currentEquipmentMixButton.SetActive(true);
        EquipmentInfoText();
    }

    public bool IsEquipmentMixAble()
    {
        //var getData = DataManager.GetDataManager().GetEquipmentData();

        //foreach (var clickEquipmentData in getData)
        //{
        //    if (currentEquipmentButton.name.Equals(clickEquipmentData.itemName))
        //    {
        //        if (clickEquipmentData.quantity >= clickEquipmentData.mixCount)
        //        {
        //            currentEquipmentMixButton.GetComponent<Image>().color = new Color(255, 219, 0, 255);
        //            return true;
        //        }
        //        else
        //        {
        //            currentEquipmentMixButton.GetComponent<Image>().color = new Color(0, 0, 0, 150);
        //            return false;
        //        }
        //    }
        //}
        //return false;
        if (currentEquipmentData.quantity >= currentEquipmentData.mixCount)
        {
            currentEquipmentMixButton.GetComponent<Image>().color = Color.yellow;
            return true;
        }
        else
        {
            currentEquipmentMixButton.GetComponent<Image>().color = Color.gray;
            return false;
        }
    }

    public void EquipmentMixClick()
    {
        var getData = DataManager.GetDataManager().GetEquipmentData();
        int dataSize = getData.Length;
        int index = 0;
        foreach (var clickEquipmentData in getData)
        {
            if (currentEquipmentButton.name.Equals(clickEquipmentData.itemName))
            {
                if (isMixAble)
                {
                    if (index == dataSize - 1)                                      // 제일 마지막 장비 일 때
                    {
                        SoundManager.GetInstance().PlayFailSound();
                        return;
                    }
                    SoundManager.GetInstance().PlayClickSound();

                    int nextEquipmentQuantity = clickEquipmentData.quantity / clickEquipmentData.mixCount;
                    int currentQuantity = clickEquipmentData.quantity % clickEquipmentData.mixCount;

                    DataManager.GetDataManager().SetEquipmentQuantity(clickEquipmentData.itemName, currentQuantity);
                    DataManager.GetDataManager().SetEquipmentQuantity(getData[index + 1].itemName, getData[index + 1].quantity + nextEquipmentQuantity);

                    isMixAble = false;
                    currentEquipmentMixButton.GetComponent<Image>().color = Color.gray;
                }
                else
                        SoundManager.GetInstance().PlayFailSound();

            }
            index++;
        }
    }

    void EquipmentInfoText()
    {
        backButton.GetComponent<RectTransform>().sizeDelta = new Vector2(730, 1080);
        eqInfoUi.SetActive(true);
        if (currentEquipmentData.quantity <= 0 && currentEquipmentData.isGainItem == false)
        {
            eqNameText.text = currentEquipmentData.itemShowName;
            eqImage.sprite = currentEquipmentData.sprite;
            eqRequiredGoldText.text= "";
            eqLevelText.text = "";
            levelUpButton.SetActive(false);
            eqRequiredGoldImage.SetActive(false);
        }
        else
        {
            eqNameText.text = currentEquipmentData.itemShowName;
            eqImage.sprite = currentEquipmentData.sprite;
            eqRequiredGoldText.text = string.Format("{0:#,0}", currentEquipmentDetailCsv[currentEquipmentData.itemLevel]["RequiredGold"]);
            eqLevelText.text = "Lv. " + currentEquipmentData.itemLevel + " / " + currentEquipmentData.itemMaxLevel;
            levelUpButton.SetActive(true);
            eqRequiredGoldImage.SetActive(true);

            int requiredGold = int.Parse(currentEquipmentDetailCsv[currentEquipmentData.itemLevel]["RequiredGold"].ToString());
            if (DataManager.GetDataManager().GetPlayerData().currentGold >= requiredGold)
            {
                levelUpButton.GetComponent<Image>().color = Color.yellow;
            }
            else
                levelUpButton.GetComponent<Image>().color = Color.gray;
        }
        //eqInfoText;


        eqInfoText.text = "보유 효과\n공격력\t\t" + currentEquipmentDetailCsv[currentEquipmentData.itemLevel]["HaveAtk"] + " -> " + currentEquipmentDetailCsv[currentEquipmentData.itemLevel + 1]["HaveAtk"] +
            "\n\n장착 효과\n공격력\t\t" + currentEquipmentDetailCsv[currentEquipmentData.itemLevel]["EquipAtk"] + " -> " + currentEquipmentDetailCsv[currentEquipmentData.itemLevel]["EquipAtk"];

    }
}