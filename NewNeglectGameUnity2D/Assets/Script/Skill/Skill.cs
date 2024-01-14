using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEditor;
public class Skill : MonoBehaviour
{
    public enum STATE
    {
        IDLE,
        MOVE,
        ATTACK
    }
    public string skillName;
    SkillData skillData;
    List<Dictionary<string, object>> skillDetail;
    bool isSkillUse;
    bool isAttackAble;
    private float timer = 0f;               // ���ư��� Ÿ�̸�

    float skillCoolTime;
    float currentSkillCoolTime;
    int skillLevel;
    float holdingTime;

    Coroutine currentCoroutine;

    public float modifyPosX;    
    public float modifyPosY;

    public float damageInterval;                   // �ֱ������� ������ �ִ� �������� �ִ� �ð�
    public bool isSummon;                           // ��ȯ ��ų����

    private BoxCollider2D boxCollider;
    private PolygonCollider2D polygonCollider;
    Vector2 boxColliderSize;
    Vector2 polygonColliderSize;

    bool isAttack;

    Animator animator;

    public bool isTargetToEnemy; // Ÿ����ġ�� �ٷ� ������ �Ǵ���. (true : ����ġ�� ���� / false �÷��̾� ��ġ�� ����)
   
    public bool isFollowEnemy; //  (���� : �÷��̾� ��ġ / ����(Ÿ��) : �� true)  (���� : �÷��̾� ��ġ / ����x false) 
    GameObject playerTarget;
    Vector3 playerTargetPos;
    public STATE curState;
    int skillDamage;    // ��ų ������
    int getPlayerAtk;   // �÷��̾��� ���� �⺻ ���ݷ�
    int playerAtkMin;   // �ּ� ���ݷ�
    int playerAtkMax;   // �ִ� ���ݷ�
    float animationClipLength;
    float moveTimer;

    bool isTriggerEnemy;

    Vector3 direction; 
    void Start()
    {
        SetSkillData();
        isAttackAble = false;
        isSkillUse = false;
        isTriggerEnemy = false;
        playerTarget = null;
        holdingTime = skillData.holdingTime;

        boxCollider = GetComponent<BoxCollider2D>();
        polygonCollider = GetComponent<PolygonCollider2D>();
        if (boxCollider)
        {
            boxCollider.size = new Vector2(boxCollider.size.x * transform.localScale.x, boxCollider.size.y * transform.localScale.y);
            boxColliderSize = boxCollider.size;
        }


        animator = GetComponent<Animator>();
        isAttack = false;
        isAttackAble = false;
        curState = STATE.IDLE;
        skillDamage = int.Parse(skillDetail[skillLevel]["SkillAtk"].ToString());
        getPlayerAtk = GameObject.Find("Player").GetComponent<Player>().GetPlayerInfo().atk;
        playerAtkMin = getPlayerAtk / 2;
        playerAtkMax = getPlayerAtk;


        if (isSummon)
            animator.SetBool("isHoldingTimeEnd", false);


        animationClipLength = GetAnimationClipLength();
        moveTimer = 0.0f;
        Debug.Log("�ִϸ��̼� Ŭ���� ��ü �ð�: " + animationClipLength + "��");
    }

    private float GetAnimationClipLength()
    {
        AnimationClip[] clips = animator.runtimeAnimatorController.animationClips;

        float sum = 0;
        foreach (AnimationClip clip in clips)
        {
            sum+= clip.length;
        }

        return sum;
    }


public STATE getState()
    {
        return curState;
    }

    public void SetState(STATE state)
    {
        curState = state;
    }

    private void Update()
    {
        if (!isAttack && isAttackAble)
        {
            if (isSummon)
            {
                timer += Time.deltaTime;
                holdingTime -= Time.deltaTime;
                if (timer >= damageInterval)
                {
                    Debug.Log("Skill DamageInterval");
                    timer = 0f;
                    ApplyDamageToEnemies();
                }


                if (holdingTime <= 0)
                {
                    animator.SetBool("isHoldingTimeEnd", true);
                    Destroy(gameObject);
                }
            }
            else
            {
                isAttack = true;
                ApplyDamageToEnemies();
            }
        }

        if(!isTargetToEnemy && isFollowEnemy)
        {
            if(isTriggerEnemy)
            {
                Destroy(gameObject);
            }
            if (playerTargetPos != null && playerTarget!=null)
            {
                if (direction.x < 0)
                    GetComponent<SpriteRenderer>().flipX = true; // �������� �̵�
                else
                    GetComponent<SpriteRenderer>().flipX = false; // ���������� �̵�


                if (moveTimer < animationClipLength)
                {
                    moveTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(moveTimer / animationClipLength); // ���� ���� (0���� 1����)

                    // ������ ��ġ ���
                    Vector3 interpolatedPosition = Vector3.Lerp(transform.position, playerTargetPos, t);

                    // ������Ʈ�� ������ ��ġ�� �̵�
                    transform.position = interpolatedPosition;
                }
            }
            else
            {
                SetSkillPos();
                moveTimer = 0.0f;
                //Destroy(gameObject);
            }
        }

    }

