using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCharacter;
using SkillSystem;

public class AttackCollider : MonoBehaviour
{
    /// <summary>
    /// </summary>
    public CharacterData m_hitmanager;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
         Debug.Log("enter: "+collision.tag);
        if (collision == null ) return;
        if (m_hitmanager == null ) return;

        if (collision.tag != "" && collision.tag != "Untagged")
        {
            if (collision.tag != m_hitmanager.tag  )
            {
                if (collision.tag == "AttackCollider")
                {
                    SkillDeployer sd= GetComponent<SkillDeployer>();
                    if (sd!= null) sd.TurnFace();
                }
                else 
                {
                    CharacterData gameObject = GameObject.FindGameObjectWithTag(collision.tag).GetComponent<CharacterData>();
                    if (gameObject != null) gameObject.Damage();
                }
            }
        }
    }

    
    // private void OnTriggerExit2D(Collision2D collision)
    // {
    // }
}
