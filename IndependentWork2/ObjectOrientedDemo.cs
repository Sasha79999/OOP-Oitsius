using System;
using System.Collections.Generic;

namespace IndependentWork2
{
    public class Product
    {
        public string Name { get; }
        public double Price { get; }

        public Product(string name, double price)
        {
            Name = name;
            Price = price;
        }
    }

    public class Cart
    {
        private List<(Product product, int quantity)> _items = new List<(Product, int)>();

        public void AddItem(Product product, int quantity)
        {
            _items.Add((product, quantity));
        }

        public double GetTotal()
        {
            double total = 0;

            foreach (var item in _items)
            {
                double itemSum = item.product.Price * item.quantity;
                double discount = ApplyDiscount(itemSum);
                double finalSum = itemSum - discount;

                Console.WriteLine($"{item.product.Name}: сума {itemSum} грн, знижка {discount} грн, підсумок {finalSum} грн.");

                total += finalSum;
            }

            return total;
        }

        private double ApplyDiscount(double sum)
        {
            if (sum > 500)
                return sum * 0.10;
            return 0;
        }
    }

    static class ObjectOrientedDemo
    {
        public static void Run()
        {
            Console.WriteLine("Об'єктна версія");

            Cart cart = new Cart();
            cart.AddItem(new Product("Ноутбук", 25000), 1);
            cart.AddItem(new Product("Мишка", 450), 2);
            cart.AddItem(new Product("Клавіатура", 800), 1);

            double total = cart.GetTotal();
            Console.WriteLine($"Загальна сума кошика: {total} грн.");
        }
    }
}