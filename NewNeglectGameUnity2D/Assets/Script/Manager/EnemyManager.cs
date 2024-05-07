using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;

public class EnemyManager : MonoBehaviour
{
    private static EnemyManager instance;

    string finalSavePath;                                                                                   // ��Ż ���

    public GameObject skullPrefab;                                                                          // �ذ� ������
    public GameObject dogPrefab;                                                                            // ������ ������
    public GameObject enemyCanvas;

    EnemyData[] newEnemyData;

    //static List<GameObject> enemyList;                                                                      // ������ �����ϴ� ����Ʈ
    static Dictionary<string, GameObject> enemyList;                                                                      // ������ �����ϴ� ����Ʈ

    List<Dictionary<string, object>> enemyDataList;                                                         // ���� ������ �޾ƿ���
    List<Dictionary<string, object>> FieldDataList;                                                         // �ʵ� ���� �޾ƿ���
    
    
    Dictionary<string, Coroutine> respawnCoroutineList;                                                         // �ʵ� ���� �޾ƿ���


    private void Awake()                                                                                    // Awake�� �ѹ��� ����.
    {
        if (instance == null)
            instance = this;

        InitEnemyPrefab();
        InitEnemyCanvas();
        GetCSVEnemyData();                                                                                  // �� ���� ��������
        CreateFieldEnemy();                                                                                  // �ʵ� ���� ��������
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
        // ���� �׾��� �� �߻��ϴ� delegate
        if (instance != null)
        {
            instance.EnemyDead(name);
        }
    }

    void InitEnemyPrefab()
    {
        // �� ������ �ʱ�ȭ
        skullPrefab = Resources.Load<GameObject>("Prefab/Enemy/Skull");
        dogPrefab = Resources.Load<GameObject>("Prefab/Enemy/Dog");
    }

    void InitEnemyCanvas()
    {
        // �� ĵ���� �ʱ�ȭ 
        enemyCanvas = Resources.Load<GameObject>("Prefab/Canvas/EnemyCanvas");
        var canvas = Instantiate(enemyCanvas);
        canvas.name = "EnemyCanvas";
    }

    void EnemyDead(string name)
    {
        // ���� �׾��ִ� List������ �̸��� �ִ���
        if (respawnCoroutineList.ContainsKey(name))
            return;
        // List�� ���ٸ� �ڷ�ƾ ���� , ������ ���� List�� �߰�
        var newRespawnCoroutine = StartCoroutine(enemyList[name].GetComponent<EnemyAction>().Respawn());
        respawnCoroutineList.Add(name, newRespawnCoroutine);
    }


    void EnemyRespawnEnd(string name)
    {
        // ���� �׾��ִ� List������ �̸��� �ִ���
        if (!respawnCoroutineList.ContainsKey(name))
            return;

        // List�� �ִٸ� �� �ڷ�ƾ ����. ������ ���� List���� ����
        StopCoroutine(respawnCoroutineList[name]); 
        respawnCoroutineList.Remove(name);
    }
    public static void EnemyRespawnEndEvent(string name)
    {
        instance.EnemyRespawnEnd(name);
    }

    public static bool IsExistEnemy()
    {
        // ���� 1���̶� �����ϴ���.
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

    #region Enemy ���� �޾ƿ���
    void GetCSVEnemyData()
    {
        enemyDataList = DataManager.GetDataManager().GetEnemyCSV();                                                           // ������ ������ ������

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

    #region �ʵ忡 �� �����ϱ�.
    void CreateFieldEnemy()
    {
        int skullNum=0;
        int dogNum=0;

        FieldDataList = DataManager.GetDataManager().GetFieldCSV();

        if (enemyList == null)
            enemyList = new Dictionary<string, GameObject>();     // �� ����Ʈ ����

        foreach (var getEnemyFieldData in FieldDataList)
        {
            GameObject enemyObject = null;
            string enemyName = getEnemyFieldData["EnemyName"].ToString();

            if (enemyName == "Skull")
            {
                enemyObject = Instantiate(skullPrefab);                                   // ���� ����
                enemyObject.GetComponent<Enemy>().SetName(skullPrefab.name + skullNum);
                skullNum++;
            }
            else if (enemyName == "Dog")
            {
                enemyObject = Instantiate(dogPrefab);                                     // �� ����
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


            // ���� �ʱ� ��ġ ����.
            Vector2 monsterPos = new Vector2();
            monsterPos.x = float.Parse(getEnemyFieldData["PosX"].ToString());
            monsterPos.y = float.Parse(getEnemyFieldData["PosY"].ToString());

            enemyObject.GetComponent<EnemyAction>().SetStartPos(monsterPos);
            enemyObject.GetComponent<Enemy>().SetDrop(getEnemyFieldData["DropItem"].ToString(), int.Parse(getEnemyFieldData["DropGold"].ToString()));

            if (enemyList.ContainsKey(enemyObject.name))
                continue;

            enemyList.Add(enemyObject.name, enemyObject);                                                 // Ȱ��ȭ �� ����Ʈ�� �߰�
        }
    }
    #endregion
}
