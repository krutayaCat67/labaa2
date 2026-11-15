namespace labaa2
{
    // Общий контракт любого противника в игре
    public interface IEnemy
    {
        string Name { get; }
        string IconName { get; }
        BigNumber MaxHealth { get; }
        BigNumber CurrentHealth { get; }
        BigNumber Reward { get; }
        bool IsDead { get; }
        string TypeTitle { get; }   // для интерфейса: NORMAL / ARMORED / ...
        string LastEvent { get; }   // что произошло при последнем ударе (для StatusText)

        // true - противник побеждён
        bool TakeDamage(BigNumber damage);
    }
}
