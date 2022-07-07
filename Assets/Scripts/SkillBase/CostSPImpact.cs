using System.Collections.Generic;
using UnityEngine;

namespace TestSkillSystem
{
    //具体影响效果
    public class CostSPImpact: IImpactEffects
    {
        
        public void Execute(SkillDeployer deployer)
        {
            Debug.Log($"消耗法力值执行：{deployer.skillData.owner.name}");
            var staus = deployer.skillData.owner.GetComponent<CharacterStateData>();
            if(staus==null)
                Debug.Log("状态为空");
            staus.SP -= deployer.skillData.costSp;
        }
    }

    public class HurtHPImpact : IImpactEffects
    {
        public void Execute(SkillDeployer deployer)
        {
            Debug.Log($"伤害生命执行:{deployer.skillData.owner.name}");
            List<Transform> Temp = deployer.skillData.attackTargets;
            for (int i = 0; i < Temp.Count; ++i)
            {
                Debug.Log($"{Temp[i].gameObject.name} hp执行前: {Temp[i].GetComponent<CharacterStateData>().HP}");
                Temp[i].GetComponent<CharacterStateData>().HP-=deployer.skillData.owner.GetComponent<CharacterStateData>().BeAttack;
                Debug.Log($"{Temp[i].gameObject.name} hp施行后: {Temp[i].GetComponent<CharacterStateData>().HP}");
            }


        }
    }
    public class AddSPImpact : IImpactEffects
    {
        public void Execute(SkillDeployer deployer)
        {
            Debug.Log($"回复法力值执行:{deployer.skillData.owner.name}");
            var staus = deployer.skillData.owner.GetComponent<CharacterStateData>();
            if(null==staus)
                Debug.Log("状态为空");
            staus.SP += deployer.skillData.owner.GetComponent<CharacterStateData>().BeAttack*0.5f;
        }
    }
}