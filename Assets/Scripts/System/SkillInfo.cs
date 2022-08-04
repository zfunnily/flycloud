using System;
using UnityEngine;
using QFramework;

namespace QFramework.FlyChess
{
    public enum SkillState
    {
        Idle,
        Shooting,
        Reload,
        EmptyBullet,
        CoolDown
    }

 //技能数据
   [Serializable]
    public class SkillData
    {
        /// <summary>技能ID</summary>
        public int ID;
        /// <summary>技能名称</summary>
        public string name;
        /// <summary>技能描述</summary>
        public string description;

        /// <summary>冷却时间</summary>
        public float coolTime;

        /// <summary>冷却剩余时间</summary> 
        public float coolRemain;

        /// <summary>魔法消耗</summary>    
        public float costSp;

        /// <summary>攻击距离</summary>    
        public float attackDistance;

        /// <summary>攻击角度</summary>    
        public float attackAngle;


        /// <summary>连击的下一个技能ID</summary>
        public int nextBatterid;

        /// <summary>伤害比率</summary>    
        public float atkRatio;

        /// <summary>持续时间</summary>    
        public float durationTime;

        /// <summary>伤害间隔</summary>    
        public float atkInterval;

        /// <summary>技能所属对象(OBJ)</summary>

        /// <summary>技能预制件名称</summary>
         public string prefabName;
        /// <summary>技能预制件对象</summary>
        public GameObject skillPrefab;
        /// <summary>动画名称</summary>    
        public string aniamtorName;

        /// <summary>受击特效名称</summary>    
        public string hitFxName;

        /// <summary>受击特效预制件</summary>    
        [HideInInspector] public GameObject hitFxPrefab;

        /// <summary>技能等级</summary>    
        public int level;

        /// <summary>攻击类型</summary>    
        public SkillAttackType attackType;
    }    
    public class SkillInfo
    {
        public BindableProperty<SkillID> ID;
        public BindableProperty<int> BulletCountInGun;

        public BindableProperty<string> Name;

        public BindableProperty<SkillState> SkillState;

        public BindableProperty<int> BulletCountOutGun;

    }
}