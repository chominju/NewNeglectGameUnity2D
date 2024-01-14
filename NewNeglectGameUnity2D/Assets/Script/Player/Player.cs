using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
//using UnityEditor;


//EditorApplication
public class Player : MonoBehaviour
{
    private static Player instance = null;

    public struct PlayerInfoData
    {
       public string name;
       public int level;
       public int atk;
       public int def;
       public int power;
       public float moveSpeed;               
       public int maxHp;
       public int currentHp;
       public int maxMp;
       public int currentMp;
       public int maxExp;
       public int currentExp;
       public int statPoint;
       public int currentGold;
    }

    public PlayerInfoData playerInfo;
    public bool isPlayerAttackAnimationStart;
    bool isDead;
    // Start is called before the first frame update

    public void PlayerAttackBegin()
    {
        isPlayerAttackAnimationStart = true;
    }

    private void Awake()
    {
        if (instance == null)
            instance = this;
        gameObject.name = "Player";
        isPlayerAttackAnimationStart = false;
        InitPlayerStat();
    }
    public PlayerInfoData GetPlayerInfo()
    {
        return playerInfo;
    }

    public void InitPlayerStat()
    {
        var getPlayerData = DataManager.GetDataManager().GetPlayerData();
        var getStatCSV = DataManager.GetDataManager().GetStatCSV();
        playerInfo.level = getPlayerData.playerLevel;
        playerInfo.atk = int.Parse(getStatCSV[getPlayerData.atkLevel]["Atk"].ToString());
        playerInfo.def = int.Parse(getStatCSV[getPlayerData.defLevel]["Def"].ToString());
        playerInfo.moveSpeed = float.Parse(getStatCSV[getPlayerData.moveSpeedLevel]["MoveSpeed"].ToString());
        playerInfo.maxHp = getPlayerData.maxHpLevel * int.Parse(getStatCSV[getPlayerData.maxHpLevel]["MaxHp"].ToString());
        playerInfo.maxMp = getPlayerData.maxMpLevel * int.Parse(getStatCSV[getPlayerData.maxMpLevel]["MaxMp"].ToString());
        playerInfo.power = playerInfo.atk + playerInfo.def + playerInfo.maxHp + playerInfo.maxMp;
        playerInfo.maxExp = int.Parse(getStatCSV[playerInfo.level]["MaxExp"].ToString());
        playerInfo.name = getPlayerData.playerName;
        playerInfo.currentHp = getPlayerData.currentHp;
        playerInfo.currentMp = playerInfo.maxMp;
        playerInfo.currentExp = getPlayerData.currentExp;
        playerInfo.statPoint = getPlayerData.statPoint;
        playerInfo.currentGold = getPlayerData.currentGold;
    }

    public static void PlayerUpdateEvent()
    {
        instance.UpdatePlayerStat();
    }


    public void UpdatePlayerStat()
    {
        var getPlayerData = DataManager.GetDataManager().GetPlayerData();
        var getStatCSV = DataManager.GetDataManager().GetStatCSV();
        playerInfo.level = getPlayerData.playerLevel;
        playerInfo.atk = int.Parse(getStatCSV[getPlayerData.atkLevel]["Atk"].ToString());
        playerInfo.def = int.Parse(getStatCSV[getPlayerData.defLevel]["Def"].ToString());
        playerInfo.moveSpeed = float.Parse(getStatCSV[getPlayerData.moveSpeedLevel]["MoveSpeed"].ToString());
        playerInfo.maxHp = int.Parse(getStatCSV[getPlayerData.maxHpLevel]["MaxHp"].ToString());
        playerInfo.maxMp = int.Parse(getStatCSV[getPlayerData.maxMpLevel]["MaxMp"].ToString());
        playerInfo.power = playerInfo.atk + playerInfo.def + playerInfo.maxHp + playerInfo.maxMp;
        playerInfo.currentHp = getPlayerData.currentHp;
        playerInfo.currentExp = getPlayerData.currentExp;
        playerInfo.maxExp = int.Parse(getStatCSV[getPlayerData.playerLevel]["MaxExp"].ToString());
        playerInfo.statPoint = getPlayerData.statPoint;
        playerInfo.currentGold = getPlayerData.currentGold;

        PlayerHaveAtkEquipmentAndSkill();
        gameObject.GetComponent<PlayerAction>().UpdatePlayerData();
    }


    public void PlayerHaveAtkEquipmentAndSkill()
    {
        // 현재는 보유중인것만 적용중. 장착효과 x
        // 플레이어 스탯 공격력 + 스킬 보유 공격력 + 장비 보유 공격력
        int playerStatAtk = playerInfo.atk;
        int skillHaveAtk = DataManager.GetDataManager().GetSkillHaveAtk();
        int equipmentHaveAtk = DataManager.GetDataManager().GetEquipmentHaveAtk();
        int equipmentEquipAtk = DataManager.GetDataManager().GetEquipmentEquipAtk();
        // atk = 공격력 ( 플레이어 atk + 보유스킬 공격력 + 보유무기 공격력)
        // power = 전투력(보이는것 atk + 플레이어스탯)
        playerInfo.atk = playerInfo.atk + skillHaveAtk + equipmentHaveAtk + equipmentEquipAtk;
        playerInfo.power += (skillHaveAtk + equipmentHaveAtk + equipmentEquipAtk);
    }

    public void PlayerEquipEquipmentAtk()
    {
        var getPlayerData = DataManager.GetDataManager().GetPlayerData();
        int equipAtk = DataManager.GetDataManager().EquipAtkEquipment(getPlayerData.equipItemName);
        if (equipAtk == -1)
            return;

        playerInfo.atk = playerInfo.atk + equipAtk;
        playerInfo.power += equipAtk;

    }
}