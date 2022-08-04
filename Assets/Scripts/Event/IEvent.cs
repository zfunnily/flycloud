using UnityEngine;
namespace QFramework.FlyChess
{
internal struct DirInputEvent
{
    public int hor, ver;
}
public struct DamageEvent
{
    public float HPCost;
    public float SPCost;
}

public struct SkillEvent
{
    public SkillID ID;
}

}