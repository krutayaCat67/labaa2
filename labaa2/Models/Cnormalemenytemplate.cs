using System.Text.Json.Serialization;

namespace labaa2.Models
{
    public class CNormalEnemyTemplate : CEnemyTemplate
    {
        [JsonIgnore] public override string TypeTitle => "Обычный";
    }
}