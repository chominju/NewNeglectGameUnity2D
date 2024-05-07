using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;

    string finalSavePath;                                                                                   // 토탈 경로

    public GameObject skullPrefab;                                                                          // 해골 프리펩
    public GameObject dogPrefab;                                                                            // 강아지 프리펩
    public GameObject enemyCanvas;

    EnemyData[] newEnemyData;

    //static List<GameObject> enemyList;                                                                      // 적들을 보관하는 리스트
    static Dictionary<string, GameObject> enemyList;                                                                      // 적들을 보관하는 리스트

    List<Dictionary<string, object>> enemyDataList;                                                         // 적들 데이터 받아오기
    List<Dictionary<string, object>> FieldDataList;                                                         // 필드 정보 받아오기
    
    
    Dictionary<string, Coroutine> respawnCoroutineList;                                                         // 필드 정보 받아오기


    private void Awake()                                                                                    // Awake로 한번만 실행.
    {
        if (instance == null)
            instance = this;

        InitEnemyPrefab();
        InitEnemyCanvas();
        GetCSVEnemyData();                                                                                  // 적 정보 가져오기
        CreateFieldEnemy();                                                                                  // 필드 정보 가져오기
        respawnCoroutineList = new Dictionary<string, Coroutine>();
    }

   public EnemyData GetEnemyData(string name)
    {
        EnemyData getData;

        foreach(var data in newEnemyData)
        {
            if (data.enemyName.Equals(name))
            {
                getData = data;
                return getData;
            }
        }
        return null;
    }
    public static EnemyManager GetEnemyManager()
    {
        return instance;
    }

    public static void EnemyDeadUpdateEvent(string name)
    {
        // 적이 죽었을 때 발생하는 delegate
        if (instance != null)
        {
            instance.EnemyDead(name);
        }
    }

    void InitEnemyPrefab()
    {
        // 적 프리펩 초기화
        skullPrefab = Resources.Load<GameObject>("Prefab/Enemy/Skull");
        dogPrefab = Resources.Load<GameObject>("Prefab/Enemy/Dog");
    }

    void InitEnemyCanvas()
    {
        // 적 캔버스 초기화 
        enemyCanvas = Resources.Load<GameObject>("Prefab/Canvas/EnemyCanvas");
        var canvas = Instantiate(enemyCanvas);
        canvas.name = "EnemyCanvas";
    }

    void EnemyDead(string name)
    {
        // 적이 죽어있는 List모음에 이름이 있는지
        if (respawnCoroutineList.ContainsKey(name))
            return;
        // List에 없다면 코루틴 실행 , 리스폰 관련 List에 추가
        var newRespawnCoroutine = StartCoroutine(enemyList[name].GetComponent<EnemyAction>().Respawn());
        respawnCoroutineList.Add(name, newRespawnCoroutine);
    }


    void EnemyRespawnEnd(string name)
    {
        // 적이 죽어있는 List모음에 이름이 있는지
        if (!respawnCoroutineList.ContainsKey(name))
            return;

        // List에 있다면 그 코루틴 종료. 리스폰 관련 List에서 삭제
        StopCoroutine(respawnCoroutineList[name]); 
        respawnCoroutineList.Remove(name);
    }
    public static void EnemyRespawnEndEvent(string name)
    {
        instance.EnemyRespawnEnd(name);
    }

    public static bool IsExistEnemy()
    {
        // 적이 1명이라도 존재하는지.
        foreach(var enemyObject in enemyList)
        {
            if (enemyObject.Value.activeSelf)
                return true;
        }
        return false;
    }
    public static Dictionary<string, GameObject> GetEnemyList()
    {
        return enemyList;
    }

    #region Enemy 정보 받아오기
    void GetCSVEnemyData()
    {
        enemyDataList = DataManager.GetDataManager().GetEnemyCSV();                                                           // 몬스터의 데이터 가져옴

        int dataSize = enemyDataList.Count;
        newEnemyData = new EnemyData[dataSize];

        for (int i = 0; i < dataSize; i++)
        {
            CreateEnemeyData(i, enemyDataList[i]);
        }
    }
    #endregion

    void CreateEnemeyData(int index , Dictionary<string, object> data)
    {
        newEnemyData[index] = new EnemyData();
        foreach (var key in data.Keys)
        {
            if (key == "EnemyName")
                newEnemyData[index].enemyName = data[key].ToString();
            else if (key == "Level")
                newEnemyData[index].level = (int)data[key];
            else if (key == "Hp")
                newEnemyData[index].hp = (int)data[key];
            else if (key == "MaxHp")
                newEnemyData[index].maxHp = (int)data[key];
            else if (key == "Exp")
                newEnemyData[index].exp = (int)data[key];
            else if (key == "Atk")
                newEnemyData[index].atk = (int)data[key];
            else if (key == "Def")
                newEnemyData[index].def = (int)data[key];
            else if (key == "Speed")
                newEnemyData[index].speed = float.Parse(data[key].ToString());
            else if (key == "AnimationSpeed")
                newEnemyData[index].animationSpeed = float.Parse(data[key].ToString());
            else
                Debug.Log("Enemy Data error");
        }
    }

    #region 필드에 적 생성하기.
    void CreateFieldEnemy()
    {
        int skullNum=0;
        int dogNum=0;

        FieldDataList = DataManager.GetDataManager().GetFieldCSV();

        if (enemyList == null)
            enemyList = new Dictionary<string, GameObject>();     // 적 리스트 생성

        foreach (var getEnemyFieldData in FieldDataList)
        {
            GameObject enemyObject = null;
            string enemyName = getEnemyFieldData["EnemyName"].ToString();

            if (enemyName == "Skull")
            {
                enemyObject = Instantiate(skullPrefab);                                   // 스컬 생성
                enemyObject.GetComponent<Enemy>().SetName(skullPrefab.name + skullNum);
                skullNum++;
            }
            else if (enemyName == "Dog")
            {
                enemyObject = Instantiate(dogPrefab);                                     // 개 생성
                enemyObject.GetComponent<Enemy>().SetName(dogPrefab.name + dogNum);
                dogNum++;
            }
            else
            {
                Debug.Log("Enemy Data Name Error");
                continue;
            }

            if (enemyObject == null)
                return;


            // 몬스터 초기 위치 설정.
            Vector2 monsterPos = new Vector2();
            monsterPos.x = float.Parse(getEnemyFieldData["PosX"].ToString());
            monsterPos.y = float.Parse(getEnemyFieldData["PosY"].ToString());

            enemyObject.GetComponent<EnemyAction>().SetStartPos(monsterPos);
            enemyObject.GetComponent<Enemy>().SetDrop(getEnemyFieldData["DropItem"].ToString(), int.Parse(getEnemyFieldData["DropGold"].ToString()));

            if (enemyList.ContainsKey(enemyObject.name))
                continue;

            enemyList.Add(enemyObject.name, enemyObject);                                                 // 활성화 후 리스트에 추가
        }
    }
    #endregion
}
