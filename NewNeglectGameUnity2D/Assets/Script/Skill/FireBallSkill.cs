using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireBallSkill : MonoBehaviour
{
    public float moveSpeed;
    public GameObject target;                                            // Ÿ��
    public Coroutine moveCoroutine;
    
    // Start is called before the first frame update
    void Start()
    {
        moveSpeed = 0.5f;
        StartCoroutine(FireBallAction());
    }

    // Update is called once per frame
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

            if (!EnemyManager.IsExistEnemy())                              // ���Ͱ� ���ٸ�
            {
                GetComponent<Skill>().SetState(Skill.STATE.IDLE);
                //curState = STATE.IDLE;
                moveSpeed = 0.0f;
                //curState = STATE.IDLE;
                //Debug.Log("���Ͱ� ����:");
                Debug.Log("Tornado Skill // No Enemy");
            }
            else
            {
                if (GetComponent<Skill>().getState() == Skill.STATE.IDLE)
                {
                    Debug.Log("Tornado Skill // SET TARGET");

                    SetTarget();
                }

                //}

                if (moveCoroutine != null)
                {
                    StopCoroutine(moveCoroutine);
                    moveCoroutine = null;
                }

                if (GetComponent<Skill>().getState() != Skill.STATE.ATTACK)
                {
                    Debug.Log("Tornado Skill // MOVE");

                    moveCoroutine = StartCoroutine(FireBallMove());                             // �̵�
                }
            }
            yield return new WaitForFixedUpdate();// WaitForSeconds(Update);
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


    public IEnumerator FireBallMove()
    {
        Debug.Log("FireBall Target : " + target.name);
        Vector3 TornadoPos = this.GetComponent<Transform>().position;
        Vector3 TargetPos = target.GetComponent<Transform>().position;

        float dist = Vector3.Distance(TargetPos,TornadoPos); // �Ÿ��� ũ�⸦ ����


        Vector3 direction = TargetPos - TornadoPos;

        while (true)
        {
            if (dist > float.Epsilon)
            {
                // ����
                if (direction.x < 0)
                    GetComponent<SpriteRenderer>().flipX = true;
                // ������
                else
                    GetComponent<SpriteRenderer>().flipX = false;
                //Debug.Log("�̵� ��");
                // �̵���
                GetComponent<Skill>().SetState(Skill.STATE.MOVE);

                moveSpeed = 0.5f;
                //transform.Translate(direction * moveSpeed * Time.deltaTime);
                transform.position = Vector3.MoveTowards(transform.position, TargetPos, Time.deltaTime * moveSpeed);// Translate(direction * moveSpeed * Time.deltaTime);

                //Vector3 newPosition = Vector3.MoveTowards(gameObject.transform.position, TargetPos, moveSpeed * Time.deltaTime);   // MoveTowards ������ٵ� 2D�� �������� ���.

                //rb2D.MovePosition(newPosition);
            }
            //else
            //    moveSpeed = 0.0f;
            yield return new WaitForFixedUpdate();
        }

        //else
        //Debug.Log("�̵��Ϸ�");
        // �̵��Ϸ�
    }
}
