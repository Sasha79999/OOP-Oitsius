using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Lab16V15
{
    public class Supplier
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Country { get; set; }

        public Supplier(int id, string name, string country)
        {
            Id = id;
            Name = name;
            Country = country;
        }
    }

    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int SupplierId { get; set; }

        public Product(int id, string name, decimal price, int supplierId)
        {
            Id = id;
            Name = name;
            Price = price;
            SupplierId = supplierId;
        }
    }

    class Program
    {
        static void PrintTime(Stopwatch stopwatch)
        {
            Console.WriteLine("Час виконання: " + stopwatch.Elapsed.TotalMilliseconds.ToString("F4") + " мс");
        }

        static void Main(string[] args)
        {
            List<Supplier> suppliers = new List<Supplier>
            {
                new Supplier(1, "Global Supplies", "Ukraine"),
                new Supplier(2, "Euro Trade", "Poland"),
                new Supplier(3, "Tech Import", "Germany"),
                new Supplier(4, "Fresh Goods", "Ukraine"),
                new Supplier(5, "Asia Market", "China"),
                new Supplier(6, "Home Depot Ltd", "USA"),
                new Supplier(7, "Nordic Supply", "Sweden"),
                new Supplier(8, "Alpine Traders", "Austria"),
                new Supplier(9, "Baltic Cargo", "Latvia"),
                new Supplier(10, "Iberia Goods", "Spain")
            };

            List<Product> products = new List<Product>
            {
                new Product(1, "Молоток", 85, 1),
                new Product(2, "Викрутка", 40, 1),
                new Product(3, "Дриль", 450, 1),
                new Product(4, "Рукавиці", 60, 1),
                new Product(5, "Лампа", 120, 2),
                new Product(6, "Кабель", 25, 3),
                new Product(7, "Клавіатура", 300, 3),
                new Product(8, "Мишка", 95, 3),
                new Product(9, "Чай", 70, 4),
                new Product(10, "Кава", 210, 4),
                new Product(11, "Ваза", 150, 5),
                new Product(12, "Килимок", 99, 6),
                new Product(13, "Ножиці", 35, 7),
                new Product(14, "Рюкзак", 380, 8),
                new Product(15, "Блокнот", 15, 2)
            };

            Stopwatch stopwatch = new Stopwatch();
            int globalSupplierId = suppliers.First(s => s.Name == "Global Supplies").Id;

            Console.WriteLine("Запит 1: продукти від Global Supplies з ціною менше 100");

            stopwatch.Restart();
            List<Product> cheap1 = (from p in products
                                    where p.SupplierId == globalSupplierId && p.Price < 100
                                    select p).ToList();
            stopwatch.Stop();
            Console.WriteLine("Синтаксис запитів:");
            foreach (Product product in cheap1)
            {
                Console.WriteLine(" - " + product.Name + " (" + product.Price + " грн)");
            }
            PrintTime(stopwatch);

            stopwatch.Restart();
            List<Product> cheap2 = products.Where(p => p.SupplierId == globalSupplierId && p.Price < 100).ToList();
            stopwatch.Stop();
            Console.WriteLine("Синтаксис методів: знайдено продуктів " + cheap2.Count);
            PrintTime(stopwatch);
            Console.WriteLine();

            Console.WriteLine("Запит 2: продукти разом з назвою постачальника (join)");

            stopwatch.Restart();
            List<string> joined1 = (from p in products
                                    join s in suppliers on p.SupplierId equals s.Id
                                    select p.Name + " - " + s.Name).ToList();
            stopwatch.Stop();
            Console.WriteLine("Синтаксис запитів:");
            foreach (string line in joined1)
            {
                Console.WriteLine(" - " + line);
            }
            PrintTime(stopwatch);

            stopwatch.Restart();
            List<string> joined2 = products.Join(suppliers, p => p.SupplierId, s => s.Id, (p, s) => p.Name + " - " + s.Name).ToList();
            stopwatch.Stop();
            Console.WriteLine("Синтаксис методів: отримано рядків " + joined2.Count);
            PrintTime(stopwatch);
            Console.WriteLine();

            Console.WriteLine("Запит 3: кількість продуктів для кожного постачальника");

            stopwatch.Restart();
            List<string> counts1 = (from s in suppliers
                                    join p in products on s.Id equals p.SupplierId into supplierProducts
                                    select s.Name + ": " + supplierProducts.Count()).ToList();
            stopwatch.Stop();
            Console.WriteLine("Синтаксис запитів:");
            foreach (string line in counts1)
            {
                Console.WriteLine(" - " + line);
            }
            PrintTime(stopwatch);

            stopwatch.Restart();
            List<string> counts2 = suppliers.GroupJoin(products, s => s.Id, p => p.SupplierId, (s, supplierProducts) => s.Name + ": " + supplierProducts.Count()).ToList();
            stopwatch.Stop();
            Console.WriteLine("Синтаксис методів: отримано рядків " + counts2.Count);
            PrintTime(stopwatch);
            Console.WriteLine();

            Console.WriteLine("Запит 4: найдешевший продукт");

            stopwatch.Restart();
            decimal minPrice1 = (from p in products select p.Price).Min();
            Product cheapest1 = (from p in products where p.Price == minPrice1 select p).First();
            stopwatch.Stop();
            Console.WriteLine("Синтаксис запитів: " + cheapest1.Name + " (" + cheapest1.Price + " грн)");
            PrintTime(stopwatch);

            stopwatch.Restart();
            decimal minPrice2 = products.Min(p => p.Price);
            Product cheapest2 = products.First(p => p.Price == minPrice2);
            stopwatch.Stop();
            Console.WriteLine("Синтаксис методів: " + cheapest2.Name + " (" + cheapest2.Price + " грн)");
            PrintTime(stopwatch);
        }
    }
}