using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSkill : MonoBehaviour
{
    public LayerMask targetLayer; // 가져올 레이어
    public Vector2 meteorRange;     // 메테오 범위
    static List<string> enemyName = new List<string>();
    static int num = 0;
    public GameObject meteorPrefab;
    // Start is called before the first frame update
    void Start()
    {
            GameObject getPlayer = GameObject.Find("Player");
            Collider2D[] colliders = Physics2D.OverlapBoxAll(getPlayer.transform.position, meteorRange, 0.0f, targetLayer);
            int enemyCount = colliders.Length;
            int randomEnemyIndex = Random.Range(0, enemyCount);
            bool isExist = false;
            if (enemyName.Count == 0)
            {
                num++;
                Debug.Log("Meteor Random EnemyName : " + colliders[randomEnemyIndex].gameObject.name);
                gameObject.transform.position = new Vector3(colliders[randomEnemyIndex].transform.position.x, colliders[randomEnemyIndex].transform.position.y + 1.28f, 0);
                enemyName.Add(colliders[randomEnemyIndex].gameObject.name);
                GameObject.Instantiate(meteorPrefab);

            }
            else
            {
                foreach (var eName in enemyName)
                {
                    if (eName.Equals(colliders[randomEnemyIndex].gameObject.name))
                    {
                        isExist = true;
                    }
                }
            if (!isExist)
            {
                num++;
                Debug.Log("Meteor Random EnemyName : " + colliders[randomEnemyIndex].gameObject.name);
                gameObject.transform.position = new Vector3(colliders[randomEnemyIndex].transform.position.x, colliders[randomEnemyIndex].transform.position.y+1.5f, 0);
                enemyName.Add(colliders[randomEnemyIndex].gameObject.name);

                if (num >= 3)
                {
                    num = 0;
                    enemyName.Clear();
                }
                else
                    GameObject.Instantiate(meteorPrefab);
            }
            else
            {
                GameObject.Instantiate(meteorPrefab);
                Destroy(gameObject);
            }
        }

        
    }
}
