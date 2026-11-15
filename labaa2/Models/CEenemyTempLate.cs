using System;

namespace labaa2.Models
{
    public abstract class CEnemyTemplate
    {
        private string name = "unknown";
        private string iconName = "";
        private int baseLife = 1;
        private double lifeModifier = 1;
        private int baseGold = 0;
        private double goldModifier = 1;
        private double spawnChance = 0;

        public string Name
        {
            get => name;
            set => name = string.IsNullOrEmpty(value) ? "unknown" : value;
        }

        public string IconName
        {
            get => iconName;
            set => iconName = value ?? "";
        }

        public int BaseLife
        {
            get => baseLife;
            set => baseLife = value > 0 ? value : 1;
        }

        public double LifeModifier
        {
            get => lifeModifier;
            set => lifeModifier = value > 0 ? value : 1;
        }

        public int BaseGold
        {
            get => baseGold;
            set => baseGold = value >= 0 ? value : 0;
        }

        public double GoldModifier
        {
            get => goldModifier;
            set => goldModifier = value > 0 ? value : 1;
        }

        public double SpawnChance
        {
            get => spawnChance;
            set => spawnChance = Math.Clamp(value, 0, 100);
        }

        // Для UI: человекочитаемое название типа
        public abstract string TypeTitle { get; }

        // Доп. параметры конструктора игрового противника (у каждого типа свои)
        public virtual object[] GetExtraArgs() => Array.Empty<object>();

        // Нужен System.Text.Json
        protected CEnemyTemplate() { }

        public void SetBase(string name, string iconName, int baseLife,
            double lifeModifier, int baseGold, double goldModifier, double spawnChance)
        {
            Name = name;
            IconName = iconName;
            BaseLife = baseLife;
            LifeModifier = lifeModifier;
            BaseGold = baseGold;
            GoldModifier = goldModifier;
            SpawnChance = spawnChance;
        }
    }
}