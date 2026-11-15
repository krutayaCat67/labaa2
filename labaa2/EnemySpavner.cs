using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using labaa2.Models;

namespace labaa2
{

    public class EnemySpawner
    {
        private readonly List<CEnemyTemplate> _templates;
        private readonly double[] _chances; // нормализованные: сумма = 1
        private readonly Random _random = new Random();
        private int _level;                 // сколько противников уже было

        public EnemySpawner(IEnumerable<CEnemyTemplate> templates)
        {
            _templates = new List<CEnemyTemplate>(templates);

            if (_templates.Count == 0)
            {
                throw new ArgumentException("Нет ни одного шаблона противника");
            }

            // Нормализация: делим каждый шанс на сумму всех шансов
            double total = 0;
            foreach (CEnemyTemplate t in _templates)
            {
                total += Math.Max(0, t.SpawnChance);
            }

            _chances = new double[_templates.Count];
            for (int i = 0; i < _templates.Count; i++)
            {
                _chances[i] = total > 0
                    ? Math.Max(0, _templates[i].SpawnChance) / total
                    : 1.0 / _templates.Count; // все шансы нулевые -> равные
            }
        }

        public IEnemy SpawnNext()
        {
            CEnemyTemplate template = PickTemplate();
            IEnemy enemy = CreateEnemy(template, _level);
            _level++;
            return enemy;
        }


        private CEnemyTemplate PickTemplate()
        {
            double roll = _random.NextDouble();
            double cumulative = 0;

            for (int i = 0; i < _templates.Count; i++)
            {
                cumulative += _chances[i];
                if (roll < cumulative)
                {
                    return _templates[i];
                }
            }

            return _templates[_templates.Count - 1]; // страховка от погрешности double
        }

        // Каждый следующий противник сильнее: значения домножаются на модификатор level раз
        private static IEnemy CreateEnemy(CEnemyTemplate template, int level)
        {
            BigNumber life = new BigNumber(Math.Max(1, template.BaseLife));
            BigNumber gold = new BigNumber(Math.Max(0, template.BaseGold));

            for (int i = 0; i < level; i++)
            {
                life = life.Scale(template.LifeModifier);
                gold = gold.Scale(template.GoldModifier);
            }

            // Тип противника определяется именем шаблона: CArmoredEnemyTemplate -> CArmoredEnemy
            string typeName = template.GetType().Name.Replace("Template", "");

            List<object> args = new List<object> { template.Name, template.IconName, life, gold };
            args.AddRange(template.GetExtraArgs());

            return EnemyFactory.CreateEnemy(typeName, args.ToArray());
        }
    }



    public class ClickGame
    {
        private readonly EnemySpawner _spawner;

        private readonly ISaveList<Player> _saver = new PlayerSaver();

        public Player Player { get; private set; } = new Player();
        public IEnemy CurrentEnemy { get; private set; }

        public ClickGame(IEnumerable<CEnemyTemplate> templates)
        {
            _spawner = new EnemySpawner(templates);
            CurrentEnemy = _spawner.SpawnNext();
        }


        // можно ли сейчас бить противника (перезарядка прошла)
        public bool CanClick => Player.Cooldown.IsReady;

        public bool Click()
        {
            if (!CanClick)
            {
                return false; // перезарядка активна — клик игнорируется
            }

            Player.Cooldown.Start(); // и сразу сбрасываем перезарядку

            bool defeated = CurrentEnemy.TakeDamage(Player.EffectiveDamage);

            if (defeated)
            {
                Player.AddGold(CurrentEnemy.Reward);
                CurrentEnemy = _spawner.SpawnNext();
            }

            return defeated;
        }

        // Сохранение прогресса через ISaveList<Player>
        public void SaveProgress(string path) => _saver.Save(Player, path);

        public void LoadProgress(string path)
        {
            try
            {
                if (File.Exists(path)) Player = _saver.Load(path);
            }
            catch (Exception)
            {
                // повреждённый файл сохранения: начинаем с нуля
            }
        }

        public bool TryUpgrade() => Player.TryUpgrade();

        public bool TryUpgradeCooldown() => Player.TryUpgradeCooldown();

        // вызывается из таймера окна
        public void Update(double dt) => Player.Update(dt);
    }
}