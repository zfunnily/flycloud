using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Debug = UnityEngine.Debug;
using QFramework;

namespace QFramework.FlyChess
{
//技能管理器
public abstract  class Skill: FlyChessController
{
    private Transform m_Transform;

    private CharacterData m_PlayerData;

    private SkillInfo m_NowSKill;
    // public List<SkillData> Skills;
    private SkillSystem m_Deployer;

    public CharacterData characterData {
        get { return m_PlayerData; }
        set
        {
            m_PlayerData= value;
        }
    }
    
    private void Awake()
    {
        m_Transform = GetComponent<Transform>();
        m_PlayerData = GetComponent<CharacterData>();
        m_Deployer = GetComponent<SkillSystem>();
    }

    private void Start()
    {
        // for (int i = 0; i < Skills.Count; i++)
        // {
        //     InitSkill(Skills[i]);
        // }
    }
  
    public abstract void Update();

    public void useSkill(int id)
    {
        // // Debug.Log("id: " + id);
        // m_NowSKill = PrepareSkill(id);
        // if (m_NowSKill == null)
        // {
        //     return;
        // }
        // GenerateSkill(m_NowSKill);
    }




    /// <summary>
    /// 生成技能
    /// </summary>
    public void GenerateSkill(SkillData data)
    {
        GameObject skill = Instantiate(data.skillPrefab, m_Transform.position, m_Transform.rotation);
        
        SkillSystem skillDeployer = skill.GetComponent<SkillSystem>();
        if (skillDeployer == null) {
            Debug.Log("SkillDeployer is nil");
        }
        skillDeployer.playerData = m_PlayerData;
        // skillDeployer.skillData = data;
        // skillDeployer.DeploySkill();
        
        //定时销毁
        Destroy(skill, data.durationTime);
        //开启cd
        StartCoroutine(CoolTimeDown(data));
    }
    /// <summary>
    /// 准备释放技能
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public SkillData PrepareSkill(int id)
    {
        // SkillData data = Skills.Find(a => a.id == id);

        // if (data != null && data.coolRemain <= 0 && m_PlayerData.SP >= data.costSp)
        // {
        //     return data;
        // }
        // Debug.Log("当前技能无法释放");
        return null;
    }
    /// <summary>
    /// 初始化技能
    /// </summary>
    /// <param name="data"></param>
    private void InitSkill(SkillData data)
    {
    //    data.skillPrefab = (GameObject)Resources.Load<GameObject>(data.prefabName);
    //    if (data.skillPrefab == null ){
    //         Debug.Log("data.skillperfab is nil :"+ data.prefabName);
    //    }
       
    //    data.owner = gameObject;
       
    }

    /// <summary>
    /// 技能冷却
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    private IEnumerator CoolTimeDown(SkillData data)
    {
        data.coolRemain = data.coolTime;
        while (data.coolRemain > 0)
        {
            yield return new WaitForSeconds(1);
            data.coolRemain--;
        }
        // Debug.Log("技能CD完毕over");
    }
}
}