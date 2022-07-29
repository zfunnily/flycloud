using UnityEngine;
using UnityEngine.UI;    //添加UI命名空间
using System.Collections;

namespace PlayerCharacter
{
public abstract class CharacterData : MonoBehaviour{

     public float HP = 100;
     public float SP = 100;
     public float BeAttack = 10;//攻击力
    [SerializeField] public float      m_speed = 2.0f;
    [SerializeField] public float      m_jumpForce = 7.5f;
    [SerializeField] public float      m_rollForce = 6.0f;
    [SerializeField] public bool       m_noBlood = false;
    public Transform m_attackTrigger;
    public Vector2 attackPosition = new Vector2(0,0);
    public Slider HPStrip;    // 添加血条Slider的引用
    public float deathMove = 0;

    private int                 m_facingDirection = 1;


    public int facingDirection
    {
        get { return m_facingDirection; }
        set
        {
            m_facingDirection = value;
        }
    }

    public bool Dead() { return HP <= 0;}
    public abstract bool Damage();
    public abstract bool Dodge();
    public abstract bool Block();

}
}