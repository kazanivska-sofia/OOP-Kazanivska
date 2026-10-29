using System;
using System.Collections.Generic;

namespace Lab13v11
{
    public static class FunctionalUtils
    {
        // 1. Map: Перетворює список List<T> у List<TResult>
        public static List<TResult> Map<T, TResult>(List<T> list, Func<T, TResult> mapper)
        {
            var result = new List<TResult>();
            foreach (var item in list)
            {
                result.Add(mapper(item));
            }
            return result;
        }

        // 2. Reduce: Згортає список до одного значення за допомогою акумулятора
        public static TAccumulate Reduce<T, TAccumulate>(List<T> list, TAccumulate initialValue, Func<TAccumulate, T, TAccumulate> accumulator)
        {
            var result = initialValue;
            foreach (var item in list)
            {
                result = accumulator(result, item);
            }
            return result;
        }

        // 3. Count: Обчислює кількість елементів, що задовольняють умову predicate
        public static int Count<T>(List<T> list, Predicate<T> predicate)
        {
            int count = 0;
            foreach (var item in list)
            {
                if (predicate(item))
                {
                    count++;
                }
            }
            return count;
        }

        // 4. Average: Обчислює середнє значення елементів за допомогою вибору числового поля
        public static double Average<T>(List<T> list, Func<T, double> selector)
        {
            if (list == null || list.Count == 0) return 0;

            double sum = 0;
            foreach (var item in list)
            {
                sum += selector(item);
            }
            return sum / list.Count;
        }

        // 5. Distinct: Повертає список унікальних елементів
        public static List<T> Distinct<T>(List<T> list)
        {
            var result = new List<T>();
            foreach (var item in list)
            {
                if (!result.Contains(item))
                {
                    result.Add(item);
                }
            }
            return result;
        }
    }
}
