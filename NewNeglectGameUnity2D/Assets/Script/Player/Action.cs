using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Action : MonoBehaviour
{
    public enum DIR
    {
        LEFT,
        RIGHT
    }

    public enum STATE
    {
        IDLE,
        WALK,
        ATTACK
    }

    public float moveSpeed;                                              // 스피드
    public int atk;                                                      // 공격력
    public float atkSpeed;                                               // 공격 속도
    public Animator animator;                                            // 애니메이터
    public Rigidbody2D rb2D;                                             // 리지드바디
    public Collider2D coli2D;                                            // 콜라이더
    public GameObject target;                                            // 타켓
    public Coroutine curCoroutine;                                       // 현재 코루틴
    public Coroutine moveCoroutine;
    public DIR preDir;
    public DIR curDir;
    public STATE curState;

    public void StopMoveCoroutine()
    {
        if (moveCoroutine != null)
        {
            StopCoroutine(moveCoroutine);
            moveCoroutine = null;
        }
    }

    public void SetAnimation()
    {
        switch (curState)
        {
            case STATE.IDLE:
                {
                    Idle();
                    break;
                }
            case STATE.WALK:
                {
                    Walk();
                    break;
                }
            case STATE.ATTACK:
                {
                    Attack();
                    //curCoroutine = StartCoroutine(AttackToMonster());
                    break;
                }
        }
    }

    public abstract IEnumerator CharacterAction();

    public abstract void SetTarget();

    public abstract IEnumerator Move();

    public abstract IEnumerator AttackToTarget();

    public abstract IEnumerator CharacterDamage(int damage, float interval);

    public virtual IEnumerator ChanageColorCharacter(float interval)
    {
        GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(interval / 2.0f);
        GetComponent<SpriteRenderer>().color = Color.white;
    }


    public void Idle()
    {
        animator.SetBool("isWalk", false);
        animator.SetBool("isAttack", false);
        curState = STATE.IDLE;
    }
    public void Walk()
    {
        animator.SetBool("isAttack", false);
        animator.SetBool("isWalk", true);
        curState = STATE.WALK;
    }

    public void WalkOff()
    {
        animator.SetBool("isWalk", false);
        curState = STATE.IDLE;
    }

    public void Attack()
    {
        animator.speed = 0.333f;
        atkSpeed = animator.speed / (2.0f * animator.speed);
        //target = null;
        curState = STATE.ATTACK;
        animator.SetBool("isWalk", false);
        animator.SetBool("isAttack", true);
    }

    public IEnumerator AttackOff()
    {
        yield return new WaitForSeconds(animator.speed / (2.0f * animator.speed));

        animator.speed = 1;
        atkSpeed = animator.speed / (2.0f * animator.speed);
        animator.SetBool("isAttack", false);
        curState = STATE.IDLE;
    }

    public DIR GetCurDir()
    {
        return curDir;
    }

    public STATE GetCurState()
    {
        return curState;
    }
}
