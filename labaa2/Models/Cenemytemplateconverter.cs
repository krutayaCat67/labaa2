using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace labaa2.Models
{
    // Полиморфная сериализация: в JSON добавляется поле-дискриминатор "$type"
    public class EnemyTemplateConverter : JsonConverter<CEnemyTemplate>
    {
        public override CEnemyTemplate Read(ref Utf8JsonReader reader,
            Type typeToConvert, JsonSerializerOptions options)
        {
            using JsonDocument doc = JsonDocument.ParseValue(ref reader);
            string raw = doc.RootElement.GetRawText();

            // Старые файлы без $type грузим как обычного противника
            string type = "CNormalEnemyTemplate";
            if (doc.RootElement.TryGetProperty("$type", out JsonElement t))
                type = t.GetString();

            // ВАЖНО: каждый новый тип противника нужно дописать сюда
            switch (type)
            {
                case "CNormalEnemyTemplate":
                    return JsonSerializer.Deserialize<CNormalEnemyTemplate>(raw, options);
                case "CArmoredEnemyTemplate":
                    return JsonSerializer.Deserialize<CArmoredEnemyTemplate>(raw, options);
                case "CRegeneratingEnemyTemplate":
                    return JsonSerializer.Deserialize<CRegeneratingEnemyTemplate>(raw, options);
                default:
                    throw new NotSupportedException($"Unknown type: {type}");
            }
        }

        public override void Write(Utf8JsonWriter writer, CEnemyTemplate value,
            JsonSerializerOptions options)
        {
            // Сериализуем по реальному типу, чтобы попали поля наследника
            string json = JsonSerializer.Serialize(value, value.GetType(), options);
            using JsonDocument doc = JsonDocument.Parse(json);

            writer.WriteStartObject();
            writer.WriteString("$type", value.GetType().Name);
            foreach (JsonProperty p in doc.RootElement.EnumerateObject())
                p.WriteTo(writer);
            writer.WriteEndObject();
        }
    }
}