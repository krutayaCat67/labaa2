using System;
using System.Text.Json.Serialization;

namespace labaa2.Models
{
    // Лечится с шансом при получении урона
    public class CRegeneratingEnemyTemplate : CEnemyTemplate
    {
        private double healChance = 20;   // %
        private double healPercent = 10;  // % от максимального здоровья

        public double HealChance
        {
            get => healChance;
            set => healChance = Math.Clamp(value, 0, 100);
        }

        public double HealPercent
        {
            get => healPercent;
            set => healPercent = value > 0 ? value : 10;
        }
        public override object[] GetExtraArgs() => new object[] { HealChance, HealPercent };
        [JsonIgnore] public override string TypeTitle => "Лечащийся";
    }
}
