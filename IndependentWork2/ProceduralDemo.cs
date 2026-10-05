using System;

namespace IndependentWork2
{
    static class ProceduralDemo
    {
        public static void Run()
        {
            Console.WriteLine("Процедурна версія");

            string[] names = { "Ноутбук", "Мишка", "Клавіатура" };
            double[] prices = { 25000, 450, 800 };
            int[] quantities = { 1, 2, 1 };

            double total = 0;

            for (int i = 0; i < names.Length; i++)
            {
                double itemSum = CalculateItemSum(prices[i], quantities[i]);
                double discount = ApplyDiscount(itemSum);
                double finalSum = itemSum - discount;

                Console.WriteLine($"{names[i]}: сума {itemSum} грн, знижка {discount} грн, підсумок {finalSum} грн.");

                total += finalSum;
            }

            Console.WriteLine($"Загальна сума кошика: {total} грн.");
        }

        static double CalculateItemSum(double price, int quantity)
        {
            return price * quantity;
        }

        static double ApplyDiscount(double sum)
        {
            if (sum > 500)
                return sum * 0.10;
            return 0;
        }
    }
}