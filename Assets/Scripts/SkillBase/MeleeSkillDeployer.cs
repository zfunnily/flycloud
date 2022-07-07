using UnityEngine;

namespace TestSkillSystem
{
    //近战释放器 测试
    public class MeleeSkillDeployer: SkillDeployer
    {
        public override void DeploySkill()
        {
            //执行选区算法
            CalculateTargets();
            
            //执行影响算法
            ImpactTargets();
            
            //其他策略 
            skillData.owner.transform.position=new Vector3(skillData.owner.transform.position.x+1,skillData.owner.transform.position.y,skillData.owner.transform.position.z);
        }
        
    }
}