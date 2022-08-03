using System;
using UnityEngine;
using QFramework;

namespace QFramework.FlyChess
{
    public interface IPlayerSystem: ISystem
    {}

    public class PlayerSystem: AbstractSystem, IPlayerSystem
    {
        protected override void OnInit()
        {
            this.RegisterEvent<DirInputEvent>(OnInputDir);
        } 

         private void OnInputDir(DirInputEvent e) 
         {
        //    mCurSnake.GetMoveDir(e.hor, e.ver); 
         }
    }
}