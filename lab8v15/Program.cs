using System;
using System.Collections.Generic;

namespace Lab8V15
{
    public class FinancialInstrument
    {
        private string _name;

        public string Name
        {
            get { return _name; }
        }

        public FinancialInstrument(string name)
        {
            _name = name;
        }

        public virtual decimal GetCurrentValue()
        {
            return 0;
        }
    }

    public class Stock : FinancialInstrument
    {
        private string _tickerSymbol;
        private decimal _sharePrice;

        public string TickerSymbol
        {
            get { return _tickerSymbol; }
        }

        public decimal SharePrice
        {
            get { return _sharePrice; }
        }

        public Stock(string name, string tickerSymbol, decimal sharePrice) : base(name)
        {
            _tickerSymbol = tickerSymbol;
            _sharePrice = sharePrice;
        }

        public override decimal GetCurrentValue()
        {
            return _sharePrice;
        }
    }

    public class Bond : FinancialInstrument
    {
        private decimal _faceValue;
        private decimal _interestRate;

        public decimal FaceValue
        {
            get { return _faceValue; }
        }

        public decimal InterestRate
        {
            get { return _interestRate; }
        }

        public Bond(string name, decimal faceValue, decimal interestRate) : base(name)
        {
            _faceValue = faceValue;
            _interestRate = interestRate;
        }

        public override decimal GetCurrentValue()
        {
            return _faceValue + _faceValue * _interestRate / 100;
        }
    }

    public class MutualFund : FinancialInstrument
    {
        private decimal _nav;
        private int _numShares;

        public decimal NAV
        {
            get { return _nav; }
        }

        public int NumShares
        {
            get { return _numShares; }
        }

        public MutualFund(string name, decimal nav, int numShares) : base(name)
        {
            _nav = nav;
            _numShares = numShares;
        }

        public override decimal GetCurrentValue()
        {
            return _nav * _numShares;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            List<FinancialInstrument> instruments = new List<FinancialInstrument>();
            instruments.Add(new Stock("Акції Apple", "AAPL", 1500));
            instruments.Add(new Bond("Державна облігація", 10000, 5));
            instruments.Add(new MutualFund("Фонд Зростання", 200, 10));

            Console.WriteLine("Вартість кожного інструмента");

            decimal total = 0;

            foreach (FinancialInstrument item in instruments)
            {
                decimal value = item.GetCurrentValue();
                Console.WriteLine(item.Name + ": " + value + " грн");
                total = total + value;
            }

            Console.WriteLine();
            Console.WriteLine("Загальна вартість усіх інструментів: " + total + " грн");
        }
    }
}