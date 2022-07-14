using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCharacter;

public class AttackCollider : MonoBehaviour
{
    /// <summary>
    /// </summary>
    public HitManager m_hitmanager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player")
        {
            m_hitmanager.AttackTrigger = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.tag == "Player")
        {
            m_hitmanager.AttackTrigger = false;
        }
    }

}
