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
    private float timer = 0f;               // 돌아가는 타이머

    float skillCoolTime;
    float currentSkillCoolTime;
    int skillLevel;
    float holdingTime;

    Coroutine currentCoroutine;

    public float modifyPosX;    
    public float modifyPosY;

    public float damageInterval;                   // 주기적으로 적에게 주는 데미지를 주는 시간
    public bool isSummon;                           // 소환 스킬인지

    private BoxCollider2D boxCollider;
    private PolygonCollider2D polygonCollider;
    Vector2 boxColliderSize;
    Vector2 polygonColliderSize;

    bool isAttack;

    Animator animator;

    public bool isTargetToEnemy; // 타겟위치에 바로 생성이 되는지. (true : 적위치에 생성 / false 플레이어 위치에 생성)
   
    public bool isFollowEnemy; //  (생성 : 플레이어 위치 / 도착(타켓) : 적 true)  (생성 : 플레이어 위치 / 도착x false) 
    GameObject playerTarget;
    Vector3 playerTargetPos;
    public STATE curState;
    int skillDamage;    // 스킬 데미지
    int getPlayerAtk;   // 플레이어의 스탯 기본 공격력
    int playerAtkMin;   // 최소 공격력
    int playerAtkMax;   // 최대 공격력
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
        Debug.Log("애니메이션 클립의 전체 시간: " + animationClipLength + "초");
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
                    GetComponent<SpriteRenderer>().flipX = true; // 왼쪽으로 이동
                else
                    GetComponent<SpriteRenderer>().flipX = false; // 오른쪽으로 이동


                if (moveTimer < animationClipLength)
                {
                    moveTimer += Time.deltaTime;
                    float t = Mathf.Clamp01(moveTimer / animationClipLength); // 보간 비율 (0부터 1까지)

                    // 보간된 위치 계산
                    Vector3 interpolatedPosition = Vector3.Lerp(transform.position, playerTargetPos, t);

                    // 오브젝트를 보간된 위치로 이동
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
        // 필드에 플레이어가 존재하지않음.
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

        // 플레이어 타겟(적) 위치에 생성해야함
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
            // 플레이어에서 위치에 생성
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
        if (boxCollider != null)                                                          // null인지 체크
        {
            if (!isTriggerEnemy)
            {
                Gizmos.color = Color.blue;
            }
            else
                Gizmos.color = Color.magenta;
           
            Gizmos.DrawWireCube(transform.position, boxCollider.size);                    // Gizmos.DrawWireSphere()를 호출하고 구를 그릴 때 필요한 위치와 반지름을 전달한다.
        }

    }
}
