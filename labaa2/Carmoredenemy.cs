using System;

namespace labaa2
{
    // Бронированный: броня - процент срезаемого урона (максимум 90%)
    public class CArmoredEnemy : CEnemy
    {
        private readonly double _armor;

        public CArmoredEnemy(string name, string iconName, BigNumber maxHealth, BigNumber reward, double armor)
            : base(name, iconName, maxHealth, reward)
        {
            _armor = Math.Clamp(armor, 0, 90);
        }

        public override string TypeTitle => "ARMORED";

        public override bool TakeDamage(BigNumber damage)
        {
            LastEvent = "ARMOR  -" + _armor.ToString("0") + "% damage";
            BigNumber reduced = damage.Scale(1.0 - _armor / 100.0);
            return ApplyDamage(reduced);
        }
    }
}
