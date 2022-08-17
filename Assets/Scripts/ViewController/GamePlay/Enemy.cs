//https://www.jianshu.com/p/e08c662d7e1c
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;
using QFramework;

namespace QFramework.FlyChess
{
public class Enemy : FlyChessController{
    public enum EnemyState 
    {
        idle,
        ready,
        run,
        attack,
        hurt,
        death,
        destory,
    }
    
    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    public EnemyState CurrentState = EnemyState.idle;
    private Transform           m_player;
    public float attackTime = 1.0f;   //设置定时器时间 3秒攻击一次
    private float attackCounter = 0; //计时器变量
    public float attackDistance = 1;//这是攻击目标的距离，
    public float attackMoveDistance = 8;//寻路的目标距离
    private float deathCounter = 0; 
    private IPlayerModel  mGameModel;
    public Slider HPStrip;    // 添加血条Slider的引用
    public Transform m_attackTrigger;
    int facingDirection;
    int speed;
    private bool isHit;
    private Vector2 direction;

    // Start is called before the first frame update
    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_player = GameObject.FindWithTag("Player").transform;
        HPStrip.value = HPStrip.maxValue = 100;    //初始化血条
        this.RegisterEvent<DamageEvent>(OnDamage);
    }
	
	// Update is called once per frame
	// void Update1 () {
    //     //Check if character just landed on the ground
    //     if (!m_grounded && m_groundSensor.State()) {
    //         m_grounded = true;
    //         m_animator.SetBool("Grounded", m_grounded);
    //     }

    //     //Check if character just started falling
    //     if(m_grounded && !m_groundSensor.State()) {
    //         m_grounded = false;
    //         m_animator.SetBool("Grounded", m_grounded);
    //     }

    //     // -- Handle input and movement --
    //     float inputX = Input.GetAxis("Horizontal");

    //     // Swap direction of sprite depending on walk direction
    //     if (inputX > 0)
    //         transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
    //     else if (inputX < 0)
    //         transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

    //      m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
    //     m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

       
    //     // -- Handle Animations --
    //     //Death
    //     if (Input.GetKeyDown("e")) {
    //         if(!m_isDead)
    //             m_animator.SetTrigger("Death");
    //         else
    //             m_animator.SetTrigger("Recover");

    //         m_isDead = !m_isDead;
    //     }
            
    //     // Hurt
    //     else if (Input.GetKeyDown("q"))
    //         m_animator.SetTrigger("Hurt");

    //     //Attack
    //     else if(Input.GetMouseButtonDown(0)) {
    //         m_animator.SetTrigger("Attack");
    //     }

    //     //Change between idle and combat idle
    //     else if (Input.GetKeyDown("f"))
    //         m_combatIdle = !m_combatIdle;

    //     //Jump
    //     else if (Input.GetKeyDown("space") && m_grounded) {
    //         m_animator.SetTrigger("Jump");
    //         m_grounded = false;
    //         m_animator.SetBool("Grounded", m_grounded);
    //         m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
    //         m_groundSensor.Disable(0.2f);
    //     }

    //     // Run
    //     else if (Mathf.Abs(inputX) > Mathf.Epsilon)
    //         m_animator.SetInteger("AnimState", 2);

    //     //Combat Idle
    //     else if (m_combatIdle)
    //         m_animator.SetInteger("AnimState", 1);

    //     //Idle
    //     else
    //         m_animator.SetInteger("AnimState", 0);
    // }

    
    EnemyState checkAttackDistance(float distance) 
    {
        if (distance > attackDistance  && distance <= attackMoveDistance)
        {
            CurrentState = EnemyState.run;
        }
        if (distance < attackDistance )
        {
            CurrentState = EnemyState.attack;
        }
        return CurrentState;
    }

    void Update () {
        // if (m_player.position.x < transform.position.x)
        // {
        //     facingDirection = -1;// 玩家在敌人的左边
        //     transform.localScale = new Vector3(5.0f, 5.0f, 1.0f);
        // }else {
        //     facingDirection = 1;
        //     transform.localScale = new Vector3(-5.0f, 5.0f, 1.0f);
        // }

        float distance = Vector3.Distance(m_player.position, transform.position);
        switch (CurrentState)
        {
            case EnemyState.idle:
                // idle
                m_attackTrigger.gameObject.SetActive(false); 
                m_animator.SetTrigger("Idle");
                checkAttackDistance(distance);
                break; 
            case EnemyState.run:
                m_attackTrigger.gameObject.SetActive(false); 
                // m_animator.SetTrigger("Run");

                Vector3 direct= transform.right  * Time.deltaTime * speed * facingDirection;
                transform.Translate(direct);

                attackCounter = attackTime;// 每次移动到最小攻击距离时就会立即攻击
                // m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);
                // m_animator.SetInteger("AnimState", 2);//移动的时候播放跑步动画

                checkAttackDistance(distance);
               
                break;
            case EnemyState.attack:
                attackCounter += Time.deltaTime;
                if (attackCounter > attackTime)//定时器功能实现
                {
                    m_attackTrigger.gameObject.SetActive(true);
                    m_animator.ResetTrigger("Idle");
                    m_animator.SetTrigger("Attack1");
                    attackCounter = 0;
                }
                checkAttackDistance(distance);
                break;
            case EnemyState.hurt:
                m_attackTrigger.gameObject.SetActive(false);
                m_animator.SetTrigger("Hurt");

                if (HPStrip.value == 0) 
                    CurrentState = EnemyState.death;
                else 
                    checkAttackDistance(distance);

                //  m_body2d.velocity = direction * speed;
                break;
            case EnemyState.death:
                m_attackTrigger.gameObject.SetActive(false);
                CurrentState = EnemyState.destory;
                m_animator.SetTrigger("Death");
                // transform.Translate(Vector3.up*-1 * Time.deltaTime);
                // transform.position = transform.position + new Vector3(0, -1*deathMove, 0);
                HPStrip.gameObject.SetActive(false);
                break;

            default:
                m_attackTrigger.gameObject.SetActive(false);

                deathCounter += Time.deltaTime;
                if (deathCounter > 3) Destroy(this.gameObject);

                // m_animator.ResetTrigger("Death");
                m_animator.ResetTrigger("Attack1");
                m_animator.ResetTrigger("Idle");
                break;

        }
        // if (m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1") || m_animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        // {
        // }else {
        // }
    }

    public void OnDamage(DamageEvent e)
    {
        if (HPStrip.value == 0)
        {
            CurrentState = EnemyState.death;
            return;
        }
        transform.localScale = new Vector3(-e.direction.x*5.0f, 5.0f, 5.0f);
        this.direction = e.direction; 
        CurrentState = EnemyState.hurt;
        HPStrip.value -= e.HPCost;
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {

        if (collision.CompareTag("Player"))
        {
            // AttackSense.Instance.HitPause(6);
            // AttackSense.Instance.CameraShake(.1f, .015f);

           var player = collision.GetComponent<Player>();
           if (player != null) player.SendCommand<DamageCommand>();
        }
    }

}

}
