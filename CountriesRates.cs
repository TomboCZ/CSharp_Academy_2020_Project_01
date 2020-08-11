using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Project_01
{
    /// <summary>
    /// Reads and shows top EU countries by theirs standard VAT rates
    /// </summary>
    public class CountriesRates
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CountriesRates" /> class
        /// </summary>
        /// <param name="inputURL">URL of Json data</param>
        public CountriesRates(string inputURL)
        {
            this.BuildCountriesList(inputURL);
        }

        /// <summary>
        /// Gets a value indicating whether CountriesList was successfully imported and is non-empty
        /// </summary>
        public bool IsValid
        {
            get { return this.CountriesList.Count > 0; }
        }

        /// <summary>
        /// Gets or sets list of imported countries and their VATs
        /// </summary>
        private List<Country> CountriesList { get; set; } = new List<Country>();

        /// <summary>
        /// Prints top n countries with min or max standard VAT rate
        /// </summary>
        /// <param name="ascending">true == ascending, false == descending</param>
        /// <param name="itemsCount">number of countries to be printed</param>
        public void PrintTopStandardRates(bool ascending, int itemsCount)
        {
            if (itemsCount < 1 || itemsCount > this.CountriesList.Count)
            {
                throw new ArgumentException("Invalid number of top countries.", nameof(itemsCount));
            }
            
            Console.WriteLine($"\nTop {itemsCount} countries with {(ascending ? "min" : "max")}. standard VAT:");

            if (ascending)
            {
                foreach (Country eachCountry in this.CountriesList.OrderBy(i => i.StandardRate).Take(itemsCount))
                {
                    eachCountry.Print();
                }
            }
            else
            {
                foreach (Country eachCountry in this.CountriesList.OrderByDescending(i => i.StandardRate).Take(itemsCount))
                {
                    eachCountry.Print();
                }
            }
        }

        /// <summary>
        /// Builds a list of countries 
        /// </summary>
        /// <param name="inputURL">Path to json data</param>
        private void BuildCountriesList(string inputURL)
        {
            const string RegexPattern = @"""[A-Z]{2}""[ :]+((?=\[)\[[^]]*\]|(?=\{)\{[^\}]*\}|\""[^""]*\"")";
            string inputData;

            Console.Write("Importing data...");
            SpinAnimation.Start();

            try
            {
                inputData = new WebClient().DownloadString(inputURL);
            }
            catch (Exception ex)
            {
                SpinAnimation.Stop();
                Console.WriteLine($"\nImport error: \"{ex.Message}\"");
                return;
            }

            // cached full json string for testing purposes only
            /*
              string inputData = "{\"last_updated\":\"2016-01-01T22:34Z\",\"disclaimer\":\"This data is compiled from official European Commission sources to be as accurate as possible, however no guarantee of accuracy is provided. Use at your own risk. Don't trust random people on the internet without doing your own research.\",\"rates\":[{\"AT\":{\"country\":\"Austria\",\"standard_rate\":20,\"reduced_rate\":10,\"reduced_rate_alt\":13,\"super_reduced_rate\":false,\"parking_rate\":12},\"BE\":{\"country\":\"Belgium\",\"standard_rate\":21,\"reduced_rate\":12,\"reduced_rate_alt\":6,\"super_reduced_rate\":false,\"parking_rate\":12},\"BG\":{\"country\":\"Bulgaria\",\"standard_rate\":20,\"reduced_rate\":9,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"CY\":{\"country\":\"Cyprus\",\"standard_rate\":19,\"reduced_rate\":9,\"reduced_rate_alt\":5,\"super_reduced_rate\":false,\"parking_rate\":false},\"CZ\":{\"country\":\"Czech Republic\",\"standard_rate\":21,\"reduced_rate\":15,\"reduced_rate_alt\":10,\"super_reduced_rate\":false,\"parking_rate\":false},\"DK\":{\"country\":\"Denmark\",\"standard_rate\":25,\"reduced_rate\":false,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"DE\":{\"country\":\"Germany\",\"standard_rate\":19,\"reduced_rate\":7,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"EE\":{\"country\":\"Estonia\",\"standard_rate\":20,\"reduced_rate\":9,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"EL\":{\"_comment\":\"While the EU uses the country code 'EL' for Greece, ISO uses 'GR' - both are included for convenience.\",\"iso_duplicate\":\"GR\",\"country\":\"Greece\",\"standard_rate\":24,\"reduced_rate\":13,\"reduced_rate_alt\":6,\"super_reduced_rate\":false,\"parking_rate\":false},\"GR\":{\"_comment\":\"Duplicate of EL for convenience; the EU uses the country code 'EL' for Greece, while ISO uses 'GR'.\",\"iso_duplicate_of\":\"EL\",\"country\":\"Greece\",\"standard_rate\":24,\"reduced_rate\":13,\"reduced_rate_alt\":6,\"super_reduced_rate\":false,\"parking_rate\":false},\"ES\":{\"country\":\"Spain\",\"standard_rate\":21,\"reduced_rate\":10,\"reduced_rate_alt\":false,\"super_reduced_rate\":4,\"parking_rate\":false},\"FI\":{\"country\":\"Finland\",\"standard_rate\":24,\"reduced_rate\":14,\"reduced_rate_alt\":10,\"super_reduced_rate\":false,\"parking_rate\":false},\"FR\":{\"country\":\"France\",\"standard_rate\":20,\"reduced_rate\":10,\"reduced_rate_alt\":5.5,\"super_reduced_rate\":2.1,\"parking_rate\":false},\"HR\":{\"country\":\"Croatia\",\"standard_rate\":25,\"reduced_rate\":13,\"reduced_rate_alt\":5,\"super_reduced_rate\":false,\"parking_rate\":false},\"IT\":{\"country\":\"Italy\",\"standard_rate\":22,\"reduced_rate\":10,\"reduced_rate_alt\":4,\"super_reduced_rate\":false,\"parking_rate\":false},\"LV\":{\"country\":\"Latvia\",\"standard_rate\":21,\"reduced_rate\":12,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"LT\":{\"country\":\"Lithuania\",\"standard_rate\":21,\"reduced_rate\":9,\"reduced_rate_alt\":5,\"super_reduced_rate\":false,\"parking_rate\":false},\"LU\":{\"country\":\"Luxembourg\",\"standard_rate\":17,\"reduced_rate\":14,\"reduced_rate_alt\":8,\"super_reduced_rate\":3,\"parking_rate\":12},\"HU\":{\"country\":\"Hungary\",\"standard_rate\":27,\"reduced_rate\":18,\"reduced_rate_alt\":5,\"super_reduced_rate\":false,\"parking_rate\":false},\"IE\":{\"country\":\"Ireland\",\"standard_rate\":23,\"reduced_rate\":13.5,\"reduced_rate_alt\":9,\"super_reduced_rate\":4.8,\"parking_rate\":13.5},\"MT\":{\"country\":\"Malta\",\"standard_rate\":18,\"reduced_rate\":7,\"reduced_rate_alt\":5,\"super_reduced_rate\":false,\"parking_rate\":false},\"NL\":{\"country\":\"Netherlands\",\"standard_rate\":21,\"reduced_rate\":6,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"PL\":{\"country\":\"Poland\",\"standard_rate\":23,\"reduced_rate\":8,\"reduced_rate_alt\":5,\"super_reduced_rate\":false,\"parking_rate\":false},\"PT\":{\"country\":\"Portugal\",\"standard_rate\":23,\"reduced_rate\":13,\"reduced_rate_alt\":6,\"super_reduced_rate\":false,\"parking_rate\":13},\"RO\":{\"country\":\"Romania\",\"standard_rate\":20,\"reduced_rate\":9,\"reduced_rate_alt\":5,\"super_reduced_rate\":false,\"parking_rate\":false},\"SI\":{\"country\":\"Slovenia\",\"standard_rate\":22,\"reduced_rate\":9.5,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"SK\":{\"country\":\"Slovakia\",\"standard_rate\":20,\"reduced_rate\":10,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"SE\":{\"country\":\"Sweden\",\"standard_rate\":25,\"reduced_rate\":12,\"reduced_rate_alt\":6,\"super_reduced_rate\":false,\"parking_rate\":false},\"UK\":{\"_comment\":\"While the EU uses the country code 'UK' for the United Kingdom, ISO uses 'GB' - both are included for convenience.\",\"iso_duplicate\":\"GB\",\"country\":\"United Kingdom\",\"standard_rate\":20,\"reduced_rate\":5,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false},\"GB\":{\"_comment\":\"Duplicate of GB for convenience; the EU uses the country code 'UK' for the United Kingdom, while ISO uses 'GB'.\",\"iso_duplicate_of\":\"UK\",\"country\":\"United Kingdom\",\"standard_rate\":20,\"reduced_rate\":5,\"reduced_rate_alt\":false,\"super_reduced_rate\":false,\"parking_rate\":false}}]}";
            */

            string countryCode = null;
            JObject countryJsonBlock = null;

            // get countries one by one by regex
            foreach (var item in Regex.Matches(inputData, RegexPattern, RegexOptions.Multiline))
            {
                // get json block of each country
                try
                {
                    countryJsonBlock = (JObject)JsonConvert.DeserializeObject("{" + item.ToString() + "}");
                    countryCode = ((JProperty)countryJsonBlock.First).Name;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Json block <{item.ToString().Substring(0, 32)}(...)> was not imported. ({ex.Message}).");
                }

                // Great Britain (UK+GB) and Grece (GR+EL) have duplicate records 
                if (countryCode == "GB" || countryCode == "EL")
                {
                    continue;
                }

                // try to create a new country and add to the list
                Country newCountry = new Country(countryCode, ((JProperty)countryJsonBlock.First).Value.ToString());
                if (newCountry.IsValid())
                {
                    this.CountriesList.Add(newCountry);
                }
            }

            SpinAnimation.Stop();
            Console.WriteLine("\nDone");
        }
    }
}
