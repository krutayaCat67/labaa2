using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Runtime.Intrinsics.Arm;
using System.Text;

namespace labaa2
{
    public class BigNumber
    {
        private const int Base = 1000;          // основание системы счисления
        private const int BaseDigits = 3;       // сколько десятичных цифр в блоке
        private const int ScaleDenominator = 1_000_000; // для умножения на дробные модификаторы

        private readonly int[] _blocks;         // readonly + private = число не изменить снаружи

        public static readonly BigNumber Zero = new BigNumber(0);
        public static readonly BigNumber One = new BigNumber(1);

        // ---------- Конструкторы ----------

        public BigNumber(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Число не может быть отрицательным");
            }

            List<int> list = new List<int>();

            while (value > 0)
            {
                list.Add((int)(value % Base)); // младший блок
                value /= Base;                 // отбрасываем его
            }

            if (list.Count == 0)
            {
                list.Add(0);                   // ноль хранится как [0]
            }

            _blocks = list.ToArray();
        }

        // Служебный: принимает массив блоков, убирает ведущие (старшие) нули и копирует массив.
        private BigNumber(int[] blocks)
        {
            if (blocks.Length == 0)
            {
                throw new ArgumentException("Массив не должен быть пустым");
            }

            int length = blocks.Length;

            while (length > 1 && blocks[length - 1] == 0)
            {
                length--;
            }

            int[] numbers = new int[length];
            Array.Copy(blocks, numbers, length);
            _blocks = numbers;
        }

        public bool IsZero => _blocks.Length == 1 && _blocks[0] == 0;

