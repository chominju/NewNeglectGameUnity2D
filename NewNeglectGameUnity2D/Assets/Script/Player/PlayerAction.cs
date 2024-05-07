using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAction : Action
{
    private float playerAtkSpeed;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = GetComponent<Player>().GetPlayerInfo().moveSpeed;
        atk = GetComponent<Player>().GetPlayerInfo().atk;
        animator = GetComponent<Animator>();
        rb2D = GetComponent<Rigidbody2D>();
        coli2D = GetComponent<Collider2D>();
        preDir = DIR.LEFT;
        curDir = preDir;
        curState = STATE.IDLE;

        playerAtkSpeed = animator.speed / (2.0f * animator.speed);

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

    public override IEnumerator CharacterAction()
    {
        while (true)
        {
            SetAnimation();

            if (!EnemyManager.IsExistEnemy())                              // 몬스터가 없다면
            {
                curState = STATE.IDLE;                                      // 가만히 서있음
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
                                                                                       
            }
            yield return new WaitForFixedUpdate();
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
                float newDistance = Vector3.Distance(transform.position,enemyPos);
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
        Vector3 direction = TargetPos - playerPos;

        // 왼쪽
        if (direction.x < 0)
            curDir = DIR.LEFT;
        else
            curDir = DIR.RIGHT;

        // 이전방향이랑 달라졌을 때,
        if (preDir != curDir)
        {
            // 왼쪽 방향
            if (curDir == DIR.LEFT)
            {
                transform.eulerAngles = new Vector3(0, 0, 0);
                Vector3 tempPos = transform.localPosition;
                tempPos.x += 0.7f;

                transform.localPosition = new Vector3(tempPos.x, tempPos.y, tempPos.z);
            }
            else
            {
                // 오른쪽 방향
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
                // 남은거리가 0(미세한)보다 클 때
                curState = STATE.WALK;
                Vector3 newPosition = Vector3.MoveTowards(rb2D.position, TargetPos, moveSpeed * Time.deltaTime);   // MoveTowards 리지드바디 2D의 움직임을 계산.

                rb2D.MovePosition(newPosition);
            }

            // 타켓이 죽으면 그냥 서있기.
            if (target.activeSelf == false)
                curState = STATE.IDLE;
            yield return new WaitForFixedUpdate();
        }
    }

}