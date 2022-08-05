using System.Collections.Generic;
using QFramework;

namespace QFramework.FlyChess
{
    public enum SkillID
    {
        UNKNOWN,
        LIGHTNING,

    }
    public interface ISkillConfigModel : IModel
    {
        public SkillConfigItem GetItemByName(SkillID skillName);
    }

    public class SkillConfigItem
    {
        public SkillConfigItem(string name, int bulletMaxCount, float attack, float frequency, float shootDistance,
            bool needBullet, float reloadSeconds, string description)
        {
            Name = name;
            BulletMaxCount = bulletMaxCount;
            Attack = attack;
            Frequency = frequency;
            ShootDistance = shootDistance;
            NeedBullet = needBullet;
            ReloadSeconds = reloadSeconds;
            Description = description;
        }
        
        public string Name { get; set; }
        public int BulletMaxCount { get; set; }
        public float Attack { get; set; }
        public float Frequency { get; set; }
        public float ShootDistance { get; set; }
        public bool NeedBullet { get; set; }
        public float ReloadSeconds { get; set; }
        public string Description { get; set; }
    }

    public class SkillConfigModel : AbstractModel, ISkillConfigModel
    {
        protected override void OnInit()
        {
        }

        private Dictionary<SkillID, SkillConfigItem> mItems = new Dictionary<SkillID, SkillConfigItem>()
        {
            { SkillID.LIGHTNING, new SkillConfigItem("闪电", 7, 1, 1, 0.5f, false, 3, "默认")}
        };

        public SkillConfigItem GetItemByName(SkillID gunName)
        {
            return mItems[gunName];
        }
    }
}