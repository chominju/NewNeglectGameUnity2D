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
        if (collision.gameObject.CompareTag("Player"))
        {
            DataManager.GetDataManager().GainEquipment(gameObject.name);
            Destroy(gameObject);
        }
    }
}
