using UnityEngine;
using PlayerCharacter;
namespace SkillSystem
{
    //近战释放器 测试
    public class MeleeSkillDeployer: SkillDeployer
    {
        public override void DeploySkill()
        {
            //其他策略 
            skillData.owner.transform.position=new Vector3(skillData.owner.transform.position.x+1,skillData.owner.transform.position.y,skillData.owner.transform.position.z);
        }
        
    }
}