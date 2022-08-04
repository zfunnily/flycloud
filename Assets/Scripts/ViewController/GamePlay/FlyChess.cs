using QFramework;
using QFramework.FlyChess;

namespace QFramework.FlyChess
{
    public class FlyChess: Architecture<FlyChess>
    {
        protected override void Init()
        {
            RegisterSystem<IEnemySystem>(new EnemySystem());

            RegisterSystem<ITimeSystem>(new TimeSystem());
            RegisterSystem<IInputSystem>(new InputSystem());
            RegisterSystem<IAISystem>(new AISystem());

            RegisterModel<IPlayerModel>(new GameModel());

            RegisterUtility<IStorage>(new PlayerPrefsStorage());
        }
    }
}