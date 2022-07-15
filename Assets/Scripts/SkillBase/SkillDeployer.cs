using System;
using System.Collections.Generic;
using UnityEngine;
using PlayerCharacter;
namespace SkillSystem
{
    
    /// <summary>
    /// 技能释放器
     /// </summary>
    public abstract class SkillDeployer : MonoBehaviour
    {
        private CharacterData m_PlayerData;
        private AttackCollider attackCollider;
        private SkillData m_SkillData;
        public int m_facingDirection;

        public SkillData skillData
        {
            get { return m_SkillData; }
            set
            {
                m_SkillData = value;
                InitDeployer();
            }
        }

        public CharacterData playerData
        {
            get { return m_PlayerData; }
            set
            {
                m_PlayerData = value;
            }
        }



        private void InitDeployer()
        {
            //Debug.Log("go");
            attackCollider = GetComponent<AttackCollider>();
            attackCollider.m_hitmanager = m_PlayerData;
        }
        
        //执行算法对象
        //选区
        public void CalculateTargets()
        {
        }
        //执行影响效果
        public void ImpactTargets()
        {
        }
        //释放方式
        //技能管理器调用，有子类实现，定义具体释放策略
        public abstract void DeploySkill();

        public void TurnFace()
        {
            m_facingDirection *=-1;
        }
    }
    
}