using System;
using System.Collections.Generic;

namespace IndependentWork1
{
    public class Employee
    {
        private string _name;
        private double _salary;

        public string Name
        {
            get { return _name; }
        }

        public double Salary
        {
            get { return _salary; }
            set { _salary = value; }
        }

        public Employee(string name, double salary)
        {
            _name = name;
            _salary = salary;
        }

        public double CalculateAnnualSalary()
        {
            return _salary * 12;
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Співробітник: {_name}, зарплата: {_salary} грн/міс, річна: {CalculateAnnualSalary()} грн.");
        }
    }

    public class Rectangle
    {
        private double _width;
        private double _height;

        public double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public double Height
        {
            get { return _height; }
            set { _height = value; }
        }

        public double Area
        {
            get { return _width * _height; }
        }

        public Rectangle(double width, double height)
        {
            _width = width;
            _height = height;
        }

        public double GetPerimeter()
        {
            return 2 * (_width + _height);
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Прямокутник {_width}x{_height}: площа {Area}, периметр {GetPerimeter()}.");
        }
    }

    public class Playlist
    {
        private string _name;
        private List<string> _songs;

        public string Name
        {
            get { return _name; }
        }

        public int SongCount
        {
            get { return _songs.Count; }
        }

        public Playlist(string name)
        {
            _name = name;
            _songs = new List<string>();
        }

        public void AddSong(string title)
        {
            _songs.Add(title);
            Console.WriteLine($"Пісню \"{title}\" додано до плейлиста \"{_name}\".");
        }

        public void PrintInfo()
        {
            Console.WriteLine($"Плейлист \"{_name}\" містить {SongCount} пісень.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Employee employee = new Employee("Олена Коваль", 15000);
            employee.PrintInfo();

            Console.WriteLine();

            Rectangle rectangle = new Rectangle(5, 8);
            rectangle.PrintInfo();

            Console.WriteLine();

            Playlist playlist = new Playlist("Ранкова підбірка");
            playlist.AddSong("Song One");
            playlist.AddSong("Song Two");
            playlist.PrintInfo();
        }
    }
}