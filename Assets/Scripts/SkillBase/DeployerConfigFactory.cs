using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace TestSkillSystem
{
    //算法工厂
    public static class DeployerConfigFactory 
    {
        
        //命名规则 TestSkillSystem + 枚举名 +AttackSelector
        //例如扇形： TestSkillSystem
        /// <summary>
        /// 创建选区算法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static IAttackSelector CreateAttackSelector(SkillData data)
        {
            string name = $"TestSkillSystem.{data.selectorType}AttackSelector";
           // Debug.Log(name);
            return CreateObject<IAttackSelector>(name);
        }
        //影响效果命名规范
        //同上 TestSkillSystem. + impactype[?] +Impact 
        /// <summary>
        /// 创建影响算法
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static List<IImpactEffects> CreateImpactEffects(SkillData data)
        {
            List<IImpactEffects> temp= new List<IImpactEffects>();
            for (int i = 0; i < data .impactype.Count; ++i)
            {
                string className = $"TestSkillSystem.{data.impactype[i]}Impact";
                //Debug.Log(className);
                temp.Add( CreateObject<IImpactEffects>(className));
            }

            return temp;
        }
        
        private static T CreateObject<T>(string className) where T : class
        {

            Type type = Type.GetType(className);
            if (type == null)
            {
                Debug.Log($"Type为空ClssName为：{className} ");
            }
            return Activator.CreateInstance(type) as T;
        }
    }

}
