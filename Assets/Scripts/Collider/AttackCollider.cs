using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCharacter;
using SkillSystem;
using CameraNS;

public class AttackCollider : MonoBehaviour
{
    /// <summary>
    /// </summary>
    public CharacterData m_player;

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
        if (collision == null ) return;
        // if (m_player == null ) return;


        string gameTag = this.gameObject.tag;
        // Debug.Log("OnTriggerEnter2D.." + gameTag + "; collision.tag: " + collision.tag);
        if ((gameTag == "PlayerCollider" && collision.tag == "Enemy") || 
            (gameTag == "EnemyCollider" && collision.tag == "Player"))  
        {
            CharacterData gameObject = GameObject.FindGameObjectWithTag(collision.tag).GetComponent<CharacterData>();
            if (gameObject != null) gameObject.Damage();
            return;
        }


        // 1. 如何判断近战攻击击中了子弹？
        // 2. 如何将弹反后的子弹视为由玩家发动的攻击？
        // 3. https://www.cnblogs.com/OtusScops/p/14710506.html
        if ((gameTag == "BulletCollider")) // 如果是子弹
        {
            if(collision.tag == "PlayerCollider") // 子弹与player碰撞
            {
               this.gameObject.tag = "PlayerAttack"; // 改变标签
                GameObject.FindWithTag("MainCamera").GetComponent<ShakeCamera>().enabled = true; // 震动屏幕
                SkillDeployer sd = GetComponent<SkillDeployer>();
                if (sd != null) sd.TurnFace();
            }
            if (collision.tag == "Player") 
            {
               CharacterData gameObject = GameObject.FindGameObjectWithTag(collision.tag).GetComponent<CharacterData>();
                if (gameObject != null) 
                {
                    if (gameObject.Dodge()) return;

                    if (!gameObject.Block()) gameObject.Damage();
                    
                    
                }
                Destroy(this.gameObject);
            }
        }else if (gameTag == "PlayerAttack")  // 由 player 反弹回来的子弹
        {
            if (collision.tag == "Enemy")
            {
               CharacterData gameObject = GameObject.FindGameObjectWithTag(collision.tag).GetComponent<CharacterData>();
                if (gameObject != null) gameObject.Damage(); 
                Destroy(this.gameObject);
            }
        }
    }

    
    // private void OnTriggerExit2D(Collision2D collision)
    // {
    // }
}
