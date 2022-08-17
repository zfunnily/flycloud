using QFramework;
using UnityEngine;

namespace QFramework.FlyChess
{
    public class DamageCommand : AbstractCommand
    {
        // public static readonly DamageCommand Single = new DamageCommand();
        
        protected override void OnExecute()
        {
            var gameModel = this.GetModel<IPlayerModel>();
            DamageEvent dmg = new DamageEvent();
            dmg.HPCost = 20;
            this.SendEvent<DamageEvent>(dmg);
        }
    }
}