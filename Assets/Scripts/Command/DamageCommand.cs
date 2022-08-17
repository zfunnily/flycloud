using QFramework;

namespace QFramework.FlyChess
{
    public class DamageCommand : AbstractCommand
    {
        public static readonly DamageCommand Single = new DamageCommand();
        
        protected override void OnExecute()
        {
        }
    }
}