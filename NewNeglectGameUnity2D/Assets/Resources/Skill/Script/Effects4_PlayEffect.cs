using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Effects4_PlayEffect : MonoBehaviour
{
    [SerializeField] private GameObject[] m_Effects;
    [SerializeField] private Vector3[] m_SpawnLocations;

    // Update is called once per frame
    void Update()
    {  
        //Play
        if (Input.GetKeyDown("space"))
        {
            for(int i = 0; i < m_Effects.Length; i++)
            {
                Vector3 spawnPosition = m_SpawnLocations[i];
                Instantiate(m_Effects[i], spawnPosition, gameObject.transform.localRotation);
            }

        }   
    }
}
