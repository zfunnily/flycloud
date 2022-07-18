using System.Collections;
using System.Collections.Generic;
using SkillSystem;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace PlayerCharacter
{
//技能管理器
public class PlayerSkill : SkillManager
{
    
    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
           useSkill(1);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
           useSkill(2);
        }

        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
           useSkill(3);
        }
    }
}
}