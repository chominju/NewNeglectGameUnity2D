using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : Action
{

    private GameObject childColiObject;                                         // �÷��̾� �ݶ��̴�(����)
    private GameObject weaponColl;                                              // �÷��̾� ���ݽ� �ݶ��̴�

    private float playerAtkSpeed;
    private Player playerComponent;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = GetComponent<Player>().GetPlayerInfo().moveSpeed;
        atk = GetComponent<Player>().GetPlayerInfo().atk;
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        coli2D = GetComponent<Collider2D>();
        childColiObject = transform.Find("PlayerCollider").gameObject;
        weaponColl = transform.Find("PlayerWeaponCollider").gameObject;
        preDir = DIR.LEFT;
        curDir = preDir;
        curState = STATE.IDLE;

        playerAtkSpeed = animator.speed / (2.0f * animator.speed);
        playerComponent = GetComponent<Player>();

        if (EnemyManager.GetEnemyManager() != null)
        {
            StartCoroutine(CharacterAction());
        }
    }



    public void UpdatePlayerData()
    {
        moveSpeed = GetComponent<Player>().GetPlayerInfo().moveSpeed;
        atk = GetComponent<Player>().GetPlayerInfo().atk;
    }

    public float GetAtkSpeed()
    {
        return playerAtkSpeed;
    }    

    // Update is called once per frame
    void Update()
    {
        //SetAnimation();
        //Action();

        //animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f
    }

    private void FixedUpdate()
    {
        //Action();
    }
    //public float GetAtkSpeed()
    //{
    //    return playerAtkSpeed;
    //}
    //void SetAnimation()
    //{
    //    switch (curState)
    //    {
    //        case STATE.IDLE:
    //            {
    //                Idle();
    //                break;
    //            }
    //        case STATE.WALK:
    //            {
    //                Walk();
    //                break;
    //            }
    //        case STATE.ATTACK:
    //            {
    //                Attack();
    //                //curCoroutine = StartCoroutine(AttackToMonster());
    //                break;
    //            }
    //    }
    //}

    public override IEnumerator CharacterAction()
    {
        while (true)
        {
            //if (target != null)
            //    Debug.Log("Player Target : " + target.name);
            //else
            //    Debug.Log("Player Target No");
            //Debug.Log("���� : " + curState);
            SetAnimation();

            if (!EnemyManager.IsExistEnemy())                              // ���Ͱ� ���ٸ�
            {
                curState = STATE.IDLE;
                //Debug.Log("���Ͱ� ����:");
            }
            else
            {
                //if (target == null || target.activeSelf == false)
                //{
                    //Debug.Log("Ÿ�ϼ��� ��");
                    //if (curCoroutine != null)
                    //    StopCoroutine(curCoroutine);
                    if (curState == STATE.IDLE)
                        SetTarget();
                //}

                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                    moveCoroutine = null;
                }

                if (curState != STATE.ATTACK)
                    moveCoroutine = StartCoroutine(Move());                             // �̵�
                                                        //{
                                                        //    curState = STATE.ATTACK;
                                                        //    curCoroutine = StartCoroutine(AttackToMonster());
                                                        //}
            }
            yield return new WaitForFixedUpdate();// WaitForSeconds(Update);
        }
    }




    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    StartCoroutine(CharacterDamage(collision.gameObject.GetComponent<Enemy>().GetAtk(), 0.3f));
        //}
    }



    public override void SetTarget()
    {
        curState = STATE.WALK;
        //target = null; // Ÿ�� ���͸� ����.
        float distance = float.MaxValue;
        var getEnemyList = EnemyManager.GetEnemyList();
        foreach (var enemyObject in getEnemyList)
        {
            if (enemyObject.Value.activeSelf && enemyObject.Value.GetComponent<Enemy>().GetHp() > 0)
            {
                Vector3 enemyPos = enemyObject.Value.GetComponent<Transform>().position;
                float newDistance = (transform.position - enemyPos).sqrMagnitude;
                if (newDistance.CompareTo(distance) < 0)
                {
                    distance = newDistance;
                    target = enemyObject.Value;
                }
            }
        }
    }

    public GameObject getTarget()
    {
        return target;
    }

    public override IEnumerator Move()
    {
        Vector3 playerPos = this.GetComponent<Transform>().position;
        Vector3 TargetPos = target.GetComponent<Transform>().position;

        float dist = (playerPos - TargetPos).sqrMagnitude; // �Ÿ��� ũ�⸦ ����

        if (playerPos.x >= TargetPos.x)
            curDir = DIR.LEFT;
        else
            curDir = DIR.RIGHT;


        if (preDir != curDir)
        {
            if (curDir == DIR.LEFT)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                Vector3 tempPos = transform.localPosition;
                tempPos.x += 0.7f;

                transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else
            {

                transform.eulerAngles = new Vector3(0, 180, 0);

                Vector3 tempPos = transform.localPosition;
                tempPos.x -= 0.7f;

                transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            preDir = curDir;
        }

        while (true)
        {
            if (dist > float.Epsilon)
            {
                //Debug.Log("�̵� ��");
                // �̵���
                curState = STATE.WALK;
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, TargetPos, moveSpeed * Time.deltaTime);   // MoveTowards ������ٵ� 2D�� �������� ���.

                rb2D.MovePosition(newPosition);
            }

            if (target.activeSelf == false)
                curState = STATE.IDLE;
            yield return new WaitForFixedUpdate();
        }
    }

    public override IEnumerator AttackToTarget()
    {
        curState = STATE.ATTACK;
        if (animator.GetBool("isAttack"))
        {
            //Debug.Log("���� ��");
            StartCoroutine(target.GetComponent<EnemyAction>().CharacterDamage(atk, animator.speed));
            yield return new WaitForSeconds(animator.speed);
            AttackOff();
            //Debug.Log("���� ��");
        }
    }

    public override IEnumerator CharacterDamage(int damage, float interval)
    {
        while (true)
        {
            StartCoroutine(ChanageColorCharacter(interval));                            // �ڷ�ƾ ����(�ǰݽ� �� ����)

            //if (playerComponent.GetPlayerInfo().def >= damage)                                       // �÷��̾��� ���� > ���� ���ݷ�
            //    damage = 1;                                                     // �������� 1�� ����
            //else
            //    damage -= playerComponent.GetPlayerInfo().def;                                       // �ƴ϶�� ���ǰ��ݷ� - ����

            ////playerInfo.currentHp -= damage;

            //if (playerComponent.GetHp() <= 0)
            //{
            //    playerComponent.SetDead(true);
            //    Debug.Log("player Die");
            //    //KillCharacter();
            //    break;
            //}
            //else
            //    playerComponent.SetHp(-damage);

                yield return new WaitForSeconds(interval);
        }
        //yield return new WaitForSeconds(1);
    }
}
