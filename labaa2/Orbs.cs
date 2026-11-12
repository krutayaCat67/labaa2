using System.Windows;
using System.Windows.Media;
namespace labaa2
{
    // Увеличение урона на 5 секунд
    public class CDamageOrb : CCollectable
    {
        public CDamageOrb(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.OrangeRed;
        }

        public override bool OnClick(ClickGame game, Point mousePosition)
        {
            if (!IsMouseOnObject(mousePosition)) return false;
            game.Player.AddEffect(new CDamageBoostEffect(5.0, 2.0)); // урон x2
            return true;
        }
    }

    // Уменьшение перезарядки на 5 секунд
    public class CCooldownOrb : CCollectable
    {
        public CCooldownOrb(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.LimeGreen;
        }

        public override bool OnClick(ClickGame game, Point mousePosition)
        {
            if (!IsMouseOnObject(mousePosition)) return false;
            game.Player.AddEffect(new CCooldownBoostEffect(5.0, 0.5)); // перезарядка x0.5
            return true;
        }
    }

    // Мгновенное золото
    public class CGoldOrb : CCollectable
    {
        public CGoldOrb(Point position, double size, double lifetime)
            : base(position, size, lifetime)
        {
            sprite.Fill = Brushes.Gold;
        }

        public override bool OnClick(ClickGame game, Point mousePosition)
        {
            if (!IsMouseOnObject(mousePosition)) return false;
            // золото считаем от награды текущего противника, чтобы бонус рос вместе с игрой
            BigNumber amount = game.CurrentEnemy.Reward.Scale(3) + BigNumber.One;
            game.Player.AddEffect(new CGoldEffect(amount));
            return true;
        }
    }
}
