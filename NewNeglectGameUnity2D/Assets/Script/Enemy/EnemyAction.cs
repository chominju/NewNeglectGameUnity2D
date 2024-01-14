using System.Collections;
using System.Collections.Generic;
using UnityEngine;


delegate void UpdateDataEnemyAction(string name);

public class EnemyAction : Action
{
    Transform playerTransform;                                              // 플레이어위치 받아오기

    float currentAngle;                                                     // 기존각도에 새로운 각도를 더함

    Vector3 endPosition;                                                    // 최종 목적지
    Vector3 prePosition;                                                    // 이전 목적지
    Vector3 startPosition;                                                  // 처음 위치(고정)

    CircleCollider2D circleCollider2D;                                      // 피격범위 콜라이더
    CircleCollider2D sensorCollider2D;                                      // 센서(플레이어 감지) 콜라이더

    bool isTriggerPlayer;                                                   // 플레이어랑 충돌했는가

    Enemy enemyCompoenet;                                                   // Enemy 컴포넌트

    GameObject player;                                                      // 플레이어 오브젝트
    GameObject damagePrefab;                                                // 데미지 프리팹
    public GameObject canvasDamageObject;                                   // 데미지 띄우려는 캔버스

    private event UpdateDataEnemyAction enemyRespawnEvent;                  // 리스폰관련 delegate


    void Start()
    {
        StopAllCoroutines();
        InitValue();
        InitComponent();
        StartCoroutine(CharacterAction());
        enemyCompoenet = GetComponent<Enemy>();
        player = GameObject.Find("Player");
        enemyRespawnEvent += EnemyManager.EnemyRespawnEndEvent;

        damagePrefab = Resources.Load<GameObject>("Prefab/DamageObject");
    }

    

    private void OnEnable()
    {
        // 활성화 될때마다 실행
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

        // 리스폰 위치로 조정
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

    public void SkillDamage(int damage)
    {
        // 스킬에 의해 데미지를 받은 경우
        enemyCompoenet.SetHp(-damage);

        if (canvasDamageObject == null)
            canvasDamageObject = GameObject.Find("EnemyCanvas");

        GameObject damageObject = Instantiate(damagePrefab, canvasDamageObject.transform);
        damageObject.GetComponent<Damage>().SetDamagePos(transform.position);
        damageObject.GetComponent<Damage>().SetDamage(damage, false);  
    }

    public void PlayerDamage(int damage)
    {
        enemyCompoenet.SetHp(-damage);

        if (canvasDamageObject == null)
            canvasDamageObject = GameObject.Find("EnemyCanvas");

        GameObject damageObject = Instantiate(damagePrefab , canvasDamageObject.transform);
        damageObject.GetComponent<Damage>().SetDamagePos(transform.position);
        damageObject.GetComponent<Damage>().SetDamage(damage,true);
    }

    public IEnumerator Respawn()
    {
        yield return new WaitForSeconds(enemyCompoenet.GetRespawnTime());
        gameObject.SetActive(true);
        enemyRespawnEvent(gameObject.name);
    }


    public override IEnumerator CharacterAction()
    {
        while (true)
        {
            // 배회하는 함수
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

        // 360보다 커지는걸 방지
        currentAngle = currentAngle % 360;

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
                endPosition = playerTransform.position;                                                         // endPosition의 원래 값을 targetTransform르오 덮어쓴다.(새로운 위치로 계속 바뀜)
            }   

            if (rb2D != null)                                                                         // 리지드바디 유무 확인
            {

                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, endPosition, moveSpeed * Time.deltaTime);   // MoveTowards 리지드바디 2D의 움직임을 계산.
                /*
                 * MoveTowards(현재 위치, 최종위치, 프레임안에 이동할 거리)
                 * 여기서 speed는 상태에 따라 바뀐다.
                 */

                rb2D.MovePosition(newPosition);                                                                 // 이동하기

                remainingDistance = Vector3.Distance(transform.position, endPosition);                            // 남은거리

                if (prePosition != endPosition)
                {
                    animator.enabled = true;
                    animator.SetBool("isWalk", true);                                                            // 걷는 상태로 변환(idle -> walking)
                    animator.speed = enemyCompoenet.GetEnemyData().animationSpeed;
                    if (this.GetComponent<Transform>().position.x >= endPosition.x)
                    {
                        animator.SetFloat("DirX", 0);
                    }
                    else
                    {
                        animator.SetFloat("DirX", 1);

                    }
                    prePosition = endPosition;
                }

            }
            yield return new WaitForFixedUpdate();                                                              // 다음 FixedUpdate까지 실행을 양보.
        }

        animator.SetBool("isWalk", false);                                                                   // 목적지에 도착함(적과 플레이어의 거리가 0보다 작아짐) 대기상태로 변환
        animator.enabled = false;
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
        // 센서와 플레이어와 충돌
        if (collision.gameObject.CompareTag("Player"))
        {
            isTriggerPlayer = true;
            // 플레이어 위치 가져오기
            SetTarget();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // 플레이어와 충돌 해제
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

}
