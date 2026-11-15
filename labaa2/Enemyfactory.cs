using System;
using System.Linq;
using System.Reflection;

namespace labaa2
{
    public class EnemyFactory
    {
        // Создаёт противника по имени класса, например "CArmoredEnemy".
        // args - параметры конструктора именно этого типа.
        public static IEnemy CreateEnemy(string typeName, params object[] args)
        {
            Type? type = Assembly.GetExecutingAssembly()
                .GetTypes()
                .FirstOrDefault(t => t.Name == typeName
                                     && !t.IsAbstract
                                     && typeof(IEnemy).IsAssignableFrom(t));

            if (type == null)
                throw new ArgumentException($"Неизвестный тип противника: {typeName}");

            return (IEnemy)Activator.CreateInstance(type, args)!;
        }
    }
}
