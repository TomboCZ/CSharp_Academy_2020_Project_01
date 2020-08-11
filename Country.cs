using Newtonsoft.Json;
using System;
using System.Globalization;

namespace Project_01
{
    /// <summary>
    /// One imported country and its VAT
    /// </summary>
    class Country
    {
        /// <summary>
        /// Country name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country standard VAT rate
        /// </summary>
        public double StandardRate { get; set; }

        /// <summary>
        /// Object's data was successfully imported
        /// </summary>
        public bool IsValid { get; set; } = false;

        /// <summary>
        /// Data template for Json deserializer
        /// </summary>
        private struct JsonTemplate
        {
            [JsonProperty("country")]
            public string Country { get; set; }

            [JsonProperty("standard_rate")]
            public string StandardRate { get; set; }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="countryCode">countryCode (ie. "UK")</param>
        /// <param name="jsonString">json data of this country</param>
        public Country(string countryCode, string jsonString)
        {
            this.IsValid = TryImport(countryCode, jsonString);
        }

        /// <summary>
        /// Prints country's name + VAT
        /// </summary>
        public void Print()
        {
            if (this.StandardRate == Double.MinValue)
            {
                Console.WriteLine($"{this.Name,-15}: N/A");
            }
            else
            {
                Console.WriteLine($"{this.Name,-15}: {this.StandardRate:N2}");
            }
        }

        /// <summary>
        /// Tries to import a new country data if possible
        /// </summary>
        /// <param name="countryCode">countryCode (ie. "UK")</param>
        /// <param name="jsonString">json data of this country</param>
        /// <returns>true == sucessfully imported, false == nothnig was imported</returns>
        private bool TryImport(string countryCode, string jsonString)
        {
            try
            {
                JsonTemplate countryData = JsonConvert.DeserializeObject<JsonTemplate>(jsonString);
                this.Name = countryData.Country;
                // some countries do not have some rates applicable (there is a "false" string instead of a decimal value) 
                // in this case the value will be recognised as a "minimal" (but not zero) rather than wrong one
                this.StandardRate = countryData.StandardRate.ToLower() == "false" ? Double.MinValue : Convert.ToDouble(countryData.StandardRate, CultureInfo.InvariantCulture);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Country {countryCode} was not imported. ({ex.Message})");
                return false; ;
            }
        }
    }
}
