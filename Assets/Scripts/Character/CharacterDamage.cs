using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PlayerCharacter
{
    
//My受伤反馈脚本
public partial class MyDamageable : MonoBehaviour
{
    [SerializeField]
    protected int curHitPoints;//当前血量
    protected Action schedule;//多播委托
    protected float myInvincibleTime = 3.5f;//无敌时间
 
    public int myMaxHipPoints;//最大血量
    public bool invincible = false;//无敌的
    public TextMesh m_HitPointsShow;//血量显示
    public UnityEvent OnDamage, OnDeath, OnResetDamage, OnBecmeVulnerable, OnHitWhileInvulunerable;//不同的受伤事件
    //[EnforceType(typeof(MyMessage.IMessageReceiver))]
    //public MonoBehaviour[] DemageReceiver;
    public List<MonoBehaviour> onDamageMessageReceivers;//伤害接收者，不一定有必要
    public int CurHitPoint { get { return curHitPoints; } }
    public void OnReSpawn()//复活
    {
        invincible = true;
        myInvincibleTime = 3.5f;
        OnResetDamage.Invoke();
        curHitPoints = myMaxHipPoints;
    }
    private void Awake()
    {
        curHitPoints = myMaxHipPoints;
        invincible = false;//开始有一段无敌时间
        if(m_HitPointsShow)
            m_HitPointsShow.text = curHitPoints.ToString();
    }
 
    //受到攻击时调用
    // public bool OnGetDamage(MyDamageable.DamageMessage data)
    // {
    //     MyMessage.MesssageType type;
    //     if (invincible || curHitPoints <=0)
    //     {
    //         return false;
    //     }
    //     curHitPoints -= data.amount;
    //     if (m_HitPointsShow)
    //         m_HitPointsShow.text = curHitPoints.ToString();
    //         if (curHitPoints <= 0)
    //     {
    //         schedule += OnDeath.Invoke;//死亡
    //         type = MyMessage.MesssageType.Death;
    //     }
    //     else
    //     {
    //         schedule += OnDamage.Invoke;//仅仅受伤
    //         type = MyMessage.MesssageType.Damage;
    //     }
 
    //     for (int i = 0;i< onDamageMessageReceivers.Count; i++)//向接收者传递信息
    //     {
    //         var receiver = onDamageMessageReceivers[i] as MyMessage.IMessageReceiver;
    //         receiver.OnMessageReceive(type,data.damager,data);
    //     }
    //     return true;
    // }
    //直接死亡，特殊情况可以直接调用，比如跳进沼泽或者岩浆
    public bool Death()
    {
        if (invincible || curHitPoints <= 0)
        {
            return false;
        }
        curHitPoints = 0;
        schedule += OnDeath.Invoke;
        return true;
    }
    private void LateUpdate()
    {
        if(schedule!=null)
        {
            schedule.Invoke();//使用多播委托
            schedule = null;
        }
    }
    private void Update()
    {
        if(invincible)//处于无敌状态
        {
            myInvincibleTime -= Time.deltaTime;
            if(myInvincibleTime <= 0)//无敌时间到
            {
                invincible = false;
                myInvincibleTime = 3.5f;
                OnBecmeVulnerable.Invoke();//恢复正常状态，直接调用，不委托
            }
        }
    }
}
}