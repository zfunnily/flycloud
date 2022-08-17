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
    public Vector2 direction;
}

public struct SkillEvent
{
    public SkillID ID;
}

public struct SuddenFrameEvent
{

}

}