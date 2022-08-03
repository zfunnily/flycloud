using UnityEngine;
using QFramework;

namespace QFramework.FlyChess
{
    //暂时表现为点击开始按钮的效果
    public class FlyChessController: MonoBehaviour, IController
    {
        // public void Start() => this.SendCommand(new InitGameCommand(20, 20));
        IArchitecture IBelongToArchitecture.GetArchitecture() => FlyChess.Interface;
    }
}