using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;
using QFramework;

namespace QFramework.FlyChess
{

    /// <summary>
    /// 攻击类型 群体/单体
    /// </summary>
    public enum SkillAttackType
    {
        Group,
        Alone
    }

   
    public interface ISkillSystem: ISystem
    {
        SkillInfo CurrSkill{get;}
        Queue<SkillInfo> SkillInfos{ get; }
        public void PickSkill(SkillID ID, int bulletCountInGun, int bulletCountOutGun);
        void ShiftSkill();
    }

    public class OnCurrentSkillChanged
    {
        public SkillID ID{ get; set; }
    }
    public class SkillSystem :AbstractSystem, ISkillSystem
    {
        public Queue<SkillInfo> SkillInfos=> mSkillInfos;
        private Queue<SkillInfo> mSkillInfos = new Queue<SkillInfo>();
        private FlyChessController m_PlayerData;
        public int m_facingDirection;


        public FlyChessController playerData
        {
            get { return m_PlayerData; }
            set
            {
                m_PlayerData = value;
                // attackCollider = GetComponent<AttackCollider>();
                // attackCollider.m_player = m_PlayerData;
            }
        }
        public SkillInfo CurrSkill { get; } = new SkillInfo()
        {
            ID = new BindableProperty<SkillID>()
            {
                Value = SkillID.LIGHTNING
            },
            BulletCountInGun = new BindableProperty<int>()
            {
                Value = 3
            },
            BulletCountOutGun = new BindableProperty<int>()
            {
                Value = 1
            },
            Name = new BindableProperty<string>()
            {
                Value = "手枪"
            },
            SkillState = new BindableProperty<SkillState>()
            {
                Value = SkillState.Idle
            }
        };



        protected override void OnInit()
        {
            //Debug.Log("go");
        }
        public void PickSkill(SkillID ID, int bulletCountInGun, int bulletCountOutGun)
        {
            // 如果是当前枪
            if (CurrSkill.ID.Value == ID)
            {
                CurrSkill.BulletCountOutGun.Value += bulletCountInGun;
                CurrSkill.BulletCountOutGun.Value += bulletCountOutGun;
            } else if (mSkillInfos.Any(skillInfo => skillInfo.ID.Value == ID))
            {
                var skillInfo = mSkillInfos.First(info => info.ID.Value == ID);
                skillInfo.BulletCountOutGun.Value += bulletCountInGun;
                skillInfo.BulletCountOutGun.Value += bulletCountOutGun;
            }
            else
            {
                EnqueueCurrentGun(ID, bulletCountInGun, bulletCountOutGun);
            }
        }
        
        //释放方式
        //技能管理器调用，有子类实现，定义具体释放策略
        public void ShiftSkill()
        {

        }

        public void TurnFace()
        {
            m_facingDirection *=-1;
        }

        void EnqueueCurrentGun(SkillID nextSkillID, int nextBulletCountInGun, int nextBulletCountOutGun)
        {
            var currentGunInfo = new SkillInfo
            {
                Name = new BindableProperty<string>()
                {
                    Value = CurrSkill.Name.Value
                },
                BulletCountInGun = new BindableProperty<int>()
                {
                    Value = CurrSkill.BulletCountInGun.Value
                },
                BulletCountOutGun = new BindableProperty<int>()
                {
                    Value = CurrSkill.BulletCountOutGun.Value
                },
                SkillState= new BindableProperty<SkillState>()
                {
                    Value = CurrSkill.SkillState.Value
                }
            };

            mSkillInfos.Enqueue(currentGunInfo);

            CurrSkill.ID.Value = nextSkillID;
            CurrSkill.BulletCountInGun.Value = nextBulletCountInGun;
            CurrSkill.BulletCountOutGun.Value = nextBulletCountOutGun;

            this.SendEvent(new OnCurrentSkillChanged()
            {
                ID = nextSkillID
            });
        }
    }
}