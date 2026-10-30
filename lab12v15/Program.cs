using System;
using System.Collections.Generic;

namespace Lab12V15
{
    public interface IEntity
    {
        int Id { get; set; }
    }

    public class Student : IEntity
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime EnrollmentDate { get; set; }

        public Student(int id, string firstName, string lastName, DateTime enrollmentDate)
        {
            Id = id;
            FirstName = firstName;
            LastName = lastName;
            EnrollmentDate = enrollmentDate;
        }
    }

    public interface IRepository<T> where T : IEntity
    {
        T GetById(int id);
        IEnumerable<T> GetAll();
        void Add(T item);
        void Update(T item);
        void Delete(int id);
    }

    public class InMemoryRepository<T> : IRepository<T> where T : IEntity
    {
        private List<T> _items = new List<T>();

        public T GetById(int id)
        {
            foreach (T item in _items)
            {
                if (item.Id == id)
                {
                    return item;
                }
            }
            throw new ArgumentException("Елемент з Id " + id + " не знайдено.");
        }

        public IEnumerable<T> GetAll()
        {
            return _items;
        }

        public void Add(T item)
        {
            _items.Add(item);
        }

        public void Update(T item)
        {
            for (int i = 0; i < _items.Count; i++)
            {
                if (_items[i].Id == item.Id)
                {
                    _items[i] = item;
                    return;
                }
            }
            throw new ArgumentException("Елемент з Id " + item.Id + " не знайдено.");
        }

        public void Delete(int id)
        {
            T item = GetById(id);
            _items.Remove(item);
        }
    }

    class Program
    {
        static void PrintAll(IRepository<Student> repository)
        {
            foreach (Student student in repository.GetAll())
            {
                Console.WriteLine(student.Id + ": " + student.FirstName + " " + student.LastName + ", вступ: " + student.EnrollmentDate.ToShortDateString());
            }
        }

        static void Main(string[] args)
        {
            IRepository<Student> repository = new InMemoryRepository<Student>();

            Console.WriteLine("Додавання студентів");
            repository.Add(new Student(1, "Олена", "Коваль", new DateTime(2023, 9, 1)));
            repository.Add(new Student(2, "Іван", "Петренко", new DateTime(2024, 9, 1)));
            repository.Add(new Student(3, "Марія", "Шевчук", new DateTime(2025, 9, 1)));
            PrintAll(repository);

            Console.WriteLine();
            Console.WriteLine("Отримання студента з Id = 2");
            Student found = repository.GetById(2);
            Console.WriteLine(found.FirstName + " " + found.LastName);

            Console.WriteLine();
            Console.WriteLine("Оновлення студента з Id = 2");
            repository.Update(new Student(2, "Іван", "Мельник", new DateTime(2024, 9, 1)));
            PrintAll(repository);

            Console.WriteLine();
            Console.WriteLine("Видалення студента з Id = 1");
            repository.Delete(1);
            PrintAll(repository);
        }
    }
}