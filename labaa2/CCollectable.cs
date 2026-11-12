using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using labaa2.Models;
namespace labaa2
{
    
    public abstract class CCollectable
    {
        protected Point position;   
        protected Size size;
        protected double lifetime;  // оставшееся время жизни, сек
        protected Ellipse sprite;

        protected CCollectable(Point position, double size, double lifetime)
        {
            this.position = position;
            this.size = new Size(size, size);
            this.lifetime = lifetime;

            sprite = new Ellipse
            {
                Width = size,
                Height = size,
                Fill = Brushes.BlueViolet,
                Stroke = Brushes.Black,
                StrokeThickness = 2,
                IsHitTestVisible = false, // клики обрабатывает портрет, а не сама сфера
                RenderTransform = new TranslateTransform(position.X, position.Y)
            };
        }

        public Ellipse GetSprite() => sprite;

        /// <summary>true — время жизни истекло.</summary>
        public bool UpdateLifetime(double dt)
        {
            lifetime -= dt;
            return lifetime <= 0;
        }

        public bool IsMouseOnObject(Point mouse)
        {
            double r = size.Width / 2;
            double dx = mouse.X - (position.X + r);
            double dy = mouse.Y - (position.Y + r);
            return dx * dx + dy * dy <= r * r;
        }

        /// <summary>Абстрактный: у "просто сферы" нет эффекта, его задаёт каждый наследник.</summary>
        public abstract bool OnClick(ClickGame game, Point mousePosition);
    }
}
