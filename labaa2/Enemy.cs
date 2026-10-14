using System;
using System.Collections.Generic;
using System.Text;

namespace labaa2
{
    /// <summary>
    /// Противник на сцене (не шаблон из Лабы 1).
    /// Состояние меняется только через TakeDamage.
    /// </summary>
    public class Enemy
    {
        public string Name { get; private set; }
        public string IconName { get; private set; }
        public BigNumber MaxHealth { get; private set; }
        public BigNumber CurrentHealth { get; private set; }
        public BigNumber Reward { get; private set; }

        public bool IsDead => CurrentHealth.IsZero;

        public Enemy(string name, string iconName, BigNumber maxHealth, BigNumber reward)
        {
            Name = name;
            IconName = iconName;
            MaxHealth = maxHealth.IsZero ? BigNumber.One : maxHealth;
            CurrentHealth = MaxHealth;
            Reward = reward;
        }

        /// <summary>Наносит урон. Возвращает true, если противник побеждён.</summary>
        public bool TakeDamage(BigNumber damage)
        {
            if (IsDead)
            {
                return true;
            }

            if (damage >= CurrentHealth)
            {
                CurrentHealth = BigNumber.Zero; // не уходим в минус
            }
            else
            {
                CurrentHealth = CurrentHealth - damage;
            }

            return IsDead;
        }
    }
}