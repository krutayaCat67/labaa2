using System;

namespace labaa2
{
    // Лечащийся: с шансом вместо получения урона восстанавливает часть здоровья
    public class CRegeneratingEnemy : CEnemy
    {
        private static readonly Random _random = new Random();
        private readonly double _healChance;   // %
        private readonly double _healPercent;  // % от максимума

        public CRegeneratingEnemy(string name, string iconName, BigNumber maxHealth, BigNumber reward,
            double healChance, double healPercent)
            : base(name, iconName, maxHealth, reward)
        {
            _healChance = Math.Clamp(healChance, 0, 100);
            _healPercent = Math.Clamp(healPercent, 0, 100);
        }

        public override string TypeTitle => "REGENERATING";

        public override bool TakeDamage(BigNumber damage)
        {
            LastEvent = "";

            if (!IsDead && _random.NextDouble() * 100 < _healChance)
            {
                BigNumber amount = MaxHealth.Scale(_healPercent / 100.0);
                Heal(amount);
                LastEvent = "REGENERATED  +" + amount.ToShortString();
                return false;
            }

            return ApplyDamage(damage);
        }
    }
}
