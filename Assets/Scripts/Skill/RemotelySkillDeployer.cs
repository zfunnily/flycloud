using UnityEngine;
using PlayerCharacter;
namespace SkillSystem
{
    //近战释放器 测试
    public class RemotelySkillDeployer: SkillDeployer
    {
        public int facingDirection;
        public override void DeploySkill()
        {
            //其他策略 
            //skillData.owner.transform.position=new Vector3(skillData.owner.transform.position.x,skillData.owner.transform.position.y,skillData.owner.transform.position.z);
            facingDirection = skillData.owner.GetComponent<PlayerMain>().facingDirection;
            transform.position += transform.up;
        }

        private void Update() {
            //飞行一段距离
            Vector3 direct= transform.right  * Time.deltaTime * 8 * facingDirection;
            transform.Translate(direct);
        }
        
    }
}