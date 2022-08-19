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
    private IPlayerModel        m_playerMod;
    public Slider               m_hpStrip;    // 添加血条Slider的引用

    // AI相关
    public Transform            m_attackTrigger;
    public Transform[]          m_patrolPoints; // 巡逻边界
    public Transform[]          m_chasePoints; // 追逐边界
    private int                 m_patrolSpeed = 1;
    private int                 m_chaseSpeed = 2;
    private bool                m_isHit;
    private Vector2             m_direction;
    private FSM<StateType>      m_FSM;
    private bool                m_beHit;
    private float               m_idleTimer;
    private int                 m_patrolPosition;
    private Transform           m_attackArea;

    // Start is called before the first frame update
    // Use this for initialization
    void Start () {
        m_animator      = GetComponent<Animator>();
        m_body2d        = GetComponent<Rigidbody2D>();
        m_player        = GameObject.FindWithTag("Player").transform;
        m_attackArea    = transform.Find("AttackArea").transform;
        m_playerMod     = this.GetModel<GameModel>();
        m_hpStrip.value = m_hpStrip.maxValue = 100;    //初始化血条
        m_FSM             = new FSM<StateType>();

        this.RegisterEvent<DamageEvent>(OnDamage);
        initFSM();
    }

    void initFSM()
    {
        // idle
        m_FSM.State(StateType.Idle).OnEnter(()=>{
            Debug.Log("ener idle..");
            m_attackTrigger.gameObject.SetActive(false); 
            m_animator.SetTrigger("Idle");
        }).OnUpdate(()=>{
            m_idleTimer += Time.deltaTime;
            if (m_beHit) m_FSM.ChangeState(StateType.Hit);
            if (m_player != null &&
                m_player.position.x >= this.m_chasePoints[0].position.x &&
                m_player.position.x <= this.m_chasePoints[1].position.x)
            {
                m_FSM.ChangeState(StateType.React);
            }
            if (m_idleTimer >= 1f)
            {
                m_FSM.ChangeState(StateType.Patrol);
            }
        }).OnExit(()=>{
            m_idleTimer = 0;
        });

        // patrol
        m_FSM.State(StateType.Patrol).OnEnter(()=>{
            Debug.Log("ener Patrol..");
            m_animator.SetTrigger("Run");
        }).OnUpdate(()=>{
            FlipTo(m_patrolPoints[m_patrolPosition]);
    
            var tmp = new Vector3(m_patrolPoints[m_patrolPosition].position.x, transform.position.y, transform.position.z);
            transform.position = Vector2.MoveTowards(transform.position, tmp
                , m_patrolSpeed * Time.deltaTime);
    
            if (m_beHit) m_FSM.ChangeState(StateType.Hit);

            if (m_player != null &&
                m_player.position.x >= m_chasePoints[0].position.x &&
                m_player.position.x <= m_chasePoints[1].position.x)
            {
                m_FSM.ChangeState(StateType.React);
            }

            if (Vector2.Distance(m_player.position, m_patrolPoints[m_patrolPosition].position) < .1f)
                m_FSM.ChangeState(StateType.Idle);

            if (transform.position.x == m_patrolPoints[m_patrolPosition].position.x)
                m_FSM.ChangeState(StateType.Idle);

        }).OnExit(()=>{
            m_patrolPosition++;
            if (m_patrolPosition >= m_patrolPoints.Length)
            {
                m_patrolPosition = 0;
            }
        });
        
        // chase
        m_FSM.State(StateType.Chase).OnEnter(()=>{
            Debug.Log("enter Chase..");
            m_animator.SetTrigger("Run");
        }).OnUpdate(()=>{
            FlipTo(m_player);

            if (m_player)
                transform.position = Vector2.MoveTowards(transform.position, new Vector3(m_player.position.x, transform.position.y, transform.position.z), m_chaseSpeed * Time.deltaTime);

            if (m_beHit) m_FSM.ChangeState(StateType.Hit);

            // Debug.Log("transform: " + transform.position.x + ";0: "+ chasePoints[0].position.x + ";1: " + chasePoints[1].position.x);

            if (m_player == null ||
                transform.position.x < m_chasePoints[0].position.x ||
                transform.position.x > m_chasePoints[1].position.x)
            {
            Debug.Log("enter Chase update..");
                m_FSM.ChangeState(StateType.Idle);
            }
            if (Physics2D.OverlapCircle(m_attackArea.position, 0.35f))
            {
                m_FSM.ChangeState(StateType.Attack);
            }
        }).OnExit(()=>{
        });

        // react
        m_FSM.State(StateType.React).OnEnter(()=>{
            Debug.Log("enter React..");
        }).OnUpdate(()=>{
            var info = m_animator.GetCurrentAnimatorStateInfo(0);
            if (m_beHit)
            {
                m_FSM.ChangeState(StateType.Hit);
            }

            if (info.normalizedTime >= .95f)
            {
                m_FSM.ChangeState(StateType.Chase);
            }
        }).OnExit(()=>{
        });

        // attack
        m_FSM.State(StateType.Attack).OnEnter(()=>{
            Debug.Log("ener Attack..");
            m_attackTrigger.gameObject.SetActive(true);
            m_animator.SetTrigger("Attack1");
        }).OnUpdate(()=>{
            var info = m_animator.GetCurrentAnimatorStateInfo(0);
    
            if (m_beHit) m_FSM.ChangeState(StateType.Hit);

            if (info.normalizedTime >= .95f) m_FSM.ChangeState(StateType.Chase);
        }).OnExit(()=>{
        });

        // hit
        m_FSM.State(StateType.Hit).OnEnter(()=>{
            Debug.Log("ener Hit..");
            FlipTo(m_player);

            m_animator.SetTrigger("Hurt");
            var tmp = transform.position.x+1*m_playerMod.Face.Value.x;
            transform.position = Vector2.MoveTowards(transform.position, new Vector3(tmp, transform.position.y, transform.position.z),  m_chaseSpeed * Time.deltaTime);
        }).OnUpdate(()=>{
            var info = m_animator.GetCurrentAnimatorStateInfo(0);
            if (m_hpStrip.value <= 0) m_FSM.ChangeState(StateType.Death);

            if (info.normalizedTime >= .95f) m_FSM.ChangeState(StateType.Chase);
        }).OnExit(()=>{
            m_beHit = false;
        });

        // death
        m_FSM.State(StateType.Death).OnEnter(()=>{
            Debug.Log("ener Death..");
            m_hpStrip.gameObject.SetActive(false);
            m_animator.SetTrigger("Death");
        }).OnUpdate(()=>{
        }).OnExit(()=>{
            Destroy(gameObject);
        });

        m_FSM.StartState(StateType.Idle);
    }
	
    void Update () {
        m_FSM.Update();
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
        m_beHit = true;
        this.m_direction = e.direction; 
        m_hpStrip.value -= e.HPCost;
    }

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
        if (m_attackArea != null) Gizmos.DrawWireSphere(m_attackArea.position, 0.35f);
    }

}

}
