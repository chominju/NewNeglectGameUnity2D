using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
using UnityEngine.UI;

delegate void UpdateDataEnemy(string name);
delegate void UpdateDataExp(int exp);
public class Enemy : Character
{
    // Start is called before the first frame update
    public string enemyName;
    EnemyData enemyData;

    //public bool isFollowPlayer;

    public GameObject hpBarPrefab;
    public GameObject hpBar;
    public GameObject canvas;
    RectTransform hpBarTransform;
    public float height;

    int hp;
    int maxHp;
    int exp;
    float respawnTime;

    public GameObject dropItem;
    public int dropGold;

    private event UpdateDataEnemy enemyDeadEvent;

    //Coroutine moveCoroutine;                                                    // �̵� �ڷ�ƾ ���� ����
    //Coroutine playerChaseCoroutine;                                             // �̵� �ڷ�ƾ ���� ����

    //Rigidbody2D rb2D;                                                           // ������ٵ�
    //Animator animator;                                                          // �ִϸ�����

    //Transform playerTransform;                                                    // �÷��̾���ġ �޾ƿ���

    //float currentAngle;                                                     // ���������� ���ο� ������ ����

    //Vector3 endPosition;                    // ���� ������
    //Vector3 prePosition;                    // ���� ������
    //Vector3 startPosition;                  // ó�� ��ġ(����)

    //CircleCollider2D circleCollider2D;
    //CircleCollider2D sensorCollider2D;

    //bool isTriggerPlayer;