        // ---------- Вывод ----------

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_blocks[_blocks.Length - 1]); // старший блок - без дополнения нулями

            for (int i = _blocks.Length - 2; i >= 0; i--)
            {
                sb.Append(_blocks[i].ToString().PadLeft(BaseDigits, '0'));
            }

            return sb.ToString();
        }

        // Короткая запись для интерфейса
        public string ToShortString()
        {
            string s = ToString();
            if (s.Length <= 9)
            {
                return s;
            }
            return s[0] + "." + s.Substring(1, 2) + "e" + (s.Length - 1);
        }

        // 

        public int CompareTo(BigNumber other)
        {
            ArgumentNullException.ThrowIfNull(other);

            // 1. Ведущие нули обрезаны, поэтому больше блоков = больше число
            if (_blocks.Length < other._blocks.Length) return -1;
            if (_blocks.Length > other._blocks.Length) return 1;

            // 2. Длины равны: сравниваем от старшего блока к младшему
            for (int i = _blocks.Length - 1; i >= 0; i--)
            {
                if (_blocks[i] < other._blocks[i]) return -1;
                if (_blocks[i] > other._blocks[i]) return 1;
            }

            // 3. Все блоки совпали
            return 0;
        }

        public static bool operator <(BigNumber a, BigNumber b) => a.CompareTo(b) < 0;
        public static bool operator >(BigNumber a, BigNumber b) => a.CompareTo(b) > 0;
        public static bool operator <=(BigNumber a, BigNumber b) => a.CompareTo(b) <= 0;
        public static bool operator >=(BigNumber a, BigNumber b) => a.CompareTo(b) >= 0;

        public static bool operator ==(BigNumber? a, BigNumber? b)
        {
            if (ReferenceEquals(a, b)) return true;
            if (a is null || b is null) return false;
            return a.CompareTo(b) == 0;
        }

        public static bool operator !=(BigNumber? a, BigNumber? b) => !(a == b);

        public override bool Equals(object? obj) => obj is BigNumber other && CompareTo(other) == 0;

        public override int GetHashCode()
        {
            HashCode hash = new HashCode();
            foreach (int block in _blocks)
            {
                hash.Add(block);
            }
            return hash.ToHashCode();
        }

        // Арифметика (вся логика здесь, операторы только вызывают эти методы) 

        // Сложение столбиком с переносом
        private BigNumber Add(BigNumber other)
        {
            int[] a = _blocks;
            int[] b = other._blocks;
            int carry = 0;
            int[] result = new int[Math.Max(a.Length, b.Length) + 1]; // +1 блок на случай переноса

            for (int i = 0; i < result.Length; i++)
            {
                int valA = (i < a.Length) ? a[i] : 0;
                int valB = (i < b.Length) ? b[i] : 0;
                int sum = valA + valB + carry;
                result[i] = sum % Base; // остаётся в блоке
                carry = sum / Base;     // уходит в следующий блок
            }

            return new BigNumber(result);
        }

        // Вычитание столбиком с займом (только если this >= other)
        private BigNumber Subtract(BigNumber other)
        {
            if (CompareTo(other) < 0)
            {
                throw new InvalidOperationException("Результат вычитания был бы отрицательным");
            }

            int[] a = _blocks;
            int[] b = other._blocks;
            int[] result = new int[a.Length];
            int borrow = 0;

            for (int i = 0; i < a.Length; i++)
            {
                int valB = (i < b.Length) ? b[i] : 0;
                int diff = a[i] - valB - borrow;

                if (diff < 0)
                {
                    diff += Base; // занимаем 1 у следующего блока
                    borrow = 1;
                }
                else
                {
                    borrow = 0;
                }

                result[i] = diff;
            }

            return new BigNumber(result);
        }

        // Умножение "в столбик": блок a[i] * блок b[j] попадает в позицию i + j
        private BigNumber Multiply(BigNumber other)
        {
            int[] a = _blocks;
            int[] b = other._blocks;
            int[] result = new int[a.Length + b.Length];

            for (int i = 0; i < a.Length; i++)
            {
                int carry = 0;

                for (int j = 0; j < b.Length; j++)
                {
                    int current = result[i + j] + a[i] * b[j] + carry;
                    result[i + j] = current % Base;
                    carry = current / Base;
                }

                result[i + b.Length] += carry;
            }

            return new BigNumber(result);
        }

        // Деление на обычное целое: остаток передаётся в следующий (младший) разряд
        private BigNumber Divide(int divisor)
        {
            if (divisor == 0)
            {
                throw new DivideByZeroException();
            }
            if (divisor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(divisor), "Делитель должен быть положительным");
            }

            int[] result = new int[_blocks.Length];
            long remainder = 0;

            for (int i = _blocks.Length - 1; i >= 0; i--)
            {
                long current = remainder * Base + _blocks[i]; // остаток * 1000 + очередной блок
                result[i] = (int)(current / divisor);
                remainder = current % divisor;
            }

            return new BigNumber(result);
        }

        // Деление BigNumber на BigNumber (столбиком, каждую цифру частного подбираем бинарным поиском)
        private BigNumber Divide(BigNumber divisor)
        {
            if (divisor.IsZero)
            {
                throw new DivideByZeroException();
            }

            if (divisor._blocks.Length == 1)
            {
                return Divide(divisor._blocks[0]); // быстрый путь
            }

            int[] quotient = new int[_blocks.Length];
            BigNumber remainder = Zero;
            BigNumber baseNumber = new BigNumber(Base);

            for (int i = _blocks.Length - 1; i >= 0; i--)
            {
                // остаток = остаток * 1000 + очередной блок
                remainder = remainder.Multiply(baseNumber).Add(new BigNumber(_blocks[i]));

                // ищем наибольшее q в [0, 999], что divisor * q <= остаток
                int low = 0;
                int high = Base - 1;
                while (low < high)
                {
                    int mid = (low + high + 1) / 2;
                    if (divisor.Multiply(new BigNumber(mid)) <= remainder)
                    {
                        low = mid;
                    }
                    else
                    {
                        high = mid - 1;
                    }
                }

                quotient[i] = low;
                remainder = remainder.Subtract(divisor.Multiply(new BigNumber(low)));
            }

            return new BigNumber(quotient);
        }

        public static BigNumber operator +(BigNumber a, BigNumber b) => a.Add(b);
        public static BigNumber operator -(BigNumber a, BigNumber b) => a.Subtract(b);
        public static BigNumber operator *(BigNumber a, BigNumber b) => a.Multiply(b);
        public static BigNumber operator /(BigNumber a, BigNumber b) => a.Divide(b);
        public static BigNumber operator /(BigNumber a, int b) => a.Divide(b);

        // Умножение на дробный модификатор (например 1.15):
        // this * 1150000 / 1000000, всё через уже написанные Multiply и Divide.
        public BigNumber Scale(double factor)
        {
            if (double.IsNaN(factor) || double.IsInfinity(factor) || factor < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(factor), "Модификатор должен быть конечным и неотрицательным");
            }

            long numerator = (long)Math.Round(factor * ScaleDenominator);
            BigNumber result = Multiply(new BigNumber(numerator)).Divide(ScaleDenominator);

            if (!IsZero)
            {
                // Округление вниз не должно "съедать" рост на маленьких числах
                if (factor > 1 && result <= this)
                {
                    result = Add(One);
                }
                // И противник не должен стать с нулевым здоровьем
                else if (result.IsZero && factor > 0)
                {
                    result = One;
                }
            }

            return result;
        }
    }
}
