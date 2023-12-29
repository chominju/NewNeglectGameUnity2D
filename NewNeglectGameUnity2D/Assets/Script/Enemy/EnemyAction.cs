using System.Collections;
using System.Collections.Generic;
using UnityEngine;


delegate void EnemeyUpdateData(string name);

public class EnemyAction : Action
{
    // Start is called before the first frame update
    public bool isChasePlayer;

    Coroutine playerChaseCoroutine;                                             // 이동 코루틴 참조 저장

    Transform playerTransform;                                                    // 플레이어위치 받아오기

    float currentAngle;                                                     // 기존각도에 새로운 각도를 더함

    Vector3 endPosition;                    // 최종 목적지
    Vector3 prePosition;                    // 이전 목적지
    Vector3 startPosition;                  // 처음 위치(고정)

    CircleCollider2D circleCollider2D;
    CircleCollider2D sensorCollider2D;

    bool isTriggerPlayer;

    Enemy enemyCompoenet;

    public GameObject canvas;
    GameObject player;

    private event EnemeyUpdateData enemyRespawnEvent;

    GameObject damagePrefab;

    void Start()
    {

        StopAllCoroutines();
        InitValue();
        InitComponent();
        StartCoroutine(CharacterAction());
        enemyCompoenet = GetComponent<Enemy>();
        player = GameObject.Find("Player");
        enemyRespawnEvent += EnemyManager.EnemyUpdateEvent;

        damagePrefab = Resources.Load<GameObject>("Prefab/DamageObject");
    }

    

    private void OnEnable()
    {
        StopAllCoroutines();
        InitValue();
        InitComponent();
        StartCoroutine(CharacterAction());
    }

    void InitValue()
    {
        moveSpeed = GetComponent<Enemy>().GetEnemyData().speed;
        atk = GetComponent<Enemy>().GetEnemyData().atk;
        isTriggerPlayer = false;
        currentAngle = 0;
        this.transform.position = startPosition;
    }

    void InitComponent()
    {
        if(enemyCompoenet == null)
            enemyCompoenet = GetComponent<Enemy>();
        if (rb2D == null)
            rb2D = GetComponent<Rigidbody2D>();
        if (animator == null)
            animator = GetComponent<Animator>();
        if (circleCollider2D == null)
            circleCollider2D = GetComponent<CircleCollider2D>();
        if (sensorCollider2D == null)
            sensorCollider2D = transform.Find(enemyCompoenet.GetEnemyData().enemyName + "Sensor").GetComponentInChildren<CircleCollider2D>();
    }

    public void SetStartPos(Vector2 startPos)
    {
        startPosition = startPos;

        this.transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public  override IEnumerator CharacterDamage(int damage, float interval)
    {
        while (true)
        {
            damage = player.GetComponent<Player>().playerInfo.atk;
            // StartCoroutine(ChanageColorCharacter(interval/2.0f));
            if (enemyCompoenet.GetHp() <= 0)
            {
                enemyCompoenet.SetDead(true);
            }
            else
            {
                enemyCompoenet.SetHp(-damage);
            }
            yield return new WaitForSeconds(interval);
        }
    }

    public void SkillDamage(int damage)
    {
        enemyCompoenet.SetHp(-damage);

        if (canvas == null)
            canvas = GameObject.Find("EnemyCanvas");

        GameObject damageObject = Instantiate(damagePrefab, canvas.transform);
        damageObject.GetComponent<Damage>().SetDamagePos(transform.position);
        damageObject.GetComponent<Damage>().SetDamage(damage, false);  
        //DamageUIManager.CreateNGUILabel(transform.position, damage,false);
    }

    public void PlayerDamage(int damage)
    {

        enemyCompoenet.SetHp(-damage);


        if (canvas == null)
            canvas = GameObject.Find("EnemyCanvas");

        GameObject damageObject = Instantiate(damagePrefab , canvas.transform);
        damageObject.GetComponent<Damage>().SetDamagePos(transform.position);
        damageObject.GetComponent<Damage>().SetDamage(damage,true);
        //DamageUIManager.CreateNGUILabel(transform.position, damage, true);
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(enemyCompoenet.GetRespawnTime());
        gameObject.SetActive(true);
        enemyRespawnEvent(gameObject.name);
        //GameObject.Find("NeglectGameManager").SendMessage("EnemyRespawnEnd", gameObject.name);
    }


    public override IEnumerator CharacterAction()
    {
        while (true)
        {
            //Debug.Log("Enemy " + enemyCompoenet.GetEnemyData().enemyName);
            GetWanderPos();

            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);

            moveCoroutine = StartCoroutine(Move());

            yield return new WaitForSeconds(1.0f);
        }
    }



