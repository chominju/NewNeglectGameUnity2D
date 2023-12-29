using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollider : MonoBehaviour
{
    public GameObject player;
    private BoxCollider2D boxCollider;
    Vector2 boxColliderSize;

    private float timer;
    private float damageInterval;
    // Start is called before the first frame update
    void Start()
    {
        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider)
            boxColliderSize = boxCollider.size;

        damageInterval = 0.5f;
        timer = damageInterval + 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        Collider2D[] colliders = Physics2D.OverlapBoxAll(transform.position, boxColliderSize, 0.0f);
        timer += Time.deltaTime;

        if (timer >= damageInterval)
        {
            foreach (Collider2D collider in colliders)
            if (collider.CompareTag("Enemy"))
            {
                    //player.GetComponent<Sprite    Renderer>().color = Color.red;
                    int enemyAtk = collider.gameObject.GetComponent<Enemy>().GetAtk();
                    int playerDef = player.GetComponent<Player>().GetPlayerInfo().def;
                    int damage = enemyAtk - playerDef;
                    if (damage <= 0)
                        damage = 1;

                    DataManager.GetDataManager().PlayerAttacked(damage);
                }
            timer = 0.0f;   
        }

        //if(isColliderEnemyExist==false)
        //    player.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
