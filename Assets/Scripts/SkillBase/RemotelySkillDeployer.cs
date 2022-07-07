using UnityEngine;

namespace TestSkillSystem
{
    //近战释放器 测试
    public class RemotelySkillDeployer: SkillDeployer
    {
        public override void DeploySkill()
        {
            //执行选区算法
            CalculateTargets();
            
            //执行影响算法
            ImpactTargets();
            
            //其他策略 
            //skillData.owner.transform.position=new Vector3(skillData.owner.transform.position.x,skillData.owner.transform.position.y,skillData.owner.transform.position.z);
        }

        private void Update() {
            //飞行一段距离
            playerData = skillData.owner.GetComponent<CharacterStateData>();
            Vector3 direct= transform.right  * Time.deltaTime * 2;
            if (playerData.facingDirection <= 0)
            {
                direct= transform.right * Time.deltaTime * 2;
            }
            
             transform.Translate(direct);
        }
        
    }
}