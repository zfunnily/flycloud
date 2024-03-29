using UnityEngine;
using UnityEngine.UI;    
using System;
using UnityEngine.SceneManagement;
using QFramework;



namespace QFramework.FlyChess
{
	public partial class Player : FlyChessController
	{
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
    private int                 m_currentAttack = 0;
    private float               m_timeSinceAttack = 2.0f;
    private float               m_delayToIdle = 0.0f;
    private float               m_rollDuration = 8.0f / 14.0f;
    private float               m_rollCurrentTime;
    private bool                m_noBlood;
    private bool                m_isDeath;

    private IPlayerModel        m_playerMod;
    private Slider              m_hpStrip;    // 添加血条Slider的引用
    private Transform           m_attackTrigger;
    private bool                m_isAttack;
    private int                 m_jumpCount;

    private static              Action mUpdateAction;
    public static void AddUpdateAction(Action fun) => mUpdateAction += fun;
    public static void RemoveUpdateAction(Action fun) => mUpdateAction -= fun;


    // Use this for initialization
    void Awake()
    {
        m_animator      = GetComponent<Animator>();
        m_body2d        = GetComponent<Rigidbody2D>();
        m_groundSensor  = transform.Find("GroundSensor").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR1  = transform.Find("WallSensor_R1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorR2  = transform.Find("WallSensor_R2").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL1  = transform.Find("WallSensor_L1").GetComponent<Sensor_HeroKnight>();
        m_wallSensorL2  = transform.Find("WallSensor_L2").GetComponent<Sensor_HeroKnight>();
        m_attackTrigger = transform.Find("AttackCollider").GetComponent<Transform>();
        m_hpStrip       = transform.parent.Find("CharacterInfo").Find("HP").GetComponent<Slider>();
        m_playerMod     = this.GetModel<IPlayerModel>();

        m_playerMod.Speed = new BindableProperty<float>(4.0f);
        m_hpStrip.value = m_playerMod.HP;

        this.RegisterEvent<DirInputEvent>(OnInputDir);
        this.RegisterEvent<SkillEvent>(OnSkill);
    }

    private void Update()
    {
        if (m_isDeath) return;
        mUpdateAction?.Invoke();
    }

    // Update is called once per frame
    void OnInputDir (DirInputEvent e)
    {
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

        float inputX = e.hor;
        Move(e);
        Jump();
        Attack();        

        //Hurt
        if (Input.GetKeyDown("q") && !m_rolling)
            m_animator.SetTrigger("Hurt");

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
            m_body2d.velocity = new Vector2(transform.localScale.x * m_playerMod.RollForce, m_body2d.velocity.y);
        }

        //Run
        else if (Mathf.Abs(inputX) > Mathf.Epsilon)
        {
            // Reset timer
            m_delayToIdle = 0.05f;
            m_animator.SetInteger("AnimState", 1);
        }

        else if (Input.GetKeyDown(KeyCode.B))
        {
            // SceneManager.LoadScene("Level2");//Level2为我们要切换到的场景
            transform.position = new Vector2(-77.7f, -9.92f);
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

    void Move(DirInputEvent e)
    {
        // -- Handle input and movement --
        float inputX = e.hor;

        // Swap direction of sprite depending on walk direction
        if (inputX > 0)
        {
            // GetComponent<SpriteRenderer>().flipX = false;
            m_playerMod.Face= new BindableProperty<Vector2>(new Vector2(1,1)); 
        }
        else if (inputX < 0)
        {
            // GetComponent<SpriteRenderer>().flipX = true;
            m_playerMod.Face= new BindableProperty<Vector2>(new Vector2(-1,1)); 
        }
        transform.localScale = new Vector3(m_playerMod.Face.Value.x * 3, 3, 1);

        if (!m_isAttack && !m_rolling)
            m_body2d.velocity = new Vector2(inputX * m_playerMod.Speed, m_body2d.velocity.y);
        else
        {
            m_body2d.velocity = new Vector2(inputX * m_playerMod.Speed * 2, m_body2d.velocity.y);
        }

        //Set AirSpeed in animator
        m_animator.SetFloat("AirSpeedY", m_body2d.velocity.y);

        // -- Handle Animations --
        //Wall Slide
        m_isWallSliding = (m_wallSensorR1.State() && m_wallSensorR2.State()) || (m_wallSensorL1.State() && m_wallSensorL2.State());
        m_animator.SetBool("WallSlide", m_isWallSliding);
    }

    void Jump()
    {
        if (m_grounded) m_jumpCount = 1;
        if (Input.GetKeyDown("space") && m_jumpCount > 0)
        {
            m_animator.SetTrigger("Jump");
            m_animator.SetBool("Grounded", m_grounded);

            // m_body2d.velocity = new Vector2(m_body2d.velocity.x, mGameModel.JumpForce);
            m_body2d.velocity = Vector2.up * m_playerMod.JumpForce;
            m_jumpCount--;
        }
        else if (Input.GetKey("space") && m_jumpCount == 0 && m_grounded)
        {
            m_animator.SetTrigger("Jump");
            m_animator.SetBool("Grounded", m_grounded);
            m_groundSensor.Disable(0.2f);

            m_body2d.velocity = Vector2.up *m_playerMod.JumpForce;
            m_grounded = false;
            m_jumpCount--;
        }

    }

    public void AttackIng()
    {
        // m_attackTrigger.localPosition = new Vector2((1.0f), .66f);
        m_attackTrigger.gameObject.SetActive(true); 
    }

    public void AttackOver()
    {
        m_isAttack = false;
        m_attackTrigger.gameObject.SetActive(false); 
    }
    void Attack()
    {
        if(Input.GetMouseButtonDown(0) && !m_rolling && !m_isAttack)
        {
            m_isAttack = true;
            m_currentAttack++;
            if (m_currentAttack > 3)
                m_currentAttack = 1;

            m_animator.SetTrigger("Attack" + m_currentAttack);
            // Reset timer
            m_timeSinceAttack = 2.0f;
        }

        if (m_timeSinceAttack != 0)
        {
            m_timeSinceAttack -= Time.deltaTime;
            if (m_timeSinceAttack <= 0)
            {
                m_timeSinceAttack = 0;
                m_currentAttack = 0;
            }
        }
    }

    void OnSkill(SkillEvent e)
    {
        if (e.ID == SkillID.UNKNOWN) return;
    }

    void AnimPlay()
    {
        m_animator.speed = 1;
    }

    // Animation Events
    // Called in slide animation.
    void AE_SlideDust()
    {
        Vector3 spawnPosition;

        if (m_playerMod.Face.Value.x== 1)
            spawnPosition = m_wallSensorR2.transform.position;
        else
            spawnPosition = m_wallSensorL2.transform.position;

        // Debug.Log("animation Events....");
        if (m_slideDust != null)
        {
            // Set correct arrow spawn position
            GameObject dust = Instantiate(m_slideDust, spawnPosition, gameObject.transform.localRotation) as GameObject;
            // Turn arrow in correct direction
            dust.transform.localScale = new Vector3(m_playerMod.Face.Value.x*3, 1*3, 1);
        }
    }

    void OnDamage(float hit)
    {
        m_animator.SetTrigger("Hurt");
        m_hpStrip.value -= hit;
        if (m_hpStrip.value <= 0)
        {
            m_animator.SetBool("noBlood", false);
            m_animator.SetTrigger("Death");
            m_isDeath = true;
        }
    }

    private void OnTriggerEnter2D (Collider2D collision)
    {
        if (collision.CompareTag("EnemyCollider"))
        {
            // AttackSense.Instance.HitPause(6);
            // AttackSense.Instance.CameraShake(.1f, .015f);
            // this.OnDamage(20);

            var enemy = collision.transform.parent.GetComponent<Enemy>();
            if (enemy == null) return;
            if (enemy.FSMState() == StateType.Attack)
                Debug.Log("On player.... " + collision.tag + "; this.tag: " + this.gameObject.tag +"; Current enemy state: "+enemy.FSMState());
        }

        // 1. 如何判断近战攻击击中了子弹？
        // 2. 如何将弹反后的子弹视为由玩家发动的攻击？
        // 3. https://www.cnblogs.com/OtusScops/p/14710506.html
        string gameTag = this.gameObject.tag;
        if ((gameTag == "BulletCollider")) // 如果是子弹
        {
            if(collision.tag == "PlayerCollider") // 子弹与player碰撞
            {
               this.gameObject.tag = "PlayerAttack"; // 改变标签
                // GameObject.FindWithTag("MainCamera").GetComponent<>().enabled = true; // 震动屏幕
                SkillSystem sd = GetComponent<SkillSystem>();
                if (sd != null) sd.TurnFace();
            }
            if (collision.tag == "Player") 
            {
               FlyChessController gameObject = GameObject.FindGameObjectWithTag(collision.tag).GetComponent<FlyChessController>();
                if (gameObject != null) 
                {
                    // if (gameObject.Dodge()) return;

                    // if (!gameObject.Block()) gameObject.Damage();
                    
                    
                }
                Destroy(this.gameObject);
            }
        }else if (gameTag == "PlayerAttack")  // 由 player 反弹回来的子弹
        {
            if (collision.tag == "Enemy")
            {
               FlyChessController gameObject = GameObject.FindGameObjectWithTag(collision.tag).GetComponent<FlyChessController>();
                // if (gameObject != null) gameObject.Damage(); 
                // Destroy(this.gameObject);
            }
        }
    }

	}
}
