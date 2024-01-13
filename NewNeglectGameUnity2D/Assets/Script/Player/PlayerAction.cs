using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : Action
{

    private GameObject childColiObject;                                         // 플레이어 콜라이더(몬스터)
    private GameObject weaponColl;                                              // 플레이어 공격시 콜라이더

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

    }

    private void FixedUpdate()
    {
        //Action();
    }

    public override IEnumerator CharacterAction()
    {
        while (true)
        {
            SetAnimation();

            if (!EnemyManager.IsExistEnemy())                              // 몬스터가 없다면
            {
                curState = STATE.IDLE;
            }
            else
            {
                if (curState == STATE.IDLE)
                    SetTarget();

                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                    moveCoroutine = null;
                }

                if (curState != STATE.ATTACK)
                    moveCoroutine = StartCoroutine(Move());                             // 이동
                                                                                        //{
                                                                                        //    curState = STATE.ATTACK;
                                                                                        //    curCoroutine = StartCoroutine(AttackToMonster());
                                                                                        //}
            }
            yield return new WaitForFixedUpdate();// WaitForSeconds(Update);
        }
    }





    public override void SetTarget()
    {
        curState = STATE.WALK;
        //target = null; // 타겟 몬스터를 정함.
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

        float dist = (playerPos - TargetPos).sqrMagnitude; // 거리의 크기를 구함

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
                curState = STATE.WALK;
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, TargetPos, moveSpeed * Time.deltaTime);   // MoveTowards 리지드바디 2D의 움직임을 계산.

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
            StartCoroutine(target.GetComponent<EnemyAction>().CharacterDamage(atk, animator.speed));
            yield return new WaitForSeconds(animator.speed);
            AttackOff();
        }


    }

    public override IEnumerator CharacterDamage(int damage, float interval)
    {
        Debug.Log("CharacterDamage");
        yield return new WaitForSeconds(interval);
    }
}