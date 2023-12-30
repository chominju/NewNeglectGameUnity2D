using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillPresetManager : MonoBehaviour
{
    private static SkillPresetManager instance = null;

    string[] skillNamePreset;
    GameObject[] skillPrefabPreset;                             // 스킬 프리셋 
    GameObject[] skillPresetButtonImage;                        // 스킬 프리셋 스킬 이미지
    GameObject[] skillPresetButtonCoolTimeText;                 // 스킬 프리셋 스킬 쿨타임
    GameObject[] skillPresetButtonImageBackGround;              // 스킬 프리셋 스킬 배경 이미지
    float[] skillCoolTimePreset;                                // 스킬 쿨타임 
    float[] skillCurrentCoolTimePreset;                         // 스킬 쿨타임 남은시간
    bool[] isSkillUsePreset;                                    // 스킬 사용 여부
    int skillAmount;                                            // 스킬의 전체 갯수

    private SkillData[] getSkillDataAsset;                      // 스킬 데이터 가져오기

    //public Button AutoButton;

    public Button autoButton;                                   // 오토 버튼
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
          skillAmount = 9;// DataManager.GetSkillDataAssetAmount();
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

        if (isAuto == true)         //   오토중이면 오토해제
        {
            isAuto = false;
            autoButton.GetComponent<Image>().color = Color.gray;//new Color(50, 50, 50, 150); // 오토 x
            if (autoCoroutine != null)
                StopCoroutine(autoCoroutine);
        }
        else
        {
            // 오토중이 아니면 오토설정
            autoButton.GetComponent<Image>().color = Color.yellow;// new Color(255, 215, 0, 255); // 오토 o
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
            //Debug.Log("SkillPresetUIUpdateEvent");
        }
    }

    void UpdateSkillPreset()
    {
        var getSkillPreset = DataManager.GetDataManager().GetSkillPreset();

        for (int i = 0; i < 6; i++)
        {
            if (getSkillPreset[i] == null)
                continue;
            else
            {
                int buttonNum = i + 1;
                //skillNamePreset[i] = getSkillPreset[i]''
                //skillPrefabPreset[i] = Resources.Load<GameObject>("Prefab/Skill/Prefab/" + getSkillPreset[i]);
                SetSkillPreset(getSkillPreset[i], i);
                //GameObject presetButton = gameObject.transform.Find("SkillButton" + buttonNum).gameObject;
                //if (presetButton == null)
                //    Debug.Log("Preset Load Error");
                //presetButton.transform.Find("SkillImage" + buttonNum).GetComponent<Image>().sprite = Resources.Load<Sprite>("Skill/Icon/" + "Icon_" + getSkillPreset[i]);
            }   
        }   
    }

    public void SetSkillPreset(string skillName , int num)
    {
        int buttonNum = num + 1;
        if (skillNamePreset[num] == skillName)
            return;
        skillNamePreset[num] = skillName;
        skillPrefabPreset[num] = Resources.Load<GameObject>("Prefab/Skill/" + skillName);
        for (int i = 0; i < skillAmount /*skillAmount*/; i++)
        {
            if (getSkillDataAsset[i].skillName == skillName)
            {
                skillCoolTimePreset[num] = getSkillDataAsset[i].skillCoolTime;
                skillCurrentCoolTimePreset[num] = skillCoolTimePreset[num];
                isSkillUsePreset[num] = false;
                break;
            }

        }
            GameObject presetButton = gameObject.transform.Find("SkillButton" + buttonNum).gameObject;
            skillPresetButtonImageBackGround[num] = presetButton;
            if (presetButton == null)
                Debug.Log("Preset Load Error");
            skillPresetButtonImage[num] = presetButton.transform.Find("SkillImage" + buttonNum).gameObject;
            skillPresetButtonCoolTimeText[num] = presetButton.transform.Find("SkillCoolTime" + buttonNum).gameObject;
            presetButton.transform.Find("SkillImage" + buttonNum).GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Skill/Icon/" + "Icon_" + skillName);
            skillPresetButtonImageBackGround[num].GetComponent<Image>().sprite = Resources.Load<Sprite>("Sprite/Skill/Icon/" + "Icon_" + skillName);
            skillPresetButtonImageBackGround[num].GetComponent<Image>().color = Color.gray; //new Color(0.2f, 0.2f, 0.2f);
    }


    public void SkillClick(Button skillButton)                                         // 스킬을 수동으로 사용할 때
    {
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
            SoundManager.GetInstance().PlayFailSound();
            Debug.Log("PRESET NUM ERROR");
        }
        if (tempSkill != null)
        {
            if (isSkillUsePreset[presetNum] == false)
            {
                SoundManager.GetInstance().PlayClickSound();
                SkillUse(presetNum);
            }
            else
            {
                SoundManager.GetInstance().PlayFailSound();
                Debug.Log("Skill Already Use");
            }
        }
    }
    IEnumerator AutoSkill()
    {
        //if (isAuto)
        //{
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
                //    if (skillPrefabPreset[i].GetComponent<Skill>().IsSkillUse() == false)
                //{
                //    skillPrefabPreset[i].GetComponent<Skill>().SkillUse();
                //    GameObject.Instantiate(skillPrefabPreset[i]);
                //}
            }

            yield return new WaitForSeconds(0.5f);
        }
        //foreach (var skill in skillPrefabPreset)
        //{
        //    if (skill == null)                      // 스킬이 등록되어 있지않으면
        //        continue;
        //    else
        //    {
        //        // 스킬이 있을 때(사용하지않았을 떄)
        //        if (skill.GetComponent<Skill>().IsSkillUse() == false)
        //        {
        //            skill.GetComponent<Skill>().SkillUse();
        //            GameObject.Instantiate(skill);
        //        }
        //    }
        //}
        //}
    }

    void SkillUse(int presetNum)
    {
        isSkillUsePreset[presetNum] = true;
        /*skillObject[presetNum] = */ GameObject.Instantiate(skillPrefabPreset[presetNum]);
    }

    private void Update()
    {
        for(int presetNum = 0; presetNum < 6; presetNum++)
        {
            if(isSkillUsePreset[presetNum])
            {

                //Debug.Log(skillNamePreset[presetNum] + "Time : " + skillCurrentCoolTimePreset[presetNum]);
                skillCurrentCoolTimePreset[presetNum] -= Time.deltaTime;

                skillPresetButtonCoolTimeText[presetNum].GetComponent<Text>().text = ((int)skillCurrentCoolTimePreset[presetNum]).ToString();

                float fillAmount = 1.0f -  skillCurrentCoolTimePreset[presetNum] / skillCoolTimePreset[presetNum];
                skillPresetButtonImage[presetNum].GetComponent<Image>().fillAmount = fillAmount;

                if (skillCurrentCoolTimePreset[presetNum]<=0.0f)
                {
                    //Debug.Log(skillNamePreset[presetNum] + "Time : " + skillCurrentCoolTimePreset[presetNum]+"      END");
                    isSkillUsePreset[presetNum] = false;
                    skillCurrentCoolTimePreset[presetNum] = skillCoolTimePreset[presetNum];
                    skillPresetButtonCoolTimeText[presetNum].GetComponent<Text>().text = "";
                }
            }

        }
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
}
