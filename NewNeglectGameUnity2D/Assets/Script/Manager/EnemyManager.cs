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

        Debug.Log("EnemyManager : Start : " + "InitEnemyPrefab");
        InitEnemyPrefab();
        Debug.Log("EnemyManager : Start : " + "InitEnemyCanvas");
        InitEnemyCanvas();
        Debug.Log("EnemyManager : Start : " + "GetCSVEnemyData");
        GetCSVEnemyData();                                                                                  // 적 정보 가져오기
        Debug.Log("EnemyManager : Start : " + "CreateFieldEnemy");
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

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        //foreach(var enemy in enemyList)
        //{
        //    if(enemy)
        //}
    }

    public static EnemyManager GetEnemyManager()
    {
        return instance;
    }

    public static void EnemyDeadUpdateEvent(string name)
    {
        if (instance != null)
        {
            instance.EnemyDead(name);
            //Debug.Log("UpdateEvnetTest UIManager");
        }
    }



    void InitEnemyPrefab()
    {
        skullPrefab = Resources.Load<GameObject>("Prefab/Enemy/Skull");
        dogPrefab = Resources.Load<GameObject>("Prefab/Enemy/Dog");
    }

    void InitEnemyCanvas()
    {
        enemyCanvas = Resources.Load<GameObject>("Prefab/Canvas/EnemyCanvas");
        var canvas = Instantiate(enemyCanvas);
        canvas.name = "EnemyCanvas";
    }

    void EnemyDead(string name)
    {
        if (respawnCoroutineList.ContainsKey(name))
            return;
        var newRespawnCoroutine = StartCoroutine(enemyList[name].GetComponent<EnemyAction>().Respawn());
        respawnCoroutineList.Add(name, newRespawnCoroutine);
        //enemyList[name].SetActive(false);
    }

    //void EnemyRespawnEnd(string name)
    //{
    //    if (!respawnCoroutineList.ContainsKey(name))
    //        return;
    //    StopCoroutine(respawnCoroutineList[name]);    
    //    respawnCoroutineList.Remove(name);
    //}

    public static void EnemyUpdateEvent(string name)
    {
        instance.EnemyRespawnEnd(name);
    }

    void EnemyRespawnEnd(string name)
    {
        if (!respawnCoroutineList.ContainsKey(name))
            return;
        StopCoroutine(respawnCoroutineList[name]); 
        respawnCoroutineList.Remove(name);
    }

    public static bool IsExistEnemy()
    {
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

    //#region Enemy 정보 받아오기
    //void GetCSVEnemyData()
    //{
    //    enemyDataList = DataManager.GetDataManager().GetEnemyCSV();                                                           // 몬스터의 데이터 가져옴

    //    int dataSize = enemyDataList.Count;
    //    newEnemyData = new EnemyData[dataSize];


    //    for(int i=0; i< dataSize; i++)
    //    {
    //        CreateEnemeyData(i, enemyDataList[])
    //    }

    //    foreach (var getEnemyData in enemyDataList)
    //    {
    //        finalSavePath = null;
    //        finalSavePath = DataManager.GetDataManager().GetSaveDataPath() + getEnemyData["EnemyName"].ToString() + ".asset";            // 저장파일 경로 저장
    //        if (!IsExistEnemyData())                                                                        // 몬스터 데이터가 존재하는가?(스크럽터블 오브젝트)
    //        {
    //            CreateEnemyScriptableObject(getEnemyData);                                                  // 없다면 생성
    //        }
    //    }
    //}
    //#endregion

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


    //#region Enemy 정보 생성하기
    //void CreateEnemyScriptableObject(Dictionary<string, object> data)
    //{
    //    //newEnemyData = ScriptableObject.CreateInstance<EnemyData>();                                        // 스크럽터블 오브젝트 생성

    //    foreach (var key in data.Keys)
    //    {
    //        if (key == "EnemyName")
    //            newEnemyData.enemyName = data[key].ToString();
    //        else if (key == "Level")
    //            newEnemyData.level = (int)data[key];
    //        else if (key == "Hp")
    //            newEnemyData.hp = (int)data[key];
    //        else if (key == "MaxHp")
    //            newEnemyData.maxHp = (int)data[key];
    //        else if (key == "Exp")
    //            newEnemyData.exp = (int)data[key];
    //        else if (key == "Atk")
    //            newEnemyData.atk = (int)data[key];
    //        else if (key == "Def")
    //            newEnemyData.def = (int)data[key];
    //        else if (key == "Speed")
    //            newEnemyData.speed = float.Parse(data[key].ToString());
    //        else if (key == "AnimationSpeed")
    //            newEnemyData.animationSpeed = float.Parse(data[key].ToString());
    //        else
    //            Debug.Log("Enemy Data error");
    //    }

    //    //AssetDatabase.CreateAsset(newEnemyData, finalSavePath);                                             // 에셋 생성

    //    //EditorUtility.SetDirty(newEnemyData);                                                               // 변경점
    //    //AssetDatabase.SaveAssets();                                                                         // 변경점 저장
    //}
    //#endregion

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
                enemyObject = Instantiate(skullPrefab);                                   // 해골 생성
                enemyObject.GetComponent<Enemy>().SetName(skullPrefab.name + skullNum);
                //enemyObject.name = skullPrefab.name + skullNum;
                skullNum++;
            }
            else if (enemyName == "Dog")
            {
                enemyObject = Instantiate(dogPrefab);                                     // 강아지 10마리 생성
                enemyObject.GetComponent<Enemy>().SetName(dogPrefab.name + dogNum);
                //enemyObject.name = dogPrefab.name + dogNum;
                dogNum++;
            }
            else
                Debug.Log("Enemy Data Name Error");

            if (enemyObject == null)
                return;

            Vector2 monsterPos = new Vector2();
            monsterPos.x = float.Parse(getEnemyFieldData["PosX"].ToString());
            monsterPos.y = float.Parse(getEnemyFieldData["PosY"].ToString());

            enemyObject.GetComponent<EnemyAction>().SetStartPos(monsterPos);
            enemyObject.GetComponent<Enemy>().SetDrop(getEnemyFieldData["DropItem"].ToString(), int.Parse(getEnemyFieldData["DropGold"].ToString()));

            enemyList.Add(enemyObject.name, enemyObject);                                                 // 활성화 후 리스트에 추가
        }
    }
    #endregion

    #region 스크럽터블 오브젝트 존재여부
    //bool IsExistEnemyData()
    //{
    //    EnemyData enemyData = AssetDatabase.LoadAssetAtPath(finalSavePath, typeof(EnemyData)) as EnemyData; // 해당 경로에 스크럽터블 오브젝트가 있는지
    //    return enemyData != null;                                                                           // 있으면 true
    //}
    #endregion

}
