using System;
using System.Collections.Generic;
using System.Linq;

namespace Lab17V15
{
    public class Specification
    {
        public int Id { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        public Specification(int id, string key, string value)
        {
            Id = id;
            Key = key;
            Value = value;
        }
    }

    public class Component
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Specification> Specifications { get; set; }

        public Component(int id, string name, List<Specification> specifications)
        {
            Id = id;
            Name = name;
            Specifications = specifications;
        }
    }

    public class Computer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<Component> Components { get; set; }

        public Computer(int id, string name, List<Component> components)
        {
            Id = id;
            Name = name;
            Components = components;
        }
    }

    public static class ComponentExtensions
    {
        public static string GetSpecification(this Component component, string key)
        {
            foreach (Specification specification in component.Specifications)
            {
                if (specification.Key == key)
                {
                    return specification.Value;
                }
            }
            return "не знайдено";
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<Computer> computers = new List<Computer>
            {
                new Computer(1, "Ігровий ПК", new List<Component>
                {
                    new Component(1, "Процесор", new List<Specification>
                    {
                        new Specification(1, "Модель", "Ryzen 5 5600"),
                        new Specification(2, "Ядра", "6")
                    }),
                    new Component(2, "Відеокарта", new List<Specification>
                    {
                        new Specification(3, "Модель", "RTX 3060"),
                        new Specification(4, "Пам'ять", "12 ГБ")
                    }),
                    new Component(3, "Оперативна пам'ять", new List<Specification>
                    {
                        new Specification(5, "Обсяг", "16 ГБ"),
                        new Specification(6, "Тип", "DDR4")
                    })
                }),
                new Computer(2, "Офісний ПК", new List<Component>
                {
                    new Component(4, "Процесор", new List<Specification>
                    {
                        new Specification(7, "Модель", "Intel i3-12100"),
                        new Specification(8, "Ядра", "4")
                    }),
                    new Component(5, "Накопичувач", new List<Specification>
                    {
                        new Specification(9, "Тип", "SSD"),
                        new Specification(10, "Обсяг", "256 ГБ")
                    }),
                    new Component(6, "Оперативна пам'ять", new List<Specification>
                    {
                        new Specification(11, "Обсяг", "8 ГБ"),
                        new Specification(12, "Тип", "DDR4")
                    })
                }),
                new Computer(3, "Ноутбук", new List<Component>
                {
                    new Component(7, "Процесор", new List<Specification>
                    {
                        new Specification(13, "Модель", "Intel i5-1135G7"),
                        new Specification(14, "Ядра", "4")
                    }),
                    new Component(8, "Екран", new List<Specification>
                    {
                        new Specification(15, "Діагональ", "15.6 дюйма"),
                        new Specification(16, "Розширення", "1920x1080")
                    }),
                    new Component(9, "Акумулятор", new List<Specification>
                    {
                        new Specification(17, "Ємність", "4000 мАг"),
                        new Specification(18, "Час роботи", "8 год")
                    })
                }),
                new Computer(4, "Сервер", new List<Component>
                {
                    new Component(10, "Процесор", new List<Specification>
                    {
                        new Specification(19, "Модель", "Xeon E-2236"),
                        new Specification(20, "Ядра", "6")
                    }),
                    new Component(11, "Оперативна пам'ять", new List<Specification>
                    {
                        new Specification(21, "Обсяг", "64 ГБ"),
                        new Specification(22, "Тип", "DDR4 ECC")
                    }),
                    new Component(12, "Накопичувач", new List<Specification>
                    {
                        new Specification(23, "Тип", "HDD"),
                        new Specification(24, "Обсяг", "4 ТБ")
                    })
                })
            };

            Console.WriteLine("SelectMany: усі характеристики всіх компонентів усіх комп'ютерів");
            List<Specification> allSpecifications = computers
                .SelectMany(computer => computer.Components)
                .SelectMany(component => component.Specifications)
                .ToList();
            Console.WriteLine("Усього характеристик: " + allSpecifications.Count);
            foreach (Specification specification in allSpecifications)
            {
                Console.WriteLine(" - " + specification.Key + ": " + specification.Value);
            }

            Console.WriteLine();
            Console.WriteLine("Пагінація: усі компоненти по 4 на сторінку");
            List<Component> allComponents = computers.SelectMany(computer => computer.Components).ToList();
            int pageSize = 4;
            int page = 1;
            List<Component> pageItems = allComponents.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            while (pageItems.Count > 0)
            {
                Console.WriteLine("Сторінка " + page);
                foreach (Component component in pageItems)
                {
                    Console.WriteLine(" - " + component.Name);
                }
                page++;
                pageItems = allComponents.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            }

            Console.WriteLine();
            Console.WriteLine("Метод розширення GetSpecification");
            Component processor = computers[0].Components[0];
            Console.WriteLine(processor.Name + ", модель: " + processor.GetSpecification("Модель"));
            Console.WriteLine(processor.Name + ", ядра: " + processor.GetSpecification("Ядра"));
            Console.WriteLine(processor.Name + ", кеш: " + processor.GetSpecification("Кеш"));
        }
    }
}