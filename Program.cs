using System;
using System.Diagnostics.CodeAnalysis;

namespace Project_01
{
    class Program       
    {
        static void Main()
        {
            CountriesRates CountriesRates = new CountriesRates("https://euvatrates.com/rates.json");
            if (CountriesRates.IsValid)
            {
                CountriesRates.PrintTopStandardRates(ascending: true, itemsCount: 3);
                CountriesRates.PrintTopStandardRates(ascending: false, itemsCount: 3);
            }
        }
    }
}
