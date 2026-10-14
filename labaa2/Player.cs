using labaa2.Models;

namespace labaa2
{
    /// <summary>
    /// Игрок. Золото, урон и цена улучшения меняются только через методы.
    /// </summary>
    public class Player
    {
        private const double UpgradeCostGrowth = 1.4; // цена улучшения растёт экспоненциально

        public BigNumber Gold { get; private set; } = BigNumber.Zero;
        public BigNumber Damage { get; private set; } = BigNumber.One;
        public BigNumber UpgradeCost { get; private set; } = new BigNumber(10);
        public int DamageLevel { get; private set; } = 1;

        public bool CanUpgrade => Gold >= UpgradeCost;

        public void AddGold(BigNumber amount)
        {
            Gold = Gold + amount;
        }

        /// <summary>Покупает улучшение урона. Возвращает false, если золота не хватает.</summary>
        public bool TryUpgrade()
        {
            if (!CanUpgrade)
            {
                return false;
            }

            Gold = Gold - UpgradeCost;
            Damage = Damage + Damage / 5 + BigNumber.One;      // +20% и ещё +1
            UpgradeCost = UpgradeCost.Scale(UpgradeCostGrowth); // каждое улучшение дороже
            DamageLevel++;
            return true;
        }
    }
}