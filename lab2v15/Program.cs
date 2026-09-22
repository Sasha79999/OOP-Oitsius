using System;

namespace Lab2V15
{
    class House
    {
        private string _address;
        private string _type;
        private int _floors;

        public string Address
        {
            get { return _address; }
            set { _address = value; }
        }

        public string Type
        {
            get { return _type; }
            set { _type = value; }
        }

        public int Floors
        {
            get { return _floors; }
            set
            {
                if (value > 0)
                    _floors = value;
                else
                    _floors = 1;
            }
        }

        public House() : this("N/A", "Apartment", 1)
        {
        }

        public House(string address, string type, int floors)
        {
            _address = address;
            _type = type;
            Floors = floors;
        }

        public string GetHouseInfo()
        {
            return $"Будинок за адресою \"{_address}\" ({_type}), поверхів: {_floors}.";
        }

        ~House()
        {
            Console.WriteLine($"Об'єкт \"{_address}\" знищено збирачем сміття.");
        }
    }

    class Program
    {
        static void CreateAndUseObjects()
        {
            Console.WriteLine("Creating objects");

            House house1 = new House();
            House house2 = new House("вул. Соборна, 15", "Приватний будинок", 2);
            House house3 = new House("просп. Миру, 40", "Багатоповерхівка", 9);

            Console.WriteLine("Objects created");

            Console.WriteLine(house1.GetHouseInfo());
            Console.WriteLine(house2.GetHouseInfo());
            Console.WriteLine(house3.GetHouseInfo());
        }

        static void Main(string[] args)
        {
            CreateAndUseObjects();

            Console.WriteLine("End of Main, preparing for GC");

            GC.Collect();
            GC.WaitForPendingFinalizers();
        }
    }
}