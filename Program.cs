using System;
using System.Diagnostics.CodeAnalysis;

namespace Project_01
{
    public class Program       
    {
        /// <summary>
        /// Runs the project
        /// </summary>
        public static void Main()
        {
            CountriesRates countriesRates = new CountriesRates("https://euvatrates.com/rates.json");
            if (countriesRates.IsValid)
            {
                countriesRates.PrintTopStandardRates(ascending: true, itemsCount: 3);
                countriesRates.PrintTopStandardRates(ascending: false, itemsCount: 3);
            }
        }
    }
}
