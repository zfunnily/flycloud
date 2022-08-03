using System;
using UnityEngine;
using QFramework;

namespace QFramework.FlyChess
{
    public interface IAISystem: ISystem
    {
    }

    public class AISystem: AbstractSystem, IAISystem
    {
      protected override void OnInit() 
      {
      }
    }
}