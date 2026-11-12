using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace labaa2
{
    /// <summary>Создаёт, обновляет и удаляет бонусные сферы над сценой с противником.</summary>
    public class COrbController
    {
        private const int MaxOrbs = 3;

        private readonly List<CCollectable> _orbs = new List<CCollectable>(); // один список для всех видов
        private readonly ClickGame _game;
        private readonly Random _rng = new Random();
        private Size _sceneSize;
        private double _spawnInterval = 3.0; // сек между появлениями
        private double _time;

        public COrbController(ClickGame game, Size sceneSize)
        {
            _game = game;
            _sceneSize = sceneSize;
        }

        public IReadOnlyList<CCollectable> Orbs => _orbs;

        public void SetSceneSize(Size size) { _sceneSize = size; }

        public void Update(double dt)
        {
            _time += dt;
            if (_time >= _spawnInterval && _orbs.Count < MaxOrbs)
            {
                _time = 0;
                Spawn();
            }

            for (int i = _orbs.Count - 1; i >= 0; i--)
            {
                if (_orbs[i].UpdateLifetime(dt))
                {
                    _orbs.RemoveAt(i);
                }
            }
        }

        private void Spawn()
        {
            if (_sceneSize.Width < 100 || _sceneSize.Height < 100) return;

            double size = 36 + _rng.NextDouble() * 14;
            double lifetime = 3 + _rng.NextDouble() * 3;
            Point pos = new Point(
                _rng.NextDouble() * (_sceneSize.Width - size),
                _rng.NextDouble() * (_sceneSize.Height - size));

            CCollectable orb;
            switch (_rng.Next(3))
            {
                case 0: orb = new CDamageOrb(pos, size, lifetime); break;
                case 1: orb = new CCooldownOrb(pos, size, lifetime); break;
                default: orb = new CGoldOrb(pos, size, lifetime); break;
            }
            _orbs.Add(orb);
        }

        /// <summary>Полиморфизм: вызывается OnClick реального класса сферы.</summary>
        public bool HandleClick(Point mouse)
        {
            for (int i = _orbs.Count - 1; i >= 0; i--)
            {
                if (_orbs[i].OnClick(_game, mouse))
                {
                    _orbs.RemoveAt(i);
                    return true;
                }
            }
            return false;
        }
    }
}
