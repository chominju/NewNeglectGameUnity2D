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
        // ����Ƽ�� �����ϴ� ��ȯ �����, ȣ���� ��ȯ
        float inputAngleRadians = inputAngleDegrees * Mathf.Deg2Rad;                    

        //  ��ȯ�� ȣ���� ����Ͽ� ���� �������� ����� ���� ���͸� ����.
        return new Vector3(Mathf.Cos(inputAngleRadians), Mathf.Sin(inputAngleRadians), 0);  
    }


    public override IEnumerator Move()
    {
        // ���� ����� Vector3. Vector3�� �ִ� sqrMagnitude��� �Ӽ��� ����ؼ� ���� ���� ��ġ�� ������ ������ �뷫���� �Ÿ��� ����.(������ ũ��)
        // ������ ���� ���Ҷ��� Vector3.Distance
        // 2D ������ ����ų� �ܼ��ϰ� �� ������ �Ÿ��� ���Ҷ��� sqrMagnitude

        float remainingDistance = (transform.position - endPosition).sqrMagnitude;                              

        // ������ġ�� endPosition���̿� �����Ÿ��� 0���� ū�� Ȯ��
        while (remainingDistance > float.Epsilon + 1.0f)                                                                
        {
            // �������̶��
            if (playerTransform != null)                                                                           
            {
                // endPosition�� ���� ���� targetTransform���� �����.(���ο� ��ġ�� ��� �ٲ�)
                endPosition = playerTransform.position;                                                        
            }   

            // ������ٵ� ���� Ȯ��
            if (rb2D != null)                                                                         
            {

                // MoveTowards ������ٵ� 2D�� �������� ���.
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, endPosition, moveSpeed * Time.deltaTime);   
                /*
                 * MoveTowards(���� ��ġ, ������ġ, �����Ӿȿ� �̵��� �Ÿ�)
                 * speed�� ���¿� ���� �ٲ��.
                 */

                // �̵��ϱ�
                rb2D.MovePosition(newPosition);                                                                 

                // �����Ÿ�
                remainingDistance = Vector3.Distance(transform.position, endPosition);                            

                if (prePosition != endPosition)
                {
                    animator.enabled = true;
                    // �ȴ� ���·� ��ȯ(idle -> walking)
                    animator.SetBool("isWalk", true);                                                            
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
            yield return new WaitForFixedUpdate();                                                              
        }

        // �������� ������(���� �÷��̾��� �Ÿ��� 0���� �۾���) �����·� ��ȯ
        animator.SetBool("isWalk", false);                                                                   
        animator.enabled = false;
    }

    private void OnDrawGizmos()
    {
        if (circleCollider2D != null)                                                                                
        {
            // Gizmos.DrawWireSphere()�� ȣ���ϰ� ���� �׸� �� �ʿ��� ��ġ�� �������� �����Ѵ�.
            Gizmos.DrawWireSphere(transform.position, circleCollider2D.radius);                                   
        }

        if (sensorCollider2D != null)                                                                                
        {
            if (isTriggerPlayer)
                Gizmos.color = Color.red;
            // Gizmos.DrawWireSphere()�� ȣ���ϰ� ���� �׸� �� �ʿ��� ��ġ�� �������� �����Ѵ�.
            Gizmos.DrawWireSphere(transform.position, sensorCollider2D.radius);                                   

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

    public override void SetTarget()
    {
        // Ÿ�� ����(�÷��̾�)
        target =  GameObject.Find("Player");
        // �÷��̾��� transform ������Ʈ ��������
        playerTransform = target.GetComponent<Transform>();
        endPosition = playerTransform.position;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        // �÷��̾�� �浹 ����
        if (collision.gameObject.CompareTag("Player"))
        {
            // �÷��̾�� �浹���� �ƴϴ�
            isTriggerPlayer = false;
            // �÷��̾� transform ������Ʈ �� null
            playerTransform = null;
        }
    }

}
