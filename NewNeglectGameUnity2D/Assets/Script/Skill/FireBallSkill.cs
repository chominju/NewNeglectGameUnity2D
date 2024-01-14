using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSkill : MonoBehaviour
{
    public float moveSpeed;
    public GameObject target;                                            // 타켓
    public Coroutine moveCoroutine;
    
    void Start()
    {
        moveSpeed = 0.5f;
        StartCoroutine(FireBallAction());
    }

    void Update()
    {
        if (GetComponent<Skill>().getState() == Skill.STATE.ATTACK)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
        }
    }

    IEnumerator FireBallAction()
    {
        while (true)
        {
            Debug.Log("FireBall Target : "+target);
            if (target != null)
            {
                if (!target.activeSelf)
                {
                    GetComponent<Skill>().SetState(Skill.STATE.IDLE);
                    moveSpeed = 0.0f;
                }
            }

            if (!EnemyManager.IsExistEnemy())                              // 몬스터가 없다면
            {
                GetComponent<Skill>().SetState(Skill.STATE.IDLE);
                moveSpeed = 0.0f;
            }
            else
            {
                if (GetComponent<Skill>().getState() == Skill.STATE.IDLE)
                {
                    SetTarget();
                }

                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                    moveCoroutine = null;
                }

                if (GetComponent<Skill>().getState() != Skill.STATE.ATTACK)
                {
                    moveCoroutine = StartCoroutine(FireBallMove());                             // 이동
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetTarget()
    {
        GetComponent<Skill>().SetState(Skill.STATE.MOVE);
        target = null; // 타겟 몬스터를 정함.
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


    public IEnumerator FireBallMove()
    {
        Debug.Log("FireBall Target : " + target.name);
        Vector3 TornadoPos = this.GetComponent<Transform>().position;
        Vector3 TargetPos = target.GetComponent<Transform>().position;

        float dist = Vector3.Distance(TargetPos,TornadoPos); // 거리의 크기를 구함


        Vector3 direction = TargetPos - TornadoPos;

        while (true)
        {
            if (dist > float.Epsilon)
            {
                // 왼쪽
                if (direction.x < 0)
                    GetComponent<SpriteRenderer>().flipX = true;
                // 오른쪽
                else
                    GetComponent<SpriteRenderer>().flipX = false;
                // 이동중
                GetComponent<Skill>().SetState(Skill.STATE.MOVE);

                moveSpeed = 0.5f;
                transform.position = Vector3.MoveTowards(transform.position, TargetPos, Time.deltaTime * moveSpeed);

            }
            yield return new WaitForFixedUpdate();
        }

    }
}
