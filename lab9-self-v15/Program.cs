using System;
using System.Collections.Generic;

namespace Lab9Self
{
    public class Product
    {
        public string Name { get; set; }
        public decimal Price { get; set; }

        public Product(string name, decimal price)
        {
            Name = name;
            Price = price;
        }
    }

    public static class CollectionUtils
    {
        public static IEnumerable<TResult> Map<TSource, TResult>(IEnumerable<TSource> collection, Func<TSource, TResult> mapper)
        {
            List<TResult> result = new List<TResult>();

            foreach (TSource item in collection)
            {
                result.Add(mapper(item));
            }

            return result;
        }

        public static T MaxBy<T, TKey>(IEnumerable<T> collection, Func<T, TKey> keySelector) where TKey : IComparable<TKey>
        {
            List<T> items = new List<T>(collection);

            if (items.Count == 0)
            {
                throw new InvalidOperationException("Колекція порожня");
            }

            T maxItem = items[0];
            TKey maxKey = keySelector(maxItem);

            foreach (T item in items)
            {
                TKey key = keySelector(item);

                if (key.CompareTo(maxKey) > 0)
                {
                    maxKey = key;
                    maxItem = item;
                }
            }

            return maxItem;
        }

        public static IEnumerable<T> Filter<T>(IEnumerable<T> collection, Predicate<T> predicate)
        {
            List<T> result = new List<T>();

            foreach (T item in collection)
            {
                if (predicate(item))
                {
                    result.Add(item);
                }
            }

            return result;
        }

        public static void ForEach<T>(IEnumerable<T> collection, Action<T> action)
        {
            foreach (T item in collection)
            {
                action(item);
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int> { 3, 8, 15, 4, 42, 7 };
            Console.WriteLine("List<int>: " + string.Join(", ", numbers));
            IEnumerable<int> squares = CollectionUtils.Map(numbers, n => n * n);
            Console.WriteLine("Map (квадрати): " + string.Join(", ", squares));
            Console.WriteLine("MaxBy (найбільше): " + CollectionUtils.MaxBy(numbers, n => n));
            IEnumerable<int> even = CollectionUtils.Filter(numbers, n => n % 2 == 0);
            Console.WriteLine("Filter (парні): " + string.Join(", ", even));
            Console.Write("ForEach (кожне число + 1): ");
            CollectionUtils.ForEach(numbers, n => Console.Write((n + 1) + " "));
            Console.WriteLine();
            Console.WriteLine();

            List<string> words = new List<string> { "apple", "kiwi", "banana", "fig" };
            Console.WriteLine("List<string>: " + string.Join(", ", words));
            IEnumerable<string> upper = CollectionUtils.Map(words, w => w.ToUpper());
            Console.WriteLine("Map (великі літери): " + string.Join(", ", upper));
            Console.WriteLine("MaxBy (найдовше слово): " + CollectionUtils.MaxBy(words, w => w.Length));
            IEnumerable<string> longWords = CollectionUtils.Filter(words, w => w.Length > 4);
            Console.WriteLine("Filter (довші за 4 літери): " + string.Join(", ", longWords));
            Console.WriteLine("ForEach (слово і довжина):");
            CollectionUtils.ForEach(words, w => Console.WriteLine(" - " + w + ": " + w.Length));
            Console.WriteLine();

            List<Product> products = new List<Product>
            {
                new Product("Laptop", 25000),
                new Product("Mouse", 500),
                new Product("Keyboard", 1500)
            };
            IEnumerable<string> names = CollectionUtils.Map(products, p => p.Name);
            Console.WriteLine("Map (назви продуктів): " + string.Join(", ", names));
            Product mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
            Console.WriteLine("MaxBy (найдорожчий): " + mostExpensive.Name + " - " + mostExpensive.Price);
            IEnumerable<Product> expensive = CollectionUtils.Filter(products, p => p.Price > 1000);
            Console.WriteLine("Filter (дорожчі за 1000):");
            CollectionUtils.ForEach(expensive, p => Console.WriteLine(" - " + p.Name));
            Console.WriteLine("ForEach (усі продукти):");
            CollectionUtils.ForEach(products, p => Console.WriteLine(" - " + p.Name + ": " + p.Price + " грн"));
        }
    }
}