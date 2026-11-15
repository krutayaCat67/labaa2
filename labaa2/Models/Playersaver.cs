using System.IO;
using System.Text.Json;

namespace labaa2.Models
{
    // То, что реально лежит в JSON (BigNumber хранится строкой - так он не теряет точность)
    public class PlayerData
    {
        public string Gold { get; set; } = "0";
        public string Damage { get; set; } = "1";
        public string UpgradeCost { get; set; } = "10";
        public int DamageLevel { get; set; } = 1;
        public double CooldownBase { get; set; } = 0.5;
        public string CooldownUpgradeCost { get; set; } = "25";
        public int CooldownLevel { get; set; } = 1;
    }

    // Тот же интерфейс ISaveList<T>, что и у списка противников, но для игрока
    public class PlayerSaver : ISaveList<Player>
    {
        private readonly JsonSerializerOptions _options = new JsonSerializerOptions { WriteIndented = true };

        public Player Load(string path)
        {
            Player player = new Player();
            if (!File.Exists(path)) return player;

            PlayerData? data = JsonSerializer.Deserialize<PlayerData>(File.ReadAllText(path), _options);
            if (data == null) return player;

            player.Restore(
                BigNumber.Parse(data.Gold),
                BigNumber.Parse(data.Damage),
                BigNumber.Parse(data.UpgradeCost),
                data.DamageLevel,
                data.CooldownBase,
                BigNumber.Parse(data.CooldownUpgradeCost),
                data.CooldownLevel);
            return player;
        }

        public void Save(Player player, string path)
        {
            PlayerData data = new PlayerData
            {
                Gold = player.Gold.ToString(),
                Damage = player.Damage.ToString(),
                UpgradeCost = player.UpgradeCost.ToString(),
                DamageLevel = player.DamageLevel,
                CooldownBase = player.Cooldown.BaseDuration,
                CooldownUpgradeCost = player.CooldownUpgradeCost.ToString(),
                CooldownLevel = player.CooldownLevel
            };
            File.WriteAllText(path, JsonSerializer.Serialize(data, _options));
        }
    }
}