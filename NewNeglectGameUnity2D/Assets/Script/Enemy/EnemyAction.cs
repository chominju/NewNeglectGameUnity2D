using System.Collections;
using System.Collections.Generic;
using UnityEngine;


delegate void UpdateDataEnemyAction(string name);

public class EnemyAction : Action
{
    Transform playerTransform;                                              // �÷��̾���ġ �޾ƿ���

    float currentAngle;                                                     // ���������� ���ο� ������ ����

    Vector3 endPosition;                                                    // ���� ������
    Vector3 prePosition;                                                    // ���� ������
    Vector3 startPosition;                                                  // ó�� ��ġ(����)

    CircleCollider2D circleCollider2D;                                      // �ǰݹ��� �ݶ��̴�
    CircleCollider2D sensorCollider2D;                                      // ����(�÷��̾� ����) �ݶ��̴�

    bool isTriggerPlayer;                                                   // �÷��̾�� �浹�ߴ°�

    Enemy enemyCompoenet;                                                   // Enemy ������Ʈ

    GameObject player;                                                      // �÷��̾� ������Ʈ
    GameObject damagePrefab;                                                // ������ ������
    public GameObject canvasDamageObject;                                   // ������ ������ ĵ����

    private event UpdateDataEnemyAction enemyRespawnEvent;                  // ���������� delegate


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
        // Ȱ��ȭ �ɶ����� ����
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

        // ������ ��ġ�� ����
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
        // ��ų�� ���� �������� ���� ���
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
            // ��ȸ�ϴ� �Լ�
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

        // 360���� Ŀ���°� ����
        currentAngle = currentAngle % 360;

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
                endPosition = playerTransform.position;                                                         // endPosition�� ���� ���� targetTransform���� �����.(���ο� ��ġ�� ��� �ٲ�)
            }   

            if (rb2D != null)                                                                         // ������ٵ� ���� Ȯ��
            {

                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, endPosition, moveSpeed * Time.deltaTime);   // MoveTowards ������ٵ� 2D�� �������� ���.
                /*
                 * MoveTowards(���� ��ġ, ������ġ, �����Ӿȿ� �̵��� �Ÿ�)
                 * ���⼭ speed�� ���¿� ���� �ٲ��.
                 */

                rb2D.MovePosition(newPosition);                                                                 // �̵��ϱ�

                remainingDistance = Vector3.Distance(transform.position, endPosition);                            // �����Ÿ�

                if (prePosition != endPosition)
                {
                    animator.enabled = true;
                    animator.SetBool("isWalk", true);                                                            // �ȴ� ���·� ��ȯ(idle -> walking)
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
            yield return new WaitForFixedUpdate();                                                              // ���� FixedUpdate���� ������ �纸.
        }

        animator.SetBool("isWalk", false);                                                                   // �������� ������(���� �÷��̾��� �Ÿ��� 0���� �۾���) �����·� ��ȯ
        animator.enabled = false;
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
        // ������ �÷��̾�� �浹
        if (collision.gameObject.CompareTag("Player"))
        {
            isTriggerPlayer = true;
            // �÷��̾� ��ġ ��������
            SetTarget();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        // �÷��̾�� �浹 ����
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
