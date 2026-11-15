using System.Text.Json.Serialization;

namespace labaa2.Models
{
    public class CArmoredEnemyTemplate : CEnemyTemplate
    {
        private double armor = 25;

        public double Armor
        {
            get => armor;
            set => armor = value > 0 ? value : 25;
        }
        public override object[] GetExtraArgs() => new object[] { Armor };
        [JsonIgnore] public override string TypeTitle => "Бронированный";
    }
}
