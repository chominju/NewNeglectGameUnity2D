using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;

delegate void UpdateDataEnemy(string name);
public class Enemy : Character
{
    public string enemyName;                    // ���̸�
    EnemyData enemyData;                        // DataManager���� �������� enemyData

    public GameObject hpBarPrefab;              // �� hpBar ������
    public GameObject hpBar;                    // �� hpBar ������ �� ������Ʈ
    public GameObject canvasHpBar;              // �� hpBar���� �ִ� canvas
    RectTransform hpBarTransform;               // �� hpBar�� RectTransform
    public float height;                        // ���� hpBar ������ �Ÿ�
        
    int hp;                                     // �� hp
    int maxHp;                                  // �� �ִ�ü��
    int exp;                                    // ���� �ִ� ����ġ
    float respawnTime;                          // ���� �׾��� �� ������ �ð�

    public GameObject dropItem;                 // ���� ����ϴ� ������
    public int dropGold;                        // ���� ����ϴ� ��差

    private event UpdateDataEnemy enemyDeadEvent;   // EnemyManager�� �ִ� enemyDead(������ ����)

    private void Awake()
    {
        SetEnemyData();
    }

    void Start()
    {
        // �ʱ� ����
        InitEnemyData();
        InitHpBar();
    }
    private void OnEnable()
    {
        // �ٽ� ���������� ��
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
            // hp�� �����ϱ�
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
        // �����ͼ��� ��, delegate ���� , ������Ʈ�߰�
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

            // ���� �ִ� ��ġ�� ������ ����.
            itemTemp.GetComponent<Item>().SetPos(GetComponent<Transform>());
            DataManager.GetDataManager().GainGold(dropGold);

            // ���� +1
            DataManager.GetDataManager().UpdateAchievementData("EnemyKill", false ,0);
        }

        // hpBar�� ��� �� ��ġ�� ���� �̵��ؾߵ�
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
