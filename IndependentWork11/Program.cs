using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace IndependentWork11
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public decimal UnitPrice { get; set; }

        public Product(int id, string name, string category, decimal unitPrice)
        {
            Id = id;
            Name = name;
            Category = category;
            UnitPrice = unitPrice;
        }
    }

    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string City { get; set; }

        public Customer(int id, string name, string city)
        {
            Id = id;
            Name = name;
            City = city;
        }
    }

    public class Sale
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }
        public int CustomerId { get; set; }

        public Sale(int id, int productId, int quantity, DateTime saleDate, int customerId)
        {
            Id = id;
            ProductId = productId;
            Quantity = quantity;
            SaleDate = saleDate;
            CustomerId = customerId;
        }
    }

    class Program
    {
        static List<Product> CreateProducts(Random random)
        {
            string[] categories = { "Electronics", "Clothing", "Books", "Sports", "Home" };
            List<Product> products = new List<Product>();

            for (int i = 1; i <= 50; i++)
            {
                products.Add(new Product(i, "Product " + i, categories[(i - 1) % categories.Length], random.Next(50, 2000)));
            }

            return products;
        }

        static List<Customer> CreateCustomers(Random random)
        {
            string[] cities = { "Rivne", "Kyiv", "Lviv", "Odesa", "Kharkiv", "Dnipro", "Lutsk", "Ternopil" };
            List<Customer> customers = new List<Customer>();

            for (int i = 1; i <= 50; i++)
            {
                customers.Add(new Customer(i, "Customer " + i, cities[random.Next(cities.Length)]));
            }

            return customers;
        }

        static List<Sale> CreateSales(Random random)
        {
            List<Sale> sales = new List<Sale>();

            for (int i = 1; i <= 100; i++)
            {
                DateTime date = new DateTime(2024, 1, 1).AddDays(random.Next(0, 730));
                sales.Add(new Sale(i, random.Next(1, 51), random.Next(1, 6), date, random.Next(1, 51)));
            }

            return sales;
        }

        static void TopProductsByCategory(List<Sale> sales, List<Product> products)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var result = sales
                .GroupBy(s => s.ProductId)
                .Select(g => new { ProductId = g.Key, Total = g.Sum(s => s.Quantity) })
                .Join(products, t => t.ProductId, p => p.Id, (t, p) => new { p.Name, p.Category, t.Total })
                .GroupBy(x => x.Category)
                .Select(g => new { Category = g.Key, Top = g.OrderByDescending(x => x.Total).Take(3).ToList() })
                .OrderBy(x => x.Category)
                .ToList();

            sw.Stop();

            Console.WriteLine("Pipeline 1: top 3 products by category");
            foreach (var group in result)
            {
                Console.WriteLine(group.Category);
                foreach (var item in group.Top)
                {
                    Console.WriteLine("  " + item.Name + " - " + item.Total + " pcs");
                }
            }
            Console.WriteLine("Time: " + sw.ElapsedTicks + " ticks");
            Console.WriteLine();
        }

        static void RevenueByCity(List<Sale> sales, List<Product> products, List<Customer> customers)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
                .Join(customers, x => x.CustomerId, c => c.Id, (x, c) => new { c.City, x.Revenue })
                .GroupBy(x => x.City)
                .Select(g => new { City = g.Key, Total = g.Sum(x => x.Revenue) })
                .OrderByDescending(x => x.Total)
                .ToList();

            sw.Stop();

            Console.WriteLine("Pipeline 2: revenue by customer city");
            foreach (var item in result)
            {
                Console.WriteLine(item.City + " - " + item.Total);
            }
            Console.WriteLine("Time: " + sw.ElapsedTicks + " ticks");
            Console.WriteLine();
        }

        static void SalesByMonth(List<Sale> sales, List<Product> products)
        {
            Stopwatch sw = Stopwatch.StartNew();

            var result = sales
                .Join(products, s => s.ProductId, p => p.Id, (s, p) => new { s.SaleDate.Year, s.SaleDate.Month, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
                .GroupBy(x => new { x.Year, x.Month })
                .Select(g => new { g.Key.Year, g.Key.Month, Units = g.Sum(x => x.Quantity), Revenue = g.Sum(x => x.Revenue) })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            sw.Stop();

            Console.WriteLine("Pipeline 3: sales by year and month");
            foreach (var item in result)
            {
                Console.WriteLine(item.Year + "-" + item.Month.ToString("00") + ": " + item.Units + " pcs, revenue " + item.Revenue);
            }
            Console.WriteLine("Time: " + sw.ElapsedTicks + " ticks");
            Console.WriteLine();
        }

        static void Main(string[] args)
        {
            Random random = new Random(42);

            List<Product> products = CreateProducts(random);
            List<Customer> customers = CreateCustomers(random);
            List<Sale> sales = CreateSales(random);

            Console.WriteLine("Products: " + products.Count + ", customers: " + customers.Count + ", sales: " + sales.Count);
            Console.WriteLine("Stopwatch: " + Stopwatch.Frequency + " ticks per second");
            Console.WriteLine();

            TopProductsByCategory(sales, products);
            RevenueByCity(sales, products, customers);
            SalesByMonth(sales, products);
        }
    }
}