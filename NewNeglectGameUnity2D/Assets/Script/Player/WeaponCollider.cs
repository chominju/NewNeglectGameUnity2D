using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public GameObject player;                                       // �÷��̾�
    private BoxCollider2D boxCollider;                              // �ڽ� �ݶ��̴�
    Vector2 boxColliderSize;                                        // �ڽ� �ݶ��̴� ������
    private float timer = 0f;                                       // Ÿ�̸� 
    private float damageInterval;                                   // ������ ������ �ð�
    bool isAttack;                                                  // �������ΰ�
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

        // ������
        if (isAttack)
        {
            bool isExistEnemy = false;
            foreach (Collider2D collider in colliders)
            {
                // ���� ���ԵƳ�
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

        // ���� �ִϸ��̼� ���۾���
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
            // ���� �ִϸ��̼� ������
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
