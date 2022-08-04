using QFramework;

namespace QFramework.FlyChess
{
    public class ShootCommand : AbstractCommand
    {
        public static readonly ShootCommand Single = new ShootCommand();
        
        protected override void OnExecute()
        {
            var gunSystem = this.GetSystem<ISkillSystem>();
            // gunSystem.CurrSkill.BulletCountInGun.Value--;

            var gunConfigItem = this.GetModel<ISkillConfigModel>()
                .GetItemByName(gunSystem.CurrSkill.ID.Value);
            
            this.GetSystem<ITimeSystem>().AddDelayTask(1 / gunConfigItem.Frequency, () =>
            {
                gunSystem.CurrSkill.SkillState.Value = SkillState.Idle;

                // if (gunSystem.CurrentGun.BulletCountInGun.Value == 0 &&
                //     gunSystem.CurrentGun.BulletCountOutGun.Value > 0)
                // {
                //     this.SendCommand<ReloadCommand>();
                // }
            });
        }
    }
}