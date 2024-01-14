using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public GameObject player;                                       // 플레이어
    private BoxCollider2D boxCollider;                              // 박스 콜라이더
    Vector2 boxColliderSize;                                        // 박스 콜라이더 사이즈
    private float timer = 0f;                                       // 타이머 
    private float damageInterval;                                   // 데미지 들어오는 시간
    bool isAttack;                                                  // 공격중인가
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider)
            boxColliderSize = boxCollider.size;

        damageInterval = player.GetComponent<PlayerAction>().GetAtkSpeed();

        isAttack = false;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxColliderSize, 0.0f);

        timer += Time.deltaTime;

        // 공격중
        if (isAttack)
        {
            bool isExistEnemy = false;
            foreach (Collider2D collider in colliders)
            {
                // 적이 포함됐나
                if (collider.CompareTag("Enemy"))
                {
                    isExistEnemy = true;
                }
            }

            if (isExistEnemy == false)
            {
                isAttack = false;
                StartCoroutine(player.GetComponent<PlayerAction>().AttackOff());
            }
        }

        // 공격 애니메이션 시작안함
        if (player.GetComponent<Player>().isPlayerAttackAnimationStart ==false)
        {
            if (timer >= damageInterval)
            {
                timer = 0f;

                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {

                        player.GetComponent<PlayerAction>().StopMoveCoroutine();
                        player.GetComponent<PlayerAction>().Attack();
                        isAttack = true;
                    }
                }
            }
        }
        else
        {
            // 공격 애니메이션 시작함
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    collider.gameObject.GetComponent<EnemyAction>().PlayerDamage(player.GetComponent<Player>().GetPlayerInfo().atk);
                }
            }
            player.GetComponent<Player>().isPlayerAttackAnimationStart = false;
        }
    }
}
