using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace labaa2.Models
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies;

        public CEnemyTemplateList()
        {
            enemies = new List<CEnemyTemplate>();
        }

        public void AddEnemy(string name, string iconName, int baseLife,
            double lifeModifier, int baseGold, double goldModifier, double spawnChance)
        {
            enemies.Add(new CEnemyTemplate(name, iconName, baseLife,
                lifeModifier, baseGold, goldModifier, spawnChance));
        }

        public CEnemyTemplate GetEnemyByName(string name)
        {
            return enemies.Find(e => e.Name == name);
        }

        public CEnemyTemplate GetEnemyByIndex(int id)
        {
            if (id < 0 || id >= enemies.Count) return null;
            return enemies[id];
        }

        public void DeleteEnemyByName(string name)
        {
            enemies.RemoveAll(e => e.Name == name);
        }

        public void DeleteEnemyByIndex(int id)
        {
            if (id >= 0 && id < enemies.Count)
                enemies.RemoveAt(id);
        }

        public List<string> GetListOfEnemyNames()
        {
            List<string> names = new List<string>();
            foreach (var e in enemies) names.Add(e.Name);
            return names;
        }

        public List<CEnemyTemplate> GetAllEnemies()
        {
            return enemies;
        }

        public void SaveToJson(string path)
        {
            string json = JsonSerializer.Serialize(enemies);
            File.WriteAllText(path, json);
        }

        public void LoadFromJson(string path)
        {
            if (!File.Exists(path)) return;
            string json = File.ReadAllText(path);
            enemies.Clear();

            using JsonDocument doc = JsonDocument.Parse(json);
            foreach (JsonElement el in doc.RootElement.EnumerateArray())
            {
                string name = el.GetProperty("Name").GetString();
                string icon = el.GetProperty("IconName").GetString();
                int baseLife = el.GetProperty("BaseLife").GetInt32();
                double lifeMod = el.GetProperty("LifeModifier").GetDouble();
                int baseGold = el.GetProperty("BaseGold").GetInt32();
                double goldMod = el.GetProperty("GoldModifier").GetDouble();
                double spawn = el.GetProperty("SpawnChance").GetDouble();

                enemies.Add(new CEnemyTemplate(name, icon, baseLife,
                    lifeMod, baseGold, goldMod, spawn));
            }
        }
    }
}
