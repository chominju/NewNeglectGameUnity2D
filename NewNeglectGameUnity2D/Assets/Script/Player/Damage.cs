using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private float destroyTime;
    TextMeshPro textMesh;
    TextMeshProUGUI textMeshProComp;
    //Color alpha;
    int damage;
    float timer;
    public GameObject canvas;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;
        destroyTime = 1.0f;
        //damage = 0;
        //if (textMesh == null)
        //    textMesh = GetComponent<TextMeshPro>();
        //if (damage == 0 &&  textMesh != null)
        //    textMesh.text = damage.ToString();

        if (textMeshProComp == null)
            textMeshProComp = GetComponent<TextMeshProUGUI>();
        //if (damage == 0 && textMeshProComp != null)
        //    textMeshProComp.text = damage.ToString();
        //alpha = textMesh.color;



    }

    public void SetDamage(int setDamage, bool isPlayer)
    {
        damage = setDamage;
        //if (textMesh == null)
        //    textMesh = GetComponent<TextMeshPro>();
        //if (textMesh != null)
        //    textMesh.text = damage.ToString();
        if (textMeshProComp == null)
            textMeshProComp = GetComponent<TextMeshProUGUI>();
        if (textMeshProComp != null)
            textMeshProComp.text = damage.ToString();

        if(isPlayer)
        {
            // 플레이어
            textMeshProComp.color = new Color(0, 63, 255, 255);
        }
        else
        {
            // 스킬
            textMeshProComp.color = new Color(255, 100, 0, 255);
        }
    }

    public void SetDamagePos(Vector3 enemyPos)
    {
        Vector3 damagePos = Camera.main.WorldToScreenPoint(new Vector3(enemyPos.x + 0.5f, enemyPos.y + 0.5f, 0));

        gameObject.GetComponent<RectTransform>().position = damagePos;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= destroyTime)
            DestroyObject();
        //transform.Translate(new Vector3(0, moveSpeed * Time.deltaTime, 0)); // 텍스트 위치
        //alpha.a = Mathf.Lerp(alpha.a, 0, Time.deltaTime * alphaSpeed); // 텍스트 알파값
        //textMesh.color = alpha;
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
