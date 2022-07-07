using System;
using System.Collections.Generic;
using UnityEngine;
namespace TestSkillSystem
{
    
    /// <summary>
    /// 技能释放器
     /// </summary>
    public abstract class SkillDeployer : MonoBehaviour
    {
        private CharacterStateData m_PlayerData;
        private SkillData m_SkillData;

        public SkillData skillData
        {
            get { return m_SkillData; }
            set
            {
                m_SkillData = value;
                InitDeployer();
            }
        }

        public CharacterStateData playerData
        {
            get { return m_PlayerData; }
            set
            {
                m_PlayerData = value;
            }
        }

        private IAttackSelector m_Selector;
        private List<IImpactEffects> m_Effects;

        private void InitDeployer()
        {
            m_Effects=new List<IImpactEffects>();
           
            //选取类型
            m_Selector = DeployerConfigFactory.CreateAttackSelector(skillData);
            

            //影响效果
            m_Effects = DeployerConfigFactory.CreateImpactEffects(skillData);
            
            //Debug.Log("go");
        }
        
        //执行算法对象
        //选区
        public void CalculateTargets()
        {
            skillData.attackTargets = m_Selector.SelectTarget(skillData, transform);
            
            
        }
        //执行影响效果
        public void ImpactTargets()
        {
            for (int i = 0; i < m_Effects.Count; ++i)
            {
                m_Effects[i].Execute(this);
            }
        }
        //释放方式
        //技能管理器调用，有子类实现，定义具体释放策略
        public abstract void DeploySkill();
    }
    
}