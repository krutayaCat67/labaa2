namespace labaa2
{
    // Обычный: ничего не переопределяет
    public class CNormalEnemy : CEnemy
    {
        public CNormalEnemy(string name, string iconName, BigNumber maxHealth, BigNumber reward)
            : base(name, iconName, maxHealth, reward) { }

        public override string TypeTitle => "NORMAL";
    }
}
