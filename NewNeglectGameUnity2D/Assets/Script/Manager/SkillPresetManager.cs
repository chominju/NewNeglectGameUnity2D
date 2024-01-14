using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPresetManager : MonoBehaviour
{
    private static SkillPresetManager instance = null;

    string[] skillNamePreset;
    GameObject[] skillPrefabPreset;                             // ��ų ������ 
    GameObject[] skillPresetButtonImage;                        // ��ų ������ ��ų �̹���
    GameObject[] skillPresetButtonCoolTimeText;                 // ��ų ������ ��ų ��Ÿ��
    GameObject[] skillPresetButtonImageBackGround;              // ��ų ������ ��ų ��� �̹���
    float[] skillCoolTimePreset;                                // ��ų ��Ÿ�� 
    float[] skillCurrentCoolTimePreset;                         // ��ų ��Ÿ�� �����ð�
    bool[] isSkillUsePreset;                                    // ��ų ��� ����

    private SkillData[] getSkillDataAsset;                      // ��ų ������ ��������

    //public Button AutoButton;

    public Button autoButton;                                   // ���� ��ư
    bool isAuto;

    Coroutine autoCoroutine;
        
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
            instance = this;

        InitPreset();

        isAuto = false;
        getSkillDataAsset = DataManager.GetDataManager().GetSkillData();
        autoCoroutine = null;
        UpdateSkillPreset();

    }

    void InitPreset()
    {
        skillNamePreset = new string[6];
        skillPrefabPreset = new GameObject[6];
        skillPresetButtonImage = new GameObject[6];
        skillPresetButtonImageBackGround = new GameObject[6];
        skillPresetButtonCoolTimeText = new GameObject[6];
        skillCoolTimePreset = new float[6];
        skillCurrentCoolTimePreset = new float[6];
        isSkillUsePreset = new bool[6];
    }

    public void AutoButtonClick()
    {
        SoundManager.GetInstance().PlayClickSound();

        if (isAuto == true)         //   �������̸� ��������
        {
            isAuto = false;
            autoButton.GetComponent<Image>().color = Color.gray;  // ���� x
            if (autoCoroutine != null)
                StopCoroutine(autoCoroutine);
        }
        else
        {
            // �������� �ƴϸ� ���伳��
            autoButton.GetComponent<Image>().color = Color.yellow;  // ���� o
            isAuto = true;
           
            if (autoCoroutine != null)
                StopCoroutine(autoCoroutine);
            autoCoroutine = StartCoroutine(AutoSkill());
        }
    }
    
    public static void SkillPresetUIUpdateEvent()
    {
        if (instance != null)
        {
            instance.UpdateSkillPreset();
        }
    }

    void UpdateSkillPreset()
    {
        // ��ų ������ ������Ʈ
        var getSkillPreset = DataManager.GetDataManager().GetSkillPreset();

        for (int i = 0; i < 6; i++)
        {
            // ���ִٸ� ����ĭ���� 
            if (getSkillPreset[i] == null)
                continue;
            else
            {
                int buttonNum = i + 1;
                SetSkillPreset(getSkillPreset[i], i);
            }   
        }   
    }

    public void SetSkillPreset(string skillName , int num)
    {
        int buttonNum = num + 1;
        // �̹� ���ÿϷ�
        if (skillNamePreset[num] == skillName)
            return;
        skillNamePreset[num] = skillName;
        skillPrefabPreset[num] = Resources.Load<GameObject>("Prefab/Skill/" + skillName);


        var getSkillData = DataManager.GetDataManager().GetFindSkillData(skillName);
        skillCoolTimePreset[num] = getSkillData.skillCoolTime;
        skillCurrentCoolTimePreset[num] = skillCoolTimePreset[num];
        isSkillUsePreset[num] = false;


            GameObject presetButton = gameObject.transform.Find("SkillButton" + buttonNum).gameObject;
            skillPresetButtonImageBackGround[num] = presetButton;

            if (presetButton == null)
                Debug.Log("Preset Load Error");

            skillPresetButtonImage[num] = presetButton.transform.Find("SkillImage" + buttonNum).gameObject;
            skillPresetButtonCoolTimeText[num] = presetButton.transform.Find("SkillCoolTime" + buttonNum).gameObject;
            presetButton.transform.Find("SkillImage" + buttonNum).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Skill/Icon/" + "Icon_" + skillName);
            skillPresetButtonImageBackGround[num].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Skill/Icon/" + "Icon_" + skillName);
            skillPresetButtonImageBackGround[num].GetComponent<Image>().color = Color.gray;
    }


    public void SkillClick(Button skillButton)                                      
    {
        // ��ų�� �������� ����� ��
        GameObject tempSkill = null;
        int presetNum = 0;
        if (skillButton.name == "SkillButton1")
        {
            tempSkill = skillPrefabPreset[0];
            presetNum = 0;
        }
        else if (skillButton.name == "SkillButton2")
        {
            tempSkill = skillPrefabPreset[1];
            presetNum = 1;
        }
        else if (skillButton.name == "SkillButton3")
        {
            tempSkill = skillPrefabPreset[2];
            presetNum = 2;
        }
        else if (skillButton.name == "SkillButton4")
        {
            tempSkill = skillPrefabPreset[3];
            presetNum = 3;
        }
        else if (skillButton.name == "SkillButton5")
        {
            tempSkill = skillPrefabPreset[4];
            presetNum = 4;
        }
        else if (skillButton.name == "SkillButton6")
        {
            tempSkill = skillPrefabPreset[5];
            presetNum = 5;
        }
        else
        {
            // ������ ��ȣ ����
            SoundManager.GetInstance().PlayFailSound();
            Debug.Log("PRESET NUM ERROR");
        }
        if (tempSkill != null)
        {
            if (isSkillUsePreset[presetNum] == false)
            {
                // ��ų ����� �����ʾҴٸ�
                SoundManager.GetInstance().PlayClickSound();
                SkillUse(presetNum);
            }
            else
            {
                // ��ų�� �̹� �����
                SoundManager.GetInstance().PlayFailSound();
                Debug.Log("Skill Already Use");
            }
        }
    }
    IEnumerator AutoSkill()
    {
        // ��ų ���並 Ŵ
        while (true)
        {
            for (int i = 0; i < 6; i++)
            {
                if (skillPrefabPreset[i] == null)
                    continue;
                else
                {
                    if (isSkillUsePreset[i] == false)
                    {
                        SkillUse(i);
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    void SkillUse(int presetNum)
    {
        // ��ų ��� true , ��ų ����
        isSkillUsePreset[presetNum] = true;
        GameObject.Instantiate(skillPrefabPreset[presetNum]);
    }

    private void Update()
    {
        // ��Ÿ�� ������Ʈ
        for(int presetNum = 0; presetNum < 6; presetNum++)
        {
            if(isSkillUsePreset[presetNum])
            {
                skillCurrentCoolTimePreset[presetNum] -= Time.deltaTime;

                // ������Ÿ�� �ð� text ������Ʈ
                skillPresetButtonCoolTimeText[presetNum].GetComponent<Text>().text = ((int)skillCurrentCoolTimePreset[presetNum]).ToString();

                // �׸����� ǥ��
                float fillAmount = 1.0f -  skillCurrentCoolTimePreset[presetNum] / skillCoolTimePreset[presetNum];
                skillPresetButtonImage[presetNum].GetComponent<Image>().fillAmount = fillAmount;

                if (skillCurrentCoolTimePreset[presetNum]<=0.0f)
                {
                    isSkillUsePreset[presetNum] = false;
                    skillCurrentCoolTimePreset[presetNum] = skillCoolTimePreset[presetNum];
                    skillPresetButtonCoolTimeText[presetNum].GetComponent<Text>().text = "";
                }
            }

        }
    }

}
