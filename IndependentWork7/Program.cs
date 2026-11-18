using System;
using System.Collections.Generic;

namespace IndependentWork7
{
    public class Cache<TKey, TValue> where TKey : notnull
    {
        private int _capacity;
        private Dictionary<TKey, TValue> _cache;
        private Queue<TKey> _keysOrder;

        public int Count
        {
            get { return _cache.Count; }
        }

        public Cache(int capacity)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Ємність кешу має бути більшою за 0");
            }

            _capacity = capacity;
            _cache = new Dictionary<TKey, TValue>();
            _keysOrder = new Queue<TKey>();
        }

        public void Add(TKey key, TValue value)
        {
            if (_cache.ContainsKey(key))
            {
                _cache[key] = value;
                return;
            }

            if (_cache.Count >= _capacity)
            {
                TKey oldestKey = _keysOrder.Dequeue();
                _cache.Remove(oldestKey);
            }

            _cache[key] = value;
            _keysOrder.Enqueue(key);
        }

        public TValue Get(TKey key)
        {
            if (_cache.TryGetValue(key, out TValue? value))
            {
                return value;
            }

            throw new KeyNotFoundException("Ключ не знайдено в кеші");
        }

        public bool ContainsKey(TKey key)
        {
            return _cache.ContainsKey(key);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Cache<string, string> cache = new Cache<string, string>(3);

            Console.WriteLine("Додаємо 3 елементи (ємність 3)");
            cache.Add("a", "перший");
            cache.Add("b", "другий");
            cache.Add("c", "третій");
            Console.WriteLine("Count: " + cache.Count);
            Console.WriteLine("Get(\"b\"): " + cache.Get("b"));

            Console.WriteLine("Додаємо четвертий елемент, найстаріший видаляється");
            cache.Add("d", "четвертий");
            Console.WriteLine("Count: " + cache.Count);
            Console.WriteLine("Містить a: " + cache.ContainsKey("a"));
            Console.WriteLine("Містить d: " + cache.ContainsKey("d"));

            try
            {
                cache.Get("a");
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine("Get(\"a\"): " + ex.Message);
            }
        }
    }
}