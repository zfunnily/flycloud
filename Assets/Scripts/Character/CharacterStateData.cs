using UnityEngine;
using System.Collections;

namespace PlayerCharacter
{
public class CharacterStateData : MonoBehaviour, IMessageReceiver {

     public float HP = 100;
     public float SP = 100;
     public float BeAttack = 10;//攻击力
    [SerializeField] float      m_speed = 8.0f;
    [SerializeField] float      m_jumpForce = 7.5f;
    [SerializeField] float      m_rollForce = 6.0f;
    [SerializeField] bool       m_noBlood = false;
    [SerializeField] GameObject m_slideDust;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_HeroKnight   m_groundSensor;
    private Sensor_HeroKnight   m_wallSensorR1;
    private Sensor_HeroKnight   m_wallSensorR2;
    private Sensor_HeroKnight   m_wallSensorL1;
    private Sensor_HeroKnight   m_wallSensorL2;
    private bool                m_isWallSliding = false;
    private bool                m_grounded = false;
    private bool                m_rolling = false;
    private int                 m_facingDirection = 1;
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 0.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;

    public int facingDirection
    {
        get { return m_facingDirection; }
        set
        {
            m_facingDirection = value;
        }
    }

    // Use this for initialization
    void Start ()
    {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1 = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2 = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1 = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2 = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
    }

    // Update is called once per frame
    void Update ()
    {
        // Increase timer that controls attack combo
        m_timeSinceAttack += Time.deltaTime;

        // Increase timer that checks roll duration
        if(m_rolling)
            m_rollCurrentTime += Time.deltaTime;

        // Disable rolling if timer extends duration
        if(m_rollCurrentTime > m_rollDuration)
            m_rolling = false;

        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State())
        {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if (m_grounded && !m_groundSensor.State())
        {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            GetComponent<SpriteRenderer>().flipX = false;
            m_facingDirection = 1;
        }
            
        else if (inputX < 0)
        {
            GetComponent<SpriteRenderer>().flipX = true;
            m_facingDirection = -1;
        }

        // Move
        if (!m_rolling )
            m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);

        //Death
        if (Input.GetKeyDown("e") && !m_rolling)
        {
            m_animator.SetBool("noBlood", m_noBlood);
            m_animator.SetTrigger("Death");
        }
            
        //Hurt
        else if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

        //Attack
        else if(Input.GetMouseButtonDown(0) && m_timeSinceAttack > 0.25f && !m_rolling)
        {
            m_currentAttack++;

            // Loop back to one after third attack
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            // Reset Attack combo if time since last attack is too large
            if (m_timeSinceAttack > 1.0f)
                m_currentAttack = 1;

            // Call one of three attack animations "Attack1", "Attack2", "Attack3"
            m_animator.SetTrigger("Attack" + m_currentAttack);

            // Reset timer
            m_timeSinceAttack = 0.0f;
        }

        // Block
        else if (Input.GetMouseButtonDown(1) && !m_rolling)
        {
            m_animator.SetTrigger("Block");
            m_animator.SetBool("IdleBlock", true);
        }

        else if (Input.GetMouseButtonUp(1))
            m_animator.SetBool("IdleBlock", false);

        // Roll
        else if (Input.GetKeyDown("left shift") && !m_rolling && !m_isWallSliding)
        {
            m_rolling = true;
            m_animator.SetTrigger("Roll");
            m_body2d.velocity = new Vector2(m_facingDirection * m_rollForce, m_body2d.velocity.y);
        }
            

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded && !m_rolling)
        {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }
        
        //Idle
        else
        {
            // Prevents flickering transitions to idle
            m_delayToIdle -= Time.deltaTime;
                if(m_delayToIdle < 0)
                    m_animator.SetInteger("AnimState", 0);
        }
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_facingDirection == 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        Debug.Log("animation Events....");
        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_facingDirection, 1, 1);
        }
    }

    // Animation Events
    // HeroKnight_Hurt  Called in slide animation
     //受到攻击时调用
        public bool OnGetDamage()
        {
            HP -= 1;
            Debug.Log("OnGetDamage.....");
            // for (int i = 0;i< onDamageMessageReceivers.Count; i++)//向接收者传递信息
            // {
            //     var receiver = onDamageMessageReceivers[i] as MyMessage.IMessageReceiver;
            //     receiver.OnMessageReceive(type,data.damager,data);
            // }
            return true;
        }

         public void OnMessageReceive(MesssageType type, object sender, object data)//实现消息接收的接口
        {
            // switch (type)
            // {
            //     case MesssageType.Damage:
            //         {
            //             MyDamageable.DamageMessage m_data = (MyDamageable.DamageMessage)data;
            //             Damaged(sender,m_data);
            //         }
            //         break;
            //     case MesssageType.Death:
            //         {
            //             MyDamageable.DamageMessage m_data = (MyDamageable.DamageMessage)data;
            //             Death(sender,m_data);
            //         }
            //         break;
            //     case MesssageType.Respawn://这个没什么必要
            //         break;
            // }
            return;
        }
        // private void Damaged(object sender, MyDamageable.DamageMessage data)
        // {
        //     if(isDeath || m_damage.CurHitPoint <= 0)//无敌状态或者死亡状态
        //     {
        //         return;
        //     }
        //     //伤害来源进行省略
        //     Vector3 direction = data.damageSource - transform.position;
        //     direction = transform.InverseTransformPoint(direction);
        //     direction.y = 0f;
        //     print("x :" + direction.x + "y:" + direction.z);
        //     m_animator.SetFloat(hashOfHitFromX,direction.normalized.x);
        //     m_animator.SetFloat(hashOfHitFromY,direction.normalized.z);
        //     m_animator.SetTrigger(hashOfHurt);//设置受伤trigger
        //     if (m_HurtAudioPlayer)
        //         m_HurtAudioPlayer.PlayAudioRandomly(null);
        // }
        // private void Death(object sender,MyDamageable.DamageMessage data)
        // {
        //     isDeath = true; //死亡
        //     m_DeathAudioPlayer.PlayAudioRandomly(null);//死亡音效
        //     m_animator.SetTrigger(hashOfDeath);//设置死亡trigger
        //     m_animator.ResetTrigger(hashOfHurt);//重置受伤trigger，避免状态重叠
        // }
}
}