using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Text;

namespace labaa2
{
    public class BigNumber
    {
        private const int Base = 1000;
        private readonly int[] _blocks;

        public BigNumber(long value)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value), "Число не должно быть отрацательным");
            }
            List<int> list = new List<int>();
            
            while(value > 0)
            {
                list.Add((int)(value % Base));
                value /= Base;
            }
            if(list.Count == 0)
            {
                list.Add(0);
            }
            _blocks = list.ToArray();
        }
        private BigNumber(int[] blocks)
        {
            if(blocks.Length ==0)
            {
                throw new ArgumentException("Мфссив не должен быть пустым");
            }
            int length = blocks.Length;
            
            while (length > 1 && blocks[length-1]==0)
            {
                length--;
            }
            int[] numbers = new int[length];
            Array.Copy(blocks, numbers, length);
            _blocks = numbers;


        }
        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(_blocks[_blocks.Length - 1]);
            for (int i = _blocks.Length -2;i>=0;i--)
            {
                sb.Append(_blocks[i].ToString().PadLeft(3, '0'));
            }
            return sb.ToString();
        }
    }
}