    private void Awake()
    {
        SetEnemyData();
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
    void Start()
    {
        InitValue();
        InitComponent();
        //StartCoroutine(Wander());
        InitHpBar();
    }

    void InitHpBar()
    {
        if (canvas == null)
            canvas = GameObject.Find("EnemyCanvas");
        if (hpBar == null)
        {
            hpBar = Instantiate(hpBarPrefab, canvas.transform);
            hpBar.name = gameObject.name + "hpBar";
            hpBarTransform = hpBar.GetComponent<RectTransform>();
            hpBarTransform.localScale = new Vector3(0.2f, 0.2f, 1);
        }
        hpBar.SetActive(true);
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        InitValue();
        InitComponent();
        InitHpBar();
       // StartCoroutine(Wander());
    }

    void InitValue()
    {
        //isTriggerPlayer = false;
        //currentAngle = 0;
        //this.transform.position = startPosition;
        hp = enemyData.hp;
        maxHp = enemyData.maxHp;
        respawnTime = 15.0f;
        height = 0.7f;
        isDead = false;
    }

    void InitComponent()
    {
        //if (rb2D == null)
        //    rb2D = GetComponent<Rigidbody2D>();
        //if (animator == null)
        //    animator = GetComponent<Animator>();
        //if (circleCollider2D == null)
        //    circleCollider2D = GetComponent<CircleCollider2D>();
        //if (sensorCollider2D == null)
        //    sensorCollider2D = transform.Find(enemyData.enemyName + "Sensor").GetComponentInChildren<CircleCollider2D>();
    }

    public void SetEnemyData()
    {
        enemyData = EnemyManager.GetEnemyManager().GetEnemyData(enemyName);
        //enemyData = AssetDatabase.LoadAssetAtPath(DataManager.GetDataManager().GetSaveDataPath() + enemyName + ".asset", typeof(EnemyData)) as EnemyData;
        maxHp = enemyData.maxHp;
        hp = maxHp;
        exp= enemyData.exp;

        enemyDeadEvent += EnemyManager.EnemyDeadUpdateEvent;

        gameObject.AddComponent<EnemyAction>();
    }

    //public void SetStartPos(Vector2 startPos)
    //{
    //    startPosition = startPos;

    //    this.transform.position = startPosition;
    //}

    // Update is called once per frame
    void Update()
    {
        if(hp<=0 || isDead)
        {
            isDead = false;
            hp = maxHp;
            hpBar.SetActive(false);
            gameObject.SetActive(false);
            // Debug.Log("Get Exp : " + gameObject.name);
            enemyDeadEvent(gameObject.name);
            //GameObject.Find("NeglectGameManager").SendMessage("EnemyDead", gameObject.name);
            DataManager.GetDataManager().GainExp(exp);
            //GameObject.Find("Player").SendMessage("ExpGain", exp);
            GameObject itemTemp = Instantiate(dropItem);
            itemTemp.name = dropItem.name;
            itemTemp.GetComponent<Item>().SetPos(GetComponent<Transform>());
            DataManager.GetDataManager().GainGold(dropGold);
            DataManager.GetDataManager().UpdateAchievementData("EnemyKill", false ,0);
        }

        Vector3 hpBarPos = Camera.main.WorldToScreenPoint(new Vector3(transform.position.x, transform.position.y + height, 0));

        hpBarTransform.position = hpBarPos;
        hpBar.GetComponent<Slider>().value = (float)hp / (float)maxHp;


        //if (hp < 0 && gameObject.activeSelf)
        //Debug.Log("[" + gameObject.name + "]" + "hp : " + hp + "not delete");
    }

    //public override IEnumerator DamageCharacter(int damage, float interval)
    //{
    //    while (true)
    //    {
    //       // StartCoroutine(ChanageColorCharacter(interval/2.0f));
    //        if (hp <= 0)
    //        {
    //            isDead = true;
    //            //gameObject.SetActive(false);
    //            //GameObject.Find("Manager").SendMessage("EnemyDead", gameObject.name);
    //        }
    //        else
    //        {
    //            //Debug.Log("[" + gameObject.name + "]hp/max:" + hp + "/" + maxHp);
    //            hp -= damage;
    //        }
    //        yield return new WaitForSeconds(interval);
    //    }
    //}
    //public IEnumerator Respawn()
    //{
    //    yield return new WaitForSeconds(respawnTime);
    //    gameObject.SetActive(true);
    //    //Debug.Log("[" + gameObject.name + "]respawn");
    //    GameObject.Find("Manager").SendMessage("EnemyRespawnEnd", gameObject.name);
    //}


    //public override IEnumerator Wander()
    //{
    //    while(true)
    //    {
    //        GetWanderPos();

    //        if (moveCoroutine != null)
    //            StopCoroutine(moveCoroutine);

    //        moveCoroutine = StartCoroutine(Move(rb2D, enemyData.speed));

    //        yield return new WaitForSeconds(1.0f);
    //    }
    //}



    //    void GetWanderPos()
    //{
    //    currentAngle += R
    //    m.Range(0, 360);

    //    currentAngle = Mathf.Repeat(currentAngle, 360);

    //    endPosition+= Vector3FromAngle(currentAngle);
    //}

    //Vector3 Vector3FromAngle(float inputAngleDegrees)
    //{
    //    float inputAngleRadians = inputAngleDegrees * Mathf.Deg2Rad;                    // ����Ƽ�� �����ϴ� ��ȯ �����, ȣ���� ��ȯ

    //    return new Vector3(Mathf.Cos(inputAngleRadians), Mathf.Sin(inputAngleRadians), 0);  //  ��ȯ�� ȣ���� ����Ͽ� ���� �������� ����� ���� ���͸� ����.
    //}


    //public IEnumerator Move(Rigidbody2D rigidBodyToMove, float speed)
    //{
    //    float remainingDistance = (transform.position - endPosition).sqrMagnitude;                              // ���� ����� Vector3. Vector3�� �ִ� sqrMagnitude��� �Ӽ��� ����ؼ� ���� ���� ��ġ�� ������ ������ �뷫���� �Ÿ��� ����.(������ ũ��)

    //    while (remainingDistance > float.Epsilon + 1.0f)                                                                // ������ġ�� endPosition���̿� �����Ÿ��� 0���� ū�� Ȯ��
    //    {
    //        if (playerTransform != null)                                                                           // �������̶��
    //        {
    //            endPosition = playerTransform.position;                                                         // endPosition�� ���� ���� targetTransform���� �����.(���ο� ��ġ�� ��� �ٲ�.)
    //        }

    //        if (rigidBodyToMove != null)                                                                         // ������ٵ� ���� Ȯ��
    //        {

    //            Vector3 newPosition = Vector3.MoveTowards(rigidBodyToMove.position, endPosition, speed * Time.deltaTime);   // MoveTowards ������ٵ� 2D�� �������� ���.
    //            /*
    //             * MoveTowards(���� ��ġ, ������ġ, �����Ӿȿ� �̵��� �Ÿ�)
    //             * ���⼭ speed�� ���¿� ���� �ٲ��.
    //             */

    //            rb2D.MovePosition(newPosition);                                                                 // �̵��ϱ�

    //            remainingDistance = (transform.position - endPosition).sqrMagnitude;                            // �����Ÿ��� �� �� �ִ�

    //            if(prePosition != endPosition)
    //            {
    //                animator.enabled = true;
    //                animator.SetBool("isWalk", true);                                                            // �ȴ� ���·� ��ȯ(idle -> walking)
    //                animator.speed = enemyData.animationSpeed;
    //                if (this.GetComponent<Transform>().position.x >= endPosition.x)
    //                {
    //                    animator.SetFloat("DirX", 0);
    //                    //Debug.Log(charaterData.CharaterName  + "���� �̵� ");
    //                }
    //                else
    //                {
    //                    animator.SetFloat("DirX", 1);
    //                    //Debug.Log(charaterData.CharaterName + "������ �̵� ");

    //                }
    //                prePosition = endPosition;
    //                //Debug.Log(enemyData.enemyName + "������ �޾ƿ� ");
    //            }
                
    //        }
    //        yield return new WaitForFixedUpdate();                                                              // ���� FixedUpdate���� ������ �纸.
    //    }

    //    animator.SetBool("isWalk", false);                                                                   // �������� ������(���� �÷��̾��� �Ÿ��� 0���� �۾���) �����·� ��ȯ
    //    animator.enabled= false;
    //    //Debug.Log(enemyData.enemyName + "������ ���� ");
    //}

    //private void OnDrawGizmos()
    //{
    //    if (circleCollider2D != null)                                                                                // null���� üũ
    //    {
    //        Gizmos.DrawWireSphere(transform.position, circleCollider2D.radius);                                   // Gizmos.DrawWireSphere()�� ȣ���ϰ� ���� �׸� �� �ʿ��� ��ġ�� �������� �����Ѵ�.
    //    }

    //    if (sensorCollider2D != null)                                                                                // null���� üũ
    //    {
    //        if (isTriggerPlayer)
    //            Gizmos.color = Color.red;
    //       Gizmos.DrawWireSphere(transform.position, sensorCollider2D.radius);                                   // Gizmos.DrawWireSphere()�� ȣ���ϰ� ���� �׸� �� �ʿ��� ��ġ�� �������� �����Ѵ�.
            
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{

    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        //Debug.Log(enemyData.enemyName + "�÷��̾�� Ʈ���� ���� ");
    //        isTriggerPlayer = true;

    //        GetPlayerTransform(collision.gameObject);

    //        //if (moveCoroutine != null)                                                                          // moveCoroutine�� null�� �ƴϸ� ���� ������. 
    //        //    StopCoroutine(moveCoroutine);                                                                   // �ٽ� �����̱� ���� �ϱ� ���� �������.

    //        //// playerChaseCoroutine = StartCoroutine(GetPlayerTransform(collision.gameObject));
    //        //moveCoroutine = StartCoroutine(Move(rb2d, charaterData.speed));
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        //Debug.Log(enemyData.name + "�÷��̾�� Ʈ���� ���� ");
    //        isTriggerPlayer = false;
    //        playerTransform = null;
    //        //if (playerChaseCoroutine != null)                                                                          // moveCoroutine�� null�� �ƴϸ� ���� ������. 
    //        //    StopCoroutine(playerChaseCoroutine);

    //        //if (moveCoroutine != null)
    //        //{
    //        //    StopCoroutine(moveCoroutine);                                                                   // ���� ������ ���߰� �ϰ� �����Ƿ� moveCoroutine�� ����
    //        //}

    //        //Gizmos.color = Color.white;
    //        //Gizmos.DrawWireSphere(transform.position, sensorCollider.radius);
    //        //playerTransform = null;
    //    }
    //}

    //public void GetPlayerTransform(GameObject player)
    //{
    //    playerTransform = player.GetComponent<Transform>();
    //    endPosition = playerTransform.position;

    //    //Debug.Log(enemyData.name + "�÷��̾� ��ġ �޾ƿ� ");
    //    //yield return new WaitForSeconds(1.0f);


    //}
}
