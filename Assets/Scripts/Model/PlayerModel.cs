using UnityEngine;
using UnityEngine.UI;    //添加UI命名空间
using System.Collections;
using QFramework;

namespace QFramework.FlyChess
{
    public interface IPlayerModel: IModel
    {
        BindableProperty<float> HP{ get; set; }
        BindableProperty<float> SP{ get; set;}
        BindableProperty<float> BeAttack{ get; set; }
        BindableProperty<float> Speed{ get; set;}
        BindableProperty<float> JumpForce{ get;set; }
        BindableProperty<float> RollForce{ get; set;}
        BindableProperty<float> NoBlood{ get; set;}
        // BindableProperty<Slider> HPStrip{ get; }
        BindableProperty<float> DeathMoveDistance{ get; set; } // 死亡时 移动的距离
        BindableProperty<Vector2> Face{ get; set;} // 朝向

        BindableProperty<int> Gold { get; set;}

        BindableProperty<int> Score { get; set;}
    }

    public class GameModel : AbstractModel, IPlayerModel
    {
        public BindableProperty<float> HP{ get; set; } = new BindableProperty<float>() { Value = 100 };
        public BindableProperty<float> SP{ get; set;} = new BindableProperty<float>() { Value = 100 };
        public BindableProperty<float> BeAttack{ get; set; } = new BindableProperty<float>() { Value = 10 };
        public BindableProperty<float> Speed{ get; set;} = new BindableProperty<float>() { Value = 2 };
        public BindableProperty<float> JumpForce{ get; set; } = new BindableProperty<float>() { Value = 5 };
        public BindableProperty<float> RollForce{ get; set;} = new BindableProperty<float>() { Value = 20 };
        public BindableProperty<float> NoBlood{ get; set;} = new BindableProperty<float>() { Value = 0};
        // public BindableProperty<Slider> HPStrip{ get; } = new BindableProperty<Slider>() { Value = Get };
        public BindableProperty<float> DeathMoveDistance{ get; set; } = new BindableProperty<float>() { Value = 0 };
        public BindableProperty<Vector2> Face{ get; set;} = new BindableProperty<Vector2>() { Value = new Vector2(1,1) };
        public BindableProperty<int> Gold{ get; set;} = new BindableProperty<int>() { Value = 99 };
        public BindableProperty<int> Score{ get; set;} = new BindableProperty<int>() { Value = 0 };
        public BindableProperty<int> FacingDirection{ get; set;} = new BindableProperty<int>() { Value = 0 };

        protected override void OnInit()
        {
            var storage = this.GetUtility<IStorage>();

            HP.Value = storage.LoadInt(nameof(HP), 100);
            HP.Register(v => storage.SaveFloat(nameof(HP), v));
            
            Gold.Value = storage.LoadInt(nameof(Gold), 0); 
            Gold.Register((v) => storage.SaveInt(nameof(Gold), v)); 
        }
    }
}
