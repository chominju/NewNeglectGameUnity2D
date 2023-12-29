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
        // ����� �������ΰ͸� ������. ����ȿ�� x
        // �÷��̾� ���� ���ݷ� + ��ų ���� ���ݷ� + ��� ���� ���ݷ�
        int playerStatAtk = playerInfo.atk;
        int skillHaveAtk = DataManager.GetDataManager().GetSkillHaveAtk();
        int equipmentHaveAtk = DataManager.GetDataManager().GetEquipmentHaveAtk();
        // atk = ���ݷ� ( �÷��̾� atk + ������ų ���ݷ� + �������� ���ݷ�)
        // power = ������(���̴°� atk + �÷��̾��)
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

    //public override IEnumerator DamageCharacter(int damage, float interval)     // �÷��̾ ���� ������ ��
    //{

    //    while (true)
    //    {
    //        StartCoroutine(ChanageColorCharacter(interval));                            // �ڷ�ƾ ����(�ǰݽ� �� ����)

    //        if (playerInfo.def >= damage)                                       // �÷��̾��� ���� > ���� ���ݷ�
    //            damage = 1;                                                     // �������� 1�� ����
    //        else
    //            damage -= playerInfo.def;                                       // �ƴ϶�� ���ǰ��ݷ� - ����

    //        playerInfo.currentHp -= damage;

    //        if (playerInfo.currentHp <= 0)
    //        {
    //            KillCharacter();
    //            break;
    //        }

    //        if (interval > float.Epsilon)                                       // interval �ð���ŭ ���.
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