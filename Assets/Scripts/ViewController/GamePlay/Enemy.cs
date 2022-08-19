// move: https://www.jianshu.com/p/e08c662d7e1c
// AI: https://www.bilibili.com/video/BV1zf4y1r7FJ?spm_id_from=333.337.search-card.all.click&vd_source=3b8bb2d4a2160770e25d2a56b850c4b9
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AI;
using System.Collections;
using QFramework;

namespace QFramework.FlyChess
{
    
public enum StateType
{
    Idle, Patrol, Chase, React, Attack, Hit, Death
}
public class Enemy : FlyChessController{
    
    private Animator            m_animator;
    private Rigidbody2D         m_body2d;
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
    int patrolSpeed = 1;
    int chaseSpeed = 2;
    public Transform[] patrolPoints; // 巡逻边界
    public Transform[] chasePoints; // 追逐边界
    private bool isHit;
    private Vector2 direction;
    private FSM<StateType> FSM;
    private float distance;
    private bool beHit;
    private float idleTimer;
    private int patrolPosition;
    private Transform attackArea;
    // Start is called before the first frame update
    // Use this for initialization
    void Start () {
        m_animator = GetComponent<Animator>();
        m_body2d = GetComponent<Rigidbody2D>();
        m_player = GameObject.FindWithTag("Player").transform;
        attackArea = transform.Find("AttackArea").transform;
        HPStrip.value = HPStrip.maxValue = 100;    //初始化血条
        this.RegisterEvent<DamageEvent>(OnDamage);
        FSM = new FSM<StateType>();
        initFSM();
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

    void initFSM()
    {
        // idle
        FSM.State(StateType.Idle).OnEnter(()=>{
            Debug.Log("ener idle..");
            m_attackTrigger.gameObject.SetActive(false); 
            m_animator.SetTrigger("Idle");
        }).OnUpdate(()=>{
            idleTimer += Time.deltaTime;
            if (beHit) FSM.ChangeState(StateType.Hit);
            if (m_player != null &&
                m_player.position.x >= this.chasePoints[0].position.x &&
                m_player.position.x <= this.chasePoints[1].position.x)
            {
                FSM.ChangeState(StateType.React);
            }
            if (idleTimer >= 1f)
            {
                FSM.ChangeState(StateType.Patrol);
            }
        }).OnExit(()=>{
            idleTimer = 0;
        });

        // patrol
        FSM.State(StateType.Patrol).OnEnter(()=>{
            Debug.Log("ener Patrol..");
            m_animator.SetTrigger("Run");
        }).OnUpdate(()=>{
            FlipTo(patrolPoints[patrolPosition]);
    
            var tmp = new Vector3(patrolPoints[patrolPosition].position.x, transform.position.y, transform.position.z);
            transform.position = Vector2.MoveTowards(transform.position, tmp
                , patrolSpeed * Time.deltaTime);
    
            if (beHit) FSM.ChangeState(StateType.Hit);

            if (m_player != null &&
                m_player.position.x >= chasePoints[0].position.x &&
                m_player.position.x <= chasePoints[1].position.x)
            {
                FSM.ChangeState(StateType.React);
            }

            if (Vector2.Distance(m_player.position, patrolPoints[patrolPosition].position) < .1f)
                FSM.ChangeState(StateType.Idle);

            if (transform.position.x == patrolPoints[patrolPosition].position.x)
                FSM.ChangeState(StateType.Idle);

        }).OnExit(()=>{
            patrolPosition++;
            if (patrolPosition >= patrolPoints.Length)
            {
                patrolPosition = 0;
            }
        });
        
        // chase
        FSM.State(StateType.Chase).OnEnter(()=>{
            Debug.Log("enter Chase..");
            m_animator.SetTrigger("Run");
        }).OnUpdate(()=>{
            FlipTo(m_player);

            if (m_player)
                transform.position = Vector2.MoveTowards(transform.position, new Vector3(m_player.position.x, transform.position.y, transform.position.z), chaseSpeed * Time.deltaTime);

            if (beHit) FSM.ChangeState(StateType.Hit);

            // Debug.Log("transform: " + transform.position.x + ";0: "+ chasePoints[0].position.x + ";1: " + chasePoints[1].position.x);

            if (m_player == null ||
                transform.position.x < chasePoints[0].position.x ||
                transform.position.x > chasePoints[1].position.x)
            {
            Debug.Log("enter Chase update..");
                FSM.ChangeState(StateType.Idle);
            }
            if (Physics2D.OverlapCircle(attackArea.position, 0.35f))
            {
                FSM.ChangeState(StateType.Attack);
            }
        }).OnExit(()=>{
        });

        // react
        FSM.State(StateType.React).OnEnter(()=>{
            Debug.Log("enter React..");
        }).OnUpdate(()=>{
            var info = m_animator.GetCurrentAnimatorStateInfo(0);
            if (beHit)
            {
                FSM.ChangeState(StateType.Hit);
            }

            if (info.normalizedTime >= .95f)
            {
                FSM.ChangeState(StateType.Chase);
            }
        }).OnExit(()=>{
        });

        // attack
        FSM.State(StateType.Attack).OnEnter(()=>{
            Debug.Log("ener Attack..");
            m_attackTrigger.gameObject.SetActive(true);
            m_animator.SetTrigger("Attack1");
        }).OnUpdate(()=>{
            var info = m_animator.GetCurrentAnimatorStateInfo(0);
    
            if (beHit) FSM.ChangeState(StateType.Hit);

            if (info.normalizedTime >= .95f) FSM.ChangeState(StateType.Chase);
        }).OnExit(()=>{
        });

        // hit
        FSM.State(StateType.Hit).OnEnter(()=>{
            Debug.Log("ener Hit..");
            FlipTo(m_player);

            m_animator.SetTrigger("Hurt");
            var tmp = transform.position.x+1*mGameModel.Face.Value.x;
            transform.position = Vector2.MoveTowards(transform.position, new Vector3(tmp, transform.position.y, transform.position.z),  chaseSpeed * Time.deltaTime);
        }).OnUpdate(()=>{
            var info = m_animator.GetCurrentAnimatorStateInfo(0);
            if (HPStrip.value <= 0) FSM.ChangeState(StateType.Death);

            if (info.normalizedTime >= .95f) FSM.ChangeState(StateType.Chase);
        }).OnExit(()=>{
            beHit = false;
        });

        // death
        FSM.State(StateType.Death).OnEnter(()=>{
            Debug.Log("ener Death..");
            HPStrip.gameObject.SetActive(false);
            m_animator.SetTrigger("Death");
        }).OnUpdate(()=>{
        }).OnExit(()=>{
            Destroy(gameObject);
        });

        FSM.StartState(StateType.Idle);
    }
	
    void Update () {
        FSM.Update();
    }

     public void FlipTo(Transform target)
    {
        if (target != null)
        {
            if (transform.position.x > target.position.x)
            {
                transform.localScale = new Vector3(5, 5, 1);
            }
            else if (transform.position.x < target.position.x)
            {
                transform.localScale = new Vector3(-5, 5, 1);
            }
        }
    }

    public void OnDamage(DamageEvent e)
    {
        beHit = true;
        this.direction = e.direction; 
        HPStrip.value -= e.HPCost;
    }

    // private void OnTriggerEnter2D (Collider2D collision)
    // {

    //     if (collision.CompareTag("Player"))
    //     {
    //         // AttackSense.Instance.HitPause(6);
    //         // AttackSense.Instance.CameraShake(.1f, .015f);

    //        var player = collision.GetComponent<Player>();
    //        if (player != null) player.SendCommand<DamageCommand>();
    //     }
    // }


     private void OnTriggerEnter2D (Collider2D collision)
    {
        // Debug.Log("other tag: "+ collision.tag.ToString() + "; this tag: " + this.gameObject.tag);
        if (collision.CompareTag("PlayerCollider"))
        {
            // 顿帧 & 屏幕震动
            AttackSense.Instance.HitPause(6);
            AttackSense.Instance.CameraShake(.1f, .015f);
            this.SendCommand<DamageCommand>();
        }

        
    }

    // 画出区域方便排错
    private void OnDrawGizmos()
    {
        if (attackArea != null) Gizmos.DrawWireSphere(attackArea.position, 0.35f);
    }

}

}
