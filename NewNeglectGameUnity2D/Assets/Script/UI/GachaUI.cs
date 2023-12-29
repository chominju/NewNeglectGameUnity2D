using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GachaUI : MonoBehaviour
{
    private static GachaUI instance = null;

    int skillGacha30Price;
    int equipmentGacha30Price;

    public GameObject gacha30UIPanel;
    public GameObject gachaFreeUIPanel;

    public GameObject[] Gacha30UI;
    public GameObject[] GachaFreeUI;

    //int []equipmentGachaQuantity;
    int []skillGachaQuantity;

    int getEquipmentDataSize;
    int getSkillDataSize;

    ItemData[] getEquipmentData;
    SkillData[] getSkillData;

    bool isEquipmentGacha;
    bool isSkillGacha;

    public Button equipmentGacha30Button;
    public Button skillGacha30Button;

    public Text playerGoldText;


    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        skillGacha30Price = 3000;
        equipmentGacha30Price = 2500;

        //Gacha30UI = new GameObject[30];
        //GachaFreeUI = new GameObject[5];

        getEquipmentData = DataManager.GetDataManager().GetEquipmentData();
        getSkillData = DataManager.GetDataManager().GetSkillData();

        getEquipmentDataSize = getEquipmentData.Length;
        getSkillDataSize = getSkillData.Length;

        //equipmentGachaQuantity = new int[getEquipmentDataSize];
        skillGachaQuantity = new int[getSkillDataSize];

        isEquipmentGacha = false;
        isSkillGacha = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void GachaUIUpdateEvent()
    {
        if (instance != null)
        {
            instance.SetGoldUI();
            instance.SetGacha30ButtonUI();
            //Debug.Log("UpdateEvnetTest GachaUI");
        }
    }

    public void SetGacha30ButtonUI()
    {
        int playerGold = DataManager.GetDataManager().GetPlayerData().currentGold;
        if(playerGold>= equipmentGacha30Price)
            equipmentGacha30Button.GetComponent<Image>().color = Color.white; // 버튼 활성화
        else
            equipmentGacha30Button.GetComponent<Image>().color = Color.gray; // 버튼 비활성화

        if (playerGold >= skillGacha30Price)
            skillGacha30Button.GetComponent<Image>().color = Color.white; // 버튼 활성화
        else
            skillGacha30Button.GetComponent<Image>().color = Color.gray; // 버튼 비활성화
    }

    public void SetGoldUI()
    {
        if (playerGoldText.IsActive())

            playerGoldText.text = string.Format("{0:#,0}", DataManager.GetDataManager().GetPlayerData().currentGold);// DataManager.GetDataManager().GetPlayerData().currentGold.ToString("NO");
    }


    public void GachaButtonClick(Button button)
    {
        int playerGold = DataManager.GetDataManager().GetPlayerData().currentGold;
        if (button.name == "EquipmentGachaFreeButton")
        {
            SoundManager.GetInstance().PlayClickSound();
            isEquipmentGacha = true;
            isSkillGacha = false;
        }
        else if (button.name == "EquipmentGacha30Button")
        {
            if(playerGold< equipmentGacha30Price)
            {
            SoundManager.GetInstance().PlayFailSound();
                return;
            }
            SoundManager.GetInstance().PlayClickSound();
            EquipmentGacha30();
            return;
        }
        else if (button.name == "SkillGachaFreeButton")
        {
            SoundManager.GetInstance().PlayClickSound();
            isEquipmentGacha = false;
            isSkillGacha = true;
        }
        else if (button.name == "SkillGacha30Button")
        {
            if (playerGold < skillGacha30Price)
            {
            SoundManager.GetInstance().PlayFailSound();
                return;
            }
            SoundManager.GetInstance().PlayClickSound();
            SkillGacha30();
            return;
        }
        else
        {
            Debug.Log("Gacha Button Error");    
            return;
        }    

    }

    public void GachaFreeReward()
    {
        if (isEquipmentGacha == true && isSkillGacha == false)   // 무료장비
        {
            EquipmentGachaFree();
        }
        else if (isEquipmentGacha == false && isSkillGacha == true) // 무료스킬
        {
            SkillGachaFree();
        }
        isEquipmentGacha = false;
        isSkillGacha = false;
    }

    #region 뽑기 관련
    void EquipmentGachaFree()
    {
            gachaFreeUIPanel.SetActive(true);

            int[] equipmentGachaQuantity = new int[getEquipmentDataSize];

            for (int i = 0; i < 5; i++)
            {
                int num = Random.Range(0, getEquipmentDataSize);

                // 무기종류 갯수보다 적은수 or 많은 수가 나오면 오류발생.
                if (num < 0 || num >= getEquipmentDataSize)
                {
                    Debug.Log("EquipmentGacha30 Error");
                    return;
                }

                GachaFreeUI[i].GetComponent<Image>().sprite = getEquipmentData[num].sprite;
                equipmentGachaQuantity[num]++;
                // getEquipmentData[num].sprite = Resources.Load()

            }

            for (int i = 0; i < getEquipmentDataSize; i++)
            {
                if (equipmentGachaQuantity[i] > 0)
                {
                    DataManager.GetDataManager().GainEquipmentGacha(getEquipmentData[i].itemName, equipmentGachaQuantity[i]);
                }
            }
    }

    void EquipmentGacha30()
    {
        gacha30UIPanel.SetActive(true);
        int[] equipmentGachaQuantity = new int[getEquipmentDataSize];

        // 골드 관련 아직 세팅을 안해서 저장 x 
        if (DataManager.GetDataManager().GetPlayerData().currentGold >= equipmentGacha30Price)
        {
            gacha30UIPanel.SetActive(true);
            // getEquipmentData = DataManager.GetEquipmentData();
            //int getEquipmentDataSize = getEquipmentData.Length;
            for (int i = 0; i < 30; i++)
            {
                int num = Random.Range(0, getEquipmentDataSize);

                // 무기종류 갯수보다 적은수 or 많은 수가 나오면 오류발생.
                if (num < 0 || num >= getEquipmentDataSize)
                {
                    Debug.Log("EquipmentGacha30 Error");
                    return;
                }

                Gacha30UI[i].GetComponent<Image>().sprite = getEquipmentData[num].sprite;
                equipmentGachaQuantity[num]++;
                // getEquipmentData[num].sprite = Resources.Load()

            }

            for (int i = 0; i < getEquipmentDataSize; i++)
            {
                if (equipmentGachaQuantity[i] > 0)
                {
                    DataManager.GetDataManager().GainEquipmentGacha(getEquipmentData[i].itemName, equipmentGachaQuantity[i]);
                }
            }

            DataManager.GetDataManager().GainGold(-equipmentGacha30Price);
        }
    }
    void SkillGachaFree()
    {
            gachaFreeUIPanel.SetActive(true);

            int[] skillGachaQuantity = new int[getSkillDataSize];

            for (int i = 0; i < 5; i++)
            {
                int num = Random.Range(0, getSkillDataSize);

                // 무기종류 갯수보다 적은수 or 많은 수가 나오면 오류발생.
                if (num < 0 || num >= getSkillDataSize)
                {
                    Debug.Log("SkillGachaFree Error");
                    return;
                }

                GachaFreeUI[i].GetComponent<Image>().sprite = getSkillData[num].sprite;
                skillGachaQuantity[num]++;
            }

            for (int i = 0; i < getSkillDataSize; i++)
            {
                if (skillGachaQuantity[i] > 0)
                {
                    DataManager.GetDataManager().GainSkillGacha(getSkillData[i].skillName, skillGachaQuantity[i]);
                }
            }

        isSkillGacha = false;
        isEquipmentGacha = false;
    }

    void SkillGacha30()
    {
        gacha30UIPanel.SetActive(true);
        int[] skillGachaQuantity = new int[getSkillDataSize];

        // 골드 관련 아직 세팅을 안해서 저장 x 
        if (DataManager.GetDataManager().GetPlayerData().currentGold >= skillGacha30Price)
        {
            gacha30UIPanel.SetActive(true);
            // getEquipmentData = DataManager.GetEquipmentData();
            //int getEquipmentDataSize = getEquipmentData.Length;
            for (int i = 0; i < 30; i++)
            {
                int num = Random.Range(0, getSkillDataSize);

                // 스킬종류 갯수보다 적은수 or 많은 수가 나오면 오류발생.
                if (num < 0 || num >= getSkillDataSize)
                {
                    Debug.Log("SkillGacha30 Error");
                    return;
                }

                Gacha30UI[i].GetComponent<Image>().sprite = getSkillData[num].sprite;
                skillGachaQuantity[num]++;
                // getEquipmentData[num].sprite = Resources.Load()

            }

            for (int i = 0; i < getSkillDataSize; i++)
            {
                if (skillGachaQuantity[i] > 0)
                {
                    DataManager.GetDataManager().GainSkillGacha(getSkillData[i].skillName, skillGachaQuantity[i]);
                }
            }

            DataManager.GetDataManager().GainGold(-skillGacha30Price);
        }
    }

    #endregion

    #region BackButtonEvent
    public void GachaBackButton()
    {
        gameObject.SetActive(false);
    }

    public void GachaFreeBackButton()
    {
        gachaFreeUIPanel.SetActive(false);
    }

    public void Gacha30BackButton()
    {
        gacha30UIPanel.SetActive(false);
    }

    #endregion




    //public void FreeGacha()
    //{
    //    if(isEquipmentGacha)
    //    {
    //        gachaFreeUIPanel.SetActive(true);

    //        int[] equipmentGachaQuantity = new int[getEquipmentDataSize];

    //        for (int i = 0; i < 5; i++)
    //        {
    //            int num = Random.Range(0, getEquipmentDataSize);

    //            // 무기종류 갯수보다 적은수 or 많은 수가 나오면 오류발생.
    //            if (num < 0 || num >= getEquipmentDataSize)
    //            {
    //                Debug.Log("EquipmentGacha30 Error");
    //                return;
    //            }   

    //            GachaFreeUI[i].GetComponent<Image>().sprite = getEquipmentData[num].sprite;
    //            equipmentGachaQuantity[num]++;
    //            // getEquipmentData[num].sprite = Resources.Load()

    //        }

    //        for (int i = 0; i < getEquipmentDataSize; i++)
    //        {
    //            if (equipmentGachaQuantity[i] > 0)
    //            {
    //                DataManager.GetDataManager().GainEquipmentGacha(getEquipmentData[i].itemName, equipmentGachaQuantity[i]);
    //            }
    //        }
    //    }

    //    else if(isSkillGacha)
    //    {
    //        gachaFreeUIPanel.SetActive(true);

    //        int[] skillGachaQuantity = new int[getSkillDataSize];

    //        for (int i = 0; i < 5; i++)
    //        {
    //            int num = Random.Range(0, getSkillDataSize);

    //            // 무기종류 갯수보다 적은수 or 많은 수가 나오면 오류발생.
    //            if (num < 0 || num >= getSkillDataSize)
    //            {
    //                Debug.Log("SkillGachaFree Error");
    //                return;
    //            }

    //            GachaFreeUI[i].GetComponent<Image>().sprite = getSkillData[num].sprite;
    //            skillGachaQuantity[num]++;
    //        }

    //        for (int i = 0; i < getSkillDataSize; i++)
    //        {
    //            if (skillGachaQuantity[i] > 0)
    //            {
    //                DataManager.GetDataManager().GainSkillGacha(getSkillData[i].skillName, skillGachaQuantity[i]);
    //            }
    //        }
    //    }

    //    isSkillGacha = false;
    //    isEquipmentGacha = false; 
    //}
}
