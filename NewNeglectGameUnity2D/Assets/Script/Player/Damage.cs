using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    private float destroyTime;                                  // 삭제시간
    TextMeshProUGUI textMeshProComp;                            // 컴포넌트
    int damage;                                                 // 데미지
    float timer;                                                // 시간
    public GameObject canvas;                                   // 캔버스

    void Start()
    {
        timer = 0;
        destroyTime = 1.0f;

        if (textMeshProComp == null)
            textMeshProComp = GetComponent<TextMeshProUGUI>();
    }

    public void SetDamage(int setDamage, bool isPlayer)
    {
        damage = setDamage;
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
        // 적보다 살짝 위쪽에 생성
        Vector3 damagePos = Camera.main.WorldToScreenPoint(new Vector3(enemyPos.x + 0.5f, enemyPos.y + 0.5f, 0));

        gameObject.GetComponent<RectTransform>().position = damagePos;
    }

    // Update is called once per frame
    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= destroyTime)
            DestroyObject();
    }

    private void DestroyObject()
    {
        Destroy(gameObject);
    }
}
