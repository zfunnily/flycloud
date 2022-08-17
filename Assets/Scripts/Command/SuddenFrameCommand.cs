using QFramework;

namespace QFramework.FlyChess
{
    public class SuddenFrameCommand : AbstractCommand
    {
        public static readonly SuddenFrameCommand Single = new SuddenFrameCommand();
        
        protected override void OnExecute()
        {
            this.SendEvent(new SuddenFrameEvent());
        }
    }
}