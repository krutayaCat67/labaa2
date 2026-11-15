using System;

namespace labaa2
{
    // Абстрактная база: общие свойства и общая логика. Особенности - в наследниках.
    public abstract class CEnemy : IEnemy
    {
        public string Name { get; }
        public string IconName { get; }
        public BigNumber MaxHealth { get; }
        public BigNumber CurrentHealth { get; protected set; }
        public BigNumber Reward { get; }
        public string LastEvent { get; protected set; } = "";

        public bool IsDead => CurrentHealth.IsZero;
        public abstract string TypeTitle { get; }

        protected CEnemy(string name, string iconName, BigNumber maxHealth, BigNumber reward)
        {
            Name = name;
            IconName = iconName;
            MaxHealth = maxHealth.IsZero ? BigNumber.One : maxHealth;
            CurrentHealth = MaxHealth;
            Reward = reward;
        }

        // virtual: наследники переопределяют через override
        public virtual bool TakeDamage(BigNumber damage)
        {
            LastEvent = "";
            return ApplyDamage(damage);
        }

        // Общая часть: вычесть урон, не уходя в минус
        protected bool ApplyDamage(BigNumber damage)
        {
            if (IsDead) return true;

            CurrentHealth = damage >= CurrentHealth ? BigNumber.Zero : CurrentHealth - damage;
            return IsDead;
        }

        // Лечение не выше максимума
        protected void Heal(BigNumber amount)
        {
            BigNumber sum = CurrentHealth + amount;
            CurrentHealth = sum > MaxHealth ? MaxHealth : sum;
        }
    }
}