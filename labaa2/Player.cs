using labaa2.Models;

namespace labaa2
{
    /// <summary>
    /// Игрок. Золото, урон, перезарядка и эффекты меняются только через методы.
    /// </summary>
    public class Player
    {
        private const double UpgradeCostGrowth = 1.4;          // цена улучшения урона
        private const double CooldownUpgradeCostGrowth = 1.6;  // цена улучшения перезарядки
        private const double CooldownUpgradeFactor = 0.9;      // каждое улучшение -10% к перезарядке

        private readonly List<CBonusEffect> _activeEffects = new List<CBonusEffect>();

        public BigNumber Gold { get; private set; } = BigNumber.Zero;
        public BigNumber Damage { get; private set; } = BigNumber.One;
        public BigNumber UpgradeCost { get; private set; } = new BigNumber(10);
        public int DamageLevel { get; private set; } = 1;

        // ---- НОВОЕ (Лаба 3) ----
        public CCooldown Cooldown { get; } = new CCooldown(0.5);
        public BigNumber CooldownUpgradeCost { get; private set; } = new BigNumber(25);
        public int CooldownLevel { get; private set; } = 1;
        public double DamageMultiplier { get; private set; } = 1.0;
        public int ActiveEffectsCount => _activeEffects.Count;

        // урон с учётом временных бонусов
        public BigNumber EffectiveDamage => Damage.Scale(DamageMultiplier);

        public bool CanUpgrade => Gold >= UpgradeCost;
        public bool CanUpgradeCooldown => Gold >= CooldownUpgradeCost && !Cooldown.IsAtMinimum;

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

        /// <summary>Покупает уменьшение перезарядки (по той же схеме, что TryUpgrade).</summary>
        public bool TryUpgradeCooldown()
        {
            if (!CanUpgradeCooldown)
            {
                return false;
            }

            Gold = Gold - CooldownUpgradeCost;
            Cooldown.ReduceBase(CooldownUpgradeFactor);
            CooldownUpgradeCost = CooldownUpgradeCost.Scale(CooldownUpgradeCostGrowth);
            CooldownLevel++;
            return true;
        }

        public void ChangeDamageMultiplier(double factor)
        {
            DamageMultiplier *= factor;
        }

        public void AddEffect(CBonusEffect effect)
        {
            effect.Apply(this);
            if (effect.Duration > 0)        // мгновенные эффекты в список не попадают
            {
                _activeEffects.Add(effect);
            }
        }

        /// <summary>Вызывается из игрового таймера: перезарядка и истечение эффектов.</summary>
        public void Update(double dt)
        {
            Cooldown.Update(dt);

            for (int i = _activeEffects.Count - 1; i >= 0; i--)
            {
                if (_activeEffects[i].Tick(dt))
                {
                    _activeEffects[i].Remove(this);
                    _activeEffects.RemoveAt(i);
                }
            }
        }
    }
}