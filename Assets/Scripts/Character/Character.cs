using UnityEngine;
using System.Collections;

namespace PlayerCharacter
{
public abstract class CharacterData : MonoBehaviour{

     public float HP = 100;
     public float SP = 100;
     public float BeAttack = 10;//攻击力
    [SerializeField] public float      m_speed = 8.0f;
    [SerializeField] public float      m_jumpForce = 7.5f;
    [SerializeField] public float      m_rollForce = 6.0f;
    [SerializeField] public bool       m_noBlood = false;
    public Transform m_attackTrigger;

    private int                 m_facingDirection = 1;


    public int facingDirection
    {
        get { return m_facingDirection; }
        set
        {
            m_facingDirection = value;
        }
    }
    public abstract bool Damage();
    public abstract bool Dodge();
    public abstract bool Block();

}
}