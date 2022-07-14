//https://www.jianshu.com/p/e08c662d7e1c
using UnityEngine;
using UnityEngine.AI;
using System.Collections;

namespace PlayerCharacter
{
    
public class Enemy : MonoBehaviour {
    public enum EnemyState 
    {
        idle,
        run,
        attack,
        death,
    }
    [SerializeField] float      m_speed = 4.0f;
    [SerializeField] float      m_jumpForce = 7.5f;

    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
    private Sensor_Bandit       m_groundSensor;
    private bool                m_grounded = false;
    private bool                m_combatIdle = false;
    private bool                m_isDead = false;
    public EnemyState CurrentState = EnemyState.idle;
    private NavMeshAgent        m_agent;
    private Transform           m_player;
    public float attackTime = .5f;   //设置定时器时间 3秒攻击一次
    private float attackCounter = 0; //计时器变量
    public float attackDistance = 2;//这是攻击目标的距离，
    public float attackMoveDistance = 10;//寻路的目标距离
    // Start is called before the first frame update
    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_groundSensor = transform.Find("GroundSensor").GetComponent<Sensor_Bandit>();
        m_agent = GetComponent<NavMeshAgent>();
        m_player = GameObject.FindWithTag("Player").transform;
    }
	
	// Update is called once per frame
	void Update1 () {
        //Check if character just landed on the ground
        if (!m_grounded && m_groundSensor.State()) {
            m_grounded = true;
            m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if(m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }

        // -- Handle input and movement --
        float inputX = Input.GetAxis("Horizontal");

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
            transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
        else if (inputX < 0)
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

         m_body2d.velocity = new Vector2(inputX * m_speed, m_body2d.velocity.y);
        m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

       
        // -- Handle Animations --
        //Death
        if (Input.GetKeyDown("e")) {
            if(!m_isDead)
                m_animator.SetTrigger("Death");
            else
                m_animator.SetTrigger("Recover");

            m_isDead = !m_isDead;
        }
            
        // Hurt
        else if (Input.GetKeyDown("q"))
            m_animator.SetTrigger("Hurt");

        //Attack
        else if(Input.GetMouseButtonDown(0)) {
            m_animator.SetTrigger("Attack");
        }

        //Change between idle and combat idle
        else if (Input.GetKeyDown("f"))
            m_combatIdle = !m_combatIdle;

        //Jump
        else if (Input.GetKeyDown("space") && m_grounded) {
            m_animator.SetTrigger("Jump");
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
            m_body2d.velocity = new Vector2(m_body2d.velocity.x, m_jumpForce);
            m_groundSensor.Disable(0.2f);
        }

        // Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
            m_animator.SetInteger("AnimState", 2);

        //Combat Idle
        else if (m_combatIdle)
            m_animator.SetInteger("AnimState", 1);

        //Idle
        else
            m_animator.SetInteger("AnimState", 0);
    }

    
    void Update () {
        if (!m_grounded && m_groundSensor.State()) {
        m_grounded = true;
        m_animator.SetBool("Grounded", m_grounded);
        }

        //Check if character just started falling
        if(m_grounded && !m_groundSensor.State()) {
            m_grounded = false;
            m_animator.SetBool("Grounded", m_grounded);
        }
        float distance = Vector3.Distance(m_player.position, transform.position);
        switch (CurrentState)
        {
            case EnemyState.idle:
                // idle
                m_animator.SetInteger("AnimState", 0);

                if (distance > attackDistance  && distance <= attackMoveDistance)
                {
                    CurrentState = EnemyState.run;
                }
                if (distance < attackDistance )
                {
                    CurrentState = EnemyState.attack;
                }
                break; 
            case EnemyState.run:
                attackCounter = attackTime;//每次移动到最小攻击距离时就会立即攻击
                int direction = 1;// 玩家在敌人的右边
                if (m_player.position.x < transform.position.x)
                {
                    direction = -1;// 玩家在敌人的左边
                    transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);
                }else {
                    direction = 1;
                    transform.localScale = new Vector3(-1.0f, 1.0f, 1.0f);
                }

                m_body2d.velocity = new Vector2(direction * m_speed, m_body2d.velocity.y);
                m_animator.SetFloat("AirSpeed", m_body2d.velocity.y);

                m_animator.SetInteger("AnimState", 2);//移动的时候播放跑步动画

                if (distance > attackMoveDistance)
                {
                    CurrentState = EnemyState.idle;
                }
                if (distance < attackDistance )
                {
                    CurrentState = EnemyState.attack;
                }
                break;
            case EnemyState.attack:
                attackCounter += Time.deltaTime;
                if (attackCounter > attackTime)//定时器功能实现
                {
                    m_animator.SetTrigger("Attack");
                    attackCounter = 0;
                    CurrentState = EnemyState.idle;
                }
                break;
            case EnemyState.death:
                m_isDead = !m_isDead;
                break;
        }

    }
}

}