    void GetWanderPos()
    {
        currentAngle += Random.Range(0, 360);

        currentAngle = Mathf.Repeat(currentAngle, 360);

        endPosition += Vector3FromAngle(currentAngle);
    }

    Vector3 Vector3FromAngle(float inputAngleDegrees)
    {
        float inputAngleRadians = inputAngleDegrees * Mathf.Deg2Rad;                    // 유니티가 제공하는 변환 상수로, 호도로 변환

        return new Vector3(Mathf.Cos(inputAngleRadians), Mathf.Sin(inputAngleRadians), 0);  //  변환한 호도를 사용하여 적의 방향으로 사용할 방향 벡터를 만듬.
    }


    public override IEnumerator Move()
    {
        float remainingDistance = (transform.position - endPosition).sqrMagnitude;                              // 값의 결과는 Vector3. Vector3에 있는 sqrMagnitude라는 속성을 사용해서 적의 현재 위치와 목적지 사이의 대략적인 거리를 구함.(벡터의 크기)

        while (remainingDistance > float.Epsilon + 1.0f)                                                                // 현재위치와 endPosition사이에 남은거리가 0보다 큰지 확인
        {
            if (playerTransform != null)                                                                           // 추적중이라면
            {
                endPosition = playerTransform.position;                                                         // endPosition의 원래 값을 targetTransform르오 덮어쓴다.(새로운 위치로 계속 바뀜.)
            }   

            if (rb2D != null)                                                                         // 리지드바디 유무 확인
            {

                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, endPosition, moveSpeed * Time.deltaTime);   // MoveTowards 리지드바디 2D의 움직임을 계산.
                /*
                 * MoveTowards(현재 위치, 최종위치, 프레임안에 이동할 거리)
                 * 여기서 speed는 상태에 따라 바뀐다.
                 */

                rb2D.MovePosition(newPosition);                                                                 // 이동하기

                remainingDistance = (transform.position - endPosition).sqrMagnitude;                            // 남은거리를 알 수 있다

                if (prePosition != endPosition)
                {
                    animator.enabled = true;
                    animator.SetBool("isWalk", true);                                                            // 걷는 상태로 변환(idle -> walking)
                    animator.speed = enemyCompoenet.GetEnemyData().animationSpeed;
                    if (this.GetComponent<Transform>().position.x >= endPosition.x)
                    {
                        animator.SetFloat("DirX", 0);
                        //Debug.Log(charaterData.CharaterName  + "왼쪽 이동 ");
                    }
                    else
                    {
                        animator.SetFloat("DirX", 1);
                        //Debug.Log(charaterData.CharaterName + "오른쪽 이동 ");

                    }
                    prePosition = endPosition;
                    //Debug.Log(enemyData.enemyName + "목적지 받아옴 ");
                }

            }
            yield return new WaitForFixedUpdate();                                                              // 다음 FixedUpdate까지 실행을 양보.
        }

        animator.SetBool("isWalk", false);                                                                   // 목적지에 도착함(적과 플레이어의 거리가 0보다 작아짐) 대기상태로 변환
        animator.enabled = false;
        //Debug.Log(enemyData.enemyName + "목적지 도착 ");
    }

    private void OnDrawGizmos()
    {
        if (circleCollider2D != null)                                                                                // null인지 체크
        {
            Gizmos.DrawWireSphere(transform.position, circleCollider2D.radius);                                   // Gizmos.DrawWireSphere()를 호출하고 구를 그릴 때 필요한 위치와 반지름을 전달한다.
        }

        if (sensorCollider2D != null)                                                                                // null인지 체크
        {
            if (isTriggerPlayer)
                Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, sensorCollider2D.radius);                                   // Gizmos.DrawWireSphere()를 호출하고 구를 그릴 때 필요한 위치와 반지름을 전달한다.

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log(enemyData.enemyName + "플레이어와 트리거 시작 ");
            isTriggerPlayer = true;
            SetTarget();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            isTriggerPlayer = false;
            playerTransform = null;
        }
    }

    public override void SetTarget()
    {
        target =  GameObject.Find("Player");
        playerTransform = target.GetComponent<Transform>();
        endPosition = playerTransform.position;
    }

    public override IEnumerator AttackToTarget()
    {
        yield return new WaitForSeconds(animator.speed);
    }

}
