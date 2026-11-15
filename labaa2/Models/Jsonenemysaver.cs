using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace labaa2.Models
{
    public class JsonEnemySaver : ISaveList<List<CEnemyTemplate>>
    {
        private readonly JsonSerializerOptions _options;

        public JsonEnemySaver()
        {
            _options = new JsonSerializerOptions
            {
                WriteIndented = true,
                Converters = { new EnemyTemplateConverter() }
            };
        }

        public List<CEnemyTemplate> Load(string path)
        {
            if (!File.Exists(path)) return new List<CEnemyTemplate>();
            string json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<List<CEnemyTemplate>>(json, _options)
                   ?? new List<CEnemyTemplate>();
        }

        public void Save(List<CEnemyTemplate> data, string path)
        {
            File.WriteAllText(path, JsonSerializer.Serialize(data, _options));
        }
    }
}
