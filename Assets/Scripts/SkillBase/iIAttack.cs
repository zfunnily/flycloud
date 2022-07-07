using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TestSkillSystem
{
    //选区接口
    public interface IAttackSelector
    {
        /// <summary>
        /// 搜索目标
        /// </summary>
        /// <param name="data"></param>
        /// <param name="skillTF"></param>
        /// <returns></returns>
        List<Transform> SelectTarget(SkillData data, Transform skillTF);

    }


}

namespace TestSkillSystem
{
    //影响效果接口
    public interface IImpactEffects
    {
        
        //执行
        void Execute(SkillDeployer deployer);
    }

}
