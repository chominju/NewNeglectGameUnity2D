using System.Collections;
using System.Collections.Generic;
using UnityEngine;


delegate void EnemeyUpdateData(string name);

public class EnemyAction : Action
{
    // Start is called before the first frame update
    public bool isChasePlayer;

    Coroutine playerChaseCoroutine;                                             // �̵� �ڷ�ƾ ���� ����

    Transform playerTransform;                                                    // �÷��̾���ġ �޾ƿ���

    float currentAngle;                                                     // ���������� ���ο� ������ ����

    Vector3 endPosition;                    // ���� ������
    Vector3 prePosition;                    // ���� ������
    Vector3 startPosition;                  // ó�� ��ġ(����)

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
        float inputAngleRadians = inputAngleDegrees * Mathf.Deg2Rad;                    // ����Ƽ�� �����ϴ� ��ȯ �����, ȣ���� ��ȯ

        return new Vector3(Mathf.Cos(inputAngleRadians), Mathf.Sin(inputAngleRadians), 0);  //  ��ȯ�� ȣ���� ����Ͽ� ���� �������� ����� ���� ���͸� ����.
    }


    public override IEnumerator Move()
    {
        float remainingDistance = (transform.position - endPosition).sqrMagnitude;                              // ���� ����� Vector3. Vector3�� �ִ� sqrMagnitude��� �Ӽ��� ����ؼ� ���� ���� ��ġ�� ������ ������ �뷫���� �Ÿ��� ����.(������ ũ��)

        while (remainingDistance > float.Epsilon + 1.0f)                                                                // ������ġ�� endPosition���̿� �����Ÿ��� 0���� ū�� Ȯ��
        {
            if (playerTransform != null)                                                                           // �������̶��
            {
                endPosition = playerTransform.position;                                                         // endPosition�� ���� ���� targetTransform���� �����.(���ο� ��ġ�� ��� �ٲ�.)
            }   

            if (rb2D != null)                                                                         // ������ٵ� ���� Ȯ��
            {

                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, endPosition, moveSpeed * Time.deltaTime);   // MoveTowards ������ٵ� 2D�� �������� ���.
                /*
                 * MoveTowards(���� ��ġ, ������ġ, �����Ӿȿ� �̵��� �Ÿ�)
                 * ���⼭ speed�� ���¿� ���� �ٲ��.
                 */

                rb2D.MovePosition(newPosition);                                                                 // �̵��ϱ�

                remainingDistance = (transform.position - endPosition).sqrMagnitude;                            // �����Ÿ��� �� �� �ִ�

                if (prePosition != endPosition)
                {
                    animator.enabled = true;
                    animator.SetBool("isWalk", true);                                                            // �ȴ� ���·� ��ȯ(idle -> walking)
                    animator.speed = enemyCompoenet.GetEnemyData().animationSpeed;
                    if (this.GetComponent<Transform>().position.x >= endPosition.x)
                    {
                        animator.SetFloat("DirX", 0);
                        //Debug.Log(charaterData.CharaterName  + "���� �̵� ");
                    }
                    else
                    {
                        animator.SetFloat("DirX", 1);
                        //Debug.Log(charaterData.CharaterName + "������ �̵� ");

                    }
                    prePosition = endPosition;
                    //Debug.Log(enemyData.enemyName + "������ �޾ƿ� ");
                }

            }
            yield return new WaitForFixedUpdate();                                                              // ���� FixedUpdate���� ������ �纸.
        }

        animator.SetBool("isWalk", false);                                                                   // �������� ������(���� �÷��̾��� �Ÿ��� 0���� �۾���) �����·� ��ȯ
        animator.enabled = false;
        //Debug.Log(enemyData.enemyName + "������ ���� ");
    }

    private void OnDrawGizmos()
    {
        if (circleCollider2D != null)                                                                                // null���� üũ
        {
            Gizmos.DrawWireSphere(transform.position, circleCollider2D.radius);                                   // Gizmos.DrawWireSphere()�� ȣ���ϰ� ���� �׸� �� �ʿ��� ��ġ�� �������� �����Ѵ�.
        }

        if (sensorCollider2D != null)                                                                                // null���� üũ
        {
            if (isTriggerPlayer)
                Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, sensorCollider2D.radius);                                   // Gizmos.DrawWireSphere()�� ȣ���ϰ� ���� �׸� �� �ʿ��� ��ġ�� �������� �����Ѵ�.

        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log(enemyData.enemyName + "�÷��̾�� Ʈ���� ���� ");
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
