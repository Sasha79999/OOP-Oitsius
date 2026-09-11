using System;
namespace Lab1V18 { class Bookstore { private string name; private string location; private int booksAvailable;
    public string Name
    {
        get { return name; }
        set { name = value; }
    }

    public string Location
    {
        get { return location; }
        set { location = value; }
    }

    public int BooksAvailable
    {
        get { return booksAvailable; }
        set
        {
            if (value >= 0)
                booksAvailable = value;
            else
                booksAvailable = 0;
        }
    }

    public Bookstore(string name, string location, int booksAvailable)
    {
        this.name = name;
        this.location = location;
        this.BooksAvailable = booksAvailable;
    }

    public void SellBook()
    {
        if (BooksAvailable > 0)
        {
            BooksAvailable--;
            Console.WriteLine($"Книгарня \"{Name}\" ({Location}) продала одну книгу. Залишилось книг: {BooksAvailable}.");
        }
        else
        {
            Console.WriteLine($"Книгарня \"{Name}\" ({Location}): книг немає в наявності, продаж неможливий.");
        }
    }

    ~Bookstore()
    {
        Console.WriteLine($"Об'єкт \"{Name}\" знищено збирачем сміття.");
    }
}

class Program
{
    static void Main(string[] args)
    {
        Bookstore store1 = new Bookstore("Книжковий світ", "Рівне", 15);
        Bookstore store2 = new Bookstore("Смарт Прес", "Київ", 3);
        Bookstore store3 = new Bookstore("Читай-Місто", "Львів", 0);

        store1.SellBook();
        store1.SellBook();

        store2.SellBook();
        store2.SellBook();
        store2.SellBook();
        store2.SellBook();

        store3.SellBook();

        Console.WriteLine();
        Console.WriteLine("Демонстрація роботи завершена.");
    }
}
}