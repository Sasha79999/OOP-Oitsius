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
        public static void Skip(IEnumerable<T> collection, int count, Action<T> action)
        {
            int index = 0;
            foreach (T item in collection)
            {
                if (index >= count)
                {
                    action(item);
                }
                index++;
            }
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<int> numbers = new List<int> { 10, 20, 30, 40, 50, 60 };
            
            Console.Write("Початковий List<int>: ");
            Action<int> printNumber = (n) => Console.Write(n + " ");
            foreach (int num in numbers)
            {
                printNumber(num);
            }
            Console.WriteLine();

            Console.Write("Пропускаємо перші 3 елементи. Результат: ");
            CollectionHelper<int>.Skip(numbers, 3, (n) => Console.Write(n + " "));
            Console.WriteLine();
            Console.WriteLine();

            List<Product> products = new List<Product>
            {
                new Product("Хліб", 25),
                new Product("Молоко", 40),
                new Product("Сир", 150),
                new Product("М'ясо", 300)
            };

            Console.WriteLine("Початковий List<Product>:");
            Action<Product> printProduct = (p) => Console.WriteLine(" - " + p.Name + " (" + p.Price + " грн)");
            foreach (Product p in products)
            {
                printProduct(p);
            }
            Console.WriteLine();

            Console.WriteLine("Пропускаємо перші 2 елементи. Результат:");
            CollectionHelper<Product>.Skip(products, 2, (p) => Console.WriteLine(" - " + p.Name + " (" + p.Price + " грн)"));
        }
    }
}