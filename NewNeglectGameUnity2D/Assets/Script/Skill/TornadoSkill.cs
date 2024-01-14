using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TornadoSkill : MonoBehaviour
{
    float moveSpeed;
    public GameObject target;                                            // Ÿ��
    public Coroutine moveCoroutine;

    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 0.5f;
        StartCoroutine(TornadoAction());
    }

    // Update is called once per frame
    void Update()
    {
        if(GetComponent<Skill>().getState() == Skill.STATE.ATTACK)
        {
            if (moveCoroutine != null)
                StopCoroutine(moveCoroutine);
        }
    }

    IEnumerator TornadoAction()
    {
        while (true)
        {

            if (target != null)
            {
                if (!target.activeSelf)
                {
                    GetComponent<Skill>().SetState(Skill.STATE.IDLE);
                    moveSpeed = 0.0f;
                }
            }

            if (!EnemyManager.IsExistEnemy())                              // ���Ͱ� ���ٸ�
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
                    moveCoroutine = StartCoroutine(TornadoMove());                             // �̵�
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }

    public void SetTarget()
    {
        GetComponent<Skill>().SetState(Skill.STATE.MOVE);
        target = null; // Ÿ�� ���͸� ����.
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


    public IEnumerator TornadoMove()
    {
        Debug.Log("Tornado Target : " + target.name);
        Vector3 TornadoPos = this.GetComponent<Transform>().position;
        Vector3 TargetPos = target.GetComponent<Transform>().position;

        float dist = (TornadoPos - TargetPos).sqrMagnitude; // �Ÿ��� ũ�⸦ ����


        Vector3 direction = TargetPos - TornadoPos;
        direction.Normalize(); // ���� ���͸� ����ȭ�Ͽ� ���̸� 1�� ����

        while (true)
        {
            if (dist > float.Epsilon)
            {
                moveSpeed = 0.5f;
                GetComponent<Skill>().SetState(Skill.STATE.MOVE);

                transform.Translate(direction * moveSpeed * Time.deltaTime);
            }
            else
                moveSpeed = 0.0f;
            yield return new WaitForFixedUpdate();
        }
    }
}
