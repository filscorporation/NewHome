namespace Assets.Source.UpgradeManagement
{
    /// <summary>
    /// Upgrade for the golem
    /// </summary>
    public class Upgrade
    {
        public UpgradeType Type;

        public IUpgradeHolder Holder;

        public Upgrade(UpgradeType type, IUpgradeHolder holder)
        {
            Type = type;
            Holder = holder;
        }
    }
}
