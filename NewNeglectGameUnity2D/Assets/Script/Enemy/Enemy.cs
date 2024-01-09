using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;

delegate void UpdateDataEnemy(string name);
public class Enemy : Character
{
    public string enemyName;                    // 적이름
    EnemyData enemyData;                        // DataManager에서 가져오는 enemyData

    public GameObject hpBarPrefab;              // 적 hpBar 프리팹
    public GameObject hpBar;                    // 적 hpBar 생성한 뒤 오브젝트
    public GameObject canvasHpBar;              // 적 hpBar들이 있는 canvas
    RectTransform hpBarTransform;               // 적 hpBar의 RectTransform
    public float height;                        // 적과 hpBar 사이의 거리
        
    int hp;                                     // 적 hp
    int maxHp;                                  // 적 최대체력
    int exp;                                    // 적이 주는 경험치
    float respawnTime;                          // 적이 죽었을 떄 리스폰 시간

    public GameObject dropItem;                 // 적이 드랍하는 아이템
    public int dropGold;                        // 적이 드랍하는 골드량

    private event UpdateDataEnemy enemyDeadEvent;   // EnemyManager에 있는 enemyDead(리스폰 관련)

    private void Awake()
    {
        SetEnemyData();
    }

    void Start()
    {
        // 초기 설정
        InitEnemyData();
        InitHpBar();
    }
    private void OnEnable()
    {
        // 다시 리스폰됐을 때
        StopAllCoroutines();
        InitEnemyData();
        InitHpBar();
    }

    void InitHpBar()
    {
        if (canvasHpBar == null)
            canvasHpBar = GameObject.Find("EnemyCanvas");
        if (hpBar == null)
        {
            // hp바 설정하기
            hpBar = Instantiate(hpBarPrefab, canvasHpBar.transform);
            hpBar.name = gameObject.name + "hpBar";
            hpBarTransform = hpBar.GetComponent<RectTransform>();
            hpBarTransform.localScale = new Vector3(0.2f, 0.2f, 1);
        }
        hpBar.SetActive(true);
    }


    void InitEnemyData()
    {
        hp = enemyData.hp;
        maxHp = enemyData.maxHp;
        exp = enemyData.exp;
        respawnTime = 15.0f;
        height = 0.7f;
        isDead = false;
    }

    public void SetEnemyData()
    {
        // 데이터설정 및, delegate 설정 , 컴포넌트추가
        enemyData = EnemyManager.GetEnemyManager().GetEnemyData(enemyName);
        enemyDeadEvent += EnemyManager.EnemyDeadUpdateEvent;
        gameObject.AddComponent<EnemyAction>();
    }

    // Update is called once per frame
    void Update()
    {
        if(hp<=0 || isDead)
        {
            isDead = false;
            hp = maxHp;
            hpBar.SetActive(false);
            gameObject.SetActive(false);
            enemyDeadEvent(gameObject.name);
            DataManager.GetDataManager().GainExp(exp);
            GameObject itemTemp = Instantiate(dropItem);
            itemTemp.name = dropItem.name;

            // 적이 있던 위치에 아이템 생성.
            itemTemp.GetComponent<Item>().SetPos(GetComponent<Transform>());
            DataManager.GetDataManager().GainGold(dropGold);

            // 업적 +1
            DataManager.GetDataManager().UpdateAchievementData("EnemyKill", false ,0);
        }

        // hpBar는 계속 적 위치에 따라 이동해야됨
        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0));

        hpBarTransform.position = hpBarPos;
        hpBar.GetComponent<Slider>().value = (float)hp / (float)maxHp;
    }
    public int GetHp()
    {
        return hp;
    }

    public void SetName(string name)
    {
        gameObject.name = name;
        hpBar.name = name + "HpBar";
    }

    public void SetDrop(string dropItemName, int gold)
    {
        string temp = "Prefab\\Weapon\\" + dropItemName;
        dropItem = Resources.Load<GameObject>(temp);
        dropGold = gold;
    }

    public EnemyData GetEnemyData()
    {
        return enemyData;
    }

    public float GetRespawnTime()
    {
        return respawnTime;
    }

    public void SetHp(int hpValue)
    {
        hp += hpValue;
    }

    public int GetAtk()
    {
        return enemyData.atk;
    }
}
