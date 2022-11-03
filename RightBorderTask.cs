using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;

namespace Autocomplete
{
    public class RightBorderTask
    {
        /// <returns>
        /// Возвращает индекс правой границы. 
        /// То есть индекс минимального элемента, который не начинается с prefix и большего prefix.
        /// Если такого нет, то возвращает items.Length
        /// </returns>
        /// <remarks>
        /// Функция должна быть НЕ рекурсивной
        /// и работать за O(log(items.Length)*L), где L — ограничение сверху на длину фразы
        /// </remarks>
        public static int GetRightBorderIndex(IReadOnlyList<string> phrases, string prefix, int left, int right)
        {
            while (left + 1 != right)
            {
                var m = (left + right) / 2;

                if (string.Compare(prefix, 0, phrases[m], 0, prefix.Length, StringComparison.OrdinalIgnoreCase) < 0)
                    right = m;
                else
                    left = m;
            }

            return right;
        }
    }
}