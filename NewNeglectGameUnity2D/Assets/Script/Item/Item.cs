using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public void SetPos(Transform pos)
    {
        transform.position = pos.position;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // ÇÃ·¹ÀÌ¾î¿Í Ãæµ¹½Ã È¹µæ
        if (collision.gameObject.CompareTag("Player"))
        {
            DataManager.GetDataManager().GainEquipment(gameObject.name);
            Destroy(gameObject);
        }
    }
}
