using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Character : MonoBehaviour
{
    // public CharaterData playerData;

    // Start is called before the first frame update

    public bool isDead;

     
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void SetDead(bool dead)
    {
        isDead = dead;
    }

    public virtual void KillCharacter()
    {
        Destroy(gameObject);
    }

    public virtual IEnumerator ChanageColorCharacter(float interval)
    {
        GetComponent<SpriteRenderer>().color = Color.red;

        yield return new WaitForSeconds(interval /2.0f);

        GetComponent<SpriteRenderer>().color = Color.white;
    }
}
