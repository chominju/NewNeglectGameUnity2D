using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCollider : MonoBehaviour
{
    public GameObject player;
    static Dictionary<string,Coroutine> Coroutines;
    private BoxCollider2D boxCollider;
    Vector2 boxColliderSize;
    private float timer = 0f;
    private float damageInterval;
    bool isAttack;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider)
            boxColliderSize = boxCollider.size;

        damageInterval = player.GetComponent<PlayerAction>().GetAtkSpeed();

        Coroutines = new Dictionary<string, Coroutine>();

        isAttack
            = false;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxColliderSize, 0.0f);

        timer += Time.deltaTime;
        if (isAttack)
        {
            bool isExistEnemy = false;
            foreach (Collider2D collider in colliders)
            {
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

        if (player.GetComponent<Player>().isPlayerAttackAnimationStart ==false)
        {
            if (timer >= damageInterval)
            {
                timer = 0f;

                //if (colliders == null)
                //{
                //    if (isAttack)
                //    {
                //        isAttack = false;
                //        StartCoroutine(player.GetComponent<PlayerAction>().AttackOff());
                //    }
                //    //if(player.GetComponent<PlayerAction>().GetCurState() == Action.STATE.ATTACK)
                //    //{

                //    //}
                //}
                //else
                //{
                foreach (Collider2D collider in colliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        //Debug.Log("Attack Enemey Name :" + collider.GetComponent<Enemy>().gameObject.name);
                        player.GetComponent<PlayerAction>().StopMoveCoroutine();
                        player.GetComponent<PlayerAction>().Attack();
                        //collider.gameObject.GetComponent<EnemyAction>().PlayerDamage(player.GetComponent<Player>().GetPlayerInfo().atk);
                        isAttack = true;
                        // 적에게 데미지를 입히는 로직을 구현합니다.
                        //EnemyHealth enemyHealth = collider.GetComponent<EnemyHealth>();
                        //enemyHealth.TakeDamage(damageAmount);
                    }
                }
            }
           // }
           // }
        }
        else
        {
            foreach (Collider2D collider in colliders)
            {
                if (collider.CompareTag("Enemy"))
                {
                    //Debug.Log("Attack Enemey Name :" + collider.GetComponent<Enemy>().gameObject.name);
                    collider.gameObject.GetComponent<EnemyAction>().PlayerDamage(player.GetComponent<Player>().GetPlayerInfo().atk);
                }
            }
            player.GetComponent<Player>().isPlayerAttackAnimationStart = false;
        }
    }

    public static Dictionary<string, Coroutine> GetCoroutines()
    {
        return Coroutines;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    if (collision.gameObject.GetComponent<Enemy>().GetHp() <= 0)
        //    {
        //        collision.gameObject.GetComponent<Enemy>().SetDead(true);
        //        //GameObject.Find("Manager").SendMessage("EnemyDead", collision.gameObject.name);
        //        return;
        //    }
        //    colliderEnemyCount++;
        //    player.GetComponent<PlayerAction>().StopMoveCoroutine();
        //    player.GetComponent<PlayerAction>().Attack();
        //    //Debug.Log("[" + collision.gameObject.name + "] Enter Before");
        //    Coroutine newCoroutine = StartCoroutine(collision.gameObject.GetComponent<EnemyAction>().CharacterDamage(player.GetComponent<Player>().GetPlayerInfo().atk, player.GetComponent<PlayerAction>().GetAtkSpeed()));
        //    //Debug.Log("[" + collision.gameObject.name + "] Enter After");
        //    if (newCoroutine != null && !Coroutines.ContainsKey(collision.gameObject.name))
        //    {
        //        Coroutines.Add(collision.gameObject.name, newCoroutine);
        //    }
        //}
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        //if (collision.gameObject.CompareTag("Enemy"))
        //{
        //    //if (collision.gameObject.activeSelf)
        //    //{
        //    //Debug.Log("[" + collision.gameObject.name + "] Exit Before");

        //    if (Coroutines.ContainsKey(collision.name))
        //    {
        //        StopCoroutine(Coroutines[collision.name]);
        //        Coroutines.Remove(collision.name);
        //        //Debug.Log("[" + collision.gameObject.name + "] Exit After");
        //        colliderEnemyCount--;
        //    }
        //    //}    

        //    if (colliderEnemyCount <= 0)
        //        StartCoroutine(player.GetComponent<PlayerAction>().AttackOff());
        //    //existCollider=false
        //    //StopCoroutine(curCoroutine);
        //}
    }

}
