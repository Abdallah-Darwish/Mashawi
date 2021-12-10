using System;
using System.Collections.Generic;
using System.Text;

namespace Mashawi
{
    public static class RandomExtensions
    {
        private static readonly char[] RandomTextPool, NumberPool;

        static RandomExtensions()
        {
            List<char> pool = new();
            for (char i = 'a'; i <= 'z'; i++)
            {
                pool.Add(i);
            }

            for (char i = 'A'; i <= 'Z'; i++)
            {
                pool.Add(i);
            }

            for (char i = '0'; i <= '9'; i++)
            {
                pool.Add(i);
            }

            pool.AddRange(".,!");
            RandomTextPool = pool.ToArray();
            NumberPool = Enumerable.Range(0, 10).Select(i => (char)('0' + i)).ToArray();
        }

        public static T NextElementAndSwap<T>(this Random rand, IList<T> lst, int newIndex)
        {
            if (newIndex >= lst.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(newIndex));
            }

            var elementIndex = rand.Next(newIndex + 1);
            var element = lst[elementIndex];
            lst[elementIndex] = lst[newIndex];
            lst[newIndex] = element;
            return element;
        }

        public static T NextElement<T>(this Random rand, IList<T> lst)
        {
            if (lst.Count == 0)
            {
                throw new InvalidOperationException("List is empty.");
            }

            return lst[rand.Next(lst.Count)];
        }

        public static bool NextBool(this Random rand) => (rand.Next() & 1) == 1;

        public static string NextText(this Random rand, int length = -1, char[]? randomTextPool = null)
        {
            if (length < 0)
            {
                length = rand.Next(1, 100);
            }
            randomTextPool ??= RandomTextPool;
            var sb = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                sb.Append(rand.NextElement(randomTextPool));
            }

            return sb.ToString();
        }
        public static string NextNumber(this Random rand, int length = -1) => NextText(rand, length, NumberPool);
        public static TimeSpan NextTimeSpan(this Random rand) => TimeSpan.FromHours(rand.Next(9, 18));
    }
}