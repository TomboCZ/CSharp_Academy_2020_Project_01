using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text;

namespace Project_01
{
    /// <summary>
    /// One imported country and its VAT
    /// </summary>
    class Country
    {
        /// <summary>
        /// Country's name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Country's standard VAT rate
        /// </summary>
        public double StandardRate { get; set; }

        /// <summary>
        /// Struct template for Json deserializer
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
            TryImport(countryCode, jsonString);
        }

        /// <summary>
        /// Deconstructor
        /// </summary>
        /// <returns>true == new object has valid data, false == object is invalid</returns>
        public bool IsValid()
        {
            return !string.IsNullOrEmpty(this.Name) && !Double.IsNaN(StandardRate);
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
        private void TryImport(string countryCode, string jsonString)
        {
            try
            {
                JsonTemplate countryData = JsonConvert.DeserializeObject<JsonTemplate>(jsonString);
                this.Name = countryData.Country;
                // some countries do not have some rates applicable (there is a "false" string instead of a decimal value) 
                // in this case the value will be recognised as a "minimal" (but nonzero) rather than wrong one
                this.StandardRate = countryData.StandardRate.ToLower() == "false" ? Double.MinValue : Convert.ToDouble(countryData.StandardRate, CultureInfo.InvariantCulture);
            }
            catch (Exception ex)
            {
                this.Name = null;
                this.StandardRate = Double.NaN;
                Console.WriteLine($"Country {countryCode} was not imported. ({ex.Message})");
            }
        }
    }
}
