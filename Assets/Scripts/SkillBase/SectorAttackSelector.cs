using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using PlayerCharacter;
namespace TestSkillSystem
{
    
    public class SectorAttackSelector : IAttackSelector
    {
       /// <summary> 
       /// 扇形/圆形选区
       /// </summary>
       /// <param name="data"></param>
       /// <param name="skillTF">技能所在位置</param>
       /// <returns></returns>
        public List<Transform> SelectTarget(SkillData data, Transform skillTF)
       {
           //获取目标
           List<string> tags = data.attackTargetTags;
           List<Transform> resTrans = new List<Transform>();
           for (int i = 0; i < tags.Count; ++i)
           {
                GameObject[] tempGOArray = GameObject.FindGameObjectsWithTag(tags[i]);
                if (tempGOArray.Length == 0)
                {
                    Debug.Log("标签数组为空");
                }
                    
                foreach (var VARIABLE in tempGOArray)
                {
                    resTrans.Add(VARIABLE.transform);
                }
           }
           //判断攻击范围
           resTrans.FindAll(res => Vector3.Distance(res.position, skillTF.position) <= data.attackDistance
                                   && Vector3.Angle(skillTF.forward,res.position-skillTF.position)<=data.attackAngle/2);
           //筛选出活得角色
           resTrans.FindAll(res => res.GetComponent<CharacterStateData>().HP > 0);
           //返回目标
           //依据 单体/群体
           if (data.attackType == SkillAttackType.Group||resTrans.Count==0 )
               return resTrans;
           //huoqu 距离最近的
           int index = 0;
           float min = Vector3.Distance(resTrans[0].position, skillTF.position);
           for (int i=0;i<resTrans.Count;++i)
           {
               float temp = Vector3.Distance(resTrans[i].position, skillTF.position);
               if (temp < min)
               {
                   min = temp;
                   index = i;
               }
           }
           return new List<Transform> {resTrans[index]};
       }
    }
}