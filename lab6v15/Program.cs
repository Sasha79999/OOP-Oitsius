using System;

namespace Lab6V15
{
    public class Clothing
    {
        private string _material;
        private string _size;

        public string Material
        {
            get { return _material; }
        }

        public string Size
        {
            get { return _size; }
        }

        public Clothing(string material, string size)
        {
            _material = material;
            _size = size;
        }

        public virtual void Wear()
        {
            Console.WriteLine($"Одяг ({_material}, розмір {_size}) одягнено.");
        }

        public string GetClothingType()
        {
            return "Одяг";
        }
    }

    public class Shirt : Clothing
    {
        private int _sleeveLength;

        public int SleeveLength
        {
            get { return _sleeveLength; }
        }

        public Shirt(string material, string size, int sleeveLength) : base(material, size)
        {
            _sleeveLength = sleeveLength;
        }

        public override void Wear()
        {
            Console.WriteLine($"Сорочку ({Material}, розмір {Size}, рукав {_sleeveLength} см) одягнено.");
        }

        public void ButtonUp()
        {
            Console.WriteLine("Сорочку застебнуто на гудзики.");
        }

        public new string GetClothingType()
        {
            return "Сорочка";
        }
    }

    public class Pants : Clothing
    {
        private int _waistSize;

        public int WaistSize
        {
            get { return _waistSize; }
        }

        public Pants(string material, string size, int waistSize) : base(material, size)
        {
            _waistSize = waistSize;
        }

        public override void Wear()
        {
            Console.WriteLine($"Штани ({Material}, розмір {Size}, талія {_waistSize} см) одягнено.");
        }

        public void ZipUp()
        {
            Console.WriteLine("Штани застебнуто на блискавку.");
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Створення об'єктів");

            Clothing clothing = new Clothing("Бавовна", "M");
            Shirt shirt = new Shirt("Льон", "L", 60);
            Pants pants = new Pants("Джинс", "L", 84);

            Console.WriteLine();
            Console.WriteLine("Власні методи похідних класів");
            shirt.ButtonUp();
            pants.ZipUp();

            Console.WriteLine();
            Console.WriteLine("Поліморфізм: Wear() через змінні типу Clothing");
            Clothing item1 = clothing;
            Clothing item2 = shirt;
            Clothing item3 = pants;
            item1.Wear();
            item2.Wear();
            item3.Wear();

            Console.WriteLine();
            Console.WriteLine("Різниця між override і new");
            Console.WriteLine("Через змінну типу Shirt: " + shirt.GetClothingType());
            Console.WriteLine("Через змінну типу Clothing: " + item2.GetClothingType());
        }
    }
}