using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public GameObject player;
    private BoxCollider2D boxCollider;
    Vector2 boxColliderSize;

    private float timer;                                    // 타이머
    private float damageInterval;                           // 데미지 들어오는 시간

    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider)
            boxColliderSize = boxCollider.size;

        damageInterval = 1.0f;
        timer = damageInterval + 0.1f;
    }

    void Update()
    {
        // 플레이어랑 적이 겹치는지 계속 보기
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxColliderSize, 0.0f);
        timer += Time.deltaTime;

        // 데미지 들어오는 시간보다 시간이 많으면
        if (timer >= damageInterval)
        {
            foreach (Collider2D collider in colliders)
            if (collider.CompareTag("Enemy"))
            {
                    int enemyAtk = collider.gameObject.GetComponent<Enemy>().GetAtk();
                    int playerDef = player.GetComponent<Player>().GetPlayerInfo().def;
                    int damage = enemyAtk - playerDef;
                    if (damage <= 0)
                        damage = 1;

                    DataManager.GetDataManager().PlayerAttacked(damage);
                }
            timer = 0.0f;   
        }
    }
}