    private void ApplyDamageToEnemies()
    {
        // PolygonCollider2D    
        bool isEnemy = false;
        List<Collider2D> overlappedColliders = new List<Collider2D>();
        ContactFilter2D contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;
        int numOverlaps = polygonCollider.OverlapCollider(contactFilter, overlappedColliders);

        foreach (Collider2D collider in overlappedColliders)
        {
            if (collider.CompareTag("Enemy"))
            {

                Debug.Log("Skill Damage Enemy : " + collider.gameObject.name);
                isEnemy = true;
                SetState(STATE.ATTACK);

                int randomDamage = Random.Range(playerAtkMin, playerAtkMax);

                collider.gameObject.GetComponent<EnemyAction>().SkillDamage(skillDamage + randomDamage);
                isTriggerEnemy = true;
            }
        }

        if (!isEnemy)
        {
            SetState(STATE.IDLE);
            isTriggerEnemy = false;
        }

    }


    public void SetSkillData()
    {
        skillData = DataManager.GetDataManager().GetFindSkillData(skillName);
        if(skillData == null)
        { 
            Debug.Log("FindSkillData Load Error");
            return;
        }
        skillCoolTime = (float)skillData.skillCoolTime;
        currentSkillCoolTime = (float)skillCoolTime;
        skillLevel = skillData.skillLevel;
        skillDetail = DataManager.GetDataManager().GetSkillDetailCsv(skillName);

        SetSkillPos();
    }

    public void SetSkillPos()
    {
        GameObject getPlayer = GameObject.Find("Player");
        // �ʵ忡 �÷��̾ ������������.
        if (getPlayer == null)
        {
            Debug.Log("No Player");
            return;
        }

        playerTarget = getPlayer.GetComponent<PlayerAction>().getTarget();
        if (playerTarget == null)
        {
            Debug.Log("No PlayerTarget(Skill.cs)");
            return;
        }

        playerTargetPos = playerTarget.GetComponent<Transform>().position;
        Debug.Log("Player Target(Skill.cs) : " + playerTarget.name);

        // �÷��̾� Ÿ��(��) ��ġ�� �����ؾ���
        if (isTargetToEnemy == true)
        {
            if (playerTarget == null)
            {
                Debug.Log("No Player Target");
            }
            else
            {
                Vector3 enmeyPos = playerTarget.GetComponent<Transform>().position;
                gameObject.transform.position = new Vector3(enmeyPos.x, enmeyPos.y, 0);
                Debug.Log("isTargetToEnemy is true : " + playerTarget.name + " << posX : " + enmeyPos.x + "posY : " + enmeyPos.y);

            }
        }
        else
        {
            // �÷��̾�� ��ġ�� ����
            Action.DIR playerDir = getPlayer.GetComponent<Action>().GetCurDir();
            Vector3 playerPos = getPlayer.GetComponent<Transform>().position;
            if (playerDir == Action.DIR.LEFT)
            {
                GetComponent<SpriteRenderer>().flipX = true;
                playerPos.x -= modifyPosX;
                playerPos.y += modifyPosY;
                gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, playerPos.z);
            }
            else
            {
                GetComponent<SpriteRenderer>().flipX = false;
                playerPos.x += modifyPosX;
                playerPos.y += modifyPosY;
                gameObject.transform.position = new Vector3(playerPos.x, playerPos.y, playerPos.z);
            }
            Debug.Log("isTargetToEnemy is false : " + " << posX : " + playerPos.x + "posY : " + playerPos.y);
            direction = (playerTargetPos - gameObject.transform.position).normalized;
        }
    }
    public bool IsSkillUse()
    {
        return isSkillUse;
    }

    public void SkillUse()
    {
        isSkillUse = true;
    }

    public void SkillCoolTimeFinish()
    {
        isSkillUse = false;
    }



    void SkillState(bool state)
    {
        isSkillUse = state;
    }


    public void SetAttackAble()
    {
        isAttackAble = true;
    }
    public void SkillAnimationFinish()
    {
       Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        if (boxCollider != null)                                                          // null���� üũ
        {
            if (!isTriggerEnemy)
            {
                Gizmos.color = Color.blue;
            }
            else
                Gizmos.color = Color.magenta;
           
            Gizmos.DrawWireCube(transform.position, boxCollider.size);                    // Gizmos.DrawWireSphere()�� ȣ���ϰ� ���� �׸� �� �ʿ��� ��ġ�� �������� �����Ѵ�.
        }

    }
}
