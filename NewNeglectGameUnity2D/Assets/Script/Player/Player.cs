using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using System.IO;
//using UnityEditor;


//EditorApplication
public class Player : Character
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
        playerInfo.atk = /*getPlayerData.atkLevel **/ int.Parse(getStatCSV[getPlayerData.atkLevel]["atk"].ToString());
        playerInfo.def = /*getPlayerData.defLevel **/ int.Parse(getStatCSV[getPlayerData.defLevel]["def"].ToString());
        playerInfo.moveSpeed = float.Parse(getStatCSV[getPlayerData.moveSpeedLevel]["moveSpeed"].ToString());
        playerInfo.maxHp = getPlayerData.maxHpLevel * int.Parse(getStatCSV[getPlayerData.maxHpLevel]["maxHp"].ToString());
        playerInfo.maxMp = getPlayerData.maxMpLevel * int.Parse(getStatCSV[getPlayerData.maxMpLevel]["maxMp"].ToString());
        playerInfo.power = playerInfo.atk + playerInfo.def + playerInfo.maxHp + playerInfo.maxMp;
        playerInfo.maxExp = int.Parse(getStatCSV[playerInfo.level]["maxExp"].ToString());
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
        //Debug.Log("UpdateEvnetTest Player");
    }


    public void UpdatePlayerStat()
    {
        var getPlayerData = DataManager.GetDataManager().GetPlayerData();
        var getStatCSV = DataManager.GetDataManager().GetStatCSV();
        playerInfo.level = getPlayerData.playerLevel;
        playerInfo.atk = /*getPlayerData.atkLevel **/ int.Parse(getStatCSV[getPlayerData.atkLevel]["atk"].ToString());
        playerInfo.def = /*getPlayerData.defLevel **/ int.Parse(getStatCSV[getPlayerData.defLevel]["def"].ToString());
        playerInfo.moveSpeed = float.Parse(getStatCSV[getPlayerData.moveSpeedLevel]["moveSpeed"].ToString());
        playerInfo.maxHp = /*getPlayerData.maxHpLevel * */int.Parse(getStatCSV[getPlayerData.maxHpLevel]["maxHp"].ToString());
        playerInfo.maxMp = /*getPlayerData.maxMpLevel * */int.Parse(getStatCSV[getPlayerData.maxMpLevel]["maxMp"].ToString());
        playerInfo.power = playerInfo.atk + playerInfo.def + playerInfo.maxHp + playerInfo.maxMp;
        playerInfo.currentHp = getPlayerData.currentHp;
        playerInfo.currentExp = getPlayerData.currentExp;
        playerInfo.maxExp = int.Parse(getStatCSV[getPlayerData.playerLevel]["maxExp"].ToString());
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
        // atk = 공격력 ( 플레이어 atk + 보유스킬 공격력 + 보유무기 공격력)
        // power = 전투력(보이는것 atk + 플레이어스탯)
        playerInfo.atk = playerInfo.atk + skillHaveAtk + equipmentHaveAtk;
        playerInfo.power += (skillHaveAtk + equipmentHaveAtk);
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

    public void LevelUp()
    {
        //NeglectGameManager.savePlayerData.playerLevel += 1;
        //NeglectGameManager.savePlayerData.statPoint += 1;
        InitPlayerStat();
    }

    //public override IEnumerator DamageCharacter(int damage, float interval)     // 플레이어가 공격 당했을 때
    //{

    //    while (true)
    //    {
    //        StartCoroutine(ChanageColorCharacter(interval));                            // 코루틴 시작(피격시 색 변경)

    //        if (playerInfo.def >= damage)                                       // 플레이어의 방어력 > 적의 공격력
    //            damage = 1;                                                     // 데미지를 1로 고정
    //        else
    //            damage -= playerInfo.def;                                       // 아니라면 적의공격력 - 방어력

    //        playerInfo.currentHp -= damage;

    //        if (playerInfo.currentHp <= 0)
    //        {
    //            KillCharacter();
    //            break;
    //        }

    //        if (interval > float.Epsilon)                                       // interval 시간만큼 대기.
    //            yield return new WaitForSeconds(interval);
    //        else
    //            break;

    //    }
    //    yield return new WaitForSeconds(1);
    //}

    public void SetHp(int hpValue)
    {
        playerInfo.currentHp += hpValue;
    }

    public int GetHp()
    {
        return playerInfo.currentHp;
    }

    //public void ExpGain(int exp)
    //{
    //    playerInfo.currentExp += exp;
    //    if (playerInfo.currentExp >= playerInfo.maxExp)
    //    {
    //        playerInfo.currentExp -= playerInfo.maxExp;
    //        DataManager.GetDataManager().PlayerLevelUp();
    //    }
    //    DataManager.GetDataManager().SetExp(playerInfo.currentExp);
    //}

    //public void GoldGain(int gold)
    //{
    //    playerInfo.currentGold += gold;
        
    //    DataManager.GetDataManager().SetGold(playerInfo.currentGold);
    //}
    private IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.5f);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      
    }

    private void OnDestroy()
    {
       
    }


}