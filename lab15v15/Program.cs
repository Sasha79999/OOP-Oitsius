using System;
using System.Collections.Generic;

namespace Lab15V15
{
    public class Notification
    {
        public string User { get; set; }
        public string Text { get; set; }

        public Notification(string user, string text)
        {
            User = user;
            Text = text;
        }
    }

    public class NotificationSystem
    {
        private HashSet<string> _enabledUsers = new HashSet<string>();
        private Queue<Notification> _queue = new Queue<Notification>();
        private Stack<Notification> _readHistory = new Stack<Notification>();

        public void EnableNotifications(string user)
        {
            if (_enabledUsers.Add(user))
            {
                Console.WriteLine("Увімкнено сповіщення для " + user);
            }
            else
            {
                Console.WriteLine(user + " уже є у списку, дублікат не додано");
            }
        }

        public void DisableNotifications(string user)
        {
            if (_enabledUsers.Remove(user))
            {
                Console.WriteLine("Вимкнено сповіщення для " + user);
            }
            else
            {
                Console.WriteLine(user + " не було у списку");
            }
        }

        public bool IsEnabled(string user)
        {
            return _enabledUsers.Contains(user);
        }

        public void AddNotification(string user, string text)
        {
            if (IsEnabled(user))
            {
                _queue.Enqueue(new Notification(user, text));
                Console.WriteLine("Додано в чергу: " + text + " (для " + user + ")");
            }
            else
            {
                Console.WriteLine("Не додано: у " + user + " вимкнені сповіщення");
            }
        }

        public void PeekNext()
        {
            if (_queue.Count > 0)
            {
                Notification next = _queue.Peek();
                Console.WriteLine("Наступне в черзі: " + next.Text + " (для " + next.User + ")");
            }
            else
            {
                Console.WriteLine("Черга порожня");
            }
        }

        public void ReadNext()
        {
            if (_queue.Count > 0)
            {
                Notification notification = _queue.Dequeue();
                _readHistory.Push(notification);
                Console.WriteLine("Прочитано: " + notification.Text + " (для " + notification.User + ")");
            }
            else
            {
                Console.WriteLine("Черга порожня, читати нічого");
            }
        }

        public void PeekLastRead()
        {
            if (_readHistory.Count > 0)
            {
                Notification last = _readHistory.Peek();
                Console.WriteLine("Останнє прочитане: " + last.Text + " (для " + last.User + ")");
            }
            else
            {
                Console.WriteLine("Історія порожня");
            }
        }

        public void ReopenLastRead()
        {
            if (_readHistory.Count > 0)
            {
                Notification notification = _readHistory.Pop();
                Console.WriteLine("Повернуто з історії: " + notification.Text + " (для " + notification.User + ")");
            }
            else
            {
                Console.WriteLine("Історія порожня");
            }
        }

        public void ShowCounts()
        {
            Console.WriteLine("Користувачів зі сповіщеннями: " + _enabledUsers.Count + ", у черзі: " + _queue.Count + ", в історії: " + _readHistory.Count);
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            NotificationSystem system = new NotificationSystem();

            Console.WriteLine("HashSet: унікальні користувачі з увімкненими сповіщеннями");
            system.EnableNotifications("Олена");
            system.EnableNotifications("Іван");
            system.EnableNotifications("Олена");
            system.EnableNotifications("Петро");
            system.DisableNotifications("Петро");
            Console.WriteLine("Олена є у списку: " + system.IsEnabled("Олена"));
            Console.WriteLine("Петро є у списку: " + system.IsEnabled("Петро"));
            system.ShowCounts();

            Console.WriteLine();
            Console.WriteLine("Queue: черга сповіщень");
            system.AddNotification("Олена", "Нове повідомлення в чаті");
            system.AddNotification("Іван", "Нагадування про зустріч");
            system.AddNotification("Олена", "Оновлення додатка");
            system.AddNotification("Петро", "Знижка на товари");
            system.PeekNext();
            system.ShowCounts();

            Console.WriteLine();
            Console.WriteLine("Читання черги: перше додане читається першим (FIFO)");
            system.ReadNext();
            system.ReadNext();
            system.ReadNext();
            system.ReadNext();
            system.ShowCounts();

            Console.WriteLine();
            Console.WriteLine("Stack: історія прочитаних сповіщень");
            system.PeekLastRead();
            Console.WriteLine("Повертаємо з історії: останнє прочитане першим (LIFO)");
            system.ReopenLastRead();
            system.ReopenLastRead();
            system.ReopenLastRead();
            system.ReopenLastRead();
            system.ShowCounts();
        }
    }
}