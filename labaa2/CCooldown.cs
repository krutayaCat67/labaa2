using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace labaa2
{

    public class CCooldown
    {
        private const double MinDuration = 0.1;

        private double _baseDuration;   // базовая длительность (её улучшают за золото)
        private double _remaining;      // сколько осталось ждать

        // множитель от временных бонусов (меняется только через ChangeMultiplier)
        public double Multiplier { get; private set; } = 1.0;

        public CCooldown(double duration)
        {
            _baseDuration = duration;
        }

        public double BaseDuration => _baseDuration;
        public double Duration => _baseDuration * Multiplier;
        public bool IsReady => _remaining <= 0;
        public bool IsAtMinimum => _baseDuration <= MinDuration + 1e-9;

        public void Start() { _remaining = Duration; }

        public void Update(double dt)
        {
            if (_remaining > 0)
            {
                _remaining -= dt;
            }
        }

        // восстановление из сохранения
        public void SetBase(double duration)
        {
            _baseDuration = Math.Max(MinDuration, duration);
        }

        // постоянное улучшение (за золото)
        public void ReduceBase(double factor)
        {
            _baseDuration = Math.Max(MinDuration, _baseDuration * factor);
        }

        // временный бонус: factor применяется, а при снятии эффекта вызывается с 1/factor
        public void ChangeMultiplier(double factor)
        {
            Multiplier *= factor;
        }
    }
}