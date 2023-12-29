using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorSkill : MonoBehaviour
{
    public LayerMask targetLayer; // 가져올 레이어
    public Vector2 meteorRange;     // 메테오 범위
    static int meteorCount =0;
    public GameObject meteorPrefab;
    // Start is called before the first frame update
    void Start()
    {
   
        GameObject getPlayer = GameObject.Find("Player");
        Collider2D[] colliders = Physics2D.OverlapBoxAll(getPlayer.transform.position, meteorRange, 0.0f ,targetLayer);
        int enemyCount = colliders.Length;
        int randomEnemyIndex = Random.Range(0, enemyCount);

        Debug.Log("Meteor Random EnemyName : " + colliders[randomEnemyIndex].gameObject.name);
        gameObject.transform.position = new Vector3(colliders[randomEnemyIndex].transform.position.x, colliders[randomEnemyIndex].transform.position.y, 0);


        //foreach (Collider2D collider in colliders)
        //{
        //    if (collider.gameObject.layer == targetLayer)
        //    {
        //        Debug.Log("Meteor EnemyName : " + collider.gameObject.name);

        //    }
        //}
        // 메테오 갯수 증가
        meteorCount++;
        if (meteorCount >= 3)
        {
            // 3개이상 되면 생성 중지
            meteorCount = 0;
            return;
        }
        else
        {
            // 3개보다 적으면 생성 
            GameObject.Instantiate(meteorPrefab);
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
