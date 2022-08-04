using QFramework;

namespace QFramework.FlyChess
{
    public class PickSkillCommand : AbstractCommand
    {
        private readonly SkillID ID;
        private readonly int mInGunBullets;
        private readonly int mOutGunBullets;

        public PickSkillCommand(SkillID id, int inGunBullets, int outGunBullets)
        {
            ID = id;
            mInGunBullets = inGunBullets;
            mOutGunBullets = outGunBullets;
        }
        
        protected override void OnExecute()
        {
            this.GetSystem<ISkillSystem>()
                .PickSkill(ID,mInGunBullets,mOutGunBullets);
        }
    }
}