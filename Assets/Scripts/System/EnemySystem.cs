using System;
using UnityEngine;
using QFramework;

namespace QFramework.FlyChess
{
    public interface IEnemySystem: ISystem
    {
    }

    public class EnemySystem: AbstractSystem, IEnemySystem
    {
      protected override void OnInit() 
      {
      }
    }
}