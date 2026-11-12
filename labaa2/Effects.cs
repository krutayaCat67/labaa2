using System;
using System.Collections.Generic;
using System.Text;

namespace labaa2
{
    /// <summary>
    /// Временный эффект на игроке. Каждый эффект хранит своё время,
    /// поэтому несколько эффектов могут действовать параллельно.
    /// </summary>
    public abstract class CBonusEffect
    {
        private double _remaining;

        public double Duration { get; }

        protected CBonusEffect(double duration)
        {
            Duration = duration;
            _remaining = duration;
        }

        /// <summary>Возвращает true, если эффект закончился.</summary>
        public bool Tick(double dt)
        {
            _remaining -= dt;
            return _remaining <= 0;
        }

        public abstract void Apply(Player player);            // обязателен у каждого эффекта
        public virtual void Remove(Player player) { }         // у мгновенных эффектов пусто
    }

    public class CDamageBoostEffect : CBonusEffect
    {
        private readonly double _factor;

        public CDamageBoostEffect(double duration, double factor) : base(duration)
        {
            _factor = factor;
        }

        public override void Apply(Player player) { player.ChangeDamageMultiplier(_factor); }
        public override void Remove(Player player) { player.ChangeDamageMultiplier(1.0 / _factor); }
    }

    public class CCooldownBoostEffect : CBonusEffect
    {
        private readonly double _factor; // например 0.5 = перезарядка в 2 раза короче

        public CCooldownBoostEffect(double duration, double factor) : base(duration)
        {
            _factor = factor;
        }

        public override void Apply(Player player) { player.Cooldown.ChangeMultiplier(_factor); }
        public override void Remove(Player player) { player.Cooldown.ChangeMultiplier(1.0 / _factor); }
    }

    // Мгновенный эффект: Duration = 0, срабатывает один раз
    public class CGoldEffect : CBonusEffect
    {
        private readonly BigNumber _amount;

        public CGoldEffect(BigNumber amount) : base(0)
        {
            _amount = amount;
        }

        public override void Apply(Player player) { player.AddGold(_amount); }
    }
}
