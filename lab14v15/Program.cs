using System;
using System.Collections.Generic;

namespace Lab14V15
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

    public static class CollectionHelper<T>
    {
        public static IEnumerable<T> Skip(IEnumerable<T> collection, int count)
        {
            List<T> result = new List<T>();
            int index = 0;

            foreach (T item in collection)
            {
                if (index >= count)
                {
                    result.Add(item);
                }
                index++;
            }

            return result;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Action<int> printNumber = (n) => Console.Write(n + " ");
            Action<string> printName = (s) => Console.WriteLine(" - " + s);
            Action<Product> printProduct = (p) => Console.WriteLine(" - " + p.Name + " (" + p.Price + " грн)");

            List<int> numbers = new List<int> { 10, 20, 30, 40, 50, 60 };
            Console.Write("Початковий List<int>: ");
            foreach (int num in numbers)
            {
                printNumber(num);
            }
            Console.WriteLine();

            Console.Write("Пропускаємо перші 3 елементи. Результат: ");
            foreach (int num in CollectionHelper<int>.Skip(numbers, 3))
            {
                printNumber(num);
            }
            Console.WriteLine();
            Console.WriteLine();

            List<string> names = new List<string> { "Олена", "Іван", "Марія", "Петро" };
            Console.WriteLine("Початковий List<string>:");
            foreach (string name in names)
            {
                printName(name);
            }

            Console.WriteLine("Пропускаємо перші 2 елементи. Результат:");
            foreach (string name in CollectionHelper<string>.Skip(names, 2))
            {
                printName(name);
            }
            Console.WriteLine();

            List<Product> products = new List<Product>
            {
                new Product("Хліб", 25),
                new Product("Молоко", 40),
                new Product("Сир", 150),
                new Product("М'ясо", 300)
            };
            Console.WriteLine("Початковий List<Product>:");
            foreach (Product product in products)
            {
                printProduct(product);
            }

            Console.WriteLine("Пропускаємо перші 2 елементи. Результат:");
            foreach (Product product in CollectionHelper<Product>.Skip(products, 2))
            {
                printProduct(product);
            }
            Console.WriteLine();

            List<int> rest = new List<int>(CollectionHelper<int>.Skip(numbers, 10));
            Console.WriteLine("Пропускаємо 10 елементів (більше, ніж у списку). Залишилось елементів: " + rest.Count);
        }
    }
}