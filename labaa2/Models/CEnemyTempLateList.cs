using System.Collections.Generic;

namespace labaa2.Models
{
    public class CEnemyTemplateList
    {
        private List<CEnemyTemplate> enemies = new List<CEnemyTemplate>();

        // Работаем через интерфейс: формат можно сменить заменой одной строки
        private readonly ISaveList<List<CEnemyTemplate>> _serializer = new JsonEnemySaver();

        public void AddEnemy(CEnemyTemplate enemy)
        {
            enemies.Add(enemy);
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

        public List<CEnemyTemplate> GetAllEnemies() => enemies;

        public void SaveToJson(string path) => _serializer.Save(enemies, path);

        public void LoadFromJson(string path) => enemies = _serializer.Load(path);
    }
}
