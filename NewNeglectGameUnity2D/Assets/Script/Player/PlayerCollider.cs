using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public GameObject player;
    private BoxCollider2D boxCollider;
    Vector2 boxColliderSize;

    private float timer;                                    // Ÿ�̸�
    private float damageInterval;                           // ������ ������ �ð�

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
        // �÷��̾�� ���� ��ġ���� ��� ����
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxColliderSize, 0.0f);
        timer += Time.deltaTime;

        // ������ ������ �ð����� �ð��� ������
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
