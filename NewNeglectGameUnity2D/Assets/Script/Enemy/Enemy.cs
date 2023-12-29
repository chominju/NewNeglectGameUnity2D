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

    //Coroutine moveCoroutine;                                                    // 이동 코루틴 참조 저장
    //Coroutine playerChaseCoroutine;                                             // 이동 코루틴 참조 저장

    //Rigidbody2D rb2D;                                                           // 리지드바디
    //Animator animator;                                                          // 애니메이터

    //Transform playerTransform;                                                    // 플레이어위치 받아오기

    //float currentAngle;                                                     // 기존각도에 새로운 각도를 더함

    //Vector3 endPosition;                    // 최종 목적지
    //Vector3 prePosition;                    // 이전 목적지
    //Vector3 startPosition;                  // 처음 위치(고정)

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
    //    float inputAngleRadians = inputAngleDegrees * Mathf.Deg2Rad;                    // 유니티가 제공하는 변환 상수로, 호도로 변환

    //    return new Vector3(Mathf.Cos(inputAngleRadians), Mathf.Sin(inputAngleRadians), 0);  //  변환한 호도를 사용하여 적의 방향으로 사용할 방향 벡터를 만듬.
    //}


    //public IEnumerator Move(Rigidbody2D rigidBodyToMove, float speed)
    //{
    //    float remainingDistance = (transform.position - endPosition).sqrMagnitude;                              // 값의 결과는 Vector3. Vector3에 있는 sqrMagnitude라는 속성을 사용해서 적의 현재 위치와 목적지 사이의 대략적인 거리를 구함.(벡터의 크기)

    //    while (remainingDistance > float.Epsilon + 1.0f)                                                                // 현재위치와 endPosition사이에 남은거리가 0보다 큰지 확인
    //    {
    //        if (playerTransform != null)                                                                           // 추적중이라면
    //        {
    //            endPosition = playerTransform.position;                                                         // endPosition의 원래 값을 targetTransform르오 덮어쓴다.(새로운 위치로 계속 바뀜.)
    //        }

    //        if (rigidBodyToMove != null)                                                                         // 리지드바디 유무 확인
    //        {

    //            Vector3 newPosition = Vector3.MoveTowards(rigidBodyToMove.position, endPosition, speed * Time.deltaTime);   // MoveTowards 리지드바디 2D의 움직임을 계산.
    //            /*
    //             * MoveTowards(현재 위치, 최종위치, 프레임안에 이동할 거리)
    //             * 여기서 speed는 상태에 따라 바뀐다.
    //             */

    //            rb2D.MovePosition(newPosition);                                                                 // 이동하기

    //            remainingDistance = (transform.position - endPosition).sqrMagnitude;                            // 남은거리를 알 수 있다

    //            if(prePosition != endPosition)
    //            {
    //                animator.enabled = true;
    //                animator.SetBool("isWalk", true);                                                            // 걷는 상태로 변환(idle -> walking)
    //                animator.speed = enemyData.animationSpeed;
    //                if (this.GetComponent<Transform>().position.x >= endPosition.x)
    //                {
    //                    animator.SetFloat("DirX", 0);
    //                    //Debug.Log(charaterData.CharaterName  + "왼쪽 이동 ");
    //                }
    //                else
    //                {
    //                    animator.SetFloat("DirX", 1);
    //                    //Debug.Log(charaterData.CharaterName + "오른쪽 이동 ");

    //                }
    //                prePosition = endPosition;
    //                //Debug.Log(enemyData.enemyName + "목적지 받아옴 ");
    //            }
                
    //        }
    //        yield return new WaitForFixedUpdate();                                                              // 다음 FixedUpdate까지 실행을 양보.
    //    }

    //    animator.SetBool("isWalk", false);                                                                   // 목적지에 도착함(적과 플레이어의 거리가 0보다 작아짐) 대기상태로 변환
    //    animator.enabled= false;
    //    //Debug.Log(enemyData.enemyName + "목적지 도착 ");
    //}

    //private void OnDrawGizmos()
    //{
    //    if (circleCollider2D != null)                                                                                // null인지 체크
    //    {
    //        Gizmos.DrawWireSphere(transform.position, circleCollider2D.radius);                                   // Gizmos.DrawWireSphere()를 호출하고 구를 그릴 때 필요한 위치와 반지름을 전달한다.
    //    }

    //    if (sensorCollider2D != null)                                                                                // null인지 체크
    //    {
    //        if (isTriggerPlayer)
    //            Gizmos.color = Color.red;
    //       Gizmos.DrawWireSphere(transform.position, sensorCollider2D.radius);                                   // Gizmos.DrawWireSphere()를 호출하고 구를 그릴 때 필요한 위치와 반지름을 전달한다.
            
    //    }
    //}

    //private void OnTriggerEnter2D(Collider2D collision)
    //{

    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        //Debug.Log(enemyData.enemyName + "플레이어와 트리거 시작 ");
    //        isTriggerPlayer = true;

    //        GetPlayerTransform(collision.gameObject);

    //        //if (moveCoroutine != null)                                                                          // moveCoroutine이 null이 아니면 적이 움직임. 
    //        //    StopCoroutine(moveCoroutine);                                                                   // 다시 움직이기 시작 하기 전에 멈춰야함.

    //        //// playerChaseCoroutine = StartCoroutine(GetPlayerTransform(collision.gameObject));
    //        //moveCoroutine = StartCoroutine(Move(rb2d, charaterData.speed));
    //    }
    //}

    //private void OnTriggerExit2D(Collider2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        //Debug.Log(enemyData.name + "플레이어와 트리거 종료 ");
    //        isTriggerPlayer = false;
    //        playerTransform = null;
    //        //if (playerChaseCoroutine != null)                                                                          // moveCoroutine이 null이 아니면 적이 움직임. 
    //        //    StopCoroutine(playerChaseCoroutine);

    //        //if (moveCoroutine != null)
    //        //{
    //        //    StopCoroutine(moveCoroutine);                                                                   // 적의 추적을 멈추게 하고 싶으므로 moveCoroutine을 중지
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

    //    //Debug.Log(enemyData.name + "플레이어 위치 받아옴 ");
    //    //yield return new WaitForSeconds(1.0f);


    //}
}
