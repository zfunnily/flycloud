using System.Collections;
using System.Collections.Generic;
using SkillSystem;
using UnityEngine;

namespace PlayerCharacter
{
//技能管理器
public class EnemySkill: SkillManager
{
    public float attackTime = 1.0f;   // 设置定时器时间 3秒攻击一次
    private float attackCounter = 0; // 计时器变量
    
    public override void Update()
    {
         attackCounter += Time.deltaTime;
         if (attackCounter > attackTime) // 定时器功能实现
         {
            attackCounter = 0;
            useSkill(1);
         }
    }
}
}